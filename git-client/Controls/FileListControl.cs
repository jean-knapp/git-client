using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GitClient.Git;
using GitClient.Services;

namespace GitClient.Controls
{
    /// <summary>
    /// The commit detail card's file list: a summary strip carrying the totals, then one 32 px row
    /// per file with a tinted status glyph, the directory in --fg3 and the file name in --fg.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("SelectionChanged")]
    public sealed class FileListControl : VirtualListControl
    {
        private const int SidePadding = 14;
        private const int RowHeight = 32;

        private sealed class Entry
        {
            public FileChange Change;
            public int Added;
            public int Removed;
        }

        private readonly List<Entry> _entries = new List<Entry>();
        private int _totalAdded;
        private int _totalRemoved;
        private string _summaryOverride;

        public FileListControl()
        {
            EmptyText = "No files in this commit.";
        }

        [Browsable(false)]
        public FileChange SelectedChange
        {
            get
            {
                int index = SelectedIndex;
                return index >= 0 && index < _entries.Count ? _entries[index].Change : null;
            }
        }

        public void SetFiles(IEnumerable<FileChange> files, IDictionary<string, LineDelta> stats, string summaryOverride = null)
        {
            var keepPath = SelectedChange?.Path;
            _entries.Clear();
            _totalAdded = 0;
            _totalRemoved = 0;
            _summaryOverride = summaryOverride;

            if (files != null)
            {
                foreach (var change in files)
                {
                    LineDelta delta = default(LineDelta);
                    bool has = stats != null && stats.TryGetValue(change.Path, out delta);
                    var entry = new Entry { Change = change, Added = has ? delta.Added : 0, Removed = has ? delta.Removed : 0 };
                    _totalAdded += entry.Added;
                    _totalRemoved += entry.Removed;
                    _entries.Add(entry);
                }
            }

            int index = keepPath == null ? -1 : _entries.FindIndex(e => e.Change.Path == keepPath);
            RestoreState(index, 0);
            Invalidate();
        }

        public void Clear() => SetFiles(null, null);

        /// <summary>Selects the first row, so a diff appears as soon as a commit is picked.</summary>
        public void SelectFirst()
        {
            if (_entries.Count == 0) return;
            SetSelection(0, true);
        }

        public bool SelectPath(string path)
        {
            int index = _entries.FindIndex(e => e.Change.Path == path);
            if (index < 0) return false;
            SetSelection(index, true);
            EnsureVisible(index);
            return true;
        }

        protected override int RowCount => _entries.Count;
        protected override int GetRowHeight(int index) => RowHeight;
        protected override int HeaderHeight => 34;

        protected override void PaintHeader(Graphics g, Rectangle bounds)
        {
            var p = P;
            Draw.Fill(g, bounds, RowSurface);
            var font = Fonts.Ui(12f);
            var summary = _summaryOverride ?? (_entries.Count + (_entries.Count == 1 ? " file changed" : " files changed"));
            Draw.Text(g, summary, font, new Rectangle(SidePadding, bounds.Y, bounds.Width - SidePadding * 2, bounds.Height), p.Foreground3, Draw.LeftMiddle);

            int x = bounds.Width - SidePadding;
            if (_totalRemoved > 0)
            {
                var text = "−" + _totalRemoved;
                int w = Draw.MeasureWidth(text, font);
                Draw.Text(g, text, font, new Rectangle(x - w, bounds.Y, w, bounds.Height), p.Deleted, Draw.LeftMiddle);
                x -= w + 8;
            }
            if (_totalAdded > 0)
            {
                var text = "+" + _totalAdded;
                int w = Draw.MeasureWidth(text, font);
                Draw.Text(g, text, font, new Rectangle(x - w, bounds.Y, w, bounds.Height), p.Added, Draw.LeftMiddle);
            }
        }

        protected override void PaintRow(Graphics g, int index, Rectangle bounds, RowState state)
        {
            var p = P;
            var surface = RowSurface;
            var entry = _entries[index];

            if ((state & RowState.Selected) != 0) Draw.Fill(g, bounds, p.SelectionOn(surface));
            else if ((state & RowState.Hot) != 0) Draw.Fill(g, bounds, p.HoverOn(surface));

            int x = SidePadding;
            IconCache.DrawLeft(g, Icons.ForChange(entry.Change.Kind), 15, p.StatusColor(entry.Change.Kind),
                new Rectangle(x, bounds.Y, 15, bounds.Height));
            x += 15 + 9;

            string directory, fileName;
            Draw.SplitPath(entry.Change.Path, out directory, out fileName);
            var font = Fonts.Ui(13f);
            int available = Math.Max(0, bounds.Width - SidePadding - x);

            int dirWidth = Draw.MeasureWidth(directory, font);
            int nameWidth = Draw.MeasureWidth(fileName, font);
            if (dirWidth + nameWidth > available) dirWidth = Math.Max(0, available - nameWidth);

            if (dirWidth > 0)
            {
                Draw.Text(g, directory, font, new Rectangle(x, bounds.Y, dirWidth, bounds.Height), p.Foreground3, Draw.LeftMiddle);
            }
            Draw.Text(g, fileName, font, new Rectangle(x + dirWidth, bounds.Y, Math.Max(0, available - dirWidth), bounds.Height),
                p.Foreground, Draw.LeftMiddle);
        }
    }
}
