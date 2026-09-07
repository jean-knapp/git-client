using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GitClient.Git;
using GitClient.Services;

namespace GitClient.Controls
{
    public sealed class FileChangeEventArgs : EventArgs
    {
        public FileChangeEventArgs(FileChange change, bool staged)
        {
            Change = change;
            Staged = staged;
        }

        public FileChange Change { get; }

        /// <summary>The group the row belongs to.</summary>
        public bool Staged { get; }
    }

    /// <summary>
    /// The Changes card's list: STAGED and UNSTAGED bands over checkbox file rows, each with a
    /// status glyph, a two-line label and the per-file added/removed counts.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("SelectionChanged")]
    public sealed class ChangesListControl : VirtualListControl
    {
        private const int SidePadding = 14;
        private const int BandHeight = 30;
        private const int FileRowHeight = 38;
        private const int EmptyHeight = 56;
        private const int CheckSize = 18;

        private enum EntryKind { Band, File, Empty }

        private sealed class Entry
        {
            public EntryKind Kind;
            public string Label;
            public string Count;
            public bool Staged;
            public FileChange Change;
            public int Added;
            public int Removed;
            public bool Warn;
        }

        private readonly List<Entry> _entries = new List<Entry>();

        /// <summary>Raised when a row's checkbox is toggled.</summary>
        public event EventHandler<FileChangeEventArgs> CheckedChanged;

        /// <summary>Raised when a file row is activated (double click).</summary>
        public event EventHandler<FileChangeEventArgs> FileActivated;

        public ChangesListControl()
        {
            EmptyText = "No local changes.";
            RowDoubleClick += OnRowDoubleClicked;
        }

        protected override bool MultiSelect => true;

        /// <summary>The file behind the current selection, or null.</summary>
        [Browsable(false)]
        public FileChange SelectedChange
        {
            get
            {
                int index = SelectedIndex;
                return index >= 0 && index < _entries.Count ? _entries[index].Change : null;
            }
        }

        [Browsable(false)]
        public IEnumerable<FileChange> SelectedChanges
        {
            get
            {
                foreach (var index in SelectedIndices)
                {
                    if (index < 0 || index >= _entries.Count) continue;
                    var change = _entries[index].Change;
                    if (change != null) yield return change;
                }
            }
        }

        /// <summary>Rebuilds the two bands, keeping the selected path and scroll position.</summary>
        public void SetStatus(RepositoryStatus status, IDictionary<string, LineDelta> stagedStats, IDictionary<string, LineDelta> unstagedStats)
        {
            var keepPath = SelectedChange?.Path;
            bool keepStaged = SelectedChange != null && SelectedChange.Staged;
            int scroll = ScrollOffset;

            _entries.Clear();
            if (status != null)
            {
                // Conflicts get their own band so a stopped merge reads at a glance.
                var conflicted = status.Unstaged.FindAll(c => c.Kind == FileChangeKind.Conflicted);
                var unstaged = status.Unstaged.FindAll(c => c.Kind != FileChangeKind.Conflicted);
                if (conflicted.Count > 0)
                {
                    AddGroup("Conflicted", conflicted, false, unstagedStats, null, true,
                        conflicted.Count + (conflicted.Count == 1 ? " file" : " files") + " · resolve to continue");
                }
                AddGroup("Staged", status.Staged, true, stagedStats,
                    "Nothing staged. Tick a file below, or press Stage all.");
                AddGroup("Unstaged", unstaged, false, unstagedStats,
                    "Nothing to stage. The working tree is clean.");
            }

            int index = -1;
            if (keepPath != null)
            {
                index = _entries.FindIndex(entry => entry.Change != null && entry.Change.Path == keepPath && entry.Staged == keepStaged);
            }
            RestoreState(index, scroll);
            Invalidate();
        }

