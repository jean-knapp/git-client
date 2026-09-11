using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace GitClient.Services
{
    /// <summary>
    /// Finds the icon of a PHP project in a working copy: a favicon.ico at the root or in the web root
    /// (public/, public_html/, web/ …), else the nearest other .ico file. Dependencies, uploads and
    /// caches are not searched.
    /// </summary>
    public static class PhpProjectIcon
    {
        private const int MaxDepth = 4;
        private const int MaxFolders = 2000;

        /// <summary>Where a PHP site's document root usually is, the repository root first.</summary>
        private static readonly string[] WebRoots = { "", "public", "public_html", "web", "htdocs", "httpdocs", "www", "html", "wwwroot" };

        private static readonly HashSet<string> SkippedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".idea", ".vscode", "vendor", "node_modules", "bower_components", "storage", "cache", "tmp", "temp", "logs", "uploads", "tests", "test",
        };

        /// <summary>The icon at <paramref name="size"/> pixels square, or null. Never throws; the caller owns the bitmap.</summary>
        public static Bitmap Load(string repositoryRoot, int size)
        {
            try
            {
                if (string.IsNullOrEmpty(repositoryRoot) || size <= 0 || !Directory.Exists(repositoryRoot)) return null;
                var root = Path.GetFullPath(repositoryRoot).TrimEnd('\\', '/');
                if (!IsPhpProject(root)) return null;
                foreach (var file in Candidates(root))
                {
                    var icon = IconImages.Load(file, size);
                    if (icon != null) return icon;
                }
            }
            catch (Exception)
            {
                // A folder we cannot read simply has no icon.
            }
            return null;
        }

        /// <summary>A composer.json, or .php files within two folders of the root.</summary>
        private static bool IsPhpProject(string root)
        {
            if (File.Exists(Path.Combine(root, "composer.json"))) return true;
            int visited = 0;
            return HasPhpFiles(root, 0, ref visited);
        }

        private static bool HasPhpFiles(string folder, int depth, ref int visited)
        {
            if (++visited > MaxFolders) return false;
            if (SafeFiles(folder, "*.php").Any()) return true;
            if (depth >= 2) return false;
            foreach (var child in SafeFolders(folder))
            {
                if (HasPhpFiles(child, depth + 1, ref visited)) return true;
            }
            return false;
        }

        /// <summary>favicon.ico where a browser asks for it, then every other .ico: favicon.ico names first, nearest first.</summary>
        private static IEnumerable<string> Candidates(string root)
        {
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var webRoot in WebRoots)
            {
                var favicon = Path.Combine(root, webRoot, "favicon.ico");
                if (File.Exists(favicon) && seen.Add(favicon)) yield return favicon;
            }

            var found = new List<KeyValuePair<int, string>>();
            int visited = 0;
            FindIcons(root, 0, found, ref visited);
            var ordered = found
                .OrderBy(f => string.Equals(Path.GetFileName(f.Value), "favicon.ico", StringComparison.OrdinalIgnoreCase) ? 0 : 1)
                .ThenBy(f => f.Key)
                .ThenBy(f => f.Value, StringComparer.OrdinalIgnoreCase);
            foreach (var file in ordered)
            {
                if (seen.Add(file.Value)) yield return file.Value;
            }
        }

        private static void FindIcons(string folder, int depth, List<KeyValuePair<int, string>> found, ref int visited)
        {
            if (++visited > MaxFolders) return;
            foreach (var file in SafeFiles(folder, "*.ico")) found.Add(new KeyValuePair<int, string>(depth, file));
            if (depth >= MaxDepth) return;
            foreach (var child in SafeFolders(folder)) FindIcons(child, depth + 1, found, ref visited);
        }

        private static IEnumerable<string> SafeFolders(string folder)
        {
            try
            {
                return Directory.GetDirectories(folder).Where(d => !SkippedFolders.Contains(Path.GetFileName(d))).OrderBy(d => d, StringComparer.OrdinalIgnoreCase).ToList();
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }

        private static IEnumerable<string> SafeFiles(string folder, string pattern)
        {
            try
            {
                return Directory.GetFiles(folder, pattern);
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}
