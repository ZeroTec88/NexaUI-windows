using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware WinForms button. Inherits from the native <see cref="Button"/> so all
/// native keyboard handling, focus, accessibility, and click behavior is preserved.
/// Visual customization is done by overriding only the paint / state hooks.
/// </summary>
[DefaultEvent(nameof(Click))]
[DefaultProperty(nameof(Text))]
public class NexaButton : Button
{
    private NexaButtonStyle _style = NexaButtonStyle.Primary;
    private NexaButtonSize _size = NexaButtonSize.Medium;
    private NexaButtonIconPosition _iconPosition = NexaButtonIconPosition.Left;
    private int _borderRadiusInDips = 6;
    private bool _loading;
    private string _loadingText = "Loading...";
    private string _originalText = string.Empty;
    private NexaIconKind _iconKind = NexaIconKind.None;

    public NexaButton()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable |
            ControlStyles.StandardClick |
            ControlStyles.StandardDoubleClick |
            ControlStyles.SupportsTransparentBackColor,
            true);

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = Color.Empty;
        FlatAppearance.MouseDownBackColor = Color.Empty;
        FlatAppearance.CheckedBackColor = Color.Empty;
        TabStop = true;
        Cursor = Cursors.Hand;

        DoubleBuffered = true;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        ApplySizeMetrics();
        RefreshColors();
    }

    [Category("NexaUI")]
    [DefaultValue(NexaButtonStyle.Primary)]
    [Description("Visual style variant of the button.")]
    public NexaButtonStyle Style
    {
        get => _style;
        set { _style = value; RefreshColors(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaButtonSize.Medium)]
    [Description("Size variant of the button. Affects font, padding and height.")]
    public NexaButtonSize SizeMode
    {
        get => _size;
        set { _size = value; ApplySizeMetrics(); RefreshColors(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaButtonIconPosition.Left)]
    [Description("Where the icon is rendered relative to the text.")]
    public NexaButtonIconPosition IconPosition
    {
        get => _iconPosition;
        set { _iconPosition = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(6)]
    [Description("Corner radius in DIPs. Use 0 for a square button.")]
    public int BorderRadius
    {
        get => _borderRadiusInDips;
        set { _borderRadiusInDips = Math.Max(0, value); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaIconKind.None)]
    [Description("Optional icon displayed alongside the text.")]
    public NexaIconKind IconKind
    {
        get => _iconKind;
        set { _iconKind = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("When true, the button shows a loading indicator and suppresses interaction.")]
    public bool Loading
    {
        get => _loading;
        set
        {
            if (_loading == value) return;
            _loading = value;
            if (_loading)
            {
                if (_originalText.Length == 0 && Text.Length > 0) _originalText = Text;
                Text = _loadingText;
            }
            else if (_originalText.Length > 0)
            {
                Text = _originalText;
                _originalText = string.Empty;
            }
            Invalidate();
        }
    }

    [Category("NexaUI")]
    [DefaultValue("Loading...")]
    [Description("Text shown while Loading is true.")]
    public string LoadingText
    {
        get => _loadingText;
        set { _loadingText = value ?? string.Empty; if (_loading) { Text = _loadingText; Invalidate(); } }
    }

    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set { base.Text = value ?? string.Empty; if (!_loading) _originalText = value ?? string.Empty; Invalidate(); }
    }

    [Browsable(false)]
    public bool IsPressed { get; private set; }

    [Browsable(false)]
    public bool IsHovered { get; private set; }

    [Browsable(false)]
    public bool IsFocused => Focused || (TabStop && ShowFocusCues && ContainsFocus);

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        IsHovered = true;
        base.OnMouseEnter(e);
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        IsHovered = false;
        IsPressed = false;
        base.OnMouseLeave(e);
        Invalidate();
    }

    protected override void OnMouseDown(MouseEventArgs mevent)
    {
        if (mevent.Button == MouseButtons.Left) IsPressed = true;
        base.OnMouseDown(mevent);
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs mevent)
    {
        IsPressed = false;
        base.OnMouseUp(mevent);
        Invalidate();
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        Invalidate();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RefreshColors();
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => { if (!IsDisposed && !Disposing) RefreshColors(); });
            return;
        }
        RefreshColors();
    }

    private void ApplySizeMetrics()
    {
        var theme = ThemeManager.Current;
        var dpi = SafeDpi();

        var padding = _size switch
        {
            NexaButtonSize.Small => new Padding(NexaDpi.Scale(8, dpi), 0, NexaDpi.Scale(8, dpi), 0),
            NexaButtonSize.Large => new Padding(NexaDpi.Scale(20, dpi), 0, NexaDpi.Scale(20, dpi), 0),
            _ => new Padding(NexaDpi.Scale(14, dpi), 0, NexaDpi.Scale(14, dpi), 0)
        };

        var verticalPad = _size switch
        {
            NexaButtonSize.Small => NexaDpi.Scale(4, dpi),
            NexaButtonSize.Large => NexaDpi.Scale(10, dpi),
            _ => NexaDpi.Scale(6, dpi)
        };

        Padding = new Padding(padding.Left, verticalPad, padding.Right, verticalPad);

        var height = _size switch
        {
            NexaButtonSize.Small => NexaDpi.Scale(26, dpi),
            NexaButtonSize.Large => NexaDpi.Scale(44, dpi),
            _ => NexaDpi.Scale(34, dpi)
        };

        if (AutoSize) return;
        if (Height < height) Height = height;
    }

    private void RefreshColors()
    {
        var theme = ThemeManager.Current;
        var role = ResolveColorRole(_style);

        BackColor = Color.Transparent;
        UseVisualStyleBackColor = false;

        var bg = (Color)theme.Palette[role].Value;
        ForeColor = ShouldUseOnAccent(_style) ? (Color)theme.Palette[NexaColorRole.TextOnAccent].Value : bg;
        FlatAppearance.BorderColor = bg;

        var dpi = SafeDpi();
        Font = theme.Typography.ToFont(_size == NexaButtonSize.Large ? NexaTypographyRole.BodyStrong : NexaTypographyRole.Button, dpi);
        Invalidate();
    }

    private static NexaColorRole ResolveColorRole(NexaButtonStyle style) => style switch
    {
        NexaButtonStyle.Primary => NexaColorRole.Primary,
        NexaButtonStyle.Secondary => NexaColorRole.Secondary,
        NexaButtonStyle.Success => NexaColorRole.Success,
        NexaButtonStyle.Warning => NexaColorRole.Warning,
        NexaButtonStyle.Danger => NexaColorRole.Danger,
        NexaButtonStyle.Outline => NexaColorRole.Primary,
        NexaButtonStyle.Ghost => NexaColorRole.Primary,
        _ => NexaColorRole.Primary
    };

    private static bool ShouldUseOnAccent(NexaButtonStyle style) => style switch
    {
        NexaButtonStyle.Outline => false,
        NexaButtonStyle.Ghost => false,
        _ => true
    };

    private float SafeDpi()
    {
        if (!IsHandleCreated) return NexaDpi.BaseDpi;
        return NexaFormsDpi.CurrentDpi(this);
    }

    private ITheme CurrentTheme() => ThemeManager.Current;

    private NexaControlState ResolveState()
    {
        if (!Enabled) return NexaControlState.Disabled;
        if (IsPressed) return NexaControlState.Pressed;
        if (IsHovered) return NexaControlState.Hover;
        return NexaControlState.Normal;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        g.Clear(Parent?.BackColor ?? SystemColors.Control);

        var theme = CurrentTheme();
        var palette = theme.Palette;
        var metrics = theme.Metrics;
        var dpi = SafeDpi();

        var baseColor = (Color)palette[ResolveColorRole(_style)].Value;
        var accent = (Color)palette[ResolveColorRole(_style)].Accent;
        var onAccent = (Color)palette[NexaColorRole.TextOnAccent].Value;
        var disabled = (Color)palette[NexaColorRole.SurfaceVariant].Value;
        var borderColor = (Color)palette[NexaColorRole.Border].Value;
        var textDisabled = (Color)palette[NexaColorRole.TextDisabled].Value;

        Color background = _style switch
        {
            NexaButtonStyle.Outline => Color.Transparent,
            NexaButtonStyle.Ghost => Color.Transparent,
            _ => baseColor
        };

        Color foreground = _style switch
        {
            NexaButtonStyle.Outline => baseColor,
            NexaButtonStyle.Ghost => baseColor,
            _ => onAccent
        };

        Color border = baseColor;

        if (!Enabled)
        {
            background = disabled;
            border = borderColor;
            foreground = textDisabled;
        }
        else if (IsPressed)
        {
            background = _style is NexaButtonStyle.Outline or NexaButtonStyle.Ghost
                ? (Color)palette[ResolveColorRole(_style)].Subtle
                : accent;
        }
        else if (IsHovered)
        {
            background = _style switch
            {
                NexaButtonStyle.Outline => (Color)palette[NexaColorRole.SurfaceVariant].Value,
                NexaButtonStyle.Ghost => (Color)palette[NexaColorRole.SurfaceVariant].Value,
                _ => accent
            };
        }

        var radius = Math.Max(0, NexaDpi.Scale(_borderRadiusInDips, dpi));
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);

        using (var path = CreateRoundedRect(rect, radius))
        using (var brush = new SolidBrush(background))
        {
            g.FillPath(brush, path);
        }

        if (_style == NexaButtonStyle.Outline)
        {
            using var borderPen = new Pen(border, NexaDpi.Scale(metrics.BorderThicknessInDips, dpi));
            using var path = CreateRoundedRect(rect, radius);
            g.DrawPath(borderPen, path);
        }

        if (IsFocused && Enabled && TabStop)
        {
            using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(metrics.FocusRingThicknessInDips, dpi));
            var focusRect = Rectangle.Inflate(rect, -NexaDpi.Scale(2, dpi), -NexaDpi.Scale(2, dpi));
            using var focusPath = CreateRoundedRect(focusRect, Math.Max(0, radius - NexaDpi.Scale(2, dpi)));
            g.DrawPath(focusPen, focusPath);
        }

        DrawContent(g, rect, foreground, dpi);
    }

    private void DrawContent(Graphics g, Rectangle rect, Color foreground, float dpi)
    {
        var hasIcon = _iconKind != NexaIconKind.None;
        var text = Text ?? string.Empty;

        var iconSize = _size switch
        {
            NexaButtonSize.Small => NexaDpi.Scale(14, dpi),
            NexaButtonSize.Large => NexaDpi.Scale(20, dpi),
            _ => NexaDpi.Scale(16, dpi)
        };

        var spacing = NexaDpi.Scale(8, dpi);
        var iconRect = Rectangle.Empty;
        var textRect = rect;
        textRect.Inflate(-Padding.Left, -Padding.Top);

        Size textSize = text.Length == 0 ? Size.Empty : TextRenderer.MeasureText(g, text, Font, textRect.Size, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        int contentWidth = textSize.Width + (hasIcon ? iconSize + spacing : 0);
        int startX = rect.X + Math.Max(Padding.Left, (rect.Width - contentWidth) / 2);
        int y = rect.Y + (rect.Height - iconSize) / 2;
        int textY = rect.Y + (rect.Height - textSize.Height) / 2;

        if (hasIcon)
        {
            iconRect = new Rectangle(startX, y, iconSize, iconSize);
            if (_iconPosition == NexaButtonIconPosition.Right)
            {
                iconRect.X = startX + contentWidth - iconSize;
            }
        }

        var textX = hasIcon
            ? (_iconPosition == NexaButtonIconPosition.Left
                ? iconRect.Right + spacing
                : startX)
            : startX;

        if (hasIcon && _loading)
        {
            DrawLoadingSpinner(g, iconRect, foreground);
        }
        else if (hasIcon)
        {
            using var bmp = NexaIconProvider.ToBitmap(_iconKind, iconRect.Size, foreground);
            if (bmp is not null) g.DrawImage(bmp, iconRect);
        }
        else if (_loading)
        {
            var spinnerSize = Math.Min(iconSize, textSize.Height);
            var spinnerRect = hasIcon ? iconRect : new Rectangle(textX, rect.Y + (rect.Height - spinnerSize) / 2, spinnerSize, spinnerSize);
            DrawLoadingSpinner(g, spinnerRect, foreground);
        }

        if (text.Length > 0)
        {
            var textDrawingRect = new Rectangle(textX, textY, textSize.Width, textSize.Height);
            TextRenderer.DrawText(g, text, Font, textDrawingRect, foreground, TextFormatFlags.SingleLine | TextFormatFlags.VerticalCenter);
        }
    }

    private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
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

    private int _spinnerTick;
    private System.Windows.Forms.Timer? _spinnerTimer;

    private void DrawLoadingSpinner(Graphics g, Rectangle rect, Color tint)
    {
        if (_spinnerTimer is null && _loading && IsHandleCreated)
        {
            _spinnerTimer = new System.Windows.Forms.Timer { Interval = 60 };
            _spinnerTimer.Tick += (_, _) =>
            {
                if (!_loading) { _spinnerTimer?.Stop(); return; }
                _spinnerTick = (_spinnerTick + 30) % 360;
                Invalidate();
            };
            _spinnerTimer.Start();
        }
        else if (!_loading && _spinnerTimer is not null)
        {
            _spinnerTimer.Stop();
            _spinnerTimer.Dispose();
            _spinnerTimer = null;
        }

        if (rect.Width <= 0 || rect.Height <= 0) return;
        var size = Math.Min(rect.Width, rect.Height);
        var square = new Rectangle(0, 0, size, size);
        square.X = rect.X + (rect.Width - size) / 2;
        square.Y = rect.Y + (rect.Height - size) / 2;

        using var pen = new Pen(tint, Math.Max(1F, size / 8F));
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        g.DrawArc(pen, square, _spinnerTick, 90);
    }
}