        private void AddGroup(string label, List<FileChange> changes, bool staged, IDictionary<string, LineDelta> stats,
            string emptyText, bool warn = false, string countOverride = null)
        {
            _entries.Add(new Entry
            {
                Kind = EntryKind.Band,
                Label = label,
                Count = countOverride ?? (changes.Count + (changes.Count == 1 ? " file" : " files")),
                Staged = staged,
                Warn = warn,
            });
            if (changes.Count == 0)
            {
                if (emptyText == null) return;
                _entries.Add(new Entry { Kind = EntryKind.Empty, Label = emptyText, Staged = staged });
                return;
            }
            foreach (var change in changes)
            {
                LineDelta delta = default(LineDelta);
                bool has = stats != null && stats.TryGetValue(change.Path, out delta);
                _entries.Add(new Entry
                {
                    Kind = EntryKind.File,
                    Change = change,
                    Staged = staged,
                    Added = has ? delta.Added : 0,
                    Removed = has ? delta.Removed : 0,
                });
            }
        }

        // ------------------------------------------------------------------ layout

        protected override int RowCount => _entries.Count;

        protected override int GetRowHeight(int index)
        {
            switch (_entries[index].Kind)
            {
                case EntryKind.Band: return BandHeight;
                case EntryKind.Empty: return EmptyHeight;
                default: return FileRowHeight;
            }
        }

        protected override bool IsSelectable(int index) => _entries[index].Kind == EntryKind.File;

        // ------------------------------------------------------------------ painting

        protected override void PaintRow(Graphics g, int index, Rectangle bounds, RowState state)
        {
            var entry = _entries[index];
            switch (entry.Kind)
            {
                case EntryKind.Band: PaintBand(g, entry, bounds); break;
                case EntryKind.Empty: PaintEmpty(g, entry, bounds); break;
                default: PaintFile(g, entry, bounds, state); break;
            }
        }

        private void PaintBand(Graphics g, Entry entry, Rectangle bounds)
        {
            var p = P;
            var surface = RowSurface;
            Draw.Fill(g, bounds, p.FillOn(surface));
            var divider = p.DividerOn(surface);
            Draw.HLine(g, 0, bounds.Y, bounds.Width, divider);
            Draw.HLine(g, 0, bounds.Bottom - 1, bounds.Width, divider);

            int x = SidePadding;
            var labelFont = Fonts.Ui(11f, true);
            var label = Tracked(entry.Label.ToUpperInvariant());
            Draw.Text(g, label, labelFont, new Rectangle(x, bounds.Y, bounds.Width - x, bounds.Height),
                entry.Warn ? p.Warning : p.Foreground3, Draw.LeftMiddle);
            x += Draw.MeasureWidth(label, labelFont) + 8;
            Draw.Text(g, entry.Count, Fonts.Ui(12f), new Rectangle(x, bounds.Y, Math.Max(0, bounds.Width - x - SidePadding), bounds.Height),
                p.Foreground3, Draw.LeftMiddle);
        }

        /// <summary>Approximates the design's 0.04em tracking on the uppercase band labels.</summary>
        private static string Tracked(string text)
        {
            if (text.Length < 2) return text;
            var sb = new System.Text.StringBuilder(text.Length * 2);
            for (int i = 0; i < text.Length; i++)
            {
                if (i > 0) sb.Append(' ');
                sb.Append(text[i]);
            }
            return sb.ToString();
        }

