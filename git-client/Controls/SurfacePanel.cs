using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using GitClient.Services;
using ModernWinForms;

namespace GitClient.Controls
{
    /// <summary>Which design token supplies a control's background.</summary>
    public enum SurfaceKind
    {
        /// <summary>Window chrome and the body base (--bg).</summary>
        Base,
        /// <summary>Cards and the command bar (--layer).</summary>
        Layer,
        /// <summary>A card: --layer plus a 1 px --cardstroke border and a corner radius.</summary>
        Card,
        /// <summary>Control fill (--fill) with a 1 px --stroke border.</summary>
        Fill,
        /// <summary>Paints nothing; the parent shows through.</summary>
        None,
        /// <summary>Uses <see cref="SurfacePanel.CustomFill"/> and <see cref="SurfacePanel.CustomBorder"/>.</summary>
        Custom,
    }

    /// <summary>
    /// The layered surface from the redesign: a container panel that paints one of the design
    /// tokens, with an optional corner radius and hairline top/bottom dividers. Used for the
    /// command bar, every card, and the banded regions inside them.
    /// </summary>
    [ToolboxItem(true)]
    [DefaultProperty("Surface")]
    public class SurfacePanel : ModernPanel
    {
        private SurfaceKind _surface = SurfaceKind.Card;
        private int _cornerRadius = 8;
        private bool _topDivider;
        private bool _bottomDivider;
        private bool _rightDivider;
        private bool _topCardStroke;

        public SurfacePanel()
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.ResizeRedraw, true);
            Theme.Changed += OnThemeChanged;
        }

        [Category("Appearance"), DefaultValue(SurfaceKind.Card)]
        public SurfaceKind Surface
        {
            get => _surface;
            set { _surface = value; ApplyBackColor(); ApplyCornerRegion(); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(8)]
        public int CornerRadius
        {
            get => _cornerRadius;
            set { _cornerRadius = Math.Max(0, value); ApplyCornerRegion(); Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool TopDivider
        {
            get => _topDivider;
            set { _topDivider = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool BottomDivider
        {
            get => _bottomDivider;
            set { _bottomDivider = value; Invalidate(); }
        }

        [Category("Appearance"), DefaultValue(false)]
        public bool RightDivider
        {
            get => _rightDivider;
            set { _rightDivider = value; Invalidate(); }
        }

        /// <summary>Draws the top rule in --cardstroke instead of --div (the command bar's top edge).</summary>
        [Category("Appearance"), DefaultValue(false)]
        public bool TopCardStroke
        {
            get => _topCardStroke;
            set { _topCardStroke = value; Invalidate(); }
        }

        /// <summary>Fill used when <see cref="Surface"/> is Custom; alpha is blended onto the parent.</summary>
        [Category("Appearance")]
        public Color CustomFill { get; set; } = Color.Transparent;

        /// <summary>Border used when <see cref="Surface"/> is Custom.</summary>
        [Category("Appearance")]
        public Color CustomBorder { get; set; } = Color.Transparent;

        /// <summary>The colour this surface paints, for children that need to blend into it.</summary>
        [Browsable(false)]
        public Color SurfaceColor
        {
            get
            {
                var p = Theme.Palette;
                switch (_surface)
                {
                    case SurfaceKind.Layer:
                    case SurfaceKind.Card: return p.Layer;
                    case SurfaceKind.Fill: return p.FillOn(ParentSurfaceColor());
                    case SurfaceKind.Custom: return ThemePalette.Flatten(CustomFill, ParentSurfaceColor());
                    case SurfaceKind.None: return ParentSurfaceColor();
                    default: return p.Background;
                }
            }
        }

        private Color ParentSurfaceColor()
        {
            for (var c = Parent; c != null; c = c.Parent)
            {
                var surface = c as SurfacePanel;
                if (surface != null) return surface.SurfaceColor;
            }
            return Theme.Palette.Background;
        }

        protected virtual void OnThemeChanged(object sender, EventArgs e)
        {
            ApplyBackColor();
            Invalidate();
        }

        private void ApplyBackColor()
        {
            // Children that inherit BackColor (and ModernScrollBar's transparent track) need an
            // opaque value to blend against.
            var color = SurfaceColor;
            if (BackColor != color) BackColor = color;
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            ApplyBackColor();
            ApplyCornerRegion();
        }

        protected override void OnParentChanged(EventArgs e)
        {
            base.OnParentChanged(e);
            ApplyBackColor();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            ApplyCornerRegion();
        }

        /// <summary>
        /// Clips the panel to its rounded outline. Child controls paint opaque rectangles that
        /// would otherwise square off the corners of a card.
        /// </summary>
        private void ApplyCornerRegion()
        {
            bool rounded = _cornerRadius > 0 && (_surface == SurfaceKind.Card || _surface == SurfaceKind.Fill || _surface == SurfaceKind.Custom);
            if (!rounded)
            {
                if (Region != null) { Region.Dispose(); Region = null; }
                return;
            }
            if (Width <= 0 || Height <= 0) return;

            var previous = Region;
            using (var path = Draw.RoundedRect(new RectangleF(0, 0, Width, Height), _cornerRadius))
            {
                Region = new Region(path);
            }
            previous?.Dispose();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            var g = e.Graphics;
            var p = Theme.Palette;
            var bounds = new Rectangle(0, 0, Width, Height);

            {
                // Even a "None" surface paints: children with a transparent BackColor ask their
                // parent to draw the backdrop, and a panel that painted nothing would leave the
                // previous frame behind them.
                var fill = SurfaceColor;
                if (_surface == SurfaceKind.None)
                {
                    Draw.Fill(g, bounds, fill);
                }
                else if (_surface == SurfaceKind.Card)
                {
                    Draw.Card(g, new Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius, fill, p.CardStrokeOn(ParentSurfaceColor()));
                }
                else if (_surface == SurfaceKind.Fill)
                {
                    Draw.Card(g, new Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius, fill, p.StrokeOn(ParentSurfaceColor()));
                }
                else if (_surface == SurfaceKind.Custom)
                {
                    Draw.Card(g, new Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius, fill,
                        ThemePalette.Flatten(CustomBorder, ParentSurfaceColor()));
                }
                else
                {
                    Draw.Fill(g, bounds, fill);
                }
            }

            var divider = p.DividerOn(SurfaceColor);
            if (_topDivider) Draw.HLine(g, 0, 0, Width, _topCardStroke ? p.CardStrokeOn(SurfaceColor) : divider);
            if (_bottomDivider) Draw.HLine(g, 0, Height - 1, Width, divider);
            if (_rightDivider) Draw.VLine(g, Width - 1, 0, Height, divider);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) Theme.Changed -= OnThemeChanged;
            base.Dispose(disposing);
        }
    }
}
