using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using GitClient.Git;
using GitClient.Graph;
using GitClient.Services;

namespace GitClient.Controls
{
    public sealed class HistoryRowEventArgs : EventArgs
    {
        public HistoryRowEventArgs(GraphRow row, RefInfo reference, Point screenLocation)
        {
            Row = row;
            Reference = reference;
            ScreenLocation = screenLocation;
        }

        /// <summary>Row under the pointer, or null for empty space.</summary>
        public GraphRow Row { get; }

        /// <summary>Ref pill under the pointer, if any.</summary>
        public RefInfo Reference { get; }

        public Point ScreenLocation { get; }
    }

    /// <summary>
    /// The History card's list: a column header, railway lanes, avatar-disc commit nodes, ref pills
    /// after the node, and the author and date columns from the redesign.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("SelectionChanged")]
    public sealed class HistoryListControl : VirtualListControl
    {
        private const int SidePadding = 14;
        private const int GraphColumnWidth = 150;
        private const int AuthorColumnWidth = 170;
        private const int DateColumnWidth = 130;
        private const int LaneSpacing = 18;
        private const int FirstLaneCenter = 23;   // inside the graph cell
        private const int PillLeft = 44;          // inside the graph cell
        private const int MessageRightPadding = 16;

        private GraphLayout _layout;
        private RepositoryStatus _status;
        private List<GraphRow> _rows = new List<GraphRow>();
        private int _rowHeight = 36;
        private bool _relativeDates;
        private string _filter = string.Empty;

        public event EventHandler<HistoryRowEventArgs> RowContextMenuRequested;
        public event EventHandler<HistoryRowEventArgs> RowActivated;
        public event EventHandler<HistoryRowEventArgs> RefActivated;

        public HistoryListControl()
        {
            EmptyText = "No commits yet. Stage some files and create the first commit.";
            RowRightClick += OnRowRightClick;
            RowDoubleClick += OnRowDoubleClick;
        }

        // ------------------------------------------------------------------ public surface

        [Category("Appearance"), DefaultValue(36)]
        public int RowHeight
        {
            get => _rowHeight;
            set
            {
                int clamped = Math.Max(30, Math.Min(48, value));
                if (clamped == _rowHeight) return;
                _rowHeight = clamped;
                Invalidate();
            }
        }

        [Category("Behavior"), DefaultValue(false)]
        public bool RelativeDates
        {
            get => _relativeDates;
            set { _relativeDates = value; Invalidate(); }
        }

        /// <summary>Filters by message, author or sha. Lanes are hidden while a filter is active.</summary>
        [Browsable(false)]
        public string Filter
        {
            get => _filter;
            set
            {
                var text = value ?? string.Empty;
                if (string.Equals(text, _filter, StringComparison.Ordinal)) return;
                _filter = text;
                RebuildRows();
            }
        }

        [Browsable(false)]
        public GraphRow SelectedRow
        {
            get
            {
                int index = SelectedIndex;
                return index >= 0 && index < _rows.Count ? _rows[index] : null;
            }
        }

        [Browsable(false)]
        public IEnumerable<CommitInfo> SelectedCommits =>
            SelectedIndices.Where(i => i >= 0 && i < _rows.Count).Select(i => _rows[i].Commit).Where(c => c != null);

        [Browsable(false)]
        public int LoadedCount => _layout?.Rows.Count(r => r.Commit != null) ?? 0;

        protected override bool MultiSelect => true;

        /// <summary>Replaces the history, keeping the selected commit and scroll position where possible.</summary>
        public void SetData(GraphLayout layout, RepositoryStatus status)
        {
            var keepSha = SelectedRow?.Sha;
            bool keepWip = SelectedRow != null && SelectedRow.IsWorkInProgress;
            int scroll = ScrollOffset;

            _layout = layout;
            _status = status;
            RebuildRows(false);

            int index = -1;
            if (keepWip) index = _rows.FindIndex(r => r.IsWorkInProgress);
            else if (keepSha != null) index = _rows.FindIndex(r => r.Sha == keepSha);
            RestoreState(index, scroll);
            Invalidate();
        }

