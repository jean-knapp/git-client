using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Controls
{
    /// <summary>State flags passed to <see cref="VirtualListControl.PaintRow"/>.</summary>
    [Flags]
    public enum RowState
    {
        None = 0,
        Hot = 1,
        Selected = 2,
        Focused = 4,
    }

    /// <summary>
    /// Base for the owner-drawn lists in the redesign. Handles variable row heights, wheel and
    /// drag scrolling, hover and selection, and paints the 4 px overlay scrollbar the design
    /// specifies (no track, 2 px radius, --fill2, inset 4 px from the right edge).
    /// </summary>
    [ToolboxItem(false)]
    public abstract class VirtualListControl : ModernControl
    {
        private const int ThumbWidth = 4;
        private const int ThumbInset = 4;
        private const int ThumbTopMargin = 6;
        private const int MinThumbHeight = 24;

        private int _scroll;
        private int _hotRow = -1;
        private readonly List<int> _selected = new List<int>();
        private int _anchor = -1;
        private bool _draggingThumb;
        private int _dragOffset;
        private bool _thumbHot;

        /// <summary>Raised when the selection changes through user input or code.</summary>
        public event EventHandler SelectionChanged;

        public event EventHandler<RowMouseEventArgs> RowClick;
        public event EventHandler<RowMouseEventArgs> RowDoubleClick;
        public event EventHandler<RowMouseEventArgs> RowRightClick;

        protected VirtualListControl()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.Selectable | ControlStyles.SupportsTransparentBackColor, true);
            TabStop = true;
            BackColor = Color.Transparent;
            Theme.Changed += OnThemeChanged;
        }

        protected ThemePalette P => Theme.Palette;

        /// <summary>Surface the rows are painted on, used to flatten translucent tokens.</summary>
        protected virtual Color RowSurface
        {
            get
            {
                for (var c = Parent; c != null; c = c.Parent)
                {
                    var surface = c as SurfacePanel;
                    if (surface != null) return surface.SurfaceColor;
                }
                return P.Layer;
            }
        }

        // ------------------------------------------------------------------ model

        protected abstract int RowCount { get; }
        protected abstract int GetRowHeight(int index);
        protected abstract void PaintRow(Graphics g, int index, Rectangle bounds, RowState state);

        /// <summary>Height of the non-scrolling header drawn at the top; 0 for none.</summary>
        protected virtual int HeaderHeight => 0;
        protected virtual void PaintHeader(Graphics g, Rectangle bounds) { }

        /// <summary>Rows that cannot be selected, such as group bands.</summary>
        protected virtual bool IsSelectable(int index) => true;

        protected virtual bool MultiSelect => false;

        /// <summary>Message shown centred when the list has no rows.</summary>
        [Category("Appearance"), DefaultValue(null)]
        public string EmptyText { get; set; }

        // ------------------------------------------------------------------ geometry

        [Browsable(false)]
        public int ScrollOffset
        {
            get => _scroll;
            set
            {
                int clamped = Math.Max(0, Math.Min(value, MaxScroll));
                if (clamped == _scroll) return;
                _scroll = clamped;
                Invalidate();
            }
        }

        protected int ViewportTop => HeaderHeight;
        protected int ViewportHeight => Math.Max(0, Height - HeaderHeight);

        /// <summary>Height of every row, for callers that size a card to its list.</summary>
        [Browsable(false)]
        public int RowsHeight => TotalHeight;

        protected int TotalHeight
        {
            get
            {
                int total = 0;
                int count = RowCount;
                for (int i = 0; i < count; i++) total += GetRowHeight(i);
                return total;
            }
        }

        private int MaxScroll => Math.Max(0, TotalHeight - ViewportHeight);

        /// <summary>Y of a row's top edge in client coordinates.</summary>
        protected int RowTop(int index)
        {
            int y = ViewportTop - _scroll;
            for (int i = 0; i < index; i++) y += GetRowHeight(i);
            return y;
        }

        public int RowIndexAt(Point point)
        {
            if (point.Y < ViewportTop) return -1;
            int y = ViewportTop - _scroll;
            int count = RowCount;
            for (int i = 0; i < count; i++)
            {
                int h = GetRowHeight(i);
                if (point.Y >= y && point.Y < y + h) return i;
                y += h;
            }
            return -1;
        }

        public void EnsureVisible(int index)
        {
            if (index < 0 || index >= RowCount) return;
            int top = 0;
            for (int i = 0; i < index; i++) top += GetRowHeight(i);
            int height = GetRowHeight(index);
            if (top < _scroll) ScrollOffset = top;
            else if (top + height > _scroll + ViewportHeight) ScrollOffset = top + height - ViewportHeight;
        }

        // ------------------------------------------------------------------ selection

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selected.Count > 0 ? _selected[_selected.Count - 1] : -1;
            set => SetSelection(value, true);
        }

        [Browsable(false)]
        public IReadOnlyList<int> SelectedIndices => _selected;

        public void SetSelection(int index, bool raiseEvent)
        {
            _selected.Clear();
            if (index >= 0 && index < RowCount && IsSelectable(index)) _selected.Add(index);
            _anchor = index;
            Invalidate();
            if (raiseEvent) SelectionChanged?.Invoke(this, EventArgs.Empty);
        }

        public void ClearSelection() => SetSelection(-1, true);

        protected bool IsSelected(int index) => _selected.Contains(index);

        // ------------------------------------------------------------------ painting

        protected virtual void OnThemeChanged(object sender, EventArgs e) => Invalidate();

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (HeaderHeight > 0) PaintHeader(g, new Rectangle(0, 0, Width, HeaderHeight));

            int count = RowCount;
            if (count == 0 && !string.IsNullOrEmpty(EmptyText))
            {
                Draw.Text(g, EmptyText, Fonts.Ui(13f), new Rectangle(24, ViewportTop, Math.Max(0, Width - 48), ViewportHeight),
                    P.Foreground3, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.WordBreak | TextFormatFlags.NoPrefix);
                return;
            }

            var clip = new Rectangle(0, ViewportTop, Width, ViewportHeight);
            var saved = g.Clip;
            g.SetClip(clip);

            int y = ViewportTop - _scroll;
            for (int i = 0; i < count; i++)
            {
                int h = GetRowHeight(i);
                if (y + h > ViewportTop && y < Height)
                {
                    var state = RowState.None;
                    if (i == _hotRow) state |= RowState.Hot;
                    if (IsSelected(i)) state |= RowState.Selected;
                    if (Focused) state |= RowState.Focused;
                    PaintRow(g, i, new Rectangle(0, y, Width, h), state);
                }
                y += h;
                if (y >= Height) break;
            }

            g.Clip = saved;
            PaintScrollThumb(g);
        }

        private void PaintScrollThumb(Graphics g)
        {
            var thumb = ThumbBounds();
            if (thumb.IsEmpty) return;
            Draw.FillRounded(g, thumb, ThumbWidth / 2f,
                _thumbHot || _draggingThumb ? P.Foreground3 : P.Fill2On(RowSurface));
        }

        private Rectangle ThumbBounds()
        {
            int total = TotalHeight;
            int viewport = ViewportHeight;
            if (total <= viewport || viewport <= 0) return Rectangle.Empty;
            int trackTop = ViewportTop + ThumbTopMargin;
            int trackHeight = viewport - ThumbTopMargin * 2;
            if (trackHeight <= MinThumbHeight) return Rectangle.Empty;
            int height = Math.Max(MinThumbHeight, (int)((long)trackHeight * viewport / total));
            int travel = trackHeight - height;
            int offset = MaxScroll <= 0 ? 0 : (int)((long)travel * _scroll / MaxScroll);
            return new Rectangle(Width - ThumbInset - ThumbWidth, trackTop + offset, ThumbWidth, height);
        }

        // ------------------------------------------------------------------ input

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            int notches = e.Delta / SystemInformation.MouseWheelScrollDelta;
            ScrollOffset = _scroll - notches * 3 * DefaultScrollStep;
            UpdateHot(e.Location);
        }

        /// <summary>Pixels one wheel step scrolls, per notch line.</summary>
        protected virtual int DefaultScrollStep => 20;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (_draggingThumb)
            {
                DragThumbTo(e.Y);
                return;
            }
            bool overThumb = ThumbBounds().Contains(e.Location);
            if (overThumb != _thumbHot) { _thumbHot = overThumb; Invalidate(); }
            UpdateHot(e.Location);
        }

        private void UpdateHot(Point point)
        {
            int index = RowIndexAt(point);
            if (index == _hotRow) return;
            _hotRow = index;
            Invalidate();
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hotRow != -1 || _thumbHot)
            {
                _hotRow = -1;
                _thumbHot = false;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            Focus();

            var thumb = ThumbBounds();
            if (e.Button == MouseButtons.Left && !thumb.IsEmpty && thumb.Contains(e.Location))
            {
                _draggingThumb = true;
                _dragOffset = e.Y - thumb.Y;
                Capture = true;
                Invalidate();
                return;
            }

            int index = RowIndexAt(e.Location);
            if (index < 0) return;

            if (e.Button == MouseButtons.Left)
            {
                if (!IsSelectable(index)) { OnRowClicked(index, e); return; }
                if (MultiSelect && (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    if (_selected.Contains(index)) _selected.Remove(index); else _selected.Add(index);
                    _anchor = index;
                    Invalidate();
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
                else if (MultiSelect && (ModifierKeys & Keys.Shift) == Keys.Shift && _anchor >= 0)
                {
                    _selected.Clear();
                    int a = Math.Min(_anchor, index), b = Math.Max(_anchor, index);
                    for (int i = a; i <= b; i++) if (IsSelectable(i)) _selected.Add(i);
                    Invalidate();
                    SelectionChanged?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    SetSelection(index, true);
                }
                OnRowClicked(index, e);
            }
            else if (e.Button == MouseButtons.Right)
            {
                if (IsSelectable(index) && !_selected.Contains(index)) SetSelection(index, true);
            }
        }

        protected virtual void OnRowClicked(int index, MouseEventArgs e)
        {
            RowClick?.Invoke(this, new RowMouseEventArgs(index, e.Location, e.Button));
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (_draggingThumb)
            {
                _draggingThumb = false;
                Capture = false;
                Invalidate();
                return;
            }
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right)
            {
                int index = RowIndexAt(e.Location);
                RowRightClick?.Invoke(this, new RowMouseEventArgs(index, e.Location, e.Button));
            }
        }

        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            int index = RowIndexAt(e.Location);
            if (index >= 0) RowDoubleClick?.Invoke(this, new RowMouseEventArgs(index, e.Location, e.Button));
        }

        private void DragThumbTo(int mouseY)
        {
            int viewport = ViewportHeight;
            int total = TotalHeight;
            if (total <= viewport) return;
            int trackTop = ViewportTop + ThumbTopMargin;
            int trackHeight = viewport - ThumbTopMargin * 2;
            var thumb = ThumbBounds();
            int travel = trackHeight - thumb.Height;
            if (travel <= 0) return;
            int offset = mouseY - _dragOffset - trackTop;
            ScrollOffset = (int)((long)Math.Max(0, Math.Min(travel, offset)) * MaxScroll / travel);
        }

        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Up: case Keys.Down: case Keys.PageUp: case Keys.PageDown:
                case Keys.Home: case Keys.End: case Keys.Space:
                    return true;
            }
            return base.IsInputKey(keyData);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            int count = RowCount;
            if (count == 0) return;
            int current = SelectedIndex;
            int target = current;
            switch (e.KeyCode)
            {
                case Keys.Up: target = PreviousSelectable(current); break;
                case Keys.Down: target = NextSelectable(current); break;
                case Keys.PageUp: target = Math.Max(0, current - Math.Max(1, ViewportHeight / Math.Max(1, GetRowHeight(0)))); break;
                case Keys.PageDown: target = Math.Min(count - 1, current + Math.Max(1, ViewportHeight / Math.Max(1, GetRowHeight(0)))); break;
                case Keys.Home: target = NextSelectable(-1); break;
                case Keys.End: target = PreviousSelectable(count); break;
                default: return;
            }
            e.Handled = true;
            if (target < 0 || target >= count) return;
            SetSelection(target, true);
            EnsureVisible(target);
        }

        private int NextSelectable(int from)
        {
            for (int i = from + 1; i < RowCount; i++) if (IsSelectable(i)) return i;
            return from;
        }

        private int PreviousSelectable(int from)
        {
            for (int i = from - 1; i >= 0; i--) if (IsSelectable(i)) return i;
            return from;
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ScrollOffset = _scroll;
        }

        protected override void OnGotFocus(EventArgs e) { base.OnGotFocus(e); Invalidate(); }
        protected override void OnLostFocus(EventArgs e) { base.OnLostFocus(e); Invalidate(); }

        /// <summary>Keeps the selection and scroll position across a data refresh.</summary>
        protected void RestoreState(int selectedIndex, int scroll)
        {
            _selected.Clear();
            if (selectedIndex >= 0 && selectedIndex < RowCount) _selected.Add(selectedIndex);
            _anchor = selectedIndex;
            _scroll = Math.Max(0, Math.Min(scroll, MaxScroll));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Theme.Changed -= OnThemeChanged;
            base.Dispose(disposing);
        }
    }

    public sealed class RowMouseEventArgs : EventArgs
    {
        public RowMouseEventArgs(int index, Point location, MouseButtons button)
        {
            Index = index;
            Location = location;
            Button = button;
        }

        public int Index { get; }
        public Point Location { get; }
        public MouseButtons Button { get; }
    }
}
