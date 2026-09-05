using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed divider line. Horizontal or vertical, with configurable thickness and stroke style.
/// Defaults to 1 logical pixel using the active theme's border color, and updates automatically
/// on theme changes.
/// </summary>
[DefaultProperty(nameof(Orientation))]
public class NexaSeparator : Control
{
    private NexaSeparatorOrientation _orientation = NexaSeparatorOrientation.Horizontal;
    private NexaSeparatorStyle _style = NexaSeparatorStyle.Solid;
    private int _thicknessInDips = 1;
    private Color _color = Color.Empty;

    public NexaSeparator()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.UserPaint |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        TabStop = false;
        BackColor = Color.Transparent;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
        SetDefaultSize();
    }

    [Category("NexaUI")]
    [DefaultValue(NexaSeparatorOrientation.Horizontal)]
    [Description("Orientation of the separator line.")]
    public NexaSeparatorOrientation Orientation
    {
        get => _orientation;
        set { _orientation = value; SetDefaultSize(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaSeparatorStyle.Solid)]
    [Description("Stroke style of the separator.")]
    public NexaSeparatorStyle Style
    {
        get => _style;
        set { _style = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(1)]
    [Description("Thickness in DIPs. Defaults to 1 logical pixel.")]
    public int ThicknessInDips
    {
        get => _thicknessInDips;
        set { _thicknessInDips = Math.Max(1, value); Invalidate(); }
    }

    [Category("NexaUI")]
    [Description("Optional explicit color. When set to Color.Empty, the theme's Border color is used.")]
    [DefaultValue(typeof(Color), "")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public Color SeparatorColor
    {
        get => _color;
        set { _color = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [Description("When true, the explicit SeparatorColor is used instead of the theme border color.")]
    [DefaultValue(false)]
    public bool UseExplicitColor { get; set; }

    protected override void OnResize(System.EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.None;

        var theme = ThemeManager.Current;
        var dpi = IsHandleCreated ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi;
        var thickness = Math.Max(1, NexaDpi.Scale(_thicknessInDips, dpi));
        var color = UseExplicitColor && _color != Color.Empty
            ? _color
            : (Color)theme.Palette[NexaColorRole.Border].Value;

        using var pen = new Pen(color, thickness);
        pen.Alignment = PenAlignment.Center;

        if (_style == NexaSeparatorStyle.Dashed)
        {
            pen.DashStyle = DashStyle.Dash;
        }

        if (_orientation == NexaSeparatorOrientation.Horizontal)
        {
            var y = Height / 2F;
            g.DrawLine(pen, 0F, y, (float)Width, y);
        }
        else
        {
            var x = Width / 2F;
            g.DrawLine(pen, x, 0F, x, (float)Height);
        }
    }

    private void SetDefaultSize()
    {
        if (_orientation == NexaSeparatorOrientation.Horizontal)
        {
            if (Width < 80) Width = 200;
            Height = Math.Max(2, Height);
            if (Height < 4) Height = 4;
        }
        else
        {
            if (Height < 24) Height = 80;
            Width = Math.Max(2, Width);
            if (Width < 4) Width = 4;
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => RefreshTheme(e.Current);

    private void RefreshTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => RefreshTheme(theme));
            return;
        }
        Invalidate();
    }
}