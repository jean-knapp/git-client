using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GitClient.Git;

namespace GitClient.Services
{
    /// <summary>
    /// Finds a GitHub token without asking for one: first whatever git already stores for
    /// github.com (Git Credential Manager keeps an OAuth token there), then the GitHub CLI. A
    /// token the user pastes is handed to git's credential store, so git and the API share it.
    /// </summary>
    public static class GitHubAuth
    {
        public const string Host = "github.com";

        /// <summary>Where the token came from, so the UI can say so.</summary>
        public sealed class Token
        {
            public Token(string value, string source, string user)
            {
                Value = value;
                Source = source;
                User = user;
            }

            public string Value { get; }
            public string Source { get; }

            /// <summary>The account git has stored, when it knows one.</summary>
            public string User { get; }
        }

        /// <summary>The token git and gh already have, or null when neither has one.</summary>
        public static async Task<Token> FindAsync()
        {
            var stored = await FromGitCredentialsAsync().ConfigureAwait(false);
            if (stored != null) return stored;
            return await FromGitHubCliAsync().ConfigureAwait(false);
        }

        /// <summary>
        /// Asks git's credential helper for github.com. Returns null when no helper is configured
        /// or nothing is stored - git then prints nothing rather than prompting, because the
        /// client runs it without a console.
        /// </summary>
        public static async Task<Token> FromGitCredentialsAsync()
        {
            try
            {
                var options = new GitRunOptions
                {
                    StandardInput = "protocol=https" + "\n" + "host=" + Host + "\n" + "\n",
                    Environment = new Dictionary<string, string>
                    {
                        // Never let the helper pop up a sign-in window from this lookup.
                        { "GIT_TERMINAL_PROMPT", "0" },
                        { "GCM_INTERACTIVE", "never" },
                    },
                };
                var result = await GitRunner.RunAsync(null,
                    new[] { "-c", "credential.interactive=never", "credential", "fill" },
                    options, System.Threading.CancellationToken.None).ConfigureAwait(false);
                if (!result.Succeeded) return null;

                string password = null, username = null;
                foreach (var line in result.StandardOutput.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    int equals = line.IndexOf('=');
                    if (equals <= 0) continue;
                    var key = line.Substring(0, equals).Trim();
                    var value = line.Substring(equals + 1).Trim();
                    if (key == "password") password = value;
                    else if (key == "username") username = value;
                }
                if (string.IsNullOrEmpty(password)) return null;
                return new Token(password, "the sign-in git already has for github.com", username);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>The GitHub CLI's token, when gh is installed and signed in.</summary>
        public static async Task<Token> FromGitHubCliAsync()
        {
            var gh = FindGitHubCli();
            if (gh == null) return null;
            try
            {
                var output = await RunAsync(gh, "auth token").ConfigureAwait(false);
                var token = (output ?? string.Empty).Trim();
                return token.Length == 0 ? null : new Token(token, "the GitHub CLI", null);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Hands a token to git's credential store so pushes stop asking for one. Silently does
        /// nothing when no credential helper is configured.
        /// </summary>
        public static async Task StoreAsync(string user, string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return;
            var input = new StringBuilder();
            input.Append("protocol=https").Append('\n');
            input.Append("host=").Append(Host).Append('\n');
            input.Append("username=").Append(string.IsNullOrWhiteSpace(user) ? "token" : user).Append('\n');
            input.Append("password=").Append(token).Append('\n');
            input.Append('\n');
            try
            {
                await GitRunner.RunAsync(null, new[] { "credential", "approve" },
                    new GitRunOptions { StandardInput = input.ToString() }, System.Threading.CancellationToken.None).ConfigureAwait(false);
            }
            catch
            {
                // Storing is a convenience; the token still works for this session.
            }
        }

        /// <summary>Page that creates a token with exactly the scopes this client needs.</summary>
        public const string TokenPageUrl =
            "https://github.com/settings/tokens/new?scopes=repo,read:org&description=Git%20Client";

        public static string FindGitHubCli()
        {
            var candidates = new List<string>();
            var path = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var dir in path.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(dir)) continue;
                try { candidates.Add(Path.Combine(dir.Trim(), "gh.exe")); } catch { }
            }
            var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
            candidates.Add(Path.Combine(programFiles, "GitHub CLI", "gh.exe"));
            foreach (var candidate in candidates)
            {
                try { if (File.Exists(candidate)) return candidate; } catch { }
            }
            return null;
        }

        private static Task<string> RunAsync(string executable, string arguments)
        {
            return Task.Run(() =>
            {
                var info = new ProcessStartInfo(executable, arguments)
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                };
                using (var process = Process.Start(info))
                {
                    var output = process.StandardOutput.ReadToEnd();
                    process.StandardError.ReadToEnd();
                    process.WaitForExit(10000);
                    return process.ExitCode == 0 ? output : null;
                }
            });
        }
    }
}