        private void RebuildRows(bool raiseSelection = true)
        {
            var previous = SelectedRow;
            _rows = new List<GraphRow>();
            if (_layout != null)
            {
                if (_filter.Length == 0)
                {
                    _rows.AddRange(_layout.Rows);
                }
                else
                {
                    foreach (var row in _layout.Rows)
                    {
                        if (row.Stash != null ? Matches(row.Stash) : row.Commit != null && Matches(row.Commit)) _rows.Add(row);
                    }
                }
            }
            int index = previous == null ? -1 : _rows.IndexOf(previous);
            RestoreState(index, 0);
            Invalidate();
            if (raiseSelection && index < 0) SetSelection(-1, true);
        }

        private bool Matches(CommitInfo commit)
        {
            return (commit.Subject ?? string.Empty).IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                || (commit.AuthorName ?? string.Empty).IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                || (commit.Sha ?? string.Empty).StartsWith(_filter, StringComparison.OrdinalIgnoreCase);
        }

        private bool Matches(StashInfo stash)
        {
            return (stash.Description ?? string.Empty).IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                || (stash.Branch ?? string.Empty).IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0
                || "stash".IndexOf(_filter, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public bool SelectSha(string sha)
        {
            if (sha == null) return false;
            int index = _rows.FindIndex(r => r.Sha == sha);
            if (index < 0) return false;
            SetSelection(index, true);
            EnsureVisible(index);
            return true;
        }

        /// <summary>
        /// Selects the working-tree row, or the checked-out commit when the working tree is clean
        /// and there is no such row (the newest commit when HEAD is not listed).
        /// </summary>
        public void SelectWorkInProgress()
        {
            if (_rows.Count == 0) return;
            int index = _rows.FindIndex(r => r.IsWorkInProgress);
            if (index < 0 && _status?.HeadSha != null) index = _rows.FindIndex(r => r.Commit != null && r.Commit.Sha == _status.HeadSha);
            if (index < 0) index = _rows.FindIndex(r => r.Commit != null);
            if (index < 0) index = 0;
            SetSelection(index, true);
            EnsureVisible(index);
        }

        // ------------------------------------------------------------------ layout

        protected override int RowCount => _rows.Count;
        protected override int GetRowHeight(int index) => _rowHeight;
        protected override int HeaderHeight => 28;
        protected override int DefaultScrollStep => _rowHeight / 2;

        private int TrackLeft => SidePadding;
        private int TrackRight => Math.Max(TrackLeft + 60, Width - SidePadding);
        private int GraphLeft => TrackLeft;
        private int CommitLeft => GraphLeft + GraphColumnWidth;
        private int DateLeft => TrackRight - DateColumnWidth;
        private int AuthorLeft => DateLeft - AuthorColumnWidth;

        private int LaneCenter(int lane) => GraphLeft + FirstLaneCenter + lane * LaneSpacing;

        private int PillStart
        {
            get
            {
                int lanes = Math.Max(1, _layout?.LaneCount ?? 1);
                return Math.Max(GraphLeft + PillLeft, LaneCenter(lanes - 1) + 14);
            }
        }

        // ------------------------------------------------------------------ painting

        protected override void PaintHeader(Graphics g, Rectangle bounds)
        {
            var p = P;
            Draw.Fill(g, bounds, RowSurface);
            var font = Fonts.Ui(12f);
            int y = bounds.Y;
            Draw.Text(g, "Graph", font, new Rectangle(GraphLeft, y, GraphColumnWidth, bounds.Height), p.Foreground3, Draw.LeftMiddle);
            Draw.Text(g, "Commit", font, new Rectangle(CommitLeft, y, Math.Max(0, AuthorLeft - CommitLeft), bounds.Height), p.Foreground3, Draw.LeftMiddle);
            if (AuthorLeft > CommitLeft + 60)
            {
                Draw.Text(g, "Author", font, new Rectangle(AuthorLeft, y, AuthorColumnWidth, bounds.Height), p.Foreground3, Draw.LeftMiddle);
                Draw.Text(g, "Date", font, new Rectangle(DateLeft, y, DateColumnWidth, bounds.Height), p.Foreground3, Draw.LeftMiddle);
            }
            Draw.HLine(g, 0, bounds.Bottom - 1, bounds.Width, p.DividerOn(RowSurface));
        }

        protected override void PaintRow(Graphics g, int index, Rectangle bounds, RowState state)
        {
            var p = P;
            var row = _rows[index];
            var surface = RowSurface;
            bool selected = (state & RowState.Selected) != 0;

            _rowBackground = selected ? p.SelectionOn(surface) : (state & RowState.Hot) != 0 ? p.HoverOn(surface) : surface;
            if (_rowBackground != surface) Draw.Fill(g, bounds, _rowBackground);

            if (selected)
            {
                var bar = new Rectangle(0, bounds.Y + 6, 3, Math.Max(4, bounds.Height - 12));
                Draw.FillRounded(g, bar, 2f, p.Accent);
            }

            if (row.IsWorkInProgress) PaintWorkingTreeRow(g, row, bounds, selected);
            else if (row.Stash != null) PaintStashRow(g, row, bounds, selected);
            else PaintCommitRow(g, row, bounds, selected);
        }

        private Color _rowBackground;

        /// <summary>
        /// A colour faded towards the row background: how commits the checked-out branch does not
        /// contain are drawn.
        /// </summary>
        private Color Faded(Color color) => ThemePalette.Flatten(Color.FromArgb(105, color), _rowBackground);

        /// <summary>A lane's colour; a stash's line is neutral.</summary>
        private Color LaneColorOf(int colorIndex) => colorIndex == GraphLayout.StashColor ? P.Foreground3 : P.LaneColor(colorIndex);

        private void PaintLanes(Graphics g, GraphRow row, Rectangle bounds)
        {
            if (_filter.Length > 0) return;   // a filtered list is not contiguous, so lanes would lie
            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int centerY = bounds.Y + bounds.Height / 2;
            int nextCenterY = centerY + bounds.Height;

            foreach (var edge in row.Edges)
            {
                int xf = LaneCenter(edge.FromColumn);
                int xt = LaneCenter(edge.ToColumn);
                var color = LaneColorOf(edge.ColorIndex);
                using (var pen = new Pen(edge.OffHead ? Faded(color) : color, 2f))
                {
                    if (edge.Dashed) pen.DashStyle = DashStyle.Dot;
                    if (xf == xt)
                    {
                        g.DrawLine(pen, xf, centerY, xt, nextCenterY);
                    }
                    else
                    {
                        float mid = (centerY + nextCenterY) / 2f;
                        g.DrawBezier(pen, xf, centerY, xf, mid, xt, mid, xt, nextCenterY);
                    }
                }
            }

            // The upward half of this row's own lane, so consecutive rows form one continuous stem.
            if (row.HasStem)
            {
                int x = LaneCenter(row.Column);
                var color = LaneColorOf(row.StemFromStash ? GraphLayout.StashColor : row.ColorIndex);
                using (var pen = new Pen(row.StemOffHead ? Faded(color) : color, 2f))
                {
                    if (row.StemFromStash) pen.DashStyle = DashStyle.Dot;
                    g.DrawLine(pen, x, bounds.Y - bounds.Height / 2, x, centerY);
                }
            }

            g.SmoothingMode = old;
        }

        private void PaintWorkingTreeRow(Graphics g, GraphRow row, Rectangle bounds, bool selected)
        {
            var p = P;
            var surface = RowSurface;
            int centerY = bounds.Y + bounds.Height / 2;
            int cx = LaneCenter(0);

            if (_filter.Length == 0)
            {
                using (var pen = new Pen(p.LaneColor(0), 2f))
                {
                    g.DrawLine(pen, cx, centerY, cx, bounds.Bottom + bounds.Height / 2);
                }
                var old = g.SmoothingMode;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(surface)) g.FillEllipse(brush, cx - 7, centerY - 7, 14, 14);
                using (var pen = new Pen(p.LaneColor(0), 1.6f) { DashStyle = DashStyle.Dot })
                {
                    g.DrawEllipse(pen, cx - 7, centerY - 7, 14, 14);
                }
                g.SmoothingMode = old;
            }

            int x = CommitLeft;
            var title = "Uncommitted changes";
            var titleFont = Fonts.Ui(14f, true);
            Draw.Text(g, title, titleFont, new Rectangle(x, bounds.Y, Math.Max(0, AuthorLeft - x), bounds.Height), p.Foreground, Draw.LeftMiddle);
            x += Draw.MeasureWidth(title, titleFont) + 12;

            foreach (var chip in ChangeChips())
            {
                if (x > AuthorLeft - 40) break;
                IconCache.DrawLeft(g, chip.Icon, 12, chip.Color, new Rectangle(x, bounds.Y, 12, bounds.Height));
                x += 12 + 5;
                var font = Fonts.Ui(12.5f);
                Draw.Text(g, chip.Text, font, new Rectangle(x, bounds.Y, Math.Max(0, AuthorLeft - x), bounds.Height), chip.Color, Draw.LeftMiddle);
                x += Draw.MeasureWidth(chip.Text, font) + 12;
            }

            if (AuthorLeft > CommitLeft + 60)
            {
                Draw.Text(g, "working tree", Fonts.Ui(13f), new Rectangle(AuthorLeft, bounds.Y, AuthorColumnWidth, bounds.Height), p.Foreground3, Draw.LeftMiddle);
                Draw.Text(g, "now", Fonts.Ui(13f), new Rectangle(DateLeft, bounds.Y, DateColumnWidth, bounds.Height), p.Foreground3, Draw.LeftMiddle);
            }
        }

        private struct ChangeChip
        {
            public string Icon;
            public string Text;
            public Color Color;
        }

        private IEnumerable<ChangeChip> ChangeChips()
        {
            if (_status == null) yield break;
            int modified = 0, added = 0, deleted = 0, conflicted = 0;
            foreach (var change in _status.Staged.Concat(_status.Unstaged))
            {
                switch (change.Kind)
                {
                    case FileChangeKind.Added:
                    case FileChangeKind.Untracked: added++; break;
                    case FileChangeKind.Deleted: deleted++; break;
                    case FileChangeKind.Conflicted: conflicted++; break;
                    default: modified++; break;
                }
            }
            if (conflicted > 0) yield return new ChangeChip { Icon = Icons.FileConflicted, Text = conflicted + " conflicted", Color = P.Warning };
            if (modified > 0) yield return new ChangeChip { Icon = Icons.FileModified, Text = modified + " modified", Color = P.Modified };
            if (added > 0) yield return new ChangeChip { Icon = Icons.FileAdded, Text = added + " added", Color = P.Added };
            if (deleted > 0) yield return new ChangeChip { Icon = Icons.FileDeleted, Text = deleted + " deleted", Color = P.Deleted };
            if (modified == 0 && added == 0 && deleted == 0 && conflicted == 0)
                yield return new ChangeChip { Icon = Icons.Check, Text = "no local changes", Color = P.Foreground3 };
        }

        private void PaintCommitRow(Graphics g, GraphRow row, Rectangle bounds, bool selected)
        {
            var p = P;
            var surface = RowSurface;
            var commit = row.Commit;
            bool isHead = _status != null && _status.HeadSha != null &&
                          string.Equals(commit.Sha, _status.HeadSha, StringComparison.OrdinalIgnoreCase);

            PaintLanes(g, row, bounds);

            int centerY = bounds.Y + bounds.Height / 2;
            int cx = LaneCenter(row.Column);
            var lane = p.LaneColor(row.ColorIndex);
            bool faded = row.OffHead;
            if (faded) lane = Faded(lane);

            var old = g.SmoothingMode;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            if (isHead)
            {
                var disc = new Rectangle(cx - 9, centerY - 9, 18, 18);
                using (var brush = new SolidBrush(lane)) g.FillEllipse(brush, disc);
                var onLane = p.Mode == ThemeMode.Light ? Color.White : Color.FromArgb(0x05, 0x20, 0x2b);
                Draw.Text(g, Draw.Initials(commit.AuthorName), Fonts.Ui(9f, true), disc, onLane, Draw.CenterMiddle);
            }
            else
            {
                var disc = new Rectangle(cx - 8, centerY - 8, 16, 16);
                using (var brush = new SolidBrush(surface)) g.FillEllipse(brush, disc);
                using (var pen = new Pen(lane, 2f)) g.DrawEllipse(pen, new Rectangle(disc.X + 1, disc.Y + 1, disc.Width - 2, disc.Height - 2));
            }
            g.SmoothingMode = old;

            int messageLeft = CommitLeft;
            if (commit.Refs.Count > 0 && _filter.Length == 0)
            {
                // Pills may run past the graph column and push the message right; cutting them off
                // at the column edge is what used to hide "this is on the remote too".
                int pillX = PillStart;
                int limit = Math.Max(CommitLeft, (AuthorLeft > CommitLeft + 60 ? AuthorLeft : TrackRight) - 160);
                foreach (var pill in BuildPills(row))
                {
                    int width = PillWidth(pill);
                    if (pillX + width > limit) break;
                    PaintPill(g, pill, new Rectangle(pillX, centerY - 10, width, 20), surface);
                    pillX += width + 5;
                }
                messageLeft = Math.Max(messageLeft, pillX + 4);
            }

            var messageFont = Fonts.Ui(14f, isHead);
            int messageRight = AuthorLeft > CommitLeft + 60 ? AuthorLeft : TrackRight;
            Draw.Text(g, commit.Subject ?? string.Empty, messageFont,
                new Rectangle(messageLeft, bounds.Y, Math.Max(0, messageRight - messageLeft - MessageRightPadding), bounds.Height),
                faded ? Faded(p.Foreground) : p.Foreground, Draw.LeftMiddle);

            if (AuthorLeft > CommitLeft + 60)
            {
                var avatar = new Rectangle(AuthorLeft, centerY - 10, 20, 20);
                var avatarFore = faded ? Faded(p.Lane2) : p.Lane2;
                Draw.Avatar(g, avatar, Draw.Initials(commit.AuthorName), Fonts.Ui(9f, true),
                    Color.FromArgb(p.Mode == ThemeMode.Light ? 36 : 56, avatarFore), avatarFore);
                Draw.Text(g, commit.AuthorName ?? string.Empty, Fonts.Ui(13f),
                    new Rectangle(AuthorLeft + 28, bounds.Y, AuthorColumnWidth - 28 - 8, bounds.Height), faded ? Faded(p.Foreground2) : p.Foreground2, Draw.LeftMiddle);

                var dateColor = selected ? p.Foreground2 : p.Foreground3;
                Draw.Text(g, FormatDate(commit.AuthorDate), Fonts.Ui(13f),
                    new Rectangle(DateLeft, bounds.Y, DateColumnWidth, bounds.Height),
                    faded ? Faded(dateColor) : dateColor, Draw.LeftMiddle);
            }
        }

        /// <summary>
        /// A stash: a box on a dashed neutral line that runs down to the commit it was made on, a
        /// "stash" pill with its branch, and what was stashed.
        /// </summary>
        private void PaintStashRow(Graphics g, GraphRow row, Rectangle bounds, bool selected)
        {
            var p = P;
            var stash = row.Stash;
            bool faded = row.OffHead;
            Color Tone(Color c) => faded ? Faded(c) : c;

            PaintLanes(g, row, bounds);

            int centerY = bounds.Y + bounds.Height / 2;
            if (_filter.Length == 0)
            {
                int cx = LaneCenter(row.Column);
                var box = new Rectangle(cx - 8, centerY - 8, 16, 16);
                Draw.FillRounded(g, box, 4f, _rowBackground);
                using (var pen = new Pen(Tone(p.Foreground3), 1.6f) { DashStyle = DashStyle.Dot })
                {
                    g.DrawRectangle(pen, box.X + 1, box.Y + 1, box.Width - 2, box.Height - 2);
                }
                IconCache.DrawCentered(g, Icons.Stash, 10, Tone(p.Foreground2), cx, centerY);
            }

            // The pill: "stash" and the branch it was made on.
            int x = _filter.Length == 0 ? PillStart : CommitLeft;
            var pillFont = Fonts.Ui(11.5f, true);
            var pillText = "stash" + (stash.Branch != null ? " · " + stash.Branch : string.Empty);
            int pillWidth = Math.Min(220, 8 + 11 + 5 + Draw.MeasureWidth(pillText, pillFont) + 8);
            var pill = new Rectangle(x, centerY - 10, pillWidth, 20);
            var fore = Tone(p.Foreground2);
            Draw.FillRounded(g, pill, 4f, ThemePalette.Flatten(Color.FromArgb(28, fore), _rowBackground));
            Draw.DrawRounded(g, new Rectangle(pill.X, pill.Y, pill.Width - 1, pill.Height - 1), 4f, ThemePalette.Flatten(Color.FromArgb(90, fore), _rowBackground));
            IconCache.DrawLeft(g, Icons.Stash, 11, fore, new Rectangle(pill.X + 8, pill.Y, 11, pill.Height));
            Draw.Text(g, pillText, pillFont, new Rectangle(pill.X + 8 + 11 + 5, pill.Y, Math.Max(0, pill.Right - 8 - (pill.X + 24)), pill.Height), fore, Draw.LeftMiddle);

            int messageLeft = Math.Max(CommitLeft, pill.Right + 9);
            int messageRight = AuthorLeft > CommitLeft + 60 ? AuthorLeft : TrackRight;
            Draw.Text(g, stash.Description ?? stash.Message ?? string.Empty, Fonts.Ui(14f),
                new Rectangle(messageLeft, bounds.Y, Math.Max(0, messageRight - messageLeft - MessageRightPadding), bounds.Height),
                Tone(p.Foreground2), Draw.LeftMiddle);

            if (AuthorLeft > CommitLeft + 60)
            {
                var dim = Tone(selected ? p.Foreground2 : p.Foreground3);
                Draw.Text(g, stash.Selector, Fonts.Ui(13f), new Rectangle(AuthorLeft, bounds.Y, AuthorColumnWidth, bounds.Height), dim, Draw.LeftMiddle);
                Draw.Text(g, FormatDate(stash.Date), Fonts.Ui(13f), new Rectangle(DateLeft, bounds.Y, DateColumnWidth, bounds.Height), dim, Draw.LeftMiddle);
            }
        }

        // ------------------------------------------------------------------ ref pills

        private sealed class RefPill
        {
            public RefInfo Primary;
            public string Text;
            public bool IsLocal;
            public bool IsRemote;
            public bool IsTag;
            public bool IsHead;

            /// <summary>A local branch whose remote is on this very commit: pushed, nothing pending.</summary>
            public bool OnRemoteToo;

            /// <summary>Remotes carrying this branch, when there is more than the tracked one.</summary>
            public string RemoteNames;
        }

        private List<RefPill> BuildPills(GraphRow row)
        {
            var pills = new List<RefPill>();
            var refs = row.Commit.Refs;
            var consumed = new HashSet<RefInfo>();

            foreach (var detached in refs.Where(r => r.Kind == RefKind.DetachedHead))
            {
                pills.Add(new RefPill { Primary = detached, Text = "HEAD", IsHead = true, IsLocal = true });
                consumed.Add(detached);
            }
            foreach (var local in refs.Where(r => r.Kind == RefKind.LocalBranch)
                                      .OrderByDescending(r => r.IsHead).ThenBy(r => r.Name, StringComparer.OrdinalIgnoreCase))
            {
                var pill = new RefPill { Primary = local, Text = local.Name, IsLocal = true, IsHead = local.IsHead };

                // A remote-tracking ref on the same commit means the branch is pushed. Fold it into
                // the branch's own pill - two pills side by side read as two different places.
                var onRemote = refs.Where(r => r.Kind == RefKind.RemoteBranch && IsSameBranch(r.Name, local.Name)).ToList();
                if (onRemote.Count > 0)
                {
                    pill.OnRemoteToo = true;
                    pill.RemoteNames = string.Join(", ", onRemote.Select(r => r.Name));
                    foreach (var r in onRemote) consumed.Add(r);
                }

                pills.Add(pill);
                consumed.Add(local);
            }
            foreach (var remote in refs.Where(r => r.Kind == RefKind.RemoteBranch && !consumed.Contains(r))
                                       .OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase))
            {
                pills.Add(new RefPill { Primary = remote, Text = remote.Name, IsRemote = true });
            }
            foreach (var tag in refs.Where(r => r.Kind == RefKind.Tag).OrderBy(r => r.Name, StringComparer.OrdinalIgnoreCase))
            {
                pills.Add(new RefPill { Primary = tag, Text = tag.Name, IsTag = true });
            }
            return pills;
        }

