using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace GitClient.Services
{
    /// <summary>User preferences and session state, persisted as XML under %APPDATA%\GitClient.</summary>
    public sealed class AppSettings
    {
        private static AppSettings _current;

        public static AppSettings Current => _current ?? (_current = Load());

        public static string FilePath
        {
            get
            {
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GitClient");
                return Path.Combine(dir, "settings.xml");
            }
        }

        public List<string> OpenRepositories { get; set; } = new List<string>();
        public string ActiveRepository { get; set; }
        public List<string> RecentRepositories { get; set; } = new List<string>();

        public string GitExecutable { get; set; }
        public string ClaudeExecutable { get; set; }
        public string ClaudeModel { get; set; }
        public int MaxCommits { get; set; } = 3000;

        /// <summary>Dark or light palette.</summary>
        public ThemeMode Theme { get; set; } = ThemeMode.Dark;

        /// <summary>History row density, 30 to 48 px.</summary>
        public int HistoryRowHeight { get; set; } = 36;

        /// <summary>Show "2 days ago" instead of an absolute date in the history.</summary>
        public bool RelativeDates { get; set; }

        /// <summary>Unified or side-by-side diff.</summary>
        public Controls.DiffLayout DiffLayout { get; set; } = Controls.DiffLayout.Unified;

        public int WindowX { get; set; } = -1;
        public int WindowY { get; set; } = -1;
        public int WindowWidth { get; set; } = 1400;
        public int WindowHeight { get; set; } = 900;
        public bool WindowMaximized { get; set; }

        public int RightPanelWidth { get; set; } = 430;
        public bool ShowOutputPanel { get; set; }

        public void AddRecent(string path)
        {
            if (string.IsNullOrEmpty(path)) return;
            RecentRepositories.RemoveAll(p => string.Equals(p, path, StringComparison.OrdinalIgnoreCase));
            RecentRepositories.Insert(0, path);
            if (RecentRepositories.Count > 20) RecentRepositories.RemoveRange(20, RecentRepositories.Count - 20);
        }

        public static AppSettings Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var serializer = new XmlSerializer(typeof(AppSettings));
                    using (var stream = File.OpenRead(FilePath))
                    {
                        var loaded = (AppSettings)serializer.Deserialize(stream);
                        if (loaded.MaxCommits < 100) loaded.MaxCommits = 100;
                        return loaded;
                    }
                }
            }
            catch
            {
                // Corrupt settings are not worth crashing over; fall back to defaults.
            }
            return new AppSettings();
        }

        public void Save()
        {
            try
            {
                var dir = Path.GetDirectoryName(FilePath);
                if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                var serializer = new XmlSerializer(typeof(AppSettings));
                using (var stream = File.Create(FilePath))
                {
                    serializer.Serialize(stream, this);
                }
            }
            catch
            {
                // Ignore persistence failures (read-only profile, etc.).
            }
        }
    }
}
