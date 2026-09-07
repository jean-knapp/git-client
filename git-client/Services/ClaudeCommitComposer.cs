using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GitClient.Services
{
    public sealed class CommitMessageSuggestion
    {
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    /// <summary>Asks the Claude Code CLI (<c>claude -p</c>) to write a commit message for the staged diff.</summary>
    public static class ClaudeCommitComposer
    {
        private const int MaxPatchCharacters = 160_000;

        /// <summary>Resolves the claude executable: explicit setting, PATH, then the default install locations.</summary>
        public static string FindExecutable(string configured)
        {
            if (!string.IsNullOrWhiteSpace(configured) && File.Exists(configured)) return configured;

            var names = new[] { "claude.exe", "claude.cmd", "claude" };
            var pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var dir in pathVar.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(dir)) continue;
                foreach (var name in names)
                {
                    try
                    {
                        var candidate = Path.Combine(dir.Trim(), name);
                        if (File.Exists(candidate)) return candidate;
                    }
                    catch { }
                }
            }

            var profile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var fallbacks = new[]
            {
                Path.Combine(profile, ".local", "bin", "claude.exe"),
                Path.Combine(appData, "npm", "claude.cmd"),
            };
            foreach (var f in fallbacks)
            {
                if (File.Exists(f)) return f;
            }
            return null;
        }

        public static async Task<CommitMessageSuggestion> ComposeAsync(
            string repositoryPath,
            string patch,
            IEnumerable<string> changedFiles,
            IEnumerable<string> recentSubjects,
            string executable,
            string model,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(executable) || !File.Exists(executable))
            {
                throw new InvalidOperationException("Claude Code CLI not found. Install it (npm install -g @anthropic-ai/claude-code) or set its path in Settings.");
            }

            var prompt = new StringBuilder();
            prompt.Append("Write a git commit message for the staged changes provided on standard input. ");
            prompt.Append("Rules: the first line is an imperative summary of at most 72 characters; then a blank line; ");
            prompt.Append("then an optional body of one to six short lines explaining what changed and why, wrapped at 72 characters. ");
            prompt.Append("Output only the commit message: no markdown, no quotes, no code fences, no preamble, no explanation.");
            var subjects = new List<string>(recentSubjects ?? Array.Empty<string>());
            if (subjects.Count > 0)
            {
                prompt.Append(" Match the style of these recent commit subjects from this repository: ");
                for (int i = 0; i < subjects.Count && i < 8; i++)
                {
                    if (i > 0) prompt.Append(" | ");
                    prompt.Append(subjects[i].Replace('\n', ' ').Trim());
                }
                prompt.Append('.');
            }

            var input = new StringBuilder();
            input.AppendLine("Changed files:");
            foreach (var f in changedFiles ?? Array.Empty<string>()) input.AppendLine("- " + f);
            input.AppendLine();
            input.AppendLine("Diff:");
            if (patch != null && patch.Length > MaxPatchCharacters)
            {
                input.Append(patch, 0, MaxPatchCharacters);
                input.AppendLine();
                input.AppendLine("[diff truncated]");
            }
            else input.AppendLine(patch ?? string.Empty);

            var args = new List<string> { "-p", prompt.ToString(), "--output-format", "text", "--tools", "", "--no-session-persistence" };
            if (!string.IsNullOrWhiteSpace(model)) { args.Add("--model"); args.Add(model.Trim()); }

            var output = await RunAsync(executable, args, repositoryPath, input.ToString(), cancellationToken).ConfigureAwait(false);
            return Parse(output);
        }

        private static CommitMessageSuggestion Parse(string output)
        {
            var text = (output ?? string.Empty).Replace("\r\n", "\n").Trim();
            if (text.StartsWith("```", StringComparison.Ordinal))
            {
                var firstBreak = text.IndexOf('\n');
                text = firstBreak >= 0 ? text.Substring(firstBreak + 1) : string.Empty;
                var fence = text.LastIndexOf("```", StringComparison.Ordinal);
                if (fence >= 0) text = text.Substring(0, fence);
                text = text.Trim();
            }
            var lines = text.Split('\n');
            var suggestion = new CommitMessageSuggestion { Subject = string.Empty, Body = string.Empty };
            int i = 0;
            while (i < lines.Length && lines[i].Trim().Length == 0) i++;
            if (i < lines.Length) suggestion.Subject = lines[i].Trim().Trim('"');
            i++;
            while (i < lines.Length && lines[i].Trim().Length == 0) i++;
            var body = new StringBuilder();
            for (; i < lines.Length; i++)
            {
                if (body.Length > 0) body.Append('\n');
                body.Append(lines[i].TrimEnd());
            }
            suggestion.Body = body.ToString().Trim();
            if (suggestion.Subject.Length == 0) throw new InvalidOperationException("Claude returned an empty message.");
            return suggestion;
        }

        private static async Task<string> RunAsync(string executable, IList<string> args, string workingDirectory, string stdin, CancellationToken cancellationToken)
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory,
                StandardOutputEncoding = new UTF8Encoding(false),
                StandardErrorEncoding = new UTF8Encoding(false),
            };
            var arguments = Git.GitRunner.BuildArguments(args);
            if (executable.EndsWith(".cmd", StringComparison.OrdinalIgnoreCase) || executable.EndsWith(".bat", StringComparison.OrdinalIgnoreCase))
            {
                psi.FileName = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe";
                psi.Arguments = "/d /c " + Git.GitRunner.Quote(executable) + " " + arguments;
            }
            else
            {
                psi.FileName = executable;
                psi.Arguments = arguments;
            }
            // Make sure the CLI never tries to open an interactive session.
            psi.EnvironmentVariables["CI"] = "1";

            var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
            var exit = new TaskCompletionSource<bool>();
            process.Exited += (s, e) => exit.TrySetResult(true);
            process.Start();
            using (process)
            using (cancellationToken.Register(() => { try { if (!process.HasExited) process.Kill(); } catch { } }))
            {
                var stdinTask = Task.Run(async () =>
                {
                    try
                    {
                        using (var writer = new StreamWriter(process.StandardInput.BaseStream, new UTF8Encoding(false)))
                        {
                            await writer.WriteAsync(stdin).ConfigureAwait(false);
                        }
                    }
                    catch { }
                });
                var stdout = process.StandardOutput.ReadToEndAsync();
                var stderr = process.StandardError.ReadToEndAsync();
                await Task.WhenAll(exit.Task, stdout, stderr, stdinTask).ConfigureAwait(false);
                process.WaitForExit();
                cancellationToken.ThrowIfCancellationRequested();
                if (process.ExitCode != 0)
                {
                    var error = stderr.Result.Trim();
                    if (error.Length == 0) error = stdout.Result.Trim();
                    if (error.Length == 0) error = "claude exited with code " + process.ExitCode;
                    throw new InvalidOperationException(error);
                }
                return stdout.Result;
            }
        }
    }
}