        /// <summary>Does "origin/main" name the same branch as the local "main"?</summary>
        private static bool IsSameBranch(string remoteName, string localName)
        {
            if (remoteName == null || localName == null) return false;
            int slash = remoteName.IndexOf('/');
            return slash > 0 && string.Equals(remoteName.Substring(slash + 1), localName, StringComparison.Ordinal);
        }

        private int PillWidth(RefPill pill)
        {
            int width = 8 + Draw.MeasureWidth(pill.Text, Fonts.Ui(11.5f, true)) + 8;
            if (pill.IsLocal) width += 11 + 5;
            if (pill.OnRemoteToo) width += 5 + 11;
            return Math.Min(220, width);
        }

        private void PaintPill(Graphics g, RefPill pill, Rectangle bounds, Color surface)
        {
            var p = P;
            Color fill, border, fore;
            bool light = p.Mode == ThemeMode.Light;

            if (pill.IsTag)
            {
                fill = light ? Color.FromArgb(26, 154, 107, 0) : Color.FromArgb(31, 255, 222, 89);
                border = light ? Color.FromArgb(82, 154, 107, 0) : Color.FromArgb(89, 255, 222, 89);
                fore = light ? Color.FromArgb(0x8a, 0x5f, 0x00) : Color.FromArgb(0xff, 0xde, 0x59);
            }
            else if (pill.IsRemote)
            {
                fill = light ? Color.FromArgb(23, 10, 143, 176) : Color.FromArgb(26, 52, 200, 235);
                border = light ? Color.FromArgb(82, 10, 143, 176) : Color.FromArgb(77, 52, 200, 235);
                fore = light ? Color.FromArgb(0x0a, 0x6f, 0x8a) : Color.FromArgb(217, 52, 200, 235);
            }
            else
            {
                fill = light ? Color.FromArgb(33, 10, 143, 176) : Color.FromArgb(46, 52, 200, 235);
                border = light ? Color.FromArgb(102, 10, 143, 176) : Color.FromArgb(115, 52, 200, 235);
                fore = p.Lane;
            }

            Draw.FillRounded(g, bounds, 4f, ThemePalette.Flatten(fill, surface));
            Draw.DrawRounded(g, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1), 4f, ThemePalette.Flatten(border, surface));

