using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using GitClient.Git;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Controls
{
    public enum DiffLayout
    {
        Unified,
        Split,
    }

    /// <summary>
    /// The diff pane: 20 px rows of monospace text, two 52 px right-aligned gutters separated from
    /// the code by a hairline, and full-width row bands in the add / remove / hunk tokens.
    /// Supports a unified and a side-by-side layout.
    /// </summary>
    [ToolboxItem(true)]
    public sealed class DiffViewControl : ModernControl
    {
        private const int RowHeight = 20;
        private const int GutterWidth = 52;
        private const int GutterPad1 = 10;
        private const int GutterPad2 = 12;
        private const int CodePad = 12;

        private sealed class Pair
        {
            public DiffLine Left;
            public DiffLine Right;
            public bool IsHunk;
            public string HunkText;
        }

        private DiffDocument _document;
        private DiffLayout _layout = DiffLayout.Unified;
        private readonly List<Pair> _pairs = new List<Pair>();
        private int _scrollY;
        private int _scrollX;
        private readonly ModernScrollBar _vScroll;
        private readonly ModernScrollBar _hScroll;
        private bool _syncingBars;
        private int _maxColumns;
        private float _fontSizePx = 12.5f;
        private string _emptyText = "Select a file to see its changes.";

        public DiffViewControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
            TabStop = true;
            BackColor = Color.Transparent;
            Theme.Changed += (s, e) => Invalidate();

            _vScroll = new ModernScrollBar { Orientation = Orientation.Vertical, Visible = false, TabStop = false, Width = ScrollBarWidth };
            _hScroll = new ModernScrollBar { Orientation = Orientation.Horizontal, Visible = false, TabStop = false, Height = ScrollBarWidth };
            _vScroll.Scroll += (s, e) => { if (!_syncingBars) { _scrollY = e.NewValue; SyncScrollBars(); Invalidate(); } };
            _hScroll.Scroll += (s, e) => { if (!_syncingBars) { _scrollX = e.NewValue; SyncScrollBars(); Invalidate(); } };
            Controls.Add(_vScroll);
            Controls.Add(_hScroll);
            ApplyScrollBarColors();
        }

        [Browsable(false)]
        public DiffDocument Document => _document;

        [Category("Appearance"), DefaultValue(DiffLayout.Unified)]
        public DiffLayout ViewLayout
        {
            get => _layout;
            set
            {
                if (_layout == value) return;
                _layout = value;
                Rebuild();
            }
        }

        [Category("Appearance"), DefaultValue(12.5f)]
        public float FontSizePx
        {
            get => _fontSizePx;
            set { _fontSizePx = Math.Max(8f, value); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue("Select a file to see its changes.")]
        public string EmptyText
        {
            get => _emptyText;
            set { _emptyText = value ?? string.Empty; Invalidate(); }
        }

        public void SetDocument(DiffDocument document)
        {
            _document = document;
            _scrollY = 0;
            _scrollX = 0;
            Rebuild();
        }

        public void Clear() => SetDocument(null);

        // ------------------------------------------------------------------ model

        private void Rebuild()
        {
            _pairs.Clear();
            _maxColumns = 0;
            if (_document != null)
            {
                if (_layout == DiffLayout.Unified) BuildUnified();
                else BuildSplit();
            }
            _scrollY = Math.Max(0, Math.Min(_scrollY, MaxScrollY));
            _scrollX = Math.Max(0, Math.Min(_scrollX, MaxScrollX));
            SyncScrollBars();
            Invalidate();
        }

        private void BuildUnified()
        {
            foreach (var line in _document.Lines)
            {
                _pairs.Add(new Pair
                {
                    Left = line,
                    IsHunk = line.Kind == DiffLineKind.HunkHeader,
                    HunkText = line.Kind == DiffLineKind.HunkHeader ? line.Text : null,
                });
                Track(line.Text);
            }
        }

        private void BuildSplit()
        {
            var lines = _document.Lines;
            int i = 0;
            while (i < lines.Count)
            {
                var line = lines[i];
                if (line.Kind == DiffLineKind.HunkHeader || line.Kind == DiffLineKind.Info || line.Kind == DiffLineKind.NoNewline)
                {
                    _pairs.Add(new Pair { IsHunk = line.Kind == DiffLineKind.HunkHeader, HunkText = line.Text, Left = line });
                    Track(line.Text);
                    i++;
                    continue;
                }
                if (line.Kind == DiffLineKind.Context)
                {
                    _pairs.Add(new Pair { Left = line, Right = line });
                    Track(line.Text);
                    i++;
                    continue;
                }

                // Pair a run of removals with the run of additions that follows it.
                var removed = new List<DiffLine>();
                while (i < lines.Count && lines[i].Kind == DiffLineKind.Removed) { removed.Add(lines[i]); Track(lines[i].Text); i++; }
                var added = new List<DiffLine>();
                while (i < lines.Count && lines[i].Kind == DiffLineKind.Added) { added.Add(lines[i]); Track(lines[i].Text); i++; }
                int count = Math.Max(removed.Count, added.Count);
                for (int k = 0; k < count; k++)
                {
                    _pairs.Add(new Pair
                    {
                        Left = k < removed.Count ? removed[k] : null,
                        Right = k < added.Count ? added[k] : null,
                    });
                }
            }
        }

        private void Track(string text)
        {
            int length = Expand(text).Length;
            if (length > _maxColumns) _maxColumns = length;
        }

        private static string Expand(string text)
        {
            if (string.IsNullOrEmpty(text)) return string.Empty;
            return text.IndexOf('\t') >= 0 ? text.Replace("\t", "    ") : text;
        }

        // ------------------------------------------------------------------ geometry

        private Font CodeFont => Fonts.Code(_fontSizePx);
        private int CharWidth => Math.Max(4, Draw.MeasureWidth("MMMMMMMMMM", CodeFont) / 10);

        private int VisibleRows => Math.Max(1, Height / RowHeight);
        private int ContentHeight => _pairs.Count * RowHeight;
        private int MaxScrollY => Math.Max(0, ContentHeight - Height);

        private int PaneWidth => _layout == DiffLayout.Split ? Math.Max(80, (Width - 1) / 2) : Width;
        private int CodeLeft => _layout == DiffLayout.Split ? GutterWidth + 1 + CodePad : GutterWidth * 2 + 1 + CodePad;
        private int CodeWidth => Math.Max(0, PaneWidth - CodeLeft);
        private int MaxScrollX => Math.Max(0, _maxColumns * CharWidth + 24 - CodeWidth);

        // ------------------------------------------------------------------ painting

        private Color Surface
        {
            get
            {
                for (var c = Parent; c != null; c = c.Parent)
                {
                    var surface = c as SurfacePanel;
                    if (surface != null) return surface.SurfaceColor;
                }
                return Theme.Palette.Layer;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var p = Theme.Palette;
            var surface = Surface;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            Draw.Fill(g, ClientRectangle, surface);

            if (_document == null || _pairs.Count == 0)
            {
                var text = _document == null ? _emptyText : (_document.Note ?? "No textual changes.");
                Draw.Text(g, text, Fonts.Ui(13f), new Rectangle(24, 0, Math.Max(0, Width - 48), Height), p.Foreground3,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
                return;
            }

            int first = _scrollY / RowHeight;
            int offset = -(_scrollY % RowHeight);
            int last = Math.Min(_pairs.Count - 1, first + VisibleRows + 1);
            var divider = p.DividerOn(surface);

            for (int i = first; i <= last; i++)
            {
                int y = offset + (i - first) * RowHeight;
                PaintPair(g, _pairs[i], y, surface, divider);
            }

            if (_layout == DiffLayout.Split)
            {
                Draw.VLine(g, PaneWidth, 0, Height, divider);
            }

            SurfacePanel.PaintRoundedParentCorners(g, this);
        }

        private void PaintPair(Graphics g, Pair pair, int y, Color surface, Color divider)
        {
            var p = Theme.Palette;

            if (pair.IsHunk)
            {
                Draw.Fill(g, new Rectangle(0, y, Width, RowHeight), p.FillOn(surface));
                Draw.Text(g, Expand(pair.HunkText), CodeFont,
                    new Rectangle(CodeLeft, y, Math.Max(0, Width - CodeLeft), RowHeight), p.DiffHunkFore, Draw.LeftMiddle);
                return;
            }

            if (_layout == DiffLayout.Unified)
            {
                PaintUnifiedRow(g, pair.Left, y, 0, Width, surface, divider, true);
                return;
            }

            PaintSplitCell(g, pair.Left, y, 0, PaneWidth, surface, divider, false);
            PaintSplitCell(g, pair.Right, y, PaneWidth + 1, PaneWidth, surface, divider, true);
        }

        private void KindColors(DiffLine line, Color surface, out Color back, out Color fore)
        {
            var p = Theme.Palette;
            if (line == null) { back = p.Fill2On(surface); fore = p.Foreground3; return; }
            switch (line.Kind)
            {
                case DiffLineKind.Added: back = p.DiffAddBack; fore = p.DiffAddFore; break;
                case DiffLineKind.Removed: back = p.DiffDelBack; fore = p.DiffDelFore; break;
                case DiffLineKind.HunkHeader: back = p.FillOn(surface); fore = p.DiffHunkFore; break;
                case DiffLineKind.Info:
                case DiffLineKind.NoNewline: back = Color.Transparent; fore = p.Foreground3; break;
                default: back = Color.Transparent; fore = p.Foreground2; break;
            }
        }

        private void PaintUnifiedRow(Graphics g, DiffLine line, int y, int x, int width, Color surface, Color divider, bool bothGutters)
        {
            var p = Theme.Palette;
            Color back, fore;
            KindColors(line, surface, out back, out fore);
            if (back.A > 0) Draw.Fill(g, new Rectangle(x, y, width, RowHeight), back);

            var gutterFont = CodeFont;
            var oldText = line != null && line.OldLineNumber > 0 ? line.OldLineNumber.ToString() : string.Empty;
            var newText = line != null && line.NewLineNumber > 0 ? line.NewLineNumber.ToString() : string.Empty;

            Draw.Text(g, oldText, gutterFont, new Rectangle(x, y, GutterWidth - GutterPad1, RowHeight), p.Foreground3, Draw.RightMiddle);
            Draw.Text(g, newText, gutterFont, new Rectangle(x + GutterWidth, y, GutterWidth - GutterPad2, RowHeight), p.Foreground3, Draw.RightMiddle);
            Draw.VLine(g, x + GutterWidth * 2, y, RowHeight, divider);

            int codeStart = x + GutterWidth * 2 + 1 + CodePad;
            PaintCode(g, line, codeStart, y, Math.Max(0, x + width - codeStart), fore);
        }

        /// <summary>
        /// Draws one code line, clipped to its pane. The text starts left of the clip when the view
        /// is scrolled horizontally, so it is drawn unclipped by the renderer and clipped by GDI+.
        /// </summary>
        private void PaintCode(Graphics g, DiffLine line, int x, int y, int width, Color fore)
        {
            if (width <= 0) return;
            var clip = g.Clip;
            g.SetClip(new Rectangle(x, y, width, RowHeight), System.Drawing.Drawing2D.CombineMode.Intersect);
            Draw.Text(g, Expand(line?.Text), CodeFont, new Rectangle(x - _scrollX, y, width + _scrollX, RowHeight), fore,
                TextFormatFlags.Left | TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.NoPrefix | TextFormatFlags.NoPadding | TextFormatFlags.NoClipping);
            g.Clip = clip;
        }

        private void PaintSplitCell(Graphics g, DiffLine line, int y, int x, int width, Color surface, Color divider, bool useNewNumbers)
        {
            var p = Theme.Palette;
            Color back, fore;
            KindColors(line, surface, out back, out fore);
            if (back.A > 0) Draw.Fill(g, new Rectangle(x, y, width, RowHeight), back);

            var number = line == null
                ? string.Empty
                : (useNewNumbers ? (line.NewLineNumber > 0 ? line.NewLineNumber.ToString() : string.Empty)
                                 : (line.OldLineNumber > 0 ? line.OldLineNumber.ToString() : string.Empty));
            Draw.Text(g, number, CodeFont, new Rectangle(x, y, GutterWidth - GutterPad2, RowHeight), p.Foreground3, Draw.RightMiddle);
            Draw.VLine(g, x + GutterWidth, y, RowHeight, divider);

            int codeStart = x + GutterWidth + 1 + CodePad;
            PaintCode(g, line, codeStart, y, Math.Max(0, x + width - codeStart), fore);
        }

        /// <summary>Thickness of the library scroll bars docked at the right and bottom edges.</summary>
        private const int ScrollBarWidth = 12;

        private void ApplyScrollBarColors()
        {
            var p = Theme.Palette;
            foreach (var bar in new[] { _vScroll, _hScroll })
            {
                if (bar == null) continue;
                bar.UseParentSkin = false;
                bar.ScrollBarColors.TrackColor = Color.Transparent;
                bar.ScrollBarColors.ThumbColor = p.Fill2On(Surface);
                bar.ScrollBarColors.ThumbHoverColor = p.Foreground3;
                bar.BackColor = Surface;
            }
        }

        /// <summary>Puts the two bars where the content needs them and matches their ranges.</summary>
        private void SyncScrollBars()
        {
            if (_vScroll == null || _hScroll == null) return;
            bool vertical = ContentHeight > Height && Height > 0;
            bool horizontal = MaxScrollX > 0 && Width > 0;

            _syncingBars = true;
            try
            {
                _vScroll.Visible = vertical;
                _hScroll.Visible = horizontal;

                int bottomInset = SurfacePanel.RoundedParentBottomInset(this);
                if (vertical)
                {
                    int height = Math.Max(0, Height - (horizontal ? ScrollBarWidth : 0) - bottomInset);
                    _vScroll.SetBounds(Width - ScrollBarWidth, 0, ScrollBarWidth, height);
                    _vScroll.Minimum = 0;
                    _vScroll.Maximum = Math.Max(0, MaxScrollY + height);
                    _vScroll.LargeChange = height;
                    _vScroll.SmallChange = RowHeight;
                    _vScroll.Value = Math.Max(0, Math.Min(_scrollY, _vScroll.MaximumScrollValue));
                }
                if (horizontal)
                {
                    int width = Math.Max(0, Width - (vertical ? ScrollBarWidth : 0) - bottomInset);
                    _hScroll.SetBounds(bottomInset > 0 ? bottomInset : 0, Height - ScrollBarWidth - (bottomInset > 0 ? 1 : 0), width, ScrollBarWidth);
                    _hScroll.Minimum = 0;
                    _hScroll.Maximum = Math.Max(0, MaxScrollX + width);
                    _hScroll.LargeChange = width;
                    _hScroll.SmallChange = CharWidth * 4;
                    _hScroll.Value = Math.Max(0, Math.Min(_scrollX, _hScroll.MaximumScrollValue));
                }
            }
            finally
            {
                _syncingBars = false;
            }
        }

        // ------------------------------------------------------------------ input

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int notches = e.Delta / SystemInformation.MouseWheelScrollDelta;
            if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                _scrollX = Math.Max(0, Math.Min(MaxScrollX, _scrollX - notches * CharWidth * 6));
            }
            else
            {
                _scrollY = Math.Max(0, Math.Min(MaxScrollY, _scrollY - notches * RowHeight * 3));
            }
            SyncScrollBars();
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up: case Keys.Down: case Keys.PageUp: case Keys.PageDown:
                case Keys.Home: case Keys.End: case Keys.Left: case Keys.Right:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            switch (e.KeyCode)
            {
                case Keys.Up: _scrollY = Math.Max(0, _scrollY - RowHeight); break;
                case Keys.Down: _scrollY = Math.Min(MaxScrollY, _scrollY + RowHeight); break;
                case Keys.PageUp: _scrollY = Math.Max(0, _scrollY - Height); break;
                case Keys.PageDown: _scrollY = Math.Min(MaxScrollY, _scrollY + Height); break;
                case Keys.Home: _scrollY = 0; break;
                case Keys.End: _scrollY = MaxScrollY; break;
                case Keys.Left: _scrollX = Math.Max(0, _scrollX - CharWidth * 4); break;
                case Keys.Right: _scrollX = Math.Min(MaxScrollX, _scrollX + CharWidth * 4); break;
                case Keys.C:
                    if (e.Control) CopyAll();
                    return;
                default: return;
            }
            e.Handled = true;
            Invalidate();
        }

        private void CopyAll()
        {
            if (_document == null) return;
            var sb = new StringBuilder();
            foreach (var line in _document.Lines)
            {
                switch (line.Kind)
                {
                    case DiffLineKind.Added: sb.Append('+'); break;
                    case DiffLineKind.Removed: sb.Append('-'); break;
                    case DiffLineKind.Context: sb.Append(' '); break;
                }
                sb.AppendLine(line.Text);
            }
            try { Clipboard.SetText(sb.ToString()); } catch { }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            _scrollY = Math.Max(0, Math.Min(_scrollY, MaxScrollY));
            _scrollX = Math.Max(0, Math.Min(_scrollX, MaxScrollX));
            SyncScrollBars();
        }
    }
}
