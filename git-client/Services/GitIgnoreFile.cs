using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GitClient.Services
{
    /// <summary>Reads, writes and appends to a repository's root .gitignore.</summary>
    public static class GitIgnoreFile
    {
        public static string PathFor(string workingDirectory) => Path.Combine(workingDirectory, ".gitignore");

        public static string Read(string workingDirectory)
        {
            var path = PathFor(workingDirectory);
            return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
        }

        public static void Write(string workingDirectory, string content)
        {
            File.WriteAllText(PathFor(workingDirectory), Normalize(content));
        }

        /// <summary>
        /// Appends the patterns that are not already listed and returns how many were written.
        /// </summary>
        public static int Append(string workingDirectory, IEnumerable<string> patterns)
        {
            var existing = Read(workingDirectory);
            var known = new HashSet<string>(Lines(existing).Select(l => l.Trim()).Where(l => l.Length > 0), StringComparer.Ordinal);
            var wanted = (patterns ?? Enumerable.Empty<string>())
                .Select(p => (p ?? string.Empty).Trim())
                .Where(p => p.Length > 0 && !known.Contains(p))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            if (wanted.Count == 0) return 0;

            var text = existing;
            if (text.Length > 0 && !text.EndsWith("\n", StringComparison.Ordinal)) text += Environment.NewLine;
            text += string.Join(Environment.NewLine, wanted) + Environment.NewLine;
            Write(workingDirectory, text);
            return wanted.Count;
        }

        /// <summary>Is this pattern already a line of the file?</summary>
        public static bool Contains(string workingDirectory, string pattern)
        {
            var wanted = (pattern ?? string.Empty).Trim();
            if (wanted.Length == 0) return false;
            return Lines(Read(workingDirectory)).Any(l => string.Equals(l.Trim(), wanted, StringComparison.Ordinal));
        }

        /// <summary>The rule that ignores one file, anchored at the repository root.</summary>
        public static string FileRule(string repoRelativePath) => "/" + Trim(repoRelativePath);

        /// <summary>The rule that ignores a directory and everything under it.</summary>
        public static string DirectoryRule(string repoRelativeDirectory) => "/" + Trim(repoRelativeDirectory) + "/";

        /// <summary>The rule that ignores every file sharing this one's extension.</summary>
        public static string ExtensionRule(string repoRelativePath)
        {
            var extension = Path.GetExtension(repoRelativePath ?? string.Empty);
            return extension.Length > 1 ? "*" + extension : null;
        }

        /// <summary>
        /// The directories between a file and the repository root, nearest first
        /// ("src/app/views/x.cs" gives "src/app/views", "src/app", "src").
        /// </summary>
        public static List<string> AncestorDirectories(string repoRelativePath)
        {
            var result = new List<string>();
            var parts = Trim(repoRelativePath).Split('/');
            for (int count = parts.Length - 1; count > 0; count--)
            {
                result.Add(string.Join("/", parts.Take(count)));
            }
            return result;
        }

        private static string Trim(string path) => (path ?? string.Empty).Replace('\\', '/').Trim('/');

        private static IEnumerable<string> Lines(string text) =>
            (text ?? string.Empty).Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

        /// <summary>git is happiest with LF, and so is every editor that later opens the file.</summary>
        private static string Normalize(string content) =>
            (content ?? string.Empty).Replace("\r\n", "\n").Replace("\r", "\n");
    }
}
