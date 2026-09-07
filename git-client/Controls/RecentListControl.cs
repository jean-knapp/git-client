using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using GitClient.Services;

namespace GitClient.Controls
{
    /// <summary>One row of the welcome screen's recent-repository list.</summary>
    public sealed class RecentEntry
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Branch { get; set; }
        public string When { get; set; }
    }

    /// <summary>
    /// The welcome screen's recent list: 52 px rows carrying a folder glyph, the repository name
    /// over its path in monospace, then the branch and when it was last opened.
    /// </summary>
    [ToolboxItem(true)]
    public sealed class RecentListControl : VirtualListControl
    {
        private const int RowHeight = 52;
        private const int SidePadding = 16;

        private readonly List<RecentEntry> _entries = new List<RecentEntry>();

        public RecentListControl()
        {
            EmptyText = "No repositories opened yet.";
        }

        [Browsable(false)]
        public RecentEntry SelectedEntry
        {
            get
            {
                int index = SelectedIndex;
                return index >= 0 && index < _entries.Count ? _entries[index] : null;
            }
        }

        public RecentEntry EntryAt(int index) => index >= 0 && index < _entries.Count ? _entries[index] : null;

        public void SetEntries(IEnumerable<RecentEntry> entries)
        {
            _entries.Clear();
            if (entries != null) _entries.AddRange(entries);
            RestoreState(-1, 0);
            Invalidate();
        }

        protected override int RowCount => _entries.Count;
        protected override int GetRowHeight(int index) => RowHeight;

        protected override void PaintRow(Graphics g, int index, Rectangle bounds, RowState state)
        {
            var p = P;
            var surface = RowSurface;
            var entry = _entries[index];

            if ((state & RowState.Selected) != 0) Draw.Fill(g, bounds, p.SelectionOn(surface));
            else if ((state & RowState.Hot) != 0) Draw.Fill(g, bounds, p.HoverOn(surface));

            if (index < _entries.Count - 1) Draw.HLine(g, 0, bounds.Bottom - 1, bounds.Width, p.DividerOn(surface));

            int x = SidePadding;
            IconCache.DrawLeft(g, Icons.Folder, 17, p.Foreground3, new Rectangle(x, bounds.Y, 17, bounds.Height));
            x += 17 + 12;

            int rightWidth = 0;
            var metaFont = Fonts.Ui(12.5f);
            if (!string.IsNullOrEmpty(entry.When)) rightWidth += Draw.MeasureWidth(entry.When, metaFont) + 14;
            if (!string.IsNullOrEmpty(entry.Branch)) rightWidth += Draw.MeasureWidth(entry.Branch, metaFont) + 12 + 5 + 14;

            int available = Math.Max(0, bounds.Width - SidePadding - rightWidth - x);
            Draw.Text(g, entry.Name, Fonts.Ui(14f, true), new Rectangle(x, bounds.Y + 8, available, 19), p.Foreground, Draw.LeftMiddle);
            Draw.Text(g, entry.Path, Fonts.Code(12f), new Rectangle(x, bounds.Y + 27, available, 17), p.Foreground3, Draw.LeftMiddle);

            int rx = bounds.Width - SidePadding;
            if (!string.IsNullOrEmpty(entry.When))
            {
                int w = Draw.MeasureWidth(entry.When, metaFont);
                Draw.Text(g, entry.When, metaFont, new Rectangle(rx - w, bounds.Y, w, bounds.Height), p.Foreground3, Draw.LeftMiddle);
                rx -= w + 14;
            }
            if (!string.IsNullOrEmpty(entry.Branch))
            {
                int w = Draw.MeasureWidth(entry.Branch, metaFont);
                Draw.Text(g, entry.Branch, metaFont, new Rectangle(rx - w, bounds.Y, w, bounds.Height), p.Foreground2, Draw.LeftMiddle);
                rx -= w + 5;
                IconCache.DrawLeft(g, Icons.Branch, 12, p.Foreground2, new Rectangle(rx - 12, bounds.Y, 12, bounds.Height));
            }
        }
    }
}
