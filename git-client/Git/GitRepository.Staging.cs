using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace GitClient.Git
{
    // Staging chosen pieces of the working tree. Everything here writes to the index only: files on
    // disk are read, never written, and nothing that is already staged is taken back out.
    public sealed partial class GitRepository
    {
        private static readonly Regex UnitHunkRegex = new Regex(@"^@@ -(\d+)(?:,(\d+))? \+(\d+)(?:,(\d+))? @@ ?(.*)$", RegexOptions.Compiled);

        /// <summary>
        /// Splits the unstaged changes into units that can be staged on their own: one per diff hunk
        /// of a modified text file, one per file for everything else (new, deleted, binary files).
        /// </summary>
        public async Task<StageSnapshot> GetStageSnapshotAsync(IEnumerable<FileChange> unstaged, CancellationToken cancellationToken)
        {
            var snapshot = new StageSnapshot();
            var changes = unstaged.Where(c => !c.Staged).ToList();
            var tracked = changes.Where(c => c.Kind != FileChangeKind.Untracked && c.Kind != FileChangeKind.Conflicted).ToList();

            var sections = tracked.Count > 0
                ? await ReadUnstagedSectionsAsync(tracked.Select(c => c.Path), cancellationToken).ConfigureAwait(false)
                : new Dictionary<string, string>(StringComparer.Ordinal);

            int next = 1;
            foreach (var change in changes)
            {
                if (change.Kind == FileChangeKind.Conflicted)
                {
                    snapshot.Skipped.Add(change.Path + " — conflicted; resolve it yourself first");
                    continue;
                }

                var file = new StageFile { Path = change.Path };
                if (change.Kind == FileChangeKind.Untracked)
                {
                    file.IsUntracked = true;
                    file.Fingerprint = UntrackedFingerprint(change.Path);
                    file.Units.Add(new StageUnit
                    {
                        File = file,
                        Kind = StageUnitKind.WholeFile,
                        Label = DescribeUntracked(change.Path),
                        Preview = PreviewUntracked(change.Path),
                    });
                }
                else
                {
                    string section;
                    if (!sections.TryGetValue(change.Path, out section) || section.Length == 0)
                    {
                        // Only the stat changed, or the path is one git had to quote: nothing to stage precisely.
                        continue;
                    }
                    file.Fingerprint = section;
                    SplitSection(file, section);
                }

                foreach (var unit in file.Units) unit.Id = "U" + (next++).ToString(CultureInfo.InvariantCulture);
                snapshot.Files.Add(file);
            }
            return snapshot;
        }

        /// <summary>
        /// Adds the chosen units to the index. Each unit is looked up again in the working tree
        /// first: a file whose chosen parts are no longer there as they were read is left alone.
        /// Files whose changes all were chosen are staged with <c>git add</c>; the others get just
        /// their chosen hunks through <c>git apply --cached</c>, checked first.
        /// </summary>
        public async Task<StageOutcome> StageUnitsAsync(IEnumerable<StageUnit> units, CancellationToken cancellationToken)
        {
            var outcome = new StageOutcome();
            var byFile = units.GroupBy(u => u.File).ToList();
            if (byFile.Count == 0) return outcome;

            // Read again rather than trusting the snapshot: the user may have kept editing, and after
            // an earlier commit of a split the hunks sit at other line numbers.
            var trackedPaths = byFile.Where(g => !g.Key.IsUntracked).Select(g => g.Key.Path).ToList();
            var current = trackedPaths.Count > 0
                ? await ReadUnstagedSectionsAsync(trackedPaths, cancellationToken).ConfigureAwait(false)
                : new Dictionary<string, string>(StringComparer.Ordinal);

            var whole = new List<string>();
            var partial = new List<KeyValuePair<StageFile, List<StageUnit>>>();
            foreach (var group in byFile)
            {
                var file = group.Key;
                if (file.IsUntracked)
                {
                    if (UntrackedFingerprint(file.Path) == file.Fingerprint) { whole.Add(file.Path); outcome.StagedUnits++; }
                    else outcome.Skipped.Add(file.Path + " — changed since Claude read it; ask again");
                    continue;
                }

                string section;
                if (!current.TryGetValue(file.Path, out section))
                {
                    outcome.Skipped.Add(file.Path + " — no longer has unstaged changes");
                    continue;
                }
                var fresh = new StageFile { Path = file.Path, Fingerprint = section };
                SplitSection(fresh, section);

                var matched = new List<StageUnit>();
                foreach (var unit in group)
                {
                    var match = fresh.Units.FirstOrDefault(u => !matched.Contains(u) && SameChange(u, unit));
                    if (match == null) { matched = null; break; }
                    matched.Add(match);
                }
                if (matched == null)
                {
                    outcome.Skipped.Add(file.Path + " — changed since Claude read it; ask again");
                    continue;
                }
                if (matched.Count == fresh.Units.Count || matched.Any(u => u.Kind == StageUnitKind.WholeFile))
                {
                    whole.Add(file.Path);
                    outcome.StagedUnits += matched.Count;
                }
                else partial.Add(new KeyValuePair<StageFile, List<StageUnit>>(fresh, matched));
            }

            if (whole.Count > 0)
            {
                var args = new List<string> { "add", "-A", "--" };
                args.AddRange(whole);
                (await RunAsync(null, cancellationToken, args.ToArray()).ConfigureAwait(false)).ThrowIfFailed();
            }

            foreach (var pair in partial)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var patch = new StringBuilder(pair.Key.Header);
                foreach (var unit in pair.Key.Units.Where(pair.Value.Contains)) patch.Append(unit.Patch);
                var options = new GitRunOptions { StandardInput = patch.ToString() };

                // --whitespace=nowarn also overrides an apply.whitespace=fix setting, which would
                // rewrite the lines being staged.
                var check = await RunAsync(options, cancellationToken, "apply", "--cached", "--check", "--whitespace=nowarn", "-").ConfigureAwait(false);
                if (!check.Succeeded)
                {
                    outcome.Skipped.Add(pair.Key.Path + " — its parts could not be staged separately (" + FirstLine(check.Message) + ")");
                    continue;
                }
                (await RunAsync(options, cancellationToken, "apply", "--cached", "--whitespace=nowarn", "-").ConfigureAwait(false)).ThrowIfFailed();
                outcome.StagedUnits += pair.Value.Count;
            }
            return outcome;
        }

        /// <summary>Same lines changed, wherever the hunk now starts.</summary>
        private static bool SameChange(StageUnit now, StageUnit then)
        {
            if (now.Kind != then.Kind) return false;
            if (now.Kind == StageUnitKind.WholeFile) return now.Patch == then.Patch;
            return HunkBody(now.Patch) == HunkBody(then.Patch);
        }

        private static string HunkBody(string patch)
        {
            int eol = patch.IndexOf('\n');
            return eol < 0 ? string.Empty : patch.Substring(eol + 1);
        }

        /// <summary>True when the index holds anything HEAD does not.</summary>
        public async Task<bool> HasStagedChangesAsync()
        {
            var result = await RunAsync("diff", "--cached", "--quiet").ConfigureAwait(false);
            return result.ExitCode == 1;
        }

        /// <summary>The raw unstaged diff of each path, exactly as git wrote it, keyed by path.</summary>
        private async Task<Dictionary<string, string>> ReadUnstagedSectionsAsync(IEnumerable<string> paths, CancellationToken cancellationToken)
        {
            // Fixed prefixes and no textconv, renames or external tools, so the text is a patch
            // git apply takes back whatever the user's diff settings are.
            var args = new List<string>
            {
                "diff", "--no-color", "--no-ext-diff", "--no-textconv", "--no-renames", "--binary",
                "--src-prefix=a/", "--dst-prefix=b/", "-U3", "--",
            };
            args.AddRange(paths);
            var result = await RunAsync(null, cancellationToken, args.ToArray()).ConfigureAwait(false);
            if (!result.Succeeded && result.ExitCode != 1) throw new GitException(result);

            var map = new Dictionary<string, string>(StringComparer.Ordinal);
            var text = result.StandardOutput ?? string.Empty;
            int start = text.StartsWith("diff --git ", StringComparison.Ordinal) ? 0 : text.IndexOf("\ndiff --git ", StringComparison.Ordinal);
            while (start >= 0 && start < text.Length)
            {
                if (text[start] == '\n') start++;
                int end = text.IndexOf("\ndiff --git ", start, StringComparison.Ordinal);
                var section = end < 0 ? text.Substring(start) : text.Substring(start, end + 1 - start);
                var path = SectionPath(section);
                if (path != null) map[path] = section;
                start = end;
            }
            return map;
        }

        /// <summary>The path of a "diff --git a/P b/P" section; null for quoted paths.</summary>
        private static string SectionPath(string section)
        {
            int eol = section.IndexOf('\n');
            var line = (eol < 0 ? section : section.Substring(0, eol)).TrimEnd('\r');
            const string Prefix = "diff --git ";
            if (!line.StartsWith(Prefix + "a/", StringComparison.Ordinal)) return null;
            var rest = line.Substring(Prefix.Length);
            if ((rest.Length - 5) % 2 != 0) return null;
            int length = (rest.Length - 5) / 2;
            var path = rest.Substring(2, length);
            return rest == "a/" + path + " b/" + path ? path : null;
        }

        private static void SplitSection(StageFile file, string section)
        {
            var lines = SplitKeepingEnds(section);
            int first = lines.FindIndex(l => l.StartsWith("@@ ", StringComparison.Ordinal));
            file.Header = string.Concat(first < 0 ? lines : lines.Take(first));

            var header = file.Header;
            bool whole = first < 0
                || header.Contains("\ndeleted file mode")
                || header.Contains("\nnew file mode")
                || header.Contains(" 160000")          // a submodule moved to another commit
                || section.IndexOf('\uFFFD') >= 0;     // not UTF-8: staging part of it could not round-trip its bytes

            if (whole)
            {
                file.Units.Add(new StageUnit
                {
                    File = file,
                    Kind = StageUnitKind.WholeFile,
                    Label = DescribeWhole(header, lines, first),
                    Preview = first < 0 ? string.Empty : Clip(string.Concat(lines.Skip(first)), 6000),
                    Patch = section,
                });
                return;
            }

            StageUnit unit = null;
            var body = new StringBuilder();
            for (int i = first; i <= lines.Count; i++)
            {
                bool atEnd = i == lines.Count;
                if (atEnd || lines[i].StartsWith("@@ ", StringComparison.Ordinal))
                {
                    if (unit != null)
                    {
                        unit.Patch = body.ToString();
                        unit.Preview = unit.Patch;
                        file.Units.Add(unit);
                    }
                    if (atEnd) break;
                    unit = new StageUnit { File = file, Kind = StageUnitKind.Hunk };
                    body.Clear();
                    var m = UnitHunkRegex.Match(lines[i].TrimEnd('\r', '\n'));
                    if (m.Success)
                    {
                        int newStart = int.Parse(m.Groups[3].Value, CultureInfo.InvariantCulture);
                        int newCount = m.Groups[4].Success ? int.Parse(m.Groups[4].Value, CultureInfo.InvariantCulture) : 1;
                        unit.Label = newCount <= 1 ? "line " + newStart : "lines " + newStart + "–" + (newStart + newCount - 1);
                        var context = m.Groups[5].Value.Trim();
                        if (context.Length > 0) unit.Label += " · " + Clip(context, 60);
                    }
                    else unit.Label = "change";
                }
                else if (lines[i].StartsWith("+", StringComparison.Ordinal)) unit.Additions++;
                else if (lines[i].StartsWith("-", StringComparison.Ordinal)) unit.Deletions++;
                body.Append(lines[i]);
            }
        }

        private static string DescribeWhole(string header, List<string> lines, int first)
        {
            if (header.Contains("\ndeleted file mode")) return "deleted file";
            if (header.Contains("\nnew file mode")) return "new file";
            if (header.Contains(" 160000")) return "submodule moved";
            if (header.Contains("GIT binary patch") || header.Contains("Binary files")) return "binary file";
            if (first < 0 && header.Contains("\nold mode")) return "permissions changed";
            return "whole file";
        }

        private string DescribeUntracked(string path)
        {
            var full = FullPathOf(path);
            if (Directory.Exists(full)) return "new folder";
            try
            {
                var info = new FileInfo(full);
                return info.Exists ? "new file, " + FormatSize(info.Length) : "new file";
            }
            catch (Exception)
            {
                return "new file";
            }
        }

        /// <summary>The start of a new file, so Claude can tell what it is for.</summary>
        private string PreviewUntracked(string path)
        {
            var full = FullPathOf(path);
            try
            {
                if (Directory.Exists(full))
                {
                    var names = Directory.EnumerateFileSystemEntries(full).Take(40).Select(Path.GetFileName);
                    return "contains: " + string.Join(", ", names);
                }
                var info = new FileInfo(full);
                if (!info.Exists) return string.Empty;
                var bytes = new byte[Math.Min(info.Length, 8000)];
                using (var stream = File.OpenRead(full)) stream.Read(bytes, 0, bytes.Length);
                if (Array.IndexOf(bytes, (byte)0) >= 0) return "(binary)";
                return Clip(new UTF8Encoding(false).GetString(bytes), 3000);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        private string UntrackedFingerprint(string path)
        {
            var full = FullPathOf(path);
            try
            {
                if (Directory.Exists(full)) return "dir";
                var info = new FileInfo(full);
                return info.Exists ? info.Length + "@" + info.LastWriteTimeUtc.Ticks : "missing";
            }
            catch (Exception)
            {
                return "unreadable";
            }
        }

        private string FullPathOf(string path) =>
            Path.Combine(WorkingDirectory, path.TrimEnd('/').Replace('/', Path.DirectorySeparatorChar));

        private static List<string> SplitKeepingEnds(string text)
        {
            var lines = new List<string>();
            int start = 0;
            while (start < text.Length)
            {
                int eol = text.IndexOf('\n', start);
                int end = eol < 0 ? text.Length : eol + 1;
                lines.Add(text.Substring(start, end - start));
                start = end;
            }
            return lines;
        }

        private static string Clip(string text, int max) =>
            text.Length <= max ? text : text.Substring(0, max) + "…";

        private static string FirstLine(string text)
        {
            var line = (text ?? string.Empty).Trim().Split('\n')[0].Trim();
            return line.Length == 0 ? "git refused the patch" : Clip(line, 140);
        }

        private static string FormatSize(long bytes)
        {
            if (bytes < 1024) return bytes + " bytes";
            if (bytes < 1024 * 1024) return (bytes / 1024) + " KB";
            return (bytes / (1024 * 1024.0)).ToString("0.#", CultureInfo.InvariantCulture) + " MB";
        }
    }

    public enum StageUnitKind
    {
        /// <summary>One hunk of a modified text file.</summary>
        Hunk,

        /// <summary>A change that only makes sense in full: a new, deleted or binary file.</summary>
        WholeFile,
    }

    /// <summary>A piece of the working tree that can be staged by itself.</summary>
    public sealed class StageUnit
    {
        /// <summary>Short id used in the conversation with Claude, e.g. "U7".</summary>
        public string Id { get; set; }
        public StageFile File { get; set; }
        public StageUnitKind Kind { get; set; }

        /// <summary>Where it is, e.g. "lines 40–58 · void Save()" or "new file, 3 KB".</summary>
        public string Label { get; set; }

        /// <summary>What Claude reads.</summary>
        public string Preview { get; set; }

        /// <summary>The hunk text given to git apply (whole-file units are staged with git add).</summary>
        public string Patch { get; set; }

        public int Additions { get; set; }
        public int Deletions { get; set; }

        public string Path => File.Path;
    }

    public sealed class StageFile
    {
        public string Path { get; set; }
        public bool IsUntracked { get; set; }

        /// <summary>The diff lines above the first hunk.</summary>
        public string Header { get; set; } = string.Empty;

        /// <summary>What the file looked like when read, to notice it changing before staging.</summary>
        public string Fingerprint { get; set; }

        public List<StageUnit> Units { get; } = new List<StageUnit>();
    }

    public sealed class StageSnapshot
    {
        public List<StageFile> Files { get; } = new List<StageFile>();

        /// <summary>Changes left out of the snapshot, with the reason.</summary>
        public List<string> Skipped { get; } = new List<string>();

        public IEnumerable<StageUnit> Units => Files.SelectMany(f => f.Units);

        public StageUnit Find(string id) =>
            Units.FirstOrDefault(u => string.Equals(u.Id, id, StringComparison.OrdinalIgnoreCase));
    }

    public sealed class StageOutcome
    {
        public int StagedUnits { get; set; }

        /// <summary>Files left unstaged, with the reason.</summary>
        public List<string> Skipped { get; } = new List<string>();
    }
}
