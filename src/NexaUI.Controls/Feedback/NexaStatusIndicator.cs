using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A compact themed status indicator provided by NexaUI. Renders a colored
/// dot and an optional label. Uses semantic theme colors for each
/// <see cref="NexaStatus"/> value.
/// </summary>
[DefaultEvent(nameof(StatusChanged))]
[DefaultProperty(nameof(Status))]
public class NexaStatusIndicator : Control
{
    private NexaStatus _status = NexaStatus.None;
    private bool _showText = true;
    private int _indicatorSizeDips = 10;
    private string _text = string.Empty;

    /// <summary>Initializes a new instance of the <see cref="NexaStatusIndicator"/> class.</summary>
    public NexaStatusIndicator()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        AccessibleRole = AccessibleRole.StaticText;
        Size = new Size(100, 20);
        SetStyle(ControlStyles.Selectable, false);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => ApplyTheme(e.Current);

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyTheme(theme));
            return;
        }
        BackColor = (Color)theme.Palette[NexaColorRole.Background].Value;
        UpdateAccessibleName();
        Invalidate();
    }

    private void UpdateAccessibleName()
    {
        var name = _status.ToString();
        if (!string.IsNullOrEmpty(_text)) name = $"{name}: {_text}";
        AccessibleName = name;
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Semantic status of the indicator.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaStatus.None)]
    [Description("Semantic status of the indicator.")]
    public NexaStatus Status
    {
        get => _status;
        set
        {
            if (_status == value) return;
            _status = value;
            UpdateAccessibleName();
            StatusChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    /// <summary>Optional text shown next to the indicator dot.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional text shown next to the indicator dot.")]
    public new string Text
    {
        get => _text;
        set
        {
            _text = value ?? string.Empty;
            UpdateAccessibleName();
            Invalidate();
        }
    }

    /// <summary>Whether to show the text label next to the dot.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to show the text label next to the dot.")]
    public bool ShowText
    {
        get => _showText;
        set { _showText = value; Invalidate(); }
    }

    /// <summary>Dot diameter in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(10)]
    [Description("Dot diameter in device-independent pixels.")]
    public int IndicatorSize
    {
        get => _indicatorSizeDips;
        set { _indicatorSizeDips = Math.Max(4, value); Invalidate(); }
    }

    /// <summary>Raised when <see cref="Status"/> changes.</summary>
    [Category("NexaUI")]
    [Description("Raised when Status changes.")]
    public event EventHandler? StatusChanged;

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var dpi = CurrentDpi();
        var palette = ThemeManager.Current.Palette;
        var typography = ThemeManager.Current.Typography;
        var w = Width;
        var h = Height;
        if (w <= 0 || h <= 0) return;

        var dotSize = NexaDpi.Scale(_indicatorSizeDips, dpi);
        var dotY = (h - dotSize) / 2f;
        var dotX = 0f;

        var color = ResolveStatusColor(palette);

        // Optional soft halo (outer ring at lower alpha)
        if (dotSize >= 6)
        {
            var halo = dotSize + NexaDpi.Scale(2, dpi);
            using (var haloBrush = new SolidBrush(Color.FromArgb(60, color)))
            {
                g.FillEllipse(haloBrush, dotX - 1, dotY - 1, halo, halo);
            }
        }

        // Solid dot
        using (var brush = new SolidBrush(color))
        {
            g.FillEllipse(brush, dotX, dotY, dotSize, dotSize);
        }

        // Optional text
        if (_showText && !string.IsNullOrEmpty(_text))
        {
            using var font = typography.ToFont(NexaTypographyRole.Body, dpi);
            var textColor = Enabled
                ? (Color)palette[NexaColorRole.TextPrimary].Value
                : (Color)palette[NexaColorRole.TextDisabled].Value;
            var textRect = new RectangleF(
                dotX + dotSize + NexaDpi.Scale(6, dpi), 0,
                w - (dotX + dotSize + NexaDpi.Scale(6, dpi)), h);
            TextRenderer.DrawText(g, _text, font, Rectangle.Round(textRect), textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding);
        }
    }

    private Color ResolveStatusColor(NexaPalette palette) => _status switch
    {
        NexaStatus.Online => (Color)palette[NexaColorRole.Success].Value,
        NexaStatus.Success => (Color)palette[NexaColorRole.Success].Value,
        NexaStatus.Offline => (Color)palette[NexaColorRole.TextDisabled].Value,
        NexaStatus.Busy => (Color)palette[NexaColorRole.Warning].Value,
        NexaStatus.Warning => (Color)palette[NexaColorRole.Warning].Value,
        NexaStatus.Error => (Color)palette[NexaColorRole.Danger].Value,
        _ => (Color)palette[NexaColorRole.TextSecondary].Value
    };

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);
}
