using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// Base class for input controls that wrap a native Windows Forms edit control
/// inside a Bootstrap-style themed host. Centralizes:
/// <list type="bullet">
///   <item>Bootstrap-inspired visual variants (Outline, Filled, Underline)</item>
///   <item>Sizing (Small, Medium, Large) and DPI-aware metrics</item>
///   <item>Label / helper-text / counter rendering</item>
///   <item>Validation icons and colored borders (None, Error, Success)</item>
///   <item>Focus glow + hover styling</item>
///   <item>Theme subscription and disposal cleanup</item>
/// </list>
/// </summary>
public abstract class NexaInputHost : UserControl
{
    private NexaInputStyle _style = NexaInputStyle.Outline;
    private NexaTextValidationState _validation = NexaTextValidationState.None;
    private NexaInputSize _size = NexaInputSize.Medium;
    private string _label = string.Empty;
    private string _helperText = string.Empty;
    private string _errorText = string.Empty;
    private bool _showCounter;
    private int _maxLength = 32767;
    private int _cornerRadiusInDips = 8;
    private bool _hovered;
    private bool _showHelperText = true;

    private readonly Panel _borderPanel;

    protected NexaInputHost()
    {
        SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.UserPaint |
            ControlStyles.SupportsTransparentBackColor,
            true);

        TabStop = false;
        BackColor = Color.Transparent;
        DoubleBuffered = true;
        AutoSize = false;
        Size = new Size(280, 70);

        _borderPanel = new Panel
        {
            Dock = DockStyle.Top,
            BackColor = Color.Transparent,
            Height = 40
        };
        _borderPanel.Paint += OnBorderPaint;
        Controls.Add(_borderPanel);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    /// <summary>The panel that hosts the native edit control. Subclasses add their edit control to this.</summary>
    protected Panel BorderHost => _borderPanel;

    /// <summary>Returns true if the inner edit control currently has focus.</summary>
    protected abstract bool InnerHasFocus();

    /// <summary>Current text length (used by the counter).</summary>
    protected abstract int InnerTextLength();

