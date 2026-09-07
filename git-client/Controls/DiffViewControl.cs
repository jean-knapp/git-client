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
        private const int ThumbSize = 4;
        private const int ThumbInset = 4;
        private const int ThumbMargin = 6;
        private const int MinThumb = 24;

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
        private int _maxColumns;
        private float _fontSizePx = 12.5f;
        private string _emptyText = "Select a file to see its changes.";
        private bool _draggingV;
        private bool _draggingH;
        private int _dragOffset;

        public DiffViewControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
            TabStop = true;
            BackColor = Color.Transparent;
            Theme.Changed += (s, e) => Invalidate();
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

            PaintThumbs(g, surface);
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

        private void PaintThumbs(Graphics g, Color surface)
        {
            var p = Theme.Palette;
            var v = VerticalThumb();
            if (!v.IsEmpty) Draw.FillRounded(g, v, ThumbSize / 2f, p.Fill2On(surface));
            var h = HorizontalThumb();
            if (!h.IsEmpty) Draw.FillRounded(g, h, ThumbSize / 2f, p.Fill2On(surface));
        }

        private Rectangle VerticalThumb()
        {
            if (ContentHeight <= Height || Height <= 0) return Rectangle.Empty;
            int track = Height - ThumbMargin * 2;
            if (track <= MinThumb) return Rectangle.Empty;
            int height = Math.Max(MinThumb, (int)((long)track * Height / ContentHeight));
            int travel = track - height;
            int offset = MaxScrollY <= 0 ? 0 : (int)((long)travel * _scrollY / MaxScrollY);
            return new Rectangle(Width - ThumbInset - ThumbSize, ThumbMargin + offset, ThumbSize, height);
        }

        private Rectangle HorizontalThumb()
        {
            if (MaxScrollX <= 0 || Width <= 0) return Rectangle.Empty;
            int track = Width - ThumbMargin * 2;
            int content = _maxColumns * CharWidth + 24;
            if (track <= MinThumb || content <= 0) return Rectangle.Empty;
            int width = Math.Max(MinThumb, (int)((long)track * CodeWidth / content));
            int travel = track - width;
            int offset = (int)((long)travel * _scrollX / MaxScrollX);
            return new Rectangle(ThumbMargin + offset, Height - ThumbInset - ThumbSize, width, ThumbSize);
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
            Invalidate();
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();
            if (e.Button != MouseButtons.Left) return;
            var v = VerticalThumb();
            if (!v.IsEmpty && v.Contains(e.Location)) { _draggingV = true; _dragOffset = e.Y - v.Y; Capture = true; return; }
            var h = HorizontalThumb();
            if (!h.IsEmpty && h.Contains(e.Location)) { _draggingH = true; _dragOffset = e.X - h.X; Capture = true; }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_draggingV)
            {
                int track = Height - ThumbMargin * 2;
                var thumb = VerticalThumb();
                int travel = track - thumb.Height;
                if (travel > 0)
                {
                    int offset = Math.Max(0, Math.Min(travel, e.Y - _dragOffset - ThumbMargin));
                    _scrollY = (int)((long)offset * MaxScrollY / travel);
                    Invalidate();
                }
            }
            else if (_draggingH)
            {
                int track = Width - ThumbMargin * 2;
                var thumb = HorizontalThumb();
                int travel = track - thumb.Width;
                if (travel > 0)
                {
                    int offset = Math.Max(0, Math.Min(travel, e.X - _dragOffset - ThumbMargin));
                    _scrollX = (int)((long)offset * MaxScrollX / travel);
                    Invalidate();
                }
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (_draggingV || _draggingH)
            {
                _draggingV = false;
                _draggingH = false;
                Capture = false;
            }
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
        }
    }
}
