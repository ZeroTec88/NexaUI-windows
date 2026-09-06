using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed radio button. Inherits from the native <see cref="RadioButton"/> so all
/// native behavior (parent-scoped grouping, keyboard activation, accessibility,
/// designer support) is preserved. Visual styling is done by overriding
/// <see cref="OnPaint"/> and re-applying theme colors on theme change.
/// </summary>
[DefaultEvent(nameof(CheckedChanged))]
[DefaultProperty(nameof(Checked))]
public class NexaRadioButton : RadioButton
{
    private NexaTextValidationState _validation = NexaTextValidationState.None;
    private string _errorText = string.Empty;
    private string _helperText = string.Empty;
    private bool _showHelperText = true;

    public NexaRadioButton()
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
    [DefaultValue(NexaTextValidationState.None)]
    [Description("Validation state. Drives the ring color when checked.")]
    public NexaTextValidationState ValidationState
    {
        get => _validation;
        set { _validation = value; RefreshColors(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Error text shown next to the radio button when ValidationState is Error.")]
    public string ErrorText
    {
        get => _errorText;
        set { _errorText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Helper text shown next to the radio button when not in error.")]
    public string HelperText
    {
        get => _helperText;
        set { _helperText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the helper/error text is rendered next to the radio button.")]
    public bool ShowHelperText
    {
        get => _showHelperText;
        set { _showHelperText = value; Invalidate(); }
    }

    [Browsable(false)]
    public bool HasError => _validation == NexaTextValidationState.Error;

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

        Color ringColor;
        Color dotColor;
        Color fill = (Color)palette[NexaColorRole.InputBackground].Value;

        if (!Enabled)
        {
            ringColor = (Color)palette[NexaColorRole.InputBorder].Value;
            dotColor = (Color)palette[NexaColorRole.TextDisabled].Value;
        }
        else if (Checked)
        {
            if (HasError)
            {
                ringColor = (Color)palette[NexaColorRole.Danger].Value;
                dotColor = (Color)palette[NexaColorRole.Danger].Value;
            }
            else
            {
                ringColor = (Color)palette[NexaColorRole.Primary].Value;
                dotColor = (Color)palette[NexaColorRole.Primary].Value;
            }
        }
        else
        {
            ringColor = (Color)palette[NexaColorRole.InputBorder].Value;
            dotColor = (Color)palette[NexaColorRole.Primary].Value;
        }

        using (var bgBrush = new SolidBrush(fill))
        {
            g.FillEllipse(bgBrush, boxRect);
        }
        using (var ringPen = new Pen(ringColor, NexaDpi.Scale(1, dpi)))
        {
            g.DrawEllipse(ringPen, boxRect);
        }

        if (Checked)
        {
            var inner = Rectangle.Inflate(boxRect, -NexaDpi.Scale(4, dpi), -NexaDpi.Scale(4, dpi));
            using var dotBrush = new SolidBrush(dotColor);
            g.FillEllipse(dotBrush, inner);
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
            TextRenderer.DrawText(g, Text, labelFont,
                new Rectangle(textStartX, 0, textWidth, Height),
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
                    : (Color)palette[NexaColorRole.TextSecondary].Value;
                var helperSize = TextRenderer.MeasureText(g, helper, helperFont);
                var helperY = (Height - helperSize.Height) / 2;
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
                        new Point(boxRect.Right + gap, Math.Max(0, helperY)),
                        helperColor);
                }
            }
        }

        if (Focused && Enabled)
        {
            using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(1, dpi));
            var focusRect = Rectangle.Inflate(boxRect, NexaDpi.Scale(3, dpi), NexaDpi.Scale(3, dpi));
            g.DrawEllipse(focusPen, focusRect);
        }
    }
}
