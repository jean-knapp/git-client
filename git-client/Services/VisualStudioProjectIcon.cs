using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace GitClient.Services
{
    /// <summary>
    /// Finds the icon of a Visual Studio project in a working copy: the ApplicationIcon a project file
    /// names, a .NET MAUI app's MauiIcon, a packaged app's logo from its Package.appxmanifest, or a web
    /// project's favicon. Applications are tried before libraries, and test projects last.
    /// </summary>
    public static class VisualStudioProjectIcon
    {
        private const int MaxDepth = 3;

        private static readonly HashSet<string> SkippedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".vs", ".idea", "bin", "obj", "packages", "node_modules", "TestResults", "artifacts",
        };

        private static readonly string[] ProjectExtensions = { ".csproj", ".vbproj", ".fsproj" };

        /// <summary>The icon at <paramref name="size"/> pixels square, or null. Never throws; the caller owns the bitmap.</summary>
        public static Bitmap Load(string repositoryRoot, int size)
        {
            try
            {
                if (string.IsNullOrEmpty(repositoryRoot) || size <= 0 || !Directory.Exists(repositoryRoot)) return null;
                var root = Path.GetFullPath(repositoryRoot).TrimEnd('\\', '/');
                var projects = new List<Project>();
                FindProjects(root, 0, projects);
                var folderName = Path.GetFileName(root);
                var ordered = projects
                    .OrderByDescending(p => p.Rank + (string.Equals(p.Name, folderName, StringComparison.OrdinalIgnoreCase) ? 1 : 0))
                    .ThenBy(p => p.Depth)
                    .ThenBy(p => p.File, StringComparer.OrdinalIgnoreCase);
                foreach (var project in ordered)
                {
                    var icon = LoadProjectIcon(project, root, size);
                    if (icon != null) return icon;
                }
            }
            catch (Exception)
            {
                // A project file we cannot make sense of simply has no icon.
            }
            return null;
        }

        // ------------------------------------------------------------------ projects

        private sealed class Project
        {
            public string File;
            public string Directory;
            public string Name;
            public int Depth;
            public XmlElement Root;

            /// <summary>Desktop and MAUI apps 3, console and web apps 2, libraries 0, test projects -5.</summary>
            public int Rank;
        }

        private static void FindProjects(string directory, int depth, List<Project> projects)
        {
            foreach (var file in SafeFiles(directory))
            {
                if (!ProjectExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase)) continue;
                var project = Describe(file, depth);
                if (project != null) projects.Add(project);
            }
            if (depth >= MaxDepth) return;
            string[] children;
            try
            {
                children = Directory.GetDirectories(directory);
            }
            catch (Exception)
            {
                return;
            }
            foreach (var child in children)
            {
                if (SkippedFolders.Contains(Path.GetFileName(child))) continue;
                FindProjects(child, depth + 1, projects);
            }
        }

        private static Project Describe(string file, int depth)
        {
            XmlElement root;
            try
            {
                root = LoadXml(file).DocumentElement;
            }
            catch (Exception)
            {
                return null;
            }
            if (root == null || root.LocalName != "Project") return null;

            var project = new Project
            {
                File = file,
                Directory = Path.GetDirectoryName(file),
                Name = Path.GetFileNameWithoutExtension(file),
                Depth = depth,
                Root = root,
            };
            var outputType = Property(root, "OutputType") ?? string.Empty;
            bool test = IsTrue(Property(root, "IsTestProject"))
                || Descendants(root, "PackageReference").Any(e => string.Equals(e.GetAttribute("Include"), "Microsoft.NET.Test.Sdk", StringComparison.OrdinalIgnoreCase))
                || Regex.IsMatch(project.Name, @"[.\-_]?Tests?$", RegexOptions.IgnoreCase);
            bool desktop = string.Equals(outputType, "WinExe", StringComparison.OrdinalIgnoreCase)
                || IsTrue(Property(root, "UseWindowsForms")) || IsTrue(Property(root, "UseWPF")) || IsTrue(Property(root, "UseMaui"))
                || Descendants(root, "MauiIcon").Any() || SafeFiles(project.Directory).Any(f => f.EndsWith(".appxmanifest", StringComparison.OrdinalIgnoreCase));
            bool console = string.Equals(outputType, "Exe", StringComparison.OrdinalIgnoreCase)
                || root.GetAttribute("Sdk").StartsWith("Microsoft.NET.Sdk.Web", StringComparison.OrdinalIgnoreCase);
            project.Rank = test ? -5 : desktop ? 3 : console ? 2 : 0;
            return project;
        }

        private static Bitmap LoadProjectIcon(Project project, string repositoryRoot, int size)
        {
            foreach (var element in Descendants(project.Root, "ApplicationIcon"))
            {
                var icon = IconImages.Load(ResolvePath(element.InnerText, project, repositoryRoot), size);
                if (icon != null) return icon;
            }
            foreach (var element in Descendants(project.Root, "MauiIcon"))
            {
                var icon = RenderMauiIcon(element, project, repositoryRoot, size);
                if (icon != null) return icon;
            }
            foreach (var manifest in SafeFiles(project.Directory).Where(f => f.EndsWith(".appxmanifest", StringComparison.OrdinalIgnoreCase)))
            {
                var icon = RenderPackageLogo(manifest, project, size);
                if (icon != null) return icon;
            }
            foreach (var name in new[] { "favicon.ico", "favicon.png", "favicon.svg" })
            {
                var icon = IconImages.Load(Path.Combine(project.Directory, "wwwroot", name), size);
                if (icon != null) return icon;
            }
            return null;
        }

        /// <summary>A path from a project file, with the usual directory properties filled in; null when it cannot be followed.</summary>
        private static string ResolvePath(string value, Project project, string repositoryRoot)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length == 0) return null;
            value = Regex.Replace(value, @"\$\((MSBuildProjectDirectory|MSBuildThisFileDirectory|ProjectDir|SolutionDir)\)", match =>
            {
                switch (match.Groups[1].Value)
                {
                    case "MSBuildProjectDirectory": return project.Directory;
                    case "SolutionDir": return repositoryRoot + "\\";
                    default: return project.Directory + "\\";
                }
            }, RegexOptions.IgnoreCase);
            if (value.Contains("$(") || value.Contains("%(") || value.Contains("*")) return null;
            value = value.Replace('/', '\\');
            try
            {
                return Path.GetFullPath(Path.IsPathRooted(value) ? value : Path.Combine(project.Directory, value));
            }
            catch (Exception)
            {
                return null;
            }
        }

        // ------------------------------------------------------------------ .NET MAUI

        /// <summary>
        /// MauiIcon: the Include image as the background, filled with Color first, and the ForegroundFile
        /// drawn over it at ForegroundScale.
        /// </summary>
        private static Bitmap RenderMauiIcon(XmlElement element, Project project, string repositoryRoot, int size)
        {
            var background = ResolvePath(Metadata(element, "Include"), project, repositoryRoot);
            var foreground = ResolvePath(Metadata(element, "ForegroundFile"), project, repositoryRoot);
            var color = ParseColor(Metadata(element, "Color"));
            float scale = float.TryParse(Metadata(element, "ForegroundScale"), NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) && parsed > 0 ? parsed : 1f;

            var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            bool drawn = false;
            using (var g = Graphics.FromImage(result))
            {
                IconImages.HighQuality(g);
                if (color.HasValue)
                {
                    g.Clear(color.Value);
                    drawn = true;
                }
                using (var image = IconImages.Load(background, size))
                {
                    if (image != null)
                    {
                        g.DrawImage(image, new Rectangle(0, 0, size, size));
                        drawn = true;
                    }
                }
                using (var image = IconImages.Load(foreground, size))
                {
                    if (image != null)
                    {
                        float side = size * scale;
                        g.DrawImage(image, new RectangleF((size - side) / 2f, (size - side) / 2f, side, side));
                        drawn = true;
                    }
                }
            }
            if (drawn) return result;
            result.Dispose();
            return null;
        }

        private static string Metadata(XmlElement element, string name)
        {
            var attribute = element.GetAttribute(name);
            if (attribute.Length > 0) return attribute;
            return element.ChildNodes.OfType<XmlElement>().FirstOrDefault(c => c.LocalName == name)?.InnerText.Trim() ?? string.Empty;
        }

        /// <summary>#RRGGBB, #AARRGGBB or a colour name.</summary>
        private static Color? ParseColor(string value)
        {
            value = (value ?? string.Empty).Trim();
            if (value.Length == 0) return null;
            if (value[0] == '#')
            {
                var hex = value.Substring(1);
                if (hex.Length == 6) hex = "FF" + hex;
                if (hex.Length == 8 && uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var argb)) return Color.FromArgb(unchecked((int)argb));
                return null;
            }
            var named = Color.FromName(value);
            return named.IsKnownColor ? named : (Color?)null;
        }

        // ------------------------------------------------------------------ packaged apps

        /// <summary>The Square44x44 or Square150x150 logo from the manifest's VisualElements, else the store logo.</summary>
        private static Bitmap RenderPackageLogo(string manifest, Project project, int size)
        {
            XmlElement root;
            try
            {
                root = LoadXml(manifest).DocumentElement;
            }
            catch (Exception)
            {
                return null;
            }
            var logos = new List<string>();
            foreach (var visual in Descendants(root, "VisualElements"))
            {
                logos.Add(visual.GetAttribute("Square44x44Logo"));
                logos.Add(visual.GetAttribute("Square150x150Logo"));
            }
            logos.AddRange(Descendants(root, "Logo").Select(e => e.InnerText.Trim()));
            foreach (var logo in logos.Where(l => !string.IsNullOrEmpty(l)).Distinct(StringComparer.OrdinalIgnoreCase))
            {
                var icon = IconImages.Load(ScaledAsset(project.Directory, logo), size);
                if (icon != null) return icon;
            }
            return null;
        }

        /// <summary>
        /// The best file for an asset the manifest names without its qualifiers, e.g.
        /// Square44x44Logo.targetsize-48_altform-unplated.png for Assets\Square44x44Logo.png: an
        /// unplated one of 32 px or more first (it has no coloured plate behind it), then the largest.
        /// </summary>
        private static string ScaledAsset(string projectDirectory, string relative)
        {
            string path;
            try
            {
                path = Path.GetFullPath(Path.Combine(projectDirectory, relative.Replace('/', '\\')));
            }
            catch (Exception)
            {
                return null;
            }
            if (File.Exists(path)) return path;
            var folder = Path.GetDirectoryName(path);
            var stem = Path.GetFileNameWithoutExtension(path);
            var extension = Path.GetExtension(path);
            if (!Directory.Exists(folder)) return null;

            var baseMatch = Regex.Match(stem, @"(\d+)x\d+");
            int baseSize = baseMatch.Success ? int.Parse(baseMatch.Groups[1].Value, CultureInfo.InvariantCulture) : 100;
            string best = null;
            int bestScore = int.MinValue;
            foreach (var file in SafeFiles(folder))
            {
                var name = Path.GetFileName(file);
                if (!name.StartsWith(stem + ".", StringComparison.OrdinalIgnoreCase) || !name.EndsWith(extension, StringComparison.OrdinalIgnoreCase)) continue;
                var qualifiers = name.Substring(stem.Length + 1, name.Length - stem.Length - 1 - extension.Length).ToLowerInvariant();
                if (qualifiers.Contains("contrast")) continue;
                int pixels = baseSize;
                foreach (var part in qualifiers.Split('_', '.'))
                {
                    if (part.StartsWith("targetsize-", StringComparison.Ordinal) && int.TryParse(part.Substring(11), out var target)) pixels = target;
                    else if (part.StartsWith("scale-", StringComparison.Ordinal) && int.TryParse(part.Substring(6), out var scale)) pixels = baseSize * scale / 100;
                }
                bool unplated = qualifiers.Contains("altform-unplated");
                int score = (unplated && pixels >= 32 ? 10000 : 0) + Math.Min(pixels, 512);
                if (score > bestScore)
                {
                    bestScore = score;
                    best = file;
                }
            }
            return best;
        }

        // ------------------------------------------------------------------ helpers

        private static string Property(XmlElement root, string name) =>
            Descendants(root, name).Select(e => e.InnerText.Trim()).FirstOrDefault(v => v.Length > 0);

        private static IEnumerable<XmlElement> Descendants(XmlElement root, string localName) =>
            root.GetElementsByTagName("*").OfType<XmlElement>().Where(e => e.LocalName == localName);

        private static bool IsTrue(string value) => string.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

        private static XmlDocument LoadXml(string path)
        {
            var document = new XmlDocument { XmlResolver = null };
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
            using (var reader = XmlReader.Create(path, settings)) document.Load(reader);
            return document;
        }

        private static IEnumerable<string> SafeFiles(string folder)
        {
            try
            {
                return Directory.GetFiles(folder);
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}
