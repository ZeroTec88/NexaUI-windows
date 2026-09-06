using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed WinForms <see cref="GroupBox"/> provided by NexaUI. Inherits the native
/// <see cref="GroupBox"/> so all layout, docking, child-controls collection, and
/// designer behavior is preserved. Adds a themed title, a modern border, and rounded
/// corners where supported.
/// </summary>
[DefaultEvent(nameof(Enter))]
[DefaultProperty(nameof(Text))]
public class NexaGroupBox : GroupBox
{
    private int _cornerRadiusDips = 3;
    private int _borderThicknessDips = 1;
    private Color _titleColor = Color.Empty;
    private Color _borderColor = Color.Empty;

    /// <summary>Initializes a new instance of the <see cref="NexaGroupBox"/> class.</summary>
    public NexaGroupBox()
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

    /// <summary>Title color. Defaults to the theme TextPrimary role.</summary>
    [Category("NexaUI")]
    [Description("Title color. Defaults to the theme TextPrimary role when left empty.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WinForms", "WFO1000", Justification = "Themed property; theme drives the value by default.")]
    public Color TitleColor
    {
        get => _titleColor;
        set { _titleColor = value; Invalidate(); }
    }

    /// <summary>Border color. Defaults to the theme Border role.</summary>
    [Category("NexaUI")]
    [Description("Border color. Defaults to the theme Border role when left empty.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WinForms", "WFO1000", Justification = "Themed property; theme drives the value by default.")]
    public Color BorderColor
    {
        get => _borderColor;
        set { _borderColor = value; Invalidate(); }
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
    [DefaultValue(3)]
    [Description("Corner radius in device-independent pixels (visual only).")]
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
        if (!IsHandleCreated) return;
        try
        {
            BackColor = (Color)theme.Palette[NexaColorRole.Surface].Value;
            ForeColor = (Color)theme.Palette[NexaColorRole.TextPrimary].Value;
            Invalidate();
        }
        catch
        {
            // Ignore painting errors during theme transitions in headless test environments.
        }
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var dpi = CurrentDpi();
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var typography = theme.Typography;

        var w = Width;
        var h = Height;
        if (w <= 0 || h <= 0) return;

        // Title sizing — let WinForms calculate the title height for layout correctness.
        var titleHeight = Font.Height + NexaDpi.Scale(2, dpi);
        var radius = NexaDpi.Scale(_cornerRadiusDips, dpi);
        var borderThickness = NexaDpi.Scale(_borderThicknessDips, dpi);

        var titleColor = _titleColor.IsEmpty
            ? (Color)palette[NexaColorRole.TextPrimary].Value
            : _titleColor;
        var borderColor = _borderColor.IsEmpty
            ? (Color)palette[NexaColorRole.Border].Value
            : _borderColor;

        // Background.
        var bgRect = new Rectangle(0, 0, w, h);
        using (var bgPath = CreateRoundedPath(bgRect, radius))
        {
            using var bgBrush = new SolidBrush(BackColor);
            g.FillPath(bgBrush, bgPath);
        }

        // Border.
        if (borderThickness > 0)
        {
            using var borderPath = CreateRoundedPath(bgRect, radius);
            using var pen = new Pen(borderColor, borderThickness);
            g.DrawPath(pen, borderPath);
        }

        // Title — clear the background behind the text so the border "breaks" around it.
        if (!string.IsNullOrEmpty(Text))
        {
            var textSize = TextRenderer.MeasureText(g, Text, Font);
            var padX = NexaDpi.Scale(4, dpi);
            var textRect = new Rectangle(padX, 0, textSize.Width, titleHeight);
            using (var clear = new SolidBrush(BackColor))
            {
                g.FillRectangle(clear, textRect);
            }
            TextRenderer.DrawText(g, Text, Font, textRect, titleColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding);
        }
    }

    internal static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
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
}