    [Category("NexaUI")]
    [DefaultValue(NexaInputStyle.Outline)]
    [Description("Visual variant of the input.")]
    public NexaInputStyle Style
    {
        get => _style;
        set { _style = value; UpdateHeight(); _borderPanel.Invalidate(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaTextValidationState.None)]
    [Description("Validation state. Drives the border color and the helper/error text color.")]
    public NexaTextValidationState ValidationState
    {
        get => _validation;
        set { _validation = value; _borderPanel.Invalidate(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaInputSize.Medium)]
    [Description("Vertical size of the input.")]
    public NexaInputSize Size
    {
        get => _size;
        set { _size = value; UpdateHeight(); _borderPanel.Invalidate(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Label rendered above the input (Bootstrap form-label). Leave empty to hide.")]
    public string Label
    {
        get => _label;
        set { _label = value ?? string.Empty; UpdateHeight(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Helper text rendered below the input (Bootstrap form-text).")]
    public string HelperText
    {
        get => _helperText;
        set { _helperText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Error text shown when ValidationState is Error. Overrides HelperText when non-empty.")]
    public string ErrorText
    {
        get => _errorText;
        set { _errorText = value ?? string.Empty; _borderPanel.Invalidate(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the helper/error text row is shown below the input.")]
    public bool ShowHelperText
    {
        get => _showHelperText;
        set { _showHelperText = value; UpdateHeight(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Show a live character counter inside the input (top-right).")]
    public bool ShowCounter
    {
        get => _showCounter;
        set { _showCounter = value; _borderPanel.Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(8)]
    [Description("Corner radius in DIPs. Ignored by the Underline style.")]
    public int CornerRadius
    {
        get => _cornerRadiusInDips;
        set { _cornerRadiusInDips = Math.Max(0, value); _borderPanel.Invalidate(); }
    }

    /// <summary>The maximum length used for the counter. Subclasses can override to forward their native editor's value.</summary>
    [Browsable(false)]
    public virtual int MaxLength
    {
        get => _maxLength;
        set { _maxLength = Math.Max(0, value); _borderPanel.Invalidate(); }
    }

    [Browsable(false)]
    public bool HasError => _validation == NexaTextValidationState.Error;

    [Browsable(false)]
    public bool HasSuccess => _validation == NexaTextValidationState.Success;

    /// <summary>The render size for the inner editor area, in pixels. Subclasses use this to pad their inner control.</summary>
    protected Rectangle InnerEditorBounds
    {
        get
        {
            var dpi = CurrentDpi();
            var padX = NexaDpi.Scale(12, dpi);
            var leftText = padX;
            var rightText = padX;
            if (HasValidationIcon()) rightText += ValidationIconReservedWidth(dpi);
            else if (_showCounter) rightText += CounterReservedWidth(dpi);
            return new Rectangle(
                leftText,
                VerticalPaddingFor(dpi),
                Math.Max(0, _borderPanel.Width - leftText - rightText),
                _borderPanel.Height - 2 * VerticalPaddingFor(dpi));
        }
    }

    /// <summary>The vertical padding to apply around the inner editor, in pixels.</summary>
    protected int VerticalPaddingFor(int dpi) => _size switch
    {
        NexaInputSize.Small => NexaDpi.Scale(4, dpi),
        NexaInputSize.Medium => NexaDpi.Scale(8, dpi),
        NexaInputSize.Large => NexaDpi.Scale(12, dpi),
        _ => NexaDpi.Scale(8, dpi),
    };

    /// <summary>Returns the effective height of the editor strip (the bordered host panel) in pixels.</summary>
    public int EditorHeight
    {
        get
        {
            var dpi = CurrentDpi();
            var baseH = _size switch
            {
                NexaInputSize.Small => 32,
                NexaInputSize.Medium => 40,
                NexaInputSize.Large => 48,
                _ => 40
            };
            return NexaDpi.Scale(baseH, dpi);
        }
    }

    /// <summary>Returns the ideal height for the host, including label/helper rows.</summary>
    public int PreferredHeight
    {
        get
        {
            var dpi = CurrentDpi();
            var labelH = string.IsNullOrEmpty(_label) ? 0 : NexaDpi.Scale(20, dpi);
            var helpH = _showHelperText && (!string.IsNullOrEmpty(_helperText) || HasError) ? NexaDpi.Scale(20, dpi) : 0;
            var gap1 = labelH > 0 ? NexaDpi.Scale(4, dpi) : 0;
            var gap2 = helpH > 0 ? NexaDpi.Scale(4, dpi) : 0;
            return labelH + gap1 + EditorHeight + gap2 + helpH;
        }
    }

    protected int CurrentDpi() => IsHandleCreated && !DesignMode ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi;

    private void UpdateHeight()
    {
        if (IsHandleCreated)
        {
            Height = PreferredHeight;
        }
    }

    protected override void OnResize(System.EventArgs e)
    {
        base.OnResize(e);
        UpdateHeight();
        _borderPanel.Invalidate();
    }

    protected override void OnMouseEnter(System.EventArgs e)
    {
        base.OnMouseEnter(e);
        _hovered = true;
        _borderPanel.Invalidate();
    }

    protected override void OnMouseLeave(System.EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovered = false;
        _borderPanel.Invalidate();
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (!IsHandleCreated) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => { if (!IsDisposed && !Disposing && IsHandleCreated) RefreshTheme(); });
            return;
        }
        RefreshTheme();
    }

    /// <summary>Re-apply the active theme. Default: re-layout the editor and invalidate. Subclasses override to recolor inner controls.</summary>
    protected virtual void RefreshTheme()
    {
        UpdateHeight();
        _borderPanel.Invalidate();
        Invalidate();
        OnThemeApplied(ThemeManager.Current);
    }

    /// <summary>Called after the host has refreshed its visuals. Subclasses should recolor their inner controls here.</summary>
    protected virtual void OnThemeApplied(ITheme theme) { }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        var y = 0;

        if (!string.IsNullOrEmpty(_label))
        {
            using var labelFont = typography.ToFont(NexaTypographyRole.Label, dpi);
            var labelColor = Enabled
                ? (Color)palette[NexaColorRole.TextPrimary].Value
                : (Color)palette[NexaColorRole.TextDisabled].Value;
            TextRenderer.DrawText(g, _label, labelFont,
                new Point(0, y), labelColor);
            y += TextRenderer.MeasureText(g, _label, labelFont).Height + NexaDpi.Scale(4, dpi);
        }

        if (_showHelperText)
        {
            string helpText = HasError && !string.IsNullOrEmpty(_errorText)
                ? _errorText
                : _helperText;
            if (!string.IsNullOrEmpty(helpText))
            {
                using var helpFont = typography.ToFont(NexaTypographyRole.Caption, dpi);
                var helpColor = HasError
                    ? (Color)palette[NexaColorRole.Danger].Value
                    : HasSuccess
                        ? (Color)palette[NexaColorRole.Success].Value
                        : (Color)palette[NexaColorRole.TextSecondary].Value;
                var helpY = Height - NexaDpi.Scale(18, dpi);
                TextRenderer.DrawText(g, helpText, helpFont,
                    new Point(0, helpY), helpColor);
            }
        }
    }

    private void OnBorderPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = CurrentDpi();

        var hasFocus = InnerHasFocus() && Enabled;
        var hovered = _hovered && Enabled;

        Color borderColor;
        Color fill;
        int borderWidth;

        if (!Enabled)
        {
            borderColor = (Color)palette[NexaColorRole.InputBorder].Value;
            borderWidth = 1;
            fill = _style == NexaInputStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (HasError)
        {
            borderColor = (Color)palette[NexaColorRole.Danger].Value;
            borderWidth = hasFocus ? 2 : 1;
            fill = (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (HasSuccess)
        {
            borderColor = (Color)palette[NexaColorRole.Success].Value;
            borderWidth = hasFocus ? 2 : 1;
            fill = (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (hasFocus)
        {
            borderColor = (Color)palette[NexaColorRole.Primary].Value;
            borderWidth = 2;
            fill = _style == NexaInputStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (hovered)
        {
            borderColor = (Color)palette[NexaColorRole.Primary].Value;
            borderWidth = 1;
            fill = _style == NexaInputStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else
        {
            borderColor = (Color)palette[NexaColorRole.InputBorder].Value;
            borderWidth = 1;
            fill = _style == NexaInputStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : _style == NexaInputStyle.Underline
                    ? Color.Transparent
                    : (Color)palette[NexaColorRole.InputBackground].Value;
        }

        var rect = new Rectangle(0, 0, _borderPanel.Width, _borderPanel.Height);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        switch (_style)
        {
            case NexaInputStyle.Underline:
                PaintUnderline(g, rect, fill, borderColor, borderWidth, palette, hasFocus);
                break;
            case NexaInputStyle.Filled:
                PaintFilled(g, rect, fill, borderColor, borderWidth, palette, hasFocus);
                break;
            case NexaInputStyle.Outline:
            default:
                PaintOutline(g, rect, fill, borderColor, borderWidth, palette, hasFocus);
                break;
        }

        if (HasValidationIcon() && Enabled)
        {
            PaintValidationIcon(g, rect, palette, dpi);
        }
        else if (_showCounter && !HasValidationIcon())
        {
            PaintCounter(g, rect, palette, dpi);
        }
    }

    private void PaintOutline(Graphics g, Rectangle rect, Color fill, Color borderColor, int borderWidth, NexaPalette palette, bool hasFocus)
    {
        var dpi = CurrentDpi();
        var radius = NexaDpi.Scale(_cornerRadiusInDips, dpi);

        var inflated = Rectangle.Inflate(rect, new Padding(-1));
        if (hasFocus)
        {
            using var glowPen = new Pen(Color.FromArgb(40, borderColor), NexaDpi.Scale(6, dpi))
            {
                Alignment = PenAlignment.Outset
            };
            using var glowPath = CreateRoundedPath(inflated, radius + NexaDpi.Scale(2, dpi));
            g.DrawPath(glowPen, glowPath);
        }

        using (var path = CreateRoundedPath(inflated, radius))
        {
            if (fill.A > 0)
            {
                using var fillBrush = new SolidBrush(fill);
                g.FillPath(fillBrush, path);
            }
            using var borderPen = new Pen(borderColor, borderWidth);
            g.DrawPath(borderPen, path);
        }
    }

    private void PaintFilled(Graphics g, Rectangle rect, Color fill, Color borderColor, int borderWidth, NexaPalette palette, bool hasFocus)
    {
        var dpi = CurrentDpi();
        var radius = NexaDpi.Scale(_cornerRadiusInDips, dpi);

        var inflated = Rectangle.Inflate(rect, new Padding(-1));
        if (hasFocus)
        {
            using var glowPen = new Pen(Color.FromArgb(40, borderColor), NexaDpi.Scale(6, dpi))
            {
                Alignment = PenAlignment.Outset
            };
            using var glowPath = CreateRoundedPath(inflated, radius + NexaDpi.Scale(2, dpi));
            g.DrawPath(glowPen, glowPath);
        }

        using (var path = CreateRoundedPath(inflated, radius))
        {
            if (fill.A > 0)
            {
                using var fillBrush = new SolidBrush(fill);
                g.FillPath(fillBrush, path);
            }
            using var borderPen = new Pen(Color.FromArgb(160, borderColor), 1F);
            g.DrawPath(borderPen, path);
        }
    }

    private void PaintUnderline(Graphics g, Rectangle rect, Color fill, Color borderColor, int borderWidth, NexaPalette palette, bool hasFocus)
    {
        if (hasFocus)
        {
            var glowRect = new Rectangle(0, rect.Bottom - NexaDpi.Scale(8, CurrentDpi()), rect.Width, NexaDpi.Scale(8, CurrentDpi()));
            using var glowBrush = new SolidBrush(Color.FromArgb(40, borderColor));
            g.FillRectangle(glowBrush, glowRect);
        }
        using var pen = new Pen(borderColor, borderWidth);
        g.DrawLine(pen, 0, rect.Bottom - pen.Width / 2, rect.Width, rect.Bottom - pen.Width / 2);
    }

    private void PaintValidationIcon(Graphics g, Rectangle rect, NexaPalette palette, int dpi)
    {
        var color = HasError
            ? (Color)palette[NexaColorRole.Danger].Value
            : (Color)palette[NexaColorRole.Success].Value;
        var icon = HasError ? NexaIconKind.Error : NexaIconKind.Check;
        var size = NexaDpi.Scale(16, dpi);
        var bmp = NexaIconProvider.ToBitmap(icon, new Size(size, size), color);
        if (bmp is null) return;
        var reserved = ValidationIconReservedWidth(dpi);
        var x = rect.Right - reserved + (reserved - bmp.Width) / 2;
        var y = (rect.Height - bmp.Height) / 2;
        g.DrawImage(bmp, x, y);
    }

    private void PaintCounter(Graphics g, Rectangle rect, NexaPalette palette, int dpi)
    {
        var theme = ThemeManager.Current;
        var typography = theme.Typography;
        var text = $"{InnerTextLength()} / {MaxLength}";
        using var font = typography.ToFont(NexaTypographyRole.Caption, dpi);
        var color = InnerTextLength() > MaxLength
            ? (Color)palette[NexaColorRole.Danger].Value
            : (Color)palette[NexaColorRole.TextSecondary].Value;
        var reserved = CounterReservedWidth(dpi);
        var measured = TextRenderer.MeasureText(g, text, font);
        var x = rect.Right - reserved + (reserved - measured.Width) / 2;
        var y = (rect.Height - measured.Height) / 2;
        TextRenderer.DrawText(g, text, font,
            new Point(x, Math.Max(0, y)), color);
    }

    protected bool HasValidationIcon() => _validation != NexaTextValidationState.None && Enabled;

    protected int ValidationIconReservedWidth(int dpi) => NexaDpi.Scale(36, dpi);
    protected int CounterReservedWidth(int dpi) => NexaDpi.Scale(60, dpi);

    internal static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0)
        {
            path.AddRectangle(rect);
            return path;
        }
        var d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        if (d <= 0) { path.AddRectangle(rect); return path; }
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
