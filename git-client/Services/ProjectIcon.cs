using System.Drawing;

namespace GitClient.Services
{
    /// <summary>
    /// A repository's own icon for its tab: an Android app's launcher icon, else a Visual Studio
    /// project's icon, else a PHP site's favicon.
    /// </summary>
    public static class ProjectIcon
    {
        /// <summary>The icon at <paramref name="size"/> pixels square, or null. Never throws; the caller owns the bitmap.</summary>
        public static Bitmap Load(string repositoryRoot, int size)
        {
            return AndroidAppIcon.Load(repositoryRoot, size)
                ?? VisualStudioProjectIcon.Load(repositoryRoot, size)
                ?? PhpProjectIcon.Load(repositoryRoot, size);
        }
    }
}