            int x = bounds.X + 8;
            if (pill.IsLocal)
            {
                IconCache.DrawLeft(g, Icons.Laptop, 11, fore, new Rectangle(x, bounds.Y, 11, bounds.Height));
                x += 11 + 5;
            }
            int textRight = bounds.Right - 8 - (pill.OnRemoteToo ? 11 + 5 : 0);
            Draw.Text(g, pill.Text, Fonts.Ui(11.5f, true), new Rectangle(x, bounds.Y, Math.Max(0, textRight - x), bounds.Height), fore, Draw.LeftMiddle);
            if (pill.OnRemoteToo)
            {
                // The cloud says the same commit is on the remote, so nothing is waiting to be pushed.
                IconCache.DrawLeft(g, Icons.Cloud, 11, fore, new Rectangle(textRight + 5, bounds.Y, 11, bounds.Height));
            }
        }

        /// <summary>Ref pill under a client point, or null.</summary>
        public RefInfo RefAt(Point point)
        {
            int index = RowIndexAt(point);
            if (index < 0 || _filter.Length > 0) return null;
            var row = _rows[index];
            if (row.Commit == null || row.Commit.Refs.Count == 0) return null;
            int centerY = RowTop(index) + _rowHeight / 2;
            int pillX = PillStart;
            int limit = CommitLeft - 6;
            foreach (var pill in BuildPills(row))
            {
                int width = PillWidth(pill);
                if (pillX + width > limit) break;
                if (new Rectangle(pillX, centerY - 10, width, 20).Contains(point)) return pill.Primary;
                pillX += width + 5;
            }
            return null;
        }