        private void PaintEmpty(Graphics g, Entry entry, Rectangle bounds)
        {
            Draw.Text(g, entry.Label, Fonts.Ui(13f),
                new Rectangle(24, bounds.Y, Math.Max(0, bounds.Width - 48), bounds.Height), P.Foreground3,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
        }

        private void PaintFile(Graphics g, Entry entry, Rectangle bounds, RowState state)
        {
            var p = P;
            var surface = RowSurface;
            if ((state & RowState.Selected) != 0) Draw.Fill(g, bounds, p.SelectionOn(surface));
            else if ((state & RowState.Hot) != 0) Draw.Fill(g, bounds, p.HoverOn(surface));

            int x = SidePadding;
            int centerY = bounds.Y + bounds.Height / 2;

            var box = new Rectangle(x, centerY - CheckSize / 2, CheckSize, CheckSize);
            if (entry.Staged)
            {
                Draw.FillRounded(g, box, 4f, p.AccentFill);
                IconCache.DrawCentered(g, Icons.Check, 13, p.AccentForeground, box.X + box.Width / 2, box.Y + box.Height / 2);
            }
            else
            {
                Draw.DrawRounded(g, new Rectangle(box.X, box.Y, box.Width - 1, box.Height - 1), 4f, p.Foreground3, 1.5f);
            }
            x += CheckSize + 10;

            var statusColor = p.StatusColor(entry.Change.Kind);
            IconCache.DrawLeft(g, Icons.ForChange(entry.Change.Kind), 15, statusColor, new Rectangle(x, bounds.Y, 15, bounds.Height));
            x += 15 + 10;

            int countsWidth = 0;
            var countFont = Fonts.Code(12f);
            string addText = entry.Added > 0 ? "+" + entry.Added : null;
            string delText = entry.Removed > 0 ? "−" + entry.Removed : null;
            if (addText != null) countsWidth += Draw.MeasureWidth(addText, countFont) + 8;
            if (delText != null) countsWidth += Draw.MeasureWidth(delText, countFont) + 8;

            int textRight = bounds.Width - SidePadding - countsWidth;
            string directory, fileName;
            Draw.SplitPath(entry.Change.OldPath != null ? entry.Change.Path : entry.Change.Path, out directory, out fileName);
            var nameFont = Fonts.Ui(13.5f);
            var dirFont = Fonts.Ui(11.5f);
            int textWidth = Math.Max(0, textRight - x);
            Draw.Text(g, fileName, nameFont, new Rectangle(x, bounds.Y + 3, textWidth, 17), p.Foreground, Draw.LeftMiddle);
            Draw.Text(g, directory.Length > 0 ? directory : ".", dirFont, new Rectangle(x, bounds.Y + 19, textWidth, 15), p.Foreground3, Draw.LeftMiddle);

            int cx = bounds.Width - SidePadding;
            if (delText != null)
            {
                int w = Draw.MeasureWidth(delText, countFont);
                Draw.Text(g, delText, countFont, new Rectangle(cx - w, bounds.Y, w, bounds.Height), p.Deleted, Draw.LeftMiddle);
                cx -= w + 8;
            }
            if (addText != null)
            {
                int w = Draw.MeasureWidth(addText, countFont);
                Draw.Text(g, addText, countFont, new Rectangle(cx - w, bounds.Y, w, bounds.Height), p.Added, Draw.LeftMiddle);
            }
        }

        // ------------------------------------------------------------------ input

        protected override void OnRowClicked(int index, MouseEventArgs e)
        {
            base.OnRowClicked(index, e);
            if (index < 0 || index >= _entries.Count) return;
            var entry = _entries[index];
            if (entry.Kind != EntryKind.File || e.Button != MouseButtons.Left) return;

            int centerY = RowTop(index) + FileRowHeight / 2;
            var box = new Rectangle(SidePadding, centerY - CheckSize / 2, CheckSize, CheckSize);
            box.Inflate(3, 3);
            if (box.Contains(e.Location))
            {
                CheckedChanged?.Invoke(this, new FileChangeEventArgs(entry.Change, entry.Staged));
            }
        }

        private void OnRowDoubleClicked(object sender, RowMouseEventArgs e)
        {
            if (e.Index < 0 || e.Index >= _entries.Count) return;
            var entry = _entries[e.Index];
            if (entry.Kind != EntryKind.File) return;
            FileActivated?.Invoke(this, new FileChangeEventArgs(entry.Change, entry.Staged));
        }
    }

}
