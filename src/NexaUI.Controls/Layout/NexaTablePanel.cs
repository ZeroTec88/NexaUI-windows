using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed WinForms <see cref="TableLayoutPanel"/> provided by NexaUI. Inherits the
/// native <see cref="TableLayoutPanel"/> so all layout (rows, columns, styles, cell
/// border, docking, designer behavior) is preserved. Adds a themed background, an
/// optional border with rounded corners, and full theme integration.
/// </summary>
[DefaultEvent(nameof(Paint))]
[DefaultProperty(nameof(ColumnCount))]
public class NexaTablePanel : TableLayoutPanel
{
    private int _cornerRadiusDips = 2;
    private int _borderThicknessDips = 1;
    private bool _borderEnabled;

    /// <summary>Initializes a new instance of the <see cref="NexaTablePanel"/> class.</summary>
    public NexaTablePanel()
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

    /// <summary>Whether a border is drawn around the panel.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether a border is drawn around the panel.")]
    public bool BorderEnabled
    {
        get => _borderEnabled;
        set { _borderEnabled = value; Invalidate(); }
    }

    /// <summary>Border thickness in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(1)]
    [Description("Border thickness in device-independent pixels.")]
    public int BorderThickness
    {
        get => _borderThicknessDips;
        set { _borderThicknessDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Corner radius in DIPs (visual only; native layout is unchanged).</summary>
    [Category("NexaUI")]
    [DefaultValue(2)]
    [Description("Corner radius in device-independent pixels.")]
    public int CornerRadius
    {
        get => _cornerRadiusDips;
        set { _cornerRadiusDips = Math.Max(0, value); Invalidate(); }
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
        BackColor = (Color)theme.Palette[NexaColorRole.Surface].Value;
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

        if (_borderEnabled && _borderThicknessDips > 0)
        {
            var radius = NexaDpi.Scale(_cornerRadiusDips, dpi);
            var borderThickness = NexaDpi.Scale(_borderThicknessDips, dpi);
            var rect = new Rectangle(0, 0, w, h);
            using var path = NexaPanel.CreateRoundedPathPublic(rect, radius);
            using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, borderThickness);
            g.DrawPath(pen, path);
        }
    }
}
