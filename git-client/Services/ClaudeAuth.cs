using System;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitClient.Forms;

namespace GitClient.Services
{
    /// <summary>
    /// The Claude Code CLI refused to work because it is not signed in, or its sign-in expired.
    /// <see cref="Exception.Message"/> is what the CLI said.
    /// </summary>
    public sealed class ClaudeSignInRequiredException : InvalidOperationException
    {
        public ClaudeSignInRequiredException(string cliMessage) : base(cliMessage) { }
    }

    /// <summary>
    /// Whether the Claude Code CLI is signed in, and getting it signed in: the CLI's own
    /// <c>claude auth status</c> and <c>claude auth login</c>. The app never sees the account's
    /// credentials; the CLI keeps them.
    /// </summary>
    public static class ClaudeAuth
    {
        // What the CLI prints when it has no usable sign-in: never signed in, an expired OAuth
        // session, or a bad API key.
        private static readonly Regex SignInProblem = new Regex(
            @"not (logged|signed) in|log ?in again|please run /login|run `?claude (auth )?login|failed to authenticate|" +
            @"oauth (session|token)|session expired|invalid api key|authentication_error|unauthori[sz]ed",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public static bool LooksLikeSignInProblem(string cliOutput) =>
            !string.IsNullOrEmpty(cliOutput) && SignInProblem.IsMatch(cliOutput);

        /// <summary>True or false from <c>claude auth status</c>; null when it could not be told.</summary>
        public static async Task<bool?> IsSignedInAsync(string executable, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(executable) || !File.Exists(executable)) return null;
            try
            {
                // Signed out, the CLI still prints its status but exits with 1, so the exit code is ignored.
                var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                var result = await ClaudeCommitComposer.RunRawAsync(executable, new[] { "auth", "status", "--json" }, home, string.Empty, cancellationToken).ConfigureAwait(false);
                var match = Regex.Match(result.Output ?? string.Empty, "\"loggedIn\"\\s*:\\s*(true|false)", RegexOptions.IgnoreCase);
                return match.Success ? string.Equals(match.Groups[1].Value, "true", StringComparison.OrdinalIgnoreCase) : (bool?)null;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Opens a terminal window running <c>claude auth login</c>, which sends the user to the
        /// browser to sign in. The window stays open afterwards so its result can be read.
        /// </summary>
        public static Process StartSignIn(string executable)
        {
            var shell = Environment.GetEnvironmentVariable("ComSpec") ?? "cmd.exe";
            var command = "title Sign in to Claude Code & \"" + executable + "\" auth login & echo. & echo You can close this window now. & pause >nul";
            var psi = new ProcessStartInfo(shell, "/d /c \"" + command + "\"")
            {
                UseShellExecute = true,
                WorkingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            };
            return Process.Start(psi);
        }

        /// <summary>
        /// Explains that Claude Code needs signing in and offers to start it. When the sign-in
        /// window closes, <paramref name="report"/> is told whether it worked, so the user knows
        /// to try again.
        /// </summary>
        public static void OfferSignIn(IWin32Window owner, string executable, string cliMessage, Action<string> report)
        {
            var message =
                "Claude Code is not signed in on this computer, or its sign-in has expired, so it cannot help here." +
                "\n\nSign in opens a terminal running  claude auth login . It opens your browser to sign in to your Anthropic " +
                "account (a Claude subscription, or the Anthropic Console). When the terminal says you are signed in, close it " +
                "and try again." +
                (string.IsNullOrWhiteSpace(cliMessage) ? string.Empty : "\n\nClaude Code said: " + cliMessage.Trim());
            if (Dialogs.Show(owner, "Sign in to Claude Code", message, "Sign in…", null, "Cancel") != DialogResult.OK) return;

            Process window;
            try
            {
                window = StartSignIn(executable);
            }
            catch (Exception ex)
            {
                Dialogs.Error(owner, "Sign in to Claude Code",
                    "Could not open the sign-in window.\n\n" + ex.Message + "\n\nRun  claude auth login  in a terminal instead.");
                return;
            }
            report?.Invoke("Finish signing in to Claude Code in the window that opened, then try again.");
            _ = ReportWhenClosedAsync(window, executable, report);
        }

        private static async Task ReportWhenClosedAsync(Process window, string executable, Action<string> report)
        {
            if (window == null || report == null) return;
            try
            {
                using (window)
                {
                    await Task.Run(() => window.WaitForExit()).ConfigureAwait(true);
                }
                var signedIn = await IsSignedInAsync(executable, CancellationToken.None).ConfigureAwait(true);
                if (signedIn == true) report("Signed in to Claude Code. Try again.");
                else if (signedIn == false) report("Claude Code is still not signed in.");
            }
            catch (Exception)
            {
                // The status line is a courtesy; a failure here changes nothing.
            }
        }
    }
}
