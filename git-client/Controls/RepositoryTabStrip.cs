using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Controls
{
    /// <summary>One repository tab: the repository name plus its current branch.</summary>
    public sealed class RepositoryTab
    {
        public string Title { get; set; }
        public string Branch { get; set; }
        public string ToolTip { get; set; }
        public object Tag { get; set; }
    }

    public sealed class TabEventArgs : EventArgs
    {
        public TabEventArgs(int index) { Index = index; }
        public int Index { get; }
    }

    /// <summary>
    /// The 38 px tab strip from the redesign: 30 px pill tabs carrying the repository name and its
    /// branch, a close button on the active tab (and on hover elsewhere), and a trailing + button.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultEvent("SelectedIndexChanged")]
    public sealed class RepositoryTabStrip : ModernControl
    {
        private const int StripPadding = 12;
        private const int TabHeight = 30;
        private const int TabGap = 4;
        private const int TabPaddingLeft = 12;
        private const int TabPaddingRight = 6;
        private const int CloseSize = 20;
        private const int AddSize = 30;

        private readonly List<RepositoryTab> _tabs = new List<RepositoryTab>();
        private int _selectedIndex = -1;
        private int _hotIndex = -1;
        private bool _hotClose;
        private bool _hotAdd;

        public event EventHandler SelectedIndexChanged;
        public event EventHandler<TabEventArgs> TabCloseRequested;
        public event EventHandler<TabEventArgs> TabContextMenuRequested;
        public event EventHandler AddRequested;

        public RepositoryTabStrip()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw | ControlStyles.SupportsTransparentBackColor, true);
            TabStop = false;
            BackColor = Color.Transparent;
            Height = 38;
            Theme.Changed += (s, e) => Invalidate();
        }

        [Browsable(false)]
        public IList<RepositoryTab> Tabs => _tabs;

        [Browsable(false)]
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                int clamped = _tabs.Count == 0 ? -1 : Math.Max(0, Math.Min(_tabs.Count - 1, value));
                if (clamped == _selectedIndex) return;
                _selectedIndex = clamped;
                Invalidate();
                SelectedIndexChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>Sets the selected index without raising the change event.</summary>
        public void SetSelectedIndexQuiet(int index)
        {
            _selectedIndex = _tabs.Count == 0 ? -1 : Math.Max(-1, Math.Min(_tabs.Count - 1, index));
            Invalidate();
        }

        public void Refresh(IEnumerable<RepositoryTab> tabs, int selectedIndex)
        {
            _tabs.Clear();
            if (tabs != null) _tabs.AddRange(tabs);
            _selectedIndex = _tabs.Count == 0 ? -1 : Math.Max(0, Math.Min(_tabs.Count - 1, selectedIndex));
            Invalidate();
        }

        // ------------------------------------------------------------------ geometry

        private int TabWidth(int index)
        {
            var tab = _tabs[index];
            bool active = index == _selectedIndex;
            int width = TabPaddingLeft;
            width += Draw.MeasureWidth(tab.Title ?? string.Empty, Fonts.Ui(13f, active));
            if (!string.IsNullOrEmpty(tab.Branch)) width += 8 + Draw.MeasureWidth(tab.Branch, Fonts.Ui(12f));
            width += 8 + CloseSize + TabPaddingRight;
            return Math.Min(280, width);
        }

        private Rectangle TabBounds(int index)
        {
            int x = StripPadding;
            for (int i = 0; i < _tabs.Count; i++)
            {
                int w = TabWidth(i);
                if (i == index) return new Rectangle(x, (Height - TabHeight) / 2, w, TabHeight);
                x += w + TabGap;
            }
            return Rectangle.Empty;
        }

        private Rectangle AddBounds()
        {
            int x = StripPadding;
            for (int i = 0; i < _tabs.Count; i++) x += TabWidth(i) + TabGap;
            return new Rectangle(x, (Height - AddSize) / 2, AddSize, AddSize);
        }

        private Rectangle CloseBounds(int index)
        {
            var bounds = TabBounds(index);
            if (bounds.IsEmpty) return Rectangle.Empty;
            return new Rectangle(bounds.Right - TabPaddingRight - CloseSize, bounds.Y + (TabHeight - CloseSize) / 2, CloseSize, CloseSize);
        }

        public int TabIndexAt(Point point)
        {
            for (int i = 0; i < _tabs.Count; i++)
            {
                if (TabBounds(i).Contains(point)) return i;
            }
            return -1;
        }

        // ------------------------------------------------------------------ painting

        private Color ParentSurface()
        {
            for (var c = Parent; c != null; c = c.Parent)
            {
                var surface = c as SurfacePanel;
                if (surface != null) return surface.SurfaceColor;
            }
            return Theme.Palette.Background;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var p = Theme.Palette;
            var surface = ParentSurface();
            Draw.Fill(g, ClientRectangle, surface);

            for (int i = 0; i < _tabs.Count; i++)
            {
                var bounds = TabBounds(i);
                if (bounds.IsEmpty || bounds.X > Width) continue;
                bool active = i == _selectedIndex;
                bool hot = i == _hotIndex;

                if (active)
                {
                    Draw.FillRounded(g, bounds, 5f, p.FillOn(surface));
                    Draw.DrawRounded(g, new Rectangle(bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1), 5f, p.StrokeOn(surface));
                }
                else if (hot)
                {
                    Draw.FillRounded(g, bounds, 5f, p.HoverOn(surface));
                }

                var tab = _tabs[i];
                int x = bounds.X + TabPaddingLeft;
                int right = bounds.Right - (TabPaddingRight + CloseSize + 8);

                var titleFont = Fonts.Ui(13f, active);
                var titleColor = active ? p.Foreground : p.Foreground2;
                int titleWidth = Math.Min(Draw.MeasureWidth(tab.Title ?? string.Empty, titleFont), Math.Max(0, right - x));
                Draw.Text(g, tab.Title, titleFont, new Rectangle(x, bounds.Y, titleWidth, bounds.Height), titleColor, Draw.LeftMiddle);
                x += titleWidth + 8;

                if (!string.IsNullOrEmpty(tab.Branch) && x < right)
                {
                    Draw.Text(g, tab.Branch, Fonts.Ui(12f), new Rectangle(x, bounds.Y, Math.Max(0, right - x), bounds.Height), p.Foreground3, Draw.LeftMiddle);
                }

                var close = CloseBounds(i);
                bool hotClose = _hotClose && i == _hotIndex;
                if (hotClose) Draw.FillRounded(g, close, 4f, p.Fill2On(surface));
                IconCache.DrawCentered(g, Icons.Cross, 9, hotClose ? p.Foreground : p.Foreground3,
                    close.X + close.Width / 2, close.Y + close.Height / 2);
            }

            var add = AddBounds();
            if (add.X + add.Width <= Width)
            {
                if (_hotAdd) Draw.FillRounded(g, add, 5f, p.HoverOn(surface));
                IconCache.DrawCentered(g, Icons.Plus, 14, p.Foreground2, add.X + add.Width / 2, add.Y + add.Height / 2);
            }
        }

        // ------------------------------------------------------------------ input

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            int index = TabIndexAt(e.Location);
            bool hotClose = index >= 0 && CloseBounds(index).Contains(e.Location);
            bool hotAdd = AddBounds().Contains(e.Location);
            if (index != _hotIndex || hotClose != _hotClose || hotAdd != _hotAdd)
            {
                _hotIndex = index;
                _hotClose = hotClose;
                _hotAdd = hotAdd;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hotIndex != -1 || _hotAdd || _hotClose)
            {
                _hotIndex = -1;
                _hotClose = false;
                _hotAdd = false;
                Invalidate();
            }
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Middle)
            {
                int middle = TabIndexAt(e.Location);
                if (middle >= 0) TabCloseRequested?.Invoke(this, new TabEventArgs(middle));
                return;
            }
            if (e.Button != MouseButtons.Left) return;
            if (AddBounds().Contains(e.Location)) return;   // the menu opens on mouse up

            int tab = TabIndexAt(e.Location);
            if (tab < 0) return;
            if (CloseBounds(tab).Contains(e.Location))
            {
                TabCloseRequested?.Invoke(this, new TabEventArgs(tab));
                return;
            }
            SelectedIndex = tab;
        }

        // Menus open on mouse up: a pop-up shown while the button is still down closes again as
        // soon as it is released.
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            if (e.Button == MouseButtons.Right)
            {
                int index = TabIndexAt(e.Location);
                if (index >= 0) TabContextMenuRequested?.Invoke(this, new TabEventArgs(index));
                return;
            }
            if (e.Button == MouseButtons.Left && AddBounds().Contains(e.Location))
            {
                AddRequested?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
