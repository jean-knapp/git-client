using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using GitClient.Git;

namespace GitClient.Services
{
    /// <summary>The units Claude picked for "stage what I describe".</summary>
    public sealed class StageSelection
    {
        public List<StageUnit> Units { get; } = new List<StageUnit>();
        public string Note { get; set; } = string.Empty;
    }

    /// <summary>One commit of a proposed split.</summary>
    public sealed class PlannedCommit
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public List<StageUnit> Units { get; } = new List<StageUnit>();

        public string Message => Body.Length > 0 ? Subject + "\n\n" + Body : Subject;
    }

    public sealed class CommitSplit
    {
        public List<PlannedCommit> Commits { get; } = new List<PlannedCommit>();

        /// <summary>Units Claude advised not to commit (debug leftovers, secrets, scratch files).</summary>
        public List<StageUnit> LeftOut { get; } = new List<StageUnit>();
        public string LeftOutReason { get; set; } = string.Empty;
    }

    /// <summary>
    /// Asks the Claude Code CLI which changes to stage. Claude runs without tools and only reads the
    /// changes; it answers with unit ids, and the app does the staging itself.
    /// </summary>
    public static class ClaudeStagePlanner
    {
        private const int MaxListingCharacters = 150_000;
        private const int MaxUnitCharacters = 12_000;
        private static readonly Regex UnitId = new Regex(@"\bU(\d+)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static async Task<StageSelection> SelectAsync(
            string repositoryPath, StageSnapshot snapshot, string instruction,
            string executable, string model, CancellationToken cancellationToken)
        {
            var prompt =
                "You help stage changes in a git repository. Standard input holds the user's request, then their unstaged " +
                "changes split into units with ids like [U1] (one diff hunk, or one whole file). Choose exactly the units " +
                "that belong to what the user asked to stage. Use only ids that are listed. When a unit mixes the requested " +
                "work with other work, include it only if it is mostly the requested work, and say so in the note. " +
                "Reply with exactly two lines and nothing else, no markdown:\n" +
                "UNITS: <space-separated ids, or nothing>\n" +
                "NOTE: <one or two short sentences: what you picked, and anything ambiguous or left out>";

            var input = new StringBuilder();
            input.AppendLine(prompt);
            input.AppendLine();
            input.AppendLine("The user wants to stage:");
            input.AppendLine(instruction.Trim());
            input.AppendLine();
            input.AppendLine("Unstaged changes:");
            AppendListing(input, snapshot);

            var output = await RunAsync(repositoryPath, input.ToString(), executable, model, cancellationToken).ConfigureAwait(false);
            var selection = new StageSelection();
            foreach (var line in Lines(output))
            {
                if (StartsWith(line, "UNITS:")) AddUnits(selection.Units, snapshot, line.Substring(6), null);
                else if (StartsWith(line, "NOTE:")) selection.Note = Join(selection.Note, line.Substring(5).Trim());
            }
            return selection;
        }

        public static async Task<CommitSplit> SplitAsync(
            string repositoryPath, StageSnapshot snapshot, string guidance, IEnumerable<string> recentSubjects,
            string executable, string model, CancellationToken cancellationToken)
        {
            var prompt = new StringBuilder();
            prompt.Append("You help split work into commits. Standard input holds all uncommitted changes of a git repository, ");
            prompt.Append("split into units with ids like [U1] (one diff hunk, or one whole file). Group the units into separate, ");
            prompt.Append("coherent commits: one per feature, fix or independent edit. Order the commits so each one builds on the ");
            prompt.Append("ones before it. Put every unit in exactly one commit, except units that look like they should not be ");
            prompt.Append("committed at all (debug output, secrets, scratch or local-only files), which go on the LEAVE line. ");
            prompt.Append("Commit subjects are imperative, at most 72 characters; bodies are optional, one to four short lines. ");
            var subjects = (recentSubjects ?? Enumerable.Empty<string>()).Take(8).Select(s => s.Replace('\n', ' ').Trim()).ToList();
            if (subjects.Count > 0) prompt.Append("Match the style of these recent subjects: " + string.Join(" | ", subjects) + ". ");
            prompt.Append("Reply in exactly this format and nothing else, no markdown:\n");
            prompt.Append("COMMIT: <subject>\nBODY: <body line, optional, may repeat>\nUNITS: <space-separated ids>\n");
            prompt.Append("(repeat the three lines for each commit, first commit first)\n");
            prompt.Append("LEAVE: <ids not to commit, or nothing>\nREASON: <why they were left out, or nothing>");

            var input = new StringBuilder();
            input.AppendLine(prompt.ToString());
            input.AppendLine();
            if (!string.IsNullOrWhiteSpace(guidance))
            {
                input.AppendLine("Guidance from the user:");
                input.AppendLine(guidance.Trim());
                input.AppendLine();
            }
            input.AppendLine("Uncommitted changes:");
            AppendListing(input, snapshot);

            var output = await RunAsync(repositoryPath, input.ToString(), executable, model, cancellationToken).ConfigureAwait(false);
            var split = new CommitSplit();
            var used = new HashSet<StageUnit>();
            PlannedCommit commit = null;
            foreach (var line in Lines(output))
            {
                if (StartsWith(line, "COMMIT:"))
                {
                    commit = new PlannedCommit { Subject = line.Substring(7).Trim().Trim('"') };
                    split.Commits.Add(commit);
                }
                else if (StartsWith(line, "BODY:") && commit != null)
                {
                    var body = line.Substring(5).Trim();
                    if (body.Length > 0) commit.Body = commit.Body.Length > 0 ? commit.Body + "\n" + body : body;
                }
                else if (StartsWith(line, "UNITS:") && commit != null) AddUnits(commit.Units, snapshot, line.Substring(6), used);
                else if (StartsWith(line, "LEAVE:")) AddUnits(split.LeftOut, snapshot, line.Substring(6), used);
                else if (StartsWith(line, "REASON:")) split.LeftOutReason = Join(split.LeftOutReason, line.Substring(7).Trim());
            }
            split.Commits.RemoveAll(c => c.Units.Count == 0 || c.Subject.Length == 0);
            if (split.Commits.Count == 0 && split.LeftOut.Count == 0)
                throw new InvalidOperationException("Claude did not propose any commits. Try again, or add some guidance.");
            return split;
        }

        /// <summary>The changes as Claude reads them: each unit with its id, where it is, and its lines.</summary>
        private static void AppendListing(StringBuilder input, StageSnapshot snapshot)
        {
            int budget = MaxListingCharacters;
            foreach (var file in snapshot.Files)
            {
                input.AppendLine();
                input.AppendLine("FILE " + file.Path);
                foreach (var unit in file.Units)
                {
                    var counts = unit.Kind == StageUnitKind.Hunk ? " (+" + unit.Additions + " -" + unit.Deletions + ")" : string.Empty;
                    input.AppendLine("[" + unit.Id + "] " + unit.Label + counts);
                    var preview = unit.Preview ?? string.Empty;
                    if (preview.Length > MaxUnitCharacters) preview = preview.Substring(0, MaxUnitCharacters) + "\n[unit truncated]";
                    if (preview.Length == 0) continue;
                    if (preview.Length > budget)
                    {
                        input.AppendLine("[content omitted: the changes are too long to show in full]");
                        continue;
                    }
                    budget -= preview.Length;
                    input.Append(preview);
                    if (!preview.EndsWith("\n", StringComparison.Ordinal)) input.AppendLine();
                }
            }
        }

        private static void AddUnits(List<StageUnit> target, StageSnapshot snapshot, string ids, HashSet<StageUnit> used)
        {
            foreach (Match m in UnitId.Matches(ids))
            {
                var unit = snapshot.Find("U" + m.Groups[1].Value);
                if (unit == null || target.Contains(unit)) continue;
                // A unit Claude put in two commits stays in the first.
                if (used != null && !used.Add(unit)) continue;
                target.Add(unit);
            }
        }

        private static async Task<string> RunAsync(string repositoryPath, string input, string executable, string model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(executable) || !File.Exists(executable))
            {
                throw new InvalidOperationException("Claude Code CLI not found. Install it (npm install -g @anthropic-ai/claude-code) or set its path in Settings.");
            }
            // Instructions and the user's words travel on standard input: through cmd.exe (claude.cmd)
            // line breaks, & and | in an argument would be taken apart as commands.
            var args = new List<string> { "-p", "Follow the instructions at the top of standard input exactly.", "--output-format", "text", "--tools", "", "--no-session-persistence" };
            if (!string.IsNullOrWhiteSpace(model)) { args.Add("--model"); args.Add(model.Trim()); }
            return await ClaudeCommitComposer.RunAsync(executable, args, repositoryPath, input, cancellationToken).ConfigureAwait(false);
        }

        private static IEnumerable<string> Lines(string output) =>
            (output ?? string.Empty).Replace("\r\n", "\n").Split('\n')
                .Select(l => l.Trim().TrimStart('*', '-', ' ').Trim())
                .Where(l => l.Length > 0 && !l.StartsWith("```", StringComparison.Ordinal));

        private static bool StartsWith(string line, string prefix) =>
            line.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);

        private static string Join(string a, string b) => a.Length == 0 ? b : a + " " + b;
    }
}
