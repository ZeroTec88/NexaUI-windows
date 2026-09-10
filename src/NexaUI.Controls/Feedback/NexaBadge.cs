using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A compact themed status/count indicator provided by NexaUI. Renders a
/// pill-shaped badge with themed background, optional rounded corners, and
/// automatic sizing based on text content.
/// </summary>
[DefaultEvent(nameof(TextChanged))]
[DefaultProperty(nameof(Text))]
public class NexaBadge : Control
{
    private NexaBadgeStyle _style = NexaBadgeStyle.Default;
    private NexaBadgeSize _size = NexaBadgeSize.Medium;
    private bool _autoSize = true;
    private int _maxCharacters = 0; // 0 = unlimited
    private string _displayText = string.Empty;

    /// <summary>Initializes a new instance of the <see cref="NexaBadge"/> class.</summary>
    public NexaBadge()
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
        Size = new Size(40, 20);
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
        UpdateDisplay();
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Badge text content.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Badge text content.")]
    public override string Text
    {
        get => base.Text;
        set
        {
            base.Text = value;
            UpdateDisplay();
        }
    }

    /// <summary>Visual style of the badge.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaBadgeStyle.Default)]
    [Description("Visual style of the badge.")]
    public NexaBadgeStyle BadgeStyle
    {
        get => _style;
        set { _style = value; UpdateDisplay(); }
    }

    /// <summary>Size variant of the badge.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaBadgeSize.Medium)]
    [Description("Size variant of the badge.")]
    public NexaBadgeSize BadgeSize
    {
        get => _size;
        set { _size = value; UpdateDisplay(); }
    }

    /// <summary>Whether the badge automatically sizes itself to fit the text.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the badge automatically sizes itself to fit the text.")]
    public new bool AutoSize
    {
        get => _autoSize;
        set { _autoSize = value; UpdateDisplay(); }
    }

    /// <summary>Maximum number of characters before truncation (e.g. 99+). 0 = unlimited.</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Maximum number of characters before truncation (e.g. 99+). 0 = unlimited.")]
    public int MaximumCharacters
    {
        get => _maxCharacters;
        set { _maxCharacters = Math.Max(0, value); UpdateDisplay(); }
    }

    private void UpdateDisplay()
    {
        var t = Text ?? string.Empty;
        if (_maxCharacters > 0 && t.Length > _maxCharacters)
        {
            // Numeric badges get "99+" style; alphabetic just truncate with ellipsis.
            if (long.TryParse(t, out _))
            {
                var limit = Math.Max(1, _maxCharacters);
                if (t.Length > limit)
                {
                    t = new string('9', limit) + "+";
                }
            }
            else
            {
                t = t.Substring(0, _maxCharacters) + "…";
            }
        }
        _displayText = t;
        AccessibleName = string.IsNullOrEmpty(t) ? "Badge" : t;
        if (_autoSize && IsHandleCreated) AutoSizeToText();
        Invalidate();
    }

    private void AutoSizeToText()
    {
        var dpi = CurrentDpi();
        var size = TextRenderer.MeasureText(
            string.IsNullOrEmpty(_displayText) ? " " : _displayText,
            ResolveFont());
        var padX = NexaDpi.Scale(8, dpi);
        var padY = NexaDpi.Scale(2, dpi);
        var h = NexaDpi.Scale(SizeFromEnum(), dpi) + padY * 2;
        var w = (int)Math.Ceiling((double)size.Width) + padX * 2;
        Size = new Size(w, h);
    }

    private int SizeFromEnum() => _size switch
    {
        NexaBadgeSize.Small => 12,
        NexaBadgeSize.Large => 18,
        _ => 14
    };

    private Font ResolveFont()
    {
        var dpi = CurrentDpi();
        var theme = ThemeManager.Current;
        var typography = theme.Typography;
        return typography.ToFont(NexaTypographyRole.Caption, dpi);
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var palette = ThemeManager.Current.Palette;
        var w = Width;
        var h = Height;
        if (w <= 0 || h <= 0) return;

        var (bg, fg) = ResolveColors(palette);
        var radius = Math.Min(h / 2, Math.Max(2, h / 2));

        var rect = new Rectangle(0, 0, w, h);
        using (var path = CreateRoundedPath(rect, radius))
        {
            using var brush = new SolidBrush(bg);
            g.FillPath(brush, path);
        }

        if (!string.IsNullOrEmpty(_displayText))
        {
            using var font = ResolveFont();
            var textRect = new Rectangle(0, 0, w, h);
            TextRenderer.DrawText(g, _displayText, font, textRect, fg,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }
    }

    private (Color bg, Color fg) ResolveColors(NexaPalette palette)
    {
        if (!Enabled) return ((Color)palette[NexaColorRole.SurfaceVariant].Value, (Color)palette[NexaColorRole.TextDisabled].Value);
        return _style switch
        {
            NexaBadgeStyle.Primary => ((Color)palette[NexaColorRole.Primary].Value, (Color)palette[NexaColorRole.TextOnAccent].Value),
            NexaBadgeStyle.Success => ((Color)palette[NexaColorRole.Success].Value, (Color)palette[NexaColorRole.TextOnAccent].Value),
            NexaBadgeStyle.Warning => ((Color)palette[NexaColorRole.Warning].Value, (Color)palette[NexaColorRole.TextOnAccent].Value),
            NexaBadgeStyle.Danger => ((Color)palette[NexaColorRole.Danger].Value, (Color)palette[NexaColorRole.TextOnAccent].Value),
            NexaBadgeStyle.Info => ((Color)palette[NexaColorRole.Info].Value, (Color)palette[NexaColorRole.TextOnAccent].Value),
            NexaBadgeStyle.Muted => (Color.Transparent, (Color)palette[NexaColorRole.TextSecondary].Value),
            _ => ((Color)palette[NexaColorRole.SurfaceVariant].Value, (Color)palette[NexaColorRole.TextPrimary].Value)
        };
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

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);
}
