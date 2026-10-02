using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GitClient.Git
{
    // Files whose only difference is their line endings. They appear when a repository marks files
    // as text in .gitattributes after they were committed with CRLF: git compares them as LF, finds
    // every line changed, and checking the file out again restores the same CRLF, so Discard cannot
    // make the change go away. Committing them once with normalized endings does.
    public sealed partial class GitRepository
    {
        /// <summary>
        /// Of <paramref name="paths"/>, the ones changed in the working tree in their line endings
        /// only: the content is identical once carriage returns are ignored.
        /// </summary>
        public async Task<HashSet<string>> LineEndingOnlyChangesAsync(IEnumerable<string> paths)
        {
            var list = paths.Where(p => !string.IsNullOrEmpty(p)).Distinct().ToList();
            var result = new HashSet<string>(StringComparer.Ordinal);
            if (list.Count == 0) return result;

            var changed = await NumstatPathsAsync(list, ignoreLineEndings: false).ConfigureAwait(false);
            if (changed.Count == 0) return result;
            var real = await NumstatPathsAsync(changed, ignoreLineEndings: true).ConfigureAwait(false);
            foreach (var path in changed)
            {
                if (!real.Contains(path)) result.Add(path);
            }
            return result;
        }

        /// <summary>
        /// Tracked files committed with CRLF that .gitattributes now says to store as LF text, and
        /// that hold no other change: they show up as fully changed as soon as they are touched.
        /// </summary>
        public async Task<List<string>> UnnormalizedFilesAsync()
        {
            var output = await QueryAsync("ls-files", "--eol", "-z").ConfigureAwait(false);
            var candidates = new List<string>();
            foreach (var record in (output ?? string.Empty).Split('\0'))
            {
                // "i/crlf  w/crlf  attr/text             \t<path>"
                int tab = record.IndexOf('\t');
                if (tab < 0) continue;
                var info = record.Substring(0, tab).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (info.Length < 3 || info[0] != "i/crlf") continue;
                // Only an explicit "text" normalizes a file already committed with CRLF; text=auto leaves it alone.
                var attr = info[2];
                if (attr != "attr/text" && !attr.StartsWith("attr/text eol=", StringComparison.Ordinal)) continue;
                candidates.Add(record.Substring(tab + 1));
            }
            if (candidates.Count == 0) return candidates;

            // Leave out anything with a real edit: renormalizing it would stage that edit too.
            var real = await NumstatPathsAsync(candidates, ignoreLineEndings: true).ConfigureAwait(false);
            return candidates.Where(p => !real.Contains(p)).ToList();
        }

        /// <summary>
        /// Stages the files with the line endings .gitattributes asks for (<c>git add --renormalize</c>).
        /// Only the index changes; the files on disk keep their content.
        /// </summary>
        public async Task RenormalizeAsync(IEnumerable<string> paths)
        {
            var list = paths.ToList();
            if (list.Count == 0) return;
            foreach (var batch in Batches(list, 200))
            {
                var args = new List<string> { "add", "--renormalize", "--" };
                args.AddRange(batch);
                (await RunAsync(args.ToArray()).ConfigureAwait(false)).ThrowIfFailed();
            }
        }

        /// <summary>The paths <c>git diff --numstat</c> reports with any added or removed line.</summary>
        private async Task<HashSet<string>> NumstatPathsAsync(IReadOnlyCollection<string> paths, bool ignoreLineEndings)
        {
            var found = new HashSet<string>(StringComparer.Ordinal);
            foreach (var batch in Batches(paths.ToList(), 200))
            {
                var args = new List<string> { "diff", "--numstat", "--no-renames", "--no-ext-diff", "--no-textconv" };
                if (ignoreLineEndings) args.Add("--ignore-cr-at-eol");
                args.Add("--");
                args.AddRange(batch);
                var result = await RunAsync(args.ToArray()).ConfigureAwait(false);
                if (!result.Succeeded && result.ExitCode != 1) throw new GitException(result);
                foreach (var line in (result.StandardOutput ?? string.Empty).Split('\n'))
                {
                    var parts = line.TrimEnd('\r').Split('\t');
                    if (parts.Length < 3) continue;
                    // With whitespace ignored, git may still list a file with nothing left to show.
                    if (parts[0] == "0" && parts[1] == "0") continue;
                    found.Add(parts[2]);
                }
            }
            return found;
        }

        private static IEnumerable<List<string>> Batches(List<string> items, int size)
        {
            for (int i = 0; i < items.Count; i += size) yield return items.GetRange(i, Math.Min(size, items.Count - i));
        }
    }
}
