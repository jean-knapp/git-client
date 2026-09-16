using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using GitClient.Services;

namespace GitClient.Controls
{
    /// <summary>
    /// The GitHub picker's list: 56 px rows carrying owner/name over the repository's description,
    /// with a Private badge and when it was last pushed to on the right.
    /// </summary>
    [ToolboxItem(true)]
    public sealed class GitHubRepositoryListControl : VirtualListControl
    {
        private const int RowHeight = 56;
        private const int SidePadding = 16;
        private const int GlyphSize = 17;

        private readonly List<GitHubRepository> _entries = new List<GitHubRepository>();

        public GitHubRepositoryListControl()
        {
            EmptyText = "No repositories.";
        }

        [Browsable(false)]
        public GitHubRepository SelectedEntry
        {
            get
            {
                int index = SelectedIndex;
                return index >= 0 && index < _entries.Count ? _entries[index] : null;
            }
        }

        /// <summary>How many rows are shown, after any filtering.</summary>
        [Browsable(false)]
        public int Count => _entries.Count;

        public GitHubRepository EntryAt(int index) => index >= 0 && index < _entries.Count ? _entries[index] : null;

        public void SetEntries(IEnumerable<GitHubRepository> entries)
        {
            _entries.Clear();
            if (entries != null) _entries.AddRange(entries);
            RestoreState(_entries.Count > 0 ? 0 : -1, 0);
            ContentChanged();
        }

        protected override int RowCount => _entries.Count;
        protected override int GetRowHeight(int index) => RowHeight;
        protected override int DefaultScrollStep => RowHeight;

        protected override void PaintRow(Graphics g, int index, Rectangle bounds, RowState state)
        {
            var p = P;
            var surface = RowSurface;
            var entry = _entries[index];
            bool selected = (state & RowState.Selected) != 0;

            var face = surface;
            if (selected)
            {
                face = p.SelectionOn(surface);
                Draw.Fill(g, bounds, face);
                Draw.Fill(g, new Rectangle(bounds.X, bounds.Y, 3, bounds.Height), p.AccentFill);
            }
            else if ((state & RowState.Hot) != 0)
            {
                face = p.HoverOn(surface);
                Draw.Fill(g, bounds, face);
            }
            if (index < _entries.Count - 1) Draw.HLine(g, 0, bounds.Bottom - 1, bounds.Width, p.DividerOn(surface));

            int x = SidePadding;
            IconCache.DrawLeft(g, Icons.Folder, GlyphSize, selected ? p.Accent : p.Foreground3, new Rectangle(x, bounds.Y, GlyphSize, bounds.Height));
            x += GlyphSize + 12;

            // The right-hand column: when it was last pushed to, and a badge for private repositories.
            var metaFont = Fonts.Ui(12.5f);
            int right = bounds.Width - SidePadding;
            var when = entry.PushedUtc.HasValue ? HistoryListControl.Relative(new DateTimeOffset(DateTime.SpecifyKind(entry.PushedUtc.Value, DateTimeKind.Utc))) : null;
            if (!string.IsNullOrEmpty(when))
            {
                int width = Draw.MeasureWidth(when, metaFont);
                Draw.Text(g, when, metaFont, new Rectangle(right - width, bounds.Y, width + 2, bounds.Height), p.Foreground3, Draw.LeftMiddle);
                right -= width + 14;
            }
            if (entry.IsPrivate)
            {
                var badgeFont = Fonts.Ui(11.5f, true);
                int width = Draw.MeasureWidth("Private", badgeFont) + 16;
                var badge = new Rectangle(right - width, bounds.Y + (bounds.Height - 20) / 2, width, 20);
                Draw.FillRounded(g, badge, 8f, p.Fill2On(face));
                Draw.Text(g, "Private", badgeFont, badge, p.Foreground2, Draw.CenterMiddle);
                right -= width + 12;
            }

            int available = Math.Max(0, right - x);
            Draw.Text(g, entry.FullName, Fonts.Ui(14f, true), new Rectangle(x, bounds.Y + 9, available, 19), p.Foreground, Draw.LeftMiddle);
            var second = string.IsNullOrWhiteSpace(entry.Description) ? entry.DefaultBranch : entry.Description.Trim();
            Draw.Text(g, second, Fonts.Ui(12.5f), new Rectangle(x, bounds.Y + 29, available, 17), p.Foreground3, Draw.LeftMiddle);
        }
    }
}
