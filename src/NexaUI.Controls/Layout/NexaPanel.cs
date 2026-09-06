using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed WinForms <see cref="Panel"/> provided by NexaUI. Inherits the native
/// <see cref="Panel"/> so all layout, docking, scrolling, and designer behavior is
/// preserved. Adds a themed background, an optional border with rounded corners, and
/// full theme integration (light/dark).
/// </summary>
[DefaultEvent(nameof(Paint))]
[DefaultProperty(nameof(SurfaceStyle))]
public class NexaPanel : Panel
{
    private NexaPanelSurfaceStyle _surface = NexaPanelSurfaceStyle.Default;
    private NexaBorderStyleEx _border = NexaBorderStyleEx.None;
    private int _cornerRadiusDips = 2;
    private int _borderThicknessDips = 1;
    private int _shadowDepthDips = 0;
    private bool _shadowEnabled;

    /// <summary>Initializes a new instance of the <see cref="NexaPanel"/> class.</summary>
    public NexaPanel()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Surface style of the panel.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaPanelSurfaceStyle.Default)]
    [Description("Surface style of the panel (default, surface, elevated, transparent).")]
    public NexaPanelSurfaceStyle SurfaceStyle
    {
        get => _surface;
        set { _surface = value; RefreshColors(); Invalidate(); }
    }

    /// <summary>Border style of the panel.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaBorderStyleEx.None)]
    [Description("Border style of the panel.")]
    public NexaBorderStyleEx BorderStyleEx
    {
        get => _border;
        set { _border = value; Invalidate(); }
    }

    /// <summary>Border color. Defaults to the theme Border role.</summary>
    [Category("NexaUI")]
    [Description("Border color. Defaults to the theme Border role when left at its default value.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WinForms", "WFO1000", Justification = "Themed property; theme drives the value by default.")]
    public Color BorderColor { get; set; } = Color.Empty;

    /// <summary>Border thickness in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(1)]
    [Description("Border thickness in device-independent pixels.")]
    public int BorderThickness
    {
        get => _borderThicknessDips;
        set { _borderThicknessDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Corner radius in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(2)]
    [Description("Corner radius in device-independent pixels.")]
    public int CornerRadius
    {
        get => _cornerRadiusDips;
        set { _cornerRadiusDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Whether a subtle drop shadow is drawn behind the panel.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether a subtle drop shadow is drawn behind the panel.")]
    public bool ShadowEnabled
    {
        get => _shadowEnabled;
        set { _shadowEnabled = value; Invalidate(); }
    }

    /// <summary>Shadow depth in DIPs (0–8).</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Shadow depth in device-independent pixels (0–8).")]
    public int ShadowDepth
    {
        get => _shadowDepthDips;
        set { _shadowDepthDips = Math.Clamp(value, 0, 8); Invalidate(); }
    }

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => ApplyTheme(e.Current);

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyTheme(theme));
            return;
        }

        var palette = theme.Palette;
        var bg = _surface switch
        {
            NexaPanelSurfaceStyle.Surface => (Color)palette[NexaColorRole.Surface].Value,
            NexaPanelSurfaceStyle.Elevated => (Color)palette[NexaColorRole.SurfaceVariant].Value,
            NexaPanelSurfaceStyle.Transparent => Color.Transparent,
            _ => (Color)palette[NexaColorRole.Background].Value
        };
        BackColor = bg;
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var dpi = CurrentDpi();
        var palette = ThemeManager.Current.Palette;
        var w = Width;
        var h = Height;
        if (w <= 0 || h <= 0) return;

        var radius = NexaDpi.Scale(_cornerRadiusDips, dpi);
        var borderThickness = NexaDpi.Scale(_borderThicknessDips, dpi);
        var shadowDepth = _shadowEnabled ? NexaDpi.Scale(_shadowDepthDips, dpi) : 0;

        var rect = new Rectangle(shadowDepth, shadowDepth, w - shadowDepth, h - shadowDepth);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        if (_surface == NexaPanelSurfaceStyle.Transparent)
        {
            // Nothing to fill — leave as-is (BackColor is transparent).
        }
        else
        {
            using var path = CreateRoundedPathInternal(rect, radius);
            using var brush = new SolidBrush(BackColor);
            g.FillPath(brush, path);
        }

        if (_border == NexaBorderStyleEx.Solid && borderThickness > 0)
        {
            var borderColor = BorderColor.IsEmpty
                ? (Color)palette[NexaColorRole.Border].Value
                : BorderColor;
            using var path = CreateRoundedPathInternal(rect, radius);
            using var pen = new Pen(borderColor, borderThickness);
            g.DrawPath(pen, path);
        }

        if (shadowDepth > 0)
        {
            DrawShadow(g, rect, radius, shadowDepth, (Color)palette[NexaColorRole.Border].Value);
        }
    }

    private void DrawShadow(Graphics g, Rectangle rect, int radius, int depth, Color shadowColor)
    {
        // Soft, multi-layer shadow that fades with distance.
        var baseAlpha = (int)(shadowColor.A * 0.25f);
        for (var i = depth; i >= 1; i--)
        {
            var offset = i;
            var bounds = new Rectangle(rect.X + offset, rect.Y + offset, rect.Width, rect.Height);
            var alpha = (int)(baseAlpha * (1f - (i - 1) / (float)depth));
            if (alpha <= 0) continue;
            using var p = CreateRoundedPathInternal(bounds, radius);
            using var pen = new Pen(Color.FromArgb(alpha, shadowColor), 1f);
            g.DrawPath(pen, p);
        }
    }

    internal static GraphicsPath CreateRoundedPathInternal(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
        {
            path.AddRectangle(rect);
            return path;
        }
        var d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>
    /// Shared rounded-rectangle path helper. Internal so other layout controls
    /// (NexaCard, NexaFlowPanel, NexaTablePanel) can reuse it.
    /// </summary>
    internal static GraphicsPath CreateRoundedPathPublic(Rectangle rect, int radius) => CreateRoundedPathInternal(rect, radius);
}
