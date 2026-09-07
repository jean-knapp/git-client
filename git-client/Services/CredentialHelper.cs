using System;
using System.IO;
using System.Threading.Tasks;
using GitClient.Git;

namespace GitClient.Services
{
    /// <summary>
    /// Helpers for the git credential helper. Without one configured, git cannot sign in to
    /// https remotes and fails with "could not read Username ... terminal prompts disabled",
    /// because the client runs git without a console.
    /// </summary>
    public static class CredentialHelper
    {
        /// <summary>Config value for Git Credential Manager, which ships with Git for Windows.</summary>
        public const string ManagerValue = "manager";

        /// <summary>Path to git-credential-manager.exe, or null when it is not installed.</summary>
        public static string FindManager()
        {
            var candidates = new System.Collections.Generic.List<string>();

            // Git for Windows keeps it next to the other helpers, two levels up from cmd\git.exe.
            try
            {
                var gitDirectory = Path.GetDirectoryName(GitRunner.GitExecutable);
                if (!string.IsNullOrEmpty(gitDirectory))
                {
                    var root = Path.GetDirectoryName(gitDirectory);
                    if (!string.IsNullOrEmpty(root))
                    {
                        candidates.Add(Path.Combine(root, "mingw64", "bin", "git-credential-manager.exe"));
                        candidates.Add(Path.Combine(root, "mingw32", "bin", "git-credential-manager.exe"));
                    }
                    candidates.Add(Path.Combine(gitDirectory, "git-credential-manager.exe"));
                }
            }
            catch { }

            var pathVar = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
            foreach (var dir in pathVar.Split(Path.PathSeparator))
            {
                if (string.IsNullOrWhiteSpace(dir)) continue;
                try { candidates.Add(Path.Combine(dir.Trim(), "git-credential-manager.exe")); } catch { }
            }

            foreach (var candidate in candidates)
            {
                try { if (File.Exists(candidate)) return candidate; } catch { }
            }
            return null;
        }

        /// <summary>The credential helper git would use, or null when none is configured.</summary>
        public static async Task<string> GetConfiguredAsync()
        {
            var value = await GitRepository.GetGlobalConfigAsync("credential.helper").ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(value)) return value.Trim();
            var system = await GitRepository.GetSystemConfigAsync("credential.helper").ConfigureAwait(false);
            return string.IsNullOrWhiteSpace(system) ? null : system.Trim();
        }

        /// <summary>Configures Git Credential Manager for the current user.</summary>
        public static Task EnableManagerAsync() => GitRepository.SetGlobalConfigAsync("credential.helper", ManagerValue);
    }
}
