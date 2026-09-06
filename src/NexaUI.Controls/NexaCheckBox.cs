using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed check box. Inherits from the native <see cref="CheckBox"/> so all native
/// behavior (data binding, keyboard activation, three-state, accessibility, designer
/// support) is preserved. Visual styling is done by overriding <see cref="OnPaint"/>
/// and re-applying theme colors on theme change.
/// </summary>
[DefaultEvent(nameof(CheckedChanged))]
[DefaultProperty(nameof(Checked))]
public class NexaCheckBox : CheckBox
{
    private NexaCheckBoxStyle _style = NexaCheckBoxStyle.Default;
    private NexaTextValidationState _validation = NexaTextValidationState.None;
    private string _errorText = string.Empty;
    private string _helperText = string.Empty;
    private bool _showHelperText = true;

    public NexaCheckBox()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable |
            ControlStyles.StandardClick |
            ControlStyles.SupportsTransparentBackColor,
            true);

        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = Color.Empty;
        FlatAppearance.MouseDownBackColor = Color.Empty;
        FlatAppearance.CheckedBackColor = Color.Empty;
        Cursor = Cursors.Hand;
        DoubleBuffered = true;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        RefreshColors();
    }

    [Category("NexaUI")]
    [DefaultValue(NexaCheckBoxStyle.Default)]
    [Description("Visual style variant of the check box.")]
    public NexaCheckBoxStyle Style
    {
        get => _style;
        set { _style = value; RefreshColors(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaTextValidationState.None)]
    [Description("Validation state. Drives the indicator color when checked.")]
    public NexaTextValidationState ValidationState
    {
        get => _validation;
        set { _validation = value; RefreshColors(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Error text shown next to the check box when ValidationState is Error.")]
    public string ErrorText
    {
        get => _errorText;
        set { _errorText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Helper text shown next to the check box when not in error.")]
    public string HelperText
    {
        get => _helperText;
        set { _helperText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the helper/error text is rendered next to the check box.")]
    public bool ShowHelperText
    {
        get => _showHelperText;
        set { _showHelperText = value; Invalidate(); }
    }

    [Browsable(false)]
    public bool HasError => _validation == NexaTextValidationState.Error;

    [Browsable(false)]
    public bool HasSuccess => _validation == NexaTextValidationState.Success;

    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the check box is in the indeterminate state. Requires ThreeState = true.")]
    public bool IsIndeterminate
    {
        get => CheckState == CheckState.Indeterminate;
        set
        {
            if (value)
            {
                ThreeState = true;
                CheckState = CheckState.Indeterminate;
            }
            else
            {
                CheckState = Checked ? CheckState.Checked : CheckState.Unchecked;
            }
        }
    }

    /// <summary>Refreshes theme-driven colors (background, text, flat chrome).</summary>
    public void RefreshColors()
    {
        if (IsDisposed || Disposing) return;
        var palette = ThemeManager.Current.Palette;
        var enabledText = (Color)palette[NexaColorRole.TextPrimary].Value;
        var disabledText = (Color)palette[NexaColorRole.TextDisabled].Value;
        ForeColor = Enabled ? enabledText : disabledText;
        BackColor = Color.Transparent;
        FlatAppearance.MouseOverBackColor = Color.Transparent;
        FlatAppearance.MouseDownBackColor = Color.Transparent;
        FlatAppearance.CheckedBackColor = Color.Transparent;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => { if (!IsDisposed && !Disposing) { RefreshColors(); Invalidate(); } });
            return;
        }
        RefreshColors();
        Invalidate();
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi;

    protected override void OnEnabledChanged(System.EventArgs e)
    {
        base.OnEnabledChanged(e);
        RefreshColors();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        var boxSize = NexaDpi.Scale(16, dpi);
        var gap = NexaDpi.Scale(8, dpi);
        var boxRect = CheckAlign == ContentAlignment.MiddleRight
            ? new Rectangle(Width - boxSize, (Height - boxSize) / 2, boxSize, boxSize)
            : new Rectangle(0, (Height - boxSize) / 2, boxSize, boxSize);

        var isChecked = CheckState == CheckState.Checked;
        var isIndeterminate = CheckState == CheckState.Indeterminate;

        Color boxBorder;
        Color boxFill;

        if (!Enabled)
        {
            boxBorder = (Color)palette[NexaColorRole.InputBorder].Value;
            boxFill = (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (isChecked || isIndeterminate)
        {
            if (HasError)
            {
                boxBorder = (Color)palette[NexaColorRole.Danger].Value;
                boxFill = (Color)palette[NexaColorRole.Danger].Value;
            }
            else if (HasSuccess)
            {
                boxBorder = (Color)palette[NexaColorRole.Success].Value;
                boxFill = (Color)palette[NexaColorRole.Success].Value;
            }
            else
            {
                boxBorder = (Color)palette[NexaColorRole.Primary].Value;
                boxFill = (Color)palette[NexaColorRole.Primary].Value;
            }
        }
        else
        {
            if (_style == NexaCheckBoxStyle.Minimal)
            {
                boxBorder = Color.Transparent;
            }
            else
            {
                boxBorder = (Color)palette[NexaColorRole.InputBorder].Value;
            }
            boxFill = _style == NexaCheckBoxStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : (Color)palette[NexaColorRole.InputBackground].Value;
        }

        var radius = NexaDpi.Scale(2, dpi);
        using (var path = CreateRoundedPath(boxRect, radius))
        {
            if (boxFill.A > 0)
            {
                using var fillBrush = new SolidBrush(boxFill);
                g.FillPath(fillBrush, path);
            }
            if (boxBorder.A > 0)
            {
                using var pen = new Pen(boxBorder, NexaDpi.Scale(1, dpi));
                g.DrawPath(pen, path);
            }
        }

        if (isChecked)
        {
            var checkColor = (Color)palette[NexaColorRole.TextOnAccent].Value;
            var bmp = NexaIconProvider.ToBitmap(NexaIconKind.Check, new Size(boxSize, boxSize), checkColor);
            if (bmp is not null)
            {
                g.DrawImage(bmp, boxRect.Left, boxRect.Top);
            }
        }
        else if (isIndeterminate)
        {
            var dash = (Color)palette[NexaColorRole.TextOnAccent].Value;
            var dashHeight = NexaDpi.Scale(2, dpi);
            var dashWidth = NexaDpi.Scale(8, dpi);
            var dashY = boxRect.Top + (boxSize - dashHeight) / 2;
            var dashX = boxRect.Left + (boxSize - dashWidth) / 2;
            using var dashBrush = new SolidBrush(dash);
            g.FillRectangle(dashBrush, new Rectangle(dashX, dashY, dashWidth, dashHeight));
        }

        var labelColor = Enabled
            ? (Color)palette[NexaColorRole.TextPrimary].Value
            : (Color)palette[NexaColorRole.TextDisabled].Value;
        var labelFont = typography.ToFont(NexaTypographyRole.Body, dpi);

        var textStartX = CheckAlign == ContentAlignment.MiddleRight
            ? 0
            : boxRect.Right + gap;
        var textWidth = CheckAlign == ContentAlignment.MiddleRight
            ? Math.Max(0, boxRect.Left - gap)
            : Math.Max(0, Width - textStartX);

        if (!string.IsNullOrEmpty(Text))
        {
            var textRect = new Rectangle(textStartX, 0, textWidth, Height);
            TextRenderer.DrawText(g, Text, labelFont, textRect,
                labelColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        if (_showHelperText)
        {
            string helper = HasError && !string.IsNullOrEmpty(_errorText)
                ? _errorText
                : _helperText;
            if (!string.IsNullOrEmpty(helper))
            {
                using var helperFont = typography.ToFont(NexaTypographyRole.Caption, dpi);
                var helperColor = HasError
                    ? (Color)palette[NexaColorRole.Danger].Value
                    : HasSuccess
                        ? (Color)palette[NexaColorRole.Success].Value
                        : (Color)palette[NexaColorRole.TextSecondary].Value;
                var helperSize = TextRenderer.MeasureText(g, helper, helperFont);
                var helperY = (Height - helperSize.Height) / 2;
                var helperX = CheckAlign == ContentAlignment.MiddleRight
                    ? 0
                    : boxRect.Right + gap;
                if (CheckAlign == ContentAlignment.MiddleRight)
                {
                    TextRenderer.DrawText(g, helper, helperFont,
                        new Rectangle(0, Math.Max(0, helperY), Math.Max(0, boxRect.Left - gap), Height),
                        helperColor,
                        TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
                }
                else
                {
                    TextRenderer.DrawText(g, helper, helperFont,
                        new Point(helperX, Math.Max(0, helperY)),
                        helperColor);
                }
            }
        }

        if (Focused && Enabled)
        {
            using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(1, dpi));
            var focusRect = Rectangle.Inflate(boxRect, NexaDpi.Scale(3, dpi), NexaDpi.Scale(3, dpi));
            var focusRadius = radius + NexaDpi.Scale(1, dpi);
            using var focusPath = CreateRoundedPath(focusRect, focusRadius);
            g.DrawPath(focusPen, focusPath);
        }
    }

    private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
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
