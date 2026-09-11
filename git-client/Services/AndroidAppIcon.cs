using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Xml;
using ModernWinForms;

namespace GitClient.Services
{
    /// <summary>
    /// Finds the launcher icon of an Android app in a working copy. The application module's manifest
    /// names the icon (android:icon, or @mipmap/ic_launcher when it does not); its res folders hold it
    /// as a PNG or WebP per screen density, or as XML - usually an adaptive icon made of a background
    /// colour and a vector drawable.
    /// </summary>
    public static class AndroidAppIcon
    {
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";
        private const string DefaultIcon = "@mipmap/ic_launcher";
        private const int MaxModuleDepth = 3;
        private const int MaxReferenceDepth = 10;

        // An adaptive icon's layers are 108dp, of which the launcher shows the middle 72dp.
        private const float AdaptiveLayerDp = 108f;
        private const float AdaptiveVisibleDp = 72f;

        private static readonly HashSet<string> SkippedFolders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".git", ".gradle", ".idea", ".kotlin", ".cxx", ".dart_tool", "build", "node_modules", "Pods", "bin", "obj", "src",
        };

        private static readonly Dictionary<string, int> Densities = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            { "ldpi", 120 }, { "mdpi", 160 }, { "tvdpi", 213 }, { "hdpi", 240 }, { "xhdpi", 320 }, { "xxhdpi", 480 }, { "xxxhdpi", 640 }, { "nodpi", 100 },
        };

        /// <summary>
        /// The app icon rendered at <paramref name="size"/> pixels square; null when the repository holds
        /// no Android app or its icon cannot be read. Never throws. The caller owns the bitmap.
        /// </summary>
        public static Bitmap Load(string repositoryRoot, int size)
        {
            try
            {
                if (string.IsNullOrEmpty(repositoryRoot) || size <= 0 || !Directory.Exists(repositoryRoot)) return null;
                var modules = new List<Module>();
                FindModules(repositoryRoot, 0, modules);
                foreach (var module in modules.OrderByDescending(m => m.Score).ThenBy(m => m.Directory.Length))
                {
                    var resources = new Resources(module.ResDirectories);
                    var reference = IconReference(module.Manifest);
                    var icon = resources.Render(reference, size, 0)
                        ?? (reference != DefaultIcon ? resources.Render(DefaultIcon, size, 0) : null);
                    if (icon != null) return icon;
                }
            }
            catch (Exception)
            {
                // A project we cannot make sense of simply has no icon.
            }
            return null;
        }

        // ------------------------------------------------------------------ modules

        private sealed class Module
        {
            public string Directory;
            public string Manifest;
            public readonly List<string> ResDirectories = new List<string>();
            public int Score;
        }

        /// <summary>
        /// Gradle modules (src/main/AndroidManifest.xml) and old Eclipse projects (AndroidManifest.xml
        /// beside res) near the top of the repository, e.g. app/ or android/app/ in a Flutter project.
        /// </summary>
        private static void FindModules(string directory, int depth, List<Module> modules)
        {
            var gradleManifest = Path.Combine(directory, "src", "main", "AndroidManifest.xml");
            var eclipseManifest = Path.Combine(directory, "AndroidManifest.xml");
            if (File.Exists(gradleManifest)) modules.Add(Describe(directory, gradleManifest, true));
            else if (File.Exists(eclipseManifest) && Directory.Exists(Path.Combine(directory, "res"))) modules.Add(Describe(directory, eclipseManifest, false));

            if (depth >= MaxModuleDepth) return;
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
                FindModules(child, depth + 1, modules);
            }
        }

        /// <summary>Where a module keeps its resources, and how likely it is to be the app rather than a library.</summary>
        private static Module Describe(string directory, string manifest, bool gradle)
        {
            var module = new Module { Directory = directory, Manifest = manifest };
            if (gradle)
            {
                var sourceSets = Path.Combine(directory, "src");
                module.ResDirectories.Add(Path.Combine(sourceSets, "main", "res"));
                // Build types and flavours may carry an icon of their own; main comes first.
                try
                {
                    module.ResDirectories.AddRange(Directory.GetDirectories(sourceSets)
                        .Where(d => !string.Equals(Path.GetFileName(d), "main", StringComparison.OrdinalIgnoreCase))
                        .OrderBy(d => d, StringComparer.OrdinalIgnoreCase)
                        .Select(d => Path.Combine(d, "res")));
                }
                catch (Exception)
                {
                }
            }
            else
            {
                module.ResDirectories.Add(Path.Combine(directory, "res"));
            }
            module.ResDirectories.RemoveAll(d => !Directory.Exists(d));

            var manifestText = ReadText(manifest);
            if (manifestText.Contains("android.intent.category.LAUNCHER")) module.Score += 4;
            foreach (var name in new[] { "build.gradle", "build.gradle.kts" })
            {
                var build = ReadText(Path.Combine(directory, name));
                // Matches the plugin id and version catalogue aliases such as libs.plugins.android.application.
                if (build.Contains("android.application")) module.Score += 3;
                if (build.Contains("android.library")) module.Score -= 3;
            }
            if (string.Equals(Path.GetFileName(directory), "app", StringComparison.OrdinalIgnoreCase)) module.Score += 1;
            return module;
        }

        /// <summary>The application's icon reference; the launcher default when it is missing or a build placeholder.</summary>
        private static string IconReference(string manifest)
        {
            try
            {
                var application = LoadXml(manifest).DocumentElement?.SelectSingleNode("application") as XmlElement;
                foreach (var attribute in new[] { "icon", "roundIcon" })
                {
                    var value = application?.GetAttribute(attribute, AndroidNamespace) ?? string.Empty;
                    if (value.StartsWith("@", StringComparison.Ordinal) && !value.StartsWith("@android:", StringComparison.Ordinal)) return value;
                }
            }
            catch (Exception)
            {
            }
            return DefaultIcon;
        }

        // ------------------------------------------------------------------ resources

        /// <summary>One module's resources: finds drawables and colours and renders them.</summary>
        private sealed class Resources
        {
            private readonly List<string> _resDirectories;
            private Dictionary<string, string> _colors;

            public Resources(List<string> resDirectories)
            {
                _resDirectories = resDirectories;
            }

            /// <summary>Renders an @type/name reference; null when it is not there or cannot be drawn.</summary>
            public Bitmap Render(string reference, int size, int depth)
            {
                if (depth > MaxReferenceDepth || !TryParseReference(reference, out var type, out var name)) return null;
                if (type == "color")
                {
                    var color = ResolveColor(reference, depth);
                    return color.HasValue ? Solid(color.Value, size) : null;
                }
                foreach (var file in Candidates(type, name))
                {
                    var bitmap = RenderFile(file, size, depth);
                    if (bitmap != null) return bitmap;
                }
                return null;
            }

            /// <summary>
            /// Files for a resource, per res folder: bitmaps from the highest density down, then XML,
            /// newest API level first. Night-mode variants are left out.
            /// </summary>
            private IEnumerable<string> Candidates(string type, string name)
            {
                foreach (var res in _resDirectories)
                {
                    string[] folders;
                    try
                    {
                        folders = Directory.GetDirectories(res);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    var rasters = new List<KeyValuePair<int, string>>();
                    var documents = new List<KeyValuePair<int, string>>();
                    foreach (var folder in folders)
                    {
                        var qualifiers = Path.GetFileName(folder).Split('-');
                        if (!string.Equals(qualifiers[0], type, StringComparison.Ordinal) || qualifiers.Contains("night")) continue;
                        int density = 160;
                        int version = 0;
                        foreach (var qualifier in qualifiers.Skip(1))
                        {
                            if (Densities.TryGetValue(qualifier, out var d)) density = d;
                            else if (qualifier == "anydpi") density = 10000;
                            else if (qualifier.Length > 1 && qualifier[0] == 'v' && int.TryParse(qualifier.Substring(1), out var v)) version = v;
                        }
                        foreach (var extension in new[] { ".png", ".webp", ".jpg", ".jpeg" })
                        {
                            var file = Path.Combine(folder, name + extension);
                            if (File.Exists(file)) rasters.Add(new KeyValuePair<int, string>(density, file));
                        }
                        var xml = Path.Combine(folder, name + ".xml");
                        if (File.Exists(xml)) documents.Add(new KeyValuePair<int, string>(density * 100 + version, xml));
                    }
                    foreach (var raster in rasters.OrderByDescending(r => r.Key)) yield return raster.Value;
                    foreach (var document in documents.OrderByDescending(d => d.Key)) yield return document.Value;
                }
            }

            private Bitmap RenderFile(string file, int size, int depth)
            {
                if (!file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)) return IconImages.Load(file, size);
                var root = LoadXml(file).DocumentElement;
                return root == null ? null : RenderElement(root, size, depth + 1);
            }

            private Bitmap RenderElement(XmlElement element, int size, int depth)
            {
                if (depth > MaxReferenceDepth) return null;
                switch (element.LocalName)
                {
                    case "adaptive-icon": return RenderAdaptive(element, size, depth);
                    case "vector": return RenderVector(element, size, depth);
                    case "bitmap": return RenderValue(Attribute(element, "src"), size, depth);
                    case "inset": return RenderInset(element, size, depth);
                    case "layer-list": return RenderLayers(element, size, depth);
                    case "shape": return RenderShape(element, size, depth);
                    case "color":
                        var color = ResolveColor(Attribute(element, "color"), depth);
                        return color.HasValue ? Solid(color.Value, size) : null;
                    case "selector":
                    case "level-list":
                    case "ripple":
                        var item = Children(element).FirstOrDefault(c => c.LocalName == "item");
                        return item == null ? null : RenderDrawableOf(item, size, depth);
                    default:
                        return null;
                }
            }

            /// <summary>A reference or a literal colour.</summary>
            private Bitmap RenderValue(string value, int size, int depth)
            {
                if (string.IsNullOrEmpty(value)) return null;
                if (value.StartsWith("#", StringComparison.Ordinal))
                {
                    var color = ParseColor(value);
                    return color.HasValue ? Solid(color.Value, size) : null;
                }
                return Render(value, size, depth + 1);
            }

            /// <summary>An element's android:drawable, or the drawable written inside it.</summary>
            private Bitmap RenderDrawableOf(XmlElement element, int size, int depth)
            {
                var reference = Attribute(element, "drawable");
                if (reference.Length > 0) return RenderValue(reference, size, depth);
                var inline = Children(element).FirstOrDefault();
                return inline == null ? null : RenderElement(inline, size, depth + 1);
            }

            /// <summary>Background under foreground, cropped to the visible middle and masked like a launcher's rounded square.</summary>
            private Bitmap RenderAdaptive(XmlElement icon, int size, int depth)
            {
                int layer = (int)Math.Round(size * AdaptiveLayerDp / AdaptiveVisibleDp);
                using (var composite = new Bitmap(layer, layer, PixelFormat.Format32bppArgb))
                {
                    bool drawn = false;
                    using (var g = Graphics.FromImage(composite))
                    {
                        HighQuality(g);
                        foreach (var part in new[] { "background", "foreground" })
                        {
                            var element = Children(icon).FirstOrDefault(c => c.LocalName == part);
                            if (element == null) continue;
                            using (var image = RenderDrawableOf(element, layer, depth))
                            {
                                if (image == null) continue;
                                g.DrawImage(image, new Rectangle(0, 0, layer, layer));
                                drawn = true;
                            }
                        }
                    }
                    if (!drawn) return null;

                    var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                    using (var g = Graphics.FromImage(result))
                    using (var brush = new TextureBrush(composite, WrapMode.Clamp))
                    using (var mask = RoundedSquare(size, size * 0.22f))
                    {
                        HighQuality(g);
                        float offset = (layer - size) / 2f;
                        brush.TranslateTransform(-offset, -offset);
                        g.FillPath(brush, mask);
                    }
                    return result;
                }
            }

            private Bitmap RenderVector(XmlElement vector, int size, int depth)
            {
                var svg = new SvgBuilder();
                float viewportWidth = ParseFloat(Attribute(vector, "viewportWidth"), 24f);
                float viewportHeight = ParseFloat(Attribute(vector, "viewportHeight"), viewportWidth);
                float alpha = ParseFloat(Attribute(vector, "alpha"), 1f);
                AppendVectorChildren(vector, svg, depth);
                var markup = new StringBuilder();
                markup.Append("<svg xmlns=\"http://www.w3.org/2000/svg\" viewBox=\"0 0 ").Append(Number(viewportWidth)).Append(' ').Append(Number(viewportHeight)).Append("\">");
                if (svg.Definitions.Length > 0) markup.Append("<defs>").Append(svg.Definitions).Append("</defs>");
                markup.Append("<g opacity=\"").Append(Number(alpha)).Append("\">").Append(svg.Body).Append("</g></svg>");
                return SvgIconRenderer.Render(markup.ToString(), size, size, Color.Black);
            }

            private sealed class SvgBuilder
            {
                public readonly StringBuilder Definitions = new StringBuilder();
                public readonly StringBuilder Body = new StringBuilder();
                public int NextId;
            }

            private void AppendVectorChildren(XmlElement parent, SvgBuilder svg, int depth)
            {
                foreach (var child in Children(parent))
                {
                    if (child.LocalName == "group")
                    {
                        // Android turns and scales a group about its pivot, then moves it.
                        float pivotX = ParseFloat(Attribute(child, "pivotX"), 0f);
                        float pivotY = ParseFloat(Attribute(child, "pivotY"), 0f);
                        float translateX = ParseFloat(Attribute(child, "translateX"), 0f);
                        float translateY = ParseFloat(Attribute(child, "translateY"), 0f);
                        float rotation = ParseFloat(Attribute(child, "rotation"), 0f);
                        float scaleX = ParseFloat(Attribute(child, "scaleX"), 1f);
                        float scaleY = ParseFloat(Attribute(child, "scaleY"), 1f);
                        svg.Body.Append("<g transform=\"translate(").Append(Number(translateX + pivotX)).Append(' ').Append(Number(translateY + pivotY))
                            .Append(") rotate(").Append(Number(rotation)).Append(") scale(").Append(Number(scaleX)).Append(' ').Append(Number(scaleY))
                            .Append(") translate(").Append(Number(-pivotX)).Append(' ').Append(Number(-pivotY)).Append(")\">");
                        AppendVectorChildren(child, svg, depth);
                        svg.Body.Append("</g>");
                    }
                    else if (child.LocalName == "path")
                    {
                        AppendPath(child, svg, depth);
                    }
                    // clip-path has no counterpart in the icon renderer; the shapes draw unclipped.
                }
            }

            private void AppendPath(XmlElement path, SvgBuilder svg, int depth)
            {
                var data = Attribute(path, "pathData");
                if (data.Length == 0) return;
                var fill = Paint(path, "fillColor", svg, depth);
                var stroke = Paint(path, "strokeColor", svg, depth);
                float strokeWidth = ParseFloat(Attribute(path, "strokeWidth"), 0f);

                svg.Body.Append("<path d=\"").Append(SecurityElement.Escape(data)).Append('"');
                svg.Body.Append(" fill=\"").Append(fill ?? "none").Append('"');
                svg.Body.Append(" fill-opacity=\"").Append(Number(ParseFloat(Attribute(path, "fillAlpha"), 1f))).Append('"');
                if (Attribute(path, "fillType") == "evenOdd") svg.Body.Append(" fill-rule=\"evenodd\"");
                if (stroke != null && strokeWidth > 0)
                {
                    svg.Body.Append(" stroke=\"").Append(stroke).Append("\" stroke-width=\"").Append(Number(strokeWidth)).Append('"');
                    svg.Body.Append(" stroke-opacity=\"").Append(Number(ParseFloat(Attribute(path, "strokeAlpha"), 1f))).Append('"');
                    var cap = Attribute(path, "strokeLineCap");
                    var join = Attribute(path, "strokeLineJoin");
                    if (cap.Length > 0) svg.Body.Append(" stroke-linecap=\"").Append(cap).Append('"');
                    if (join.Length > 0) svg.Body.Append(" stroke-linejoin=\"").Append(join).Append('"');
                }
                svg.Body.Append("/>");
            }

            /// <summary>An SVG paint for a colour attribute, or for the gradient given as an aapt:attr child.</summary>
            private string Paint(XmlElement path, string attribute, SvgBuilder svg, int depth)
            {
                var value = Attribute(path, attribute);
                if (value.Length > 0)
                {
                    var color = ResolveColor(value, depth);
                    return color.HasValue ? SvgColor(color.Value) : null;
                }
                var holder = Children(path).FirstOrDefault(c => c.LocalName == "attr" && c.GetAttribute("name") == "android:" + attribute);
                var gradient = holder == null ? null : Children(holder).FirstOrDefault(c => c.LocalName == "gradient");
                return gradient == null ? null : AppendGradient(gradient, svg, depth);
            }

            private string AppendGradient(XmlElement gradient, SvgBuilder svg, int depth)
            {
                var stops = new List<KeyValuePair<float, Color>>();
                foreach (var item in Children(gradient).Where(c => c.LocalName == "item"))
                {
                    var color = ResolveColor(Attribute(item, "color"), depth);
                    if (color.HasValue) stops.Add(new KeyValuePair<float, Color>(ParseFloat(Attribute(item, "offset"), 0f), color.Value));
                }
                if (stops.Count == 0)
                {
                    var start = ResolveColor(Attribute(gradient, "startColor"), depth);
                    var center = ResolveColor(Attribute(gradient, "centerColor"), depth);
                    var end = ResolveColor(Attribute(gradient, "endColor"), depth);
                    if (start.HasValue) stops.Add(new KeyValuePair<float, Color>(0f, start.Value));
                    if (center.HasValue) stops.Add(new KeyValuePair<float, Color>(0.5f, center.Value));
                    if (end.HasValue) stops.Add(new KeyValuePair<float, Color>(1f, end.Value));
                }
                if (stops.Count == 0) return null;

                var type = Attribute(gradient, "type");
                // A sweep gradient has no SVG form; its first colour stands in.
                if (type == "sweep") return SvgColor(stops[0].Value);

                var id = "g" + svg.NextId++;
                if (type == "radial")
                {
                    svg.Definitions.Append("<radialGradient id=\"").Append(id).Append("\" gradientUnits=\"userSpaceOnUse\" cx=\"").Append(Number(ParseFloat(Attribute(gradient, "centerX"), 0f)))
                        .Append("\" cy=\"").Append(Number(ParseFloat(Attribute(gradient, "centerY"), 0f)))
                        .Append("\" r=\"").Append(Number(ParseFloat(Attribute(gradient, "gradientRadius"), 1f))).Append("\">");
                }
                else
                {
                    svg.Definitions.Append("<linearGradient id=\"").Append(id).Append("\" gradientUnits=\"userSpaceOnUse\" x1=\"").Append(Number(ParseFloat(Attribute(gradient, "startX"), 0f)))
                        .Append("\" y1=\"").Append(Number(ParseFloat(Attribute(gradient, "startY"), 0f)))
                        .Append("\" x2=\"").Append(Number(ParseFloat(Attribute(gradient, "endX"), 0f)))
                        .Append("\" y2=\"").Append(Number(ParseFloat(Attribute(gradient, "endY"), 0f))).Append("\">");
                }
                foreach (var stop in stops)
                {
                    svg.Definitions.Append("<stop offset=\"").Append(Number(stop.Key)).Append("\" stop-color=\"").Append(SvgColor(stop.Value)).Append("\"/>");
                }
                svg.Definitions.Append(type == "radial" ? "</radialGradient>" : "</linearGradient>");
                return "url(#" + id + ")";
            }

            private Bitmap RenderInset(XmlElement inset, int size, int depth)
            {
                float all = Length(Attribute(inset, "inset"), size, 0f);
                float left = Length(Attribute(inset, "insetLeft"), size, all);
                float top = Length(Attribute(inset, "insetTop"), size, all);
                float right = Length(Attribute(inset, "insetRight"), size, all);
                float bottom = Length(Attribute(inset, "insetBottom"), size, all);
                return DrawInto(RenderDrawableOf(inset, size, depth), size, left, top, right, bottom);
            }

            private Bitmap RenderLayers(XmlElement list, int size, int depth)
            {
                var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                bool drawn = false;
                using (var g = Graphics.FromImage(result))
                {
                    HighQuality(g);
                    foreach (var item in Children(list).Where(c => c.LocalName == "item"))
                    {
                        float left = Length(Attribute(item, "left"), size, 0f);
                        float top = Length(Attribute(item, "top"), size, 0f);
                        float right = Length(Attribute(item, "right"), size, 0f);
                        float bottom = Length(Attribute(item, "bottom"), size, 0f);
                        using (var image = RenderDrawableOf(item, size, depth))
                        {
                            if (image == null) continue;
                            g.DrawImage(image, RectangleF.FromLTRB(left, top, size - right, size - bottom));
                            drawn = true;
                        }
                    }
                }
                if (drawn) return result;
                result.Dispose();
                return null;
            }

            private Bitmap RenderShape(XmlElement shape, int size, int depth)
            {
                var solid = Children(shape).FirstOrDefault(c => c.LocalName == "solid");
                var gradient = Children(shape).FirstOrDefault(c => c.LocalName == "gradient");
                var stroke = Children(shape).FirstOrDefault(c => c.LocalName == "stroke");
                var corners = Children(shape).FirstOrDefault(c => c.LocalName == "corners");
                var fillColor = solid == null ? null : ResolveColor(Attribute(solid, "color"), depth);
                var startColor = gradient == null ? null : ResolveColor(Attribute(gradient, "startColor"), depth);
                var endColor = gradient == null ? null : ResolveColor(Attribute(gradient, "endColor"), depth);
                if (!fillColor.HasValue && !startColor.HasValue && stroke == null) return null;

                var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(result))
                using (var outline = new GraphicsPath())
                {
                    HighQuality(g);
                    var bounds = new RectangleF(0, 0, size, size);
                    if (Attribute(shape, "shape") == "oval") outline.AddEllipse(bounds);
                    else
                    {
                        float radius = corners == null ? 0f : Length(Attribute(corners, "radius"), size, 0f);
                        if (radius <= 0) outline.AddRectangle(bounds);
                        else outline.AddPath(RoundedSquare(size, radius), false);
                    }
                    if (startColor.HasValue)
                    {
                        float angle = ParseFloat(Attribute(gradient, "angle"), 0f);
                        using (var brush = new LinearGradientBrush(bounds, startColor.Value, endColor ?? startColor.Value, -angle))
                        {
                            g.FillPath(brush, outline);
                        }
                    }
                    else if (fillColor.HasValue)
                    {
                        using (var brush = new SolidBrush(fillColor.Value)) g.FillPath(brush, outline);
                    }
                    var strokeColor = stroke == null ? null : ResolveColor(Attribute(stroke, "color"), depth);
                    if (strokeColor.HasValue)
                    {
                        using (var pen = new Pen(strokeColor.Value, Math.Max(1f, Length(Attribute(stroke, "width"), size, 1f)))) g.DrawPath(pen, outline);
                    }
                }
                return result;
            }

            // ------------------------------------------------------------------ colours

            /// <summary>A literal colour, an @color resource (followed through aliases and colour state lists) or an Android system colour.</summary>
            private Color? ResolveColor(string value, int depth)
            {
                if (string.IsNullOrEmpty(value) || depth > MaxReferenceDepth) return null;
                if (value.StartsWith("#", StringComparison.Ordinal)) return ParseColor(value);
                if (value.StartsWith("@android:color/", StringComparison.Ordinal))
                {
                    switch (value.Substring("@android:color/".Length))
                    {
                        case "white": return Color.White;
                        case "black": return Color.Black;
                        case "transparent": return Color.Transparent;
                        default: return null;
                    }
                }
                if (!TryParseReference(value, out var type, out var name) || type != "color") return null;
                if (Colors().TryGetValue(name, out var defined)) return ResolveColor(defined, depth + 1);

                // res/color/name.xml is a colour state list; its first item is the default look.
                foreach (var file in Candidates("color", name).Where(f => f.EndsWith(".xml", StringComparison.OrdinalIgnoreCase)))
                {
                    var item = Children(LoadXml(file).DocumentElement).FirstOrDefault(c => c.LocalName == "item");
                    var color = item == null ? null : ResolveColor(Attribute(item, "color"), depth + 1);
                    if (color.HasValue) return color;
                }
                return null;
            }

            /// <summary>Every &lt;color&gt; in the values folders; the main source set's win, night variants are skipped.</summary>
            private Dictionary<string, string> Colors()
            {
                if (_colors != null) return _colors;
                _colors = new Dictionary<string, string>(StringComparer.Ordinal);
                foreach (var res in _resDirectories)
                {
                    IEnumerable<string> folders;
                    try
                    {
                        folders = Directory.GetDirectories(res, "values*").Where(f => !Path.GetFileName(f).Split('-').Contains("night")).OrderBy(f => f.Length).ToList();
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    foreach (var folder in folders)
                    {
                        foreach (var file in SafeFiles(folder, "*.xml"))
                        {
                            XmlDocument document;
                            try
                            {
                                document = LoadXml(file);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                            foreach (var color in Children(document.DocumentElement).Where(c => c.LocalName == "color"))
                            {
                                var name = color.GetAttribute("name");
                                if (name.Length > 0 && !_colors.ContainsKey(name)) _colors[name] = color.InnerText.Trim();
                            }
                        }
                    }
                }
                return _colors;
            }
        }

        // ------------------------------------------------------------------ helpers

        private static bool TryParseReference(string value, out string type, out string name)
        {
            type = name = null;
            if (string.IsNullOrEmpty(value) || value[0] != '@' || value.StartsWith("@android:", StringComparison.Ordinal)) return false;
            var body = value.Substring(1);
            int colon = body.IndexOf(':');
            if (colon >= 0) body = body.Substring(colon + 1);
            int slash = body.IndexOf('/');
            if (slash <= 0 || slash == body.Length - 1) return false;
            type = body.Substring(0, slash);
            name = body.Substring(slash + 1);
            return true;
        }

        /// <summary>#RGB, #ARGB, #RRGGBB or #AARRGGBB.</summary>
        private static Color? ParseColor(string value)
        {
            var hex = value.Trim().TrimStart('#');
            if (hex.Length == 3 || hex.Length == 4) hex = string.Concat(hex.Select(c => new string(c, 2)));
            if (hex.Length == 6) hex = "FF" + hex;
            if (hex.Length != 8 || !uint.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var argb)) return null;
            return Color.FromArgb(unchecked((int)argb));
        }

        private static string SvgColor(Color color) => "#" + color.R.ToString("x2") + color.G.ToString("x2") + color.B.ToString("x2") + color.A.ToString("x2");

        private static string Number(float value) => value.ToString("0.####", CultureInfo.InvariantCulture);

        private static float ParseFloat(string value, float fallback)
        {
            if (string.IsNullOrEmpty(value)) return fallback;
            return float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ? parsed : fallback;
        }

        /// <summary>
        /// A dimension in pixels of a drawable <paramref name="size"/> wide: "25%", or dp measured against
        /// the 108dp an adaptive icon layer spans, which is where these drawables usually sit.
        /// </summary>
        private static float Length(string value, int size, float fallback)
        {
            if (string.IsNullOrEmpty(value)) return fallback;
            value = value.Trim();
            if (value.EndsWith("%", StringComparison.Ordinal)) return ParseFloat(value.TrimEnd('%'), float.NaN) is var percent && !float.IsNaN(percent) ? size * percent / 100f : fallback;
            foreach (var unit in new[] { "dip", "dp", "px", "sp" })
            {
                if (!value.EndsWith(unit, StringComparison.Ordinal)) continue;
                var amount = ParseFloat(value.Substring(0, value.Length - unit.Length), float.NaN);
                if (float.IsNaN(amount)) return fallback;
                return unit == "px" ? amount : size * amount / AdaptiveLayerDp;
            }
            var fraction = ParseFloat(value, float.NaN);
            return float.IsNaN(fraction) ? fallback : (fraction <= 1f ? size * fraction : fraction);
        }

        private static string Attribute(XmlElement element, string name) => element?.GetAttribute(name, AndroidNamespace) ?? string.Empty;

        private static IEnumerable<XmlElement> Children(XmlElement element) =>
            element == null ? Enumerable.Empty<XmlElement>() : element.ChildNodes.OfType<XmlElement>();

        private static XmlDocument LoadXml(string path)
        {
            var document = new XmlDocument { XmlResolver = null };
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore, XmlResolver = null };
            using (var reader = XmlReader.Create(path, settings)) document.Load(reader);
            return document;
        }

        private static string ReadText(string path)
        {
            try
            {
                return File.Exists(path) ? File.ReadAllText(path) : string.Empty;
            }
            catch (Exception)
            {
                return string.Empty;
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

        private static void HighQuality(Graphics g) => IconImages.HighQuality(g);

        private static Bitmap Solid(Color color, int size)
        {
            var bitmap = new Bitmap(size, size, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(bitmap)) g.Clear(color);
            return bitmap;
        }

        private static GraphicsPath RoundedSquare(int size, float radius)
        {
            var path = new GraphicsPath();
            float d = Math.Min(radius * 2, size);
            path.AddArc(0, 0, d, d, 180, 90);
            path.AddArc(size - d, 0, d, d, 270, 90);
            path.AddArc(size - d, size - d, d, d, 0, 90);
            path.AddArc(0, size - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        /// <summary>Draws <paramref name="image"/> into the inset rectangle of a new bitmap and disposes it.</summary>
        private static Bitmap DrawInto(Bitmap image, int size, float left, float top, float right, float bottom)
        {
            if (image == null) return null;
            using (image)
            {
                var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(result))
                {
                    HighQuality(g);
                    g.DrawImage(image, RectangleF.FromLTRB(left, top, size - right, size - bottom));
                }
                return result;
            }
        }
    }
}
