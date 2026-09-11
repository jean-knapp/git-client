using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using ModernWinForms;

namespace GitClient.Services
{
    /// <summary>Reads the image files projects keep their icons in and scales them into a square.</summary>
    internal static class IconImages
    {
        /// <summary>
        /// An .ico (its largest frame), .png, .jpg, .bmp, .gif, .webp or .svg drawn to fit
        /// <paramref name="size"/> pixels square; null when the file is missing or unreadable.
        /// </summary>
        public static Bitmap Load(string file, int size)
        {
            try
            {
                if (string.IsNullOrEmpty(file) || size <= 0 || !File.Exists(file)) return null;
                switch (Path.GetExtension(file).ToLowerInvariant())
                {
                    case ".svg":
                        return SvgIconRenderer.Render(File.ReadAllText(file), size, size, Color.Black);
                    case ".ico":
                    case ".webp":
                        // WIC reads every frame of an icon, PNG-compressed ones included, and WebP.
                        return Fit(DecodeWithWic(file), size);
                    case ".png":
                    case ".jpg":
                    case ".jpeg":
                    case ".bmp":
                    case ".gif":
                        return Fit(DecodeWithGdi(file), size);
                    default:
                        return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void HighQuality(Graphics g)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
        }

        /// <summary>Draws the image centred into a new square bitmap and disposes the original.</summary>
        private static Bitmap Fit(Bitmap decoded, int size)
        {
            if (decoded == null) return null;
            using (decoded)
            {
                var result = new Bitmap(size, size, PixelFormat.Format32bppArgb);
                using (var g = Graphics.FromImage(result))
                {
                    HighQuality(g);
                    float scale = Math.Min((float)size / decoded.Width, (float)size / decoded.Height);
                    float width = decoded.Width * scale, height = decoded.Height * scale;
                    g.DrawImage(decoded, new RectangleF((size - width) / 2f, (size - height) / 2f, width, height));
                }
                return result;
            }
        }

        private static Bitmap DecodeWithGdi(string file)
        {
            // Read into memory first, so the file is not held open by GDI+.
            using (var stream = new MemoryStream(File.ReadAllBytes(file)))
            using (var image = Image.FromStream(stream))
            {
                return new Bitmap(image);
            }
        }

        /// <summary>Decodes through the Windows imaging codecs, taking the largest frame.</summary>
        private static Bitmap DecodeWithWic(string file)
        {
            using (var stream = new MemoryStream(File.ReadAllBytes(file)))
            {
                var decoder = System.Windows.Media.Imaging.BitmapDecoder.Create(stream,
                    System.Windows.Media.Imaging.BitmapCreateOptions.PreservePixelFormat, System.Windows.Media.Imaging.BitmapCacheOption.OnLoad);
                System.Windows.Media.Imaging.BitmapSource largest = null;
                foreach (var frame in decoder.Frames)
                {
                    if (largest == null || frame.PixelWidth * frame.PixelHeight > largest.PixelWidth * largest.PixelHeight) largest = frame;
                }
                if (largest == null) return null;
                var converted = new System.Windows.Media.Imaging.FormatConvertedBitmap(largest, System.Windows.Media.PixelFormats.Bgra32, null, 0);
                int width = converted.PixelWidth, height = converted.PixelHeight;
                var bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                var data = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
                try
                {
                    converted.CopyPixels(System.Windows.Int32Rect.Empty, data.Scan0, data.Stride * height, data.Stride);
                }
                finally
                {
                    bitmap.UnlockBits(data);
                }
                return bitmap;
            }
        }
    }
}