        public GraphRow RowAt(Point point)
        {
            int index = RowIndexAt(point);
            return index < 0 ? null : _rows[index];
        }

        // ------------------------------------------------------------------ dates

        public string FormatDate(DateTimeOffset date)
        {
            var local = date.ToLocalTime();
            if (_relativeDates) return Relative(local);
            var now = DateTimeOffset.Now;
            if (local.Date == now.Date) return "Today " + local.ToString("HH:mm", CultureInfo.CurrentCulture);
            if (local.Date == now.Date.AddDays(-1)) return "Yesterday " + local.ToString("HH:mm", CultureInfo.CurrentCulture);
            if (local.Year == now.Year) return local.ToString("MMM d, HH:mm", CultureInfo.CurrentCulture);
            return local.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture);
        }

        public static string Relative(DateTimeOffset value)
        {
            var span = DateTimeOffset.Now - value;
            if (span.TotalMinutes < 1) return "just now";
            if (span.TotalMinutes < 60) return Plural((int)span.TotalMinutes, "minute") + " ago";
            if (span.TotalHours < 24) return Plural((int)span.TotalHours, "hour") + " ago";
            if (span.TotalDays < 7) return Plural((int)span.TotalDays, "day") + " ago";
            if (span.TotalDays < 35) return Plural((int)(span.TotalDays / 7), "week") + " ago";
            if (span.TotalDays < 365) return Plural((int)(span.TotalDays / 30), "month") + " ago";
            return Plural((int)(span.TotalDays / 365), "year") + " ago";
        }

        private static string Plural(int count, string unit) => count + " " + unit + (count == 1 ? string.Empty : "s");

        // ------------------------------------------------------------------ events

        private void OnRowRightClick(object sender, RowMouseEventArgs e)
        {
            var row = e.Index >= 0 && e.Index < _rows.Count ? _rows[e.Index] : null;
            RowContextMenuRequested?.Invoke(this, new HistoryRowEventArgs(row, RefAt(e.Location), PointToScreen(e.Location)));
        }

        private void OnRowDoubleClick(object sender, RowMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.Index < 0 || e.Index >= _rows.Count) return;
            var row = _rows[e.Index];
            var reference = RefAt(e.Location);
            var args = new HistoryRowEventArgs(row, reference, PointToScreen(e.Location));
            if (reference != null) RefActivated?.Invoke(this, args);
            else RowActivated?.Invoke(this, args);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            Cursor = RefAt(e.Location) != null ? Cursors.Hand : Cursors.Default;
        }
    }
}
