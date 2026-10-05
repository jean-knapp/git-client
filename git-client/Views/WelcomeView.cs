using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using GitClient.Controls;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Views
{
    public sealed class RepositoryRequestedEventArgs : EventArgs
    {
        public RepositoryRequestedEventArgs(string path) { Path = path; }
        public string Path { get; }
    }

    /// <summary>The no-repository screen: two action cards over the recent-repository list.</summary>
    public partial class WelcomeView : ModernUserControl
    {
        private const int SidePadding = 80;

        // The design caps the column at 880 px; widened a little so the action-card descriptions
        // fit on one line at 13 px instead of ellipsising.
        private const int MaxContentWidth = 1040;

        /// <summary>Raised when one of the action cards is chosen.</summary>
        public event EventHandler OpenRequested;
        public event EventHandler CloneRequested;

        /// <summary>Raised when a recent repository is picked.</summary>
        public event EventHandler<RepositoryRequestedEventArgs> RecentRequested;

        public WelcomeView()
        {
            InitializeComponent();
            ApplyTileColours();
            Theme.Changed += (s, e) => ApplyTileColours();
        }

        private void ApplyTileColours()
        {
            var p = Theme.Palette;
            openCard.TileFill = Color.FromArgb(36, p.AccentFill);
            openCard.TileForeground = p.AccentFill;
            cloneCard.TileFill = Color.FromArgb(41, p.Lane2);
            cloneCard.TileForeground = p.Mode == ThemeMode.Light ? p.Lane2 : Color.FromArgb(0xc9, 0xa6, 0xff);
        }

        /// <summary>Fills the recent list, dropping folders that no longer exist.</summary>
        public void SetRecent(IEnumerable<string> paths, Func<string, string> branchLookup)
        {
            var entries = new List<RecentEntry>();
            foreach (var path in paths ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) continue;
                DateTime? written = null;
                try { written = Directory.GetLastWriteTime(path); } catch { }
                entries.Add(new RecentEntry
                {
                    Name = new DirectoryInfo(path).Name,
                    Path = path,
                    Branch = branchLookup != null ? branchLookup(path) : null,
                    When = written.HasValue ? HistoryListControl.Relative(written.Value) : null,
                });
                if (entries.Count >= 8) break;
            }
            recentList.SetEntries(entries);
            bool any = entries.Count > 0;
            recentLabel.Visible = any;
            recentRule.Visible = any;
            recentCard.Visible = any;
            LayoutContent();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            LayoutContent();
        }

        /// <summary>Centres the 880 px content column and shares the row width between the two cards.</summary>
        private void LayoutContent()
        {
            int available = Math.Max(320, Width - SidePadding * 2);
            int width = Math.Min(MaxContentWidth, available);
            int recentHeight = recentCard.Visible ? Math.Min(316, recentList.RowsHeight + 2) : 0;
            int height = recentCard.Visible ? 258 + recentHeight : 198;
            height = Math.Min(height, Math.Max(198, Height - 40));

            contentPanel.Size = new Size(width, height);
            contentPanel.Location = new Point((Width - width) / 2, Math.Max(20, (Height - height) / 2));

            const int gap = 12;
            int cardWidth = (width - gap) / 2;
            openCard.SetBounds(0, 102, cardWidth, 96);
            cloneCard.SetBounds(cardWidth + gap, 102, width - cardWidth - gap, 96);

            headingLabel.Width = width;
            subheadLabel.Width = width;

            recentLabel.Width = Math.Max(60, recentLabel.PreferredWidth + 4);
            recentRule.SetBounds(recentLabel.Right + 10, 239, Math.Max(10, width - recentLabel.Right - 10), 1);
            recentCard.SetBounds(0, 258, width, Math.Max(60, height - 258));
        }

        private void openCard_Click(object sender, EventArgs e) => OpenRequested?.Invoke(this, EventArgs.Empty);
        private void cloneCard_Click(object sender, EventArgs e) => CloneRequested?.Invoke(this, EventArgs.Empty);

        private void recentList_RowDoubleClick(object sender, RowMouseEventArgs e) => OpenRecent(e.Index);

        private void recentList_RowClick(object sender, RowMouseEventArgs e)
        {
            // A single click opens, matching the rest of the welcome screen's card behaviour.
            if (e.Button == System.Windows.Forms.MouseButtons.Left) OpenRecent(e.Index);
        }

        private void OpenRecent(int index)
        {
            var entry = recentList.EntryAt(index);
            if (entry == null) return;
            RecentRequested?.Invoke(this, new RepositoryRequestedEventArgs(entry.Path));
        }
    }
}
