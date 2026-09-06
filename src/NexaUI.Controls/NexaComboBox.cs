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
/// A themed combo box. Inherits from the native <see cref="ComboBox"/> so all native
/// behavior (data binding, auto-complete, drop-down list, keyboard navigation,
/// selection events, designer support) is preserved. Visual styling of the editor
/// area is done by overriding <see cref="OnPaint"/>; the drop-down list uses the
/// native chrome (a Windows constraint) but is colored to match the active theme.
/// </summary>
[DefaultEvent(nameof(SelectedIndexChanged))]
[DefaultProperty(nameof(Text))]
public class NexaComboBox : ComboBox
{
    private NexaComboBoxStyle _style = NexaComboBoxStyle.Outlined;
    private NexaTextValidationState _validation = NexaTextValidationState.None;
    private string _errorText = string.Empty;
    private string _helperText = string.Empty;
    private bool _showHelperText = true;
    private string _placeholderText = string.Empty;

    public NexaComboBox()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable,
            true);

        DrawMode = DrawMode.OwnerDrawFixed;
        DoubleBuffered = true;
        TabStop = true;
        ItemHeight = 22;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        RefreshColors();
    }

    [Category("NexaUI")]
    [DefaultValue(NexaComboBoxStyle.Outlined)]
    [Description("Visual style variant of the combo box.")]
    public NexaComboBoxStyle Style
    {
        get => _style;
        set { _style = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaTextValidationState.None)]
    [Description("Validation state. Drives the border color and the helper text color.")]
    public NexaTextValidationState ValidationState
    {
        get => _validation;
        set { _validation = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Error text shown below the combo box when ValidationState is Error.")]
    public string ErrorText
    {
        get => _errorText;
        set { _errorText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Helper text shown below the combo box when not in error.")]
    public string HelperText
    {
        get => _helperText;
        set { _helperText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the helper/error text is rendered below the combo box.")]
    public bool ShowHelperText
    {
        get => _showHelperText;
        set { _showHelperText = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Placeholder text shown when no item is selected and the text is empty.")]
    public string PlaceholderText
    {
        get => _placeholderText;
        set { _placeholderText = value ?? string.Empty; Invalidate(); }
    }

    [Browsable(false)]
    public bool HasError => _validation == NexaTextValidationState.Error;

    [Browsable(false)]
    public bool HasSuccess => _validation == NexaTextValidationState.Success;

    public void RefreshColors()
    {
        if (IsDisposed || Disposing) return;
        var palette = ThemeManager.Current.Palette;
        var enabledText = (Color)palette[NexaColorRole.TextPrimary].Value;
        var disabledText = (Color)palette[NexaColorRole.TextDisabled].Value;
        ForeColor = Enabled ? enabledText : disabledText;
        BackColor = _style == NexaComboBoxStyle.Filled
            ? (Color)palette[NexaColorRole.SurfaceVariant].Value
            : (Color)palette[NexaColorRole.InputBackground].Value;
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

    protected override void OnResize(System.EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }

    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        if (e.Index < 0) return;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        var isSelected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        var isDisabled = !Enabled;

        var bg = isSelected && !isDisabled
            ? (Color)palette[NexaColorRole.Primary].Value
            : (Color)palette[NexaColorRole.Surface].Value;
        var fg = isSelected && !isDisabled
            ? (Color)palette[NexaColorRole.TextOnAccent].Value
            : isDisabled
                ? (Color)palette[NexaColorRole.TextDisabled].Value
                : (Color)palette[NexaColorRole.TextPrimary].Value;

        using (var bgBrush = new SolidBrush(bg))
        {
            e.Graphics.FillRectangle(bgBrush, e.Bounds);
        }

        var text = e.Index >= 0 && e.Index < Items.Count ? Convert.ToString(Items[e.Index]) ?? string.Empty : string.Empty;
        using var font = typography.ToFont(NexaTypographyRole.Body, dpi);
        var textRect = new Rectangle(e.Bounds.Left + 8, e.Bounds.Top, e.Bounds.Width - 8, e.Bounds.Height);
        TextRenderer.DrawText(e.Graphics, text, font, textRect, fg,
            TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);

        e.DrawFocusRectangle();
    }

    protected override void OnMeasureItem(MeasureItemEventArgs e)
    {
        base.OnMeasureItem(e);
        var dpi = CurrentDpi();
        e.ItemHeight = Math.Max(e.ItemHeight, NexaDpi.Scale(22, dpi));
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

        var rect = new Rectangle(0, 0, Width, Height);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        var hasFocus = Focused && Enabled;
        Color borderColor;
        Color fill;

        if (!Enabled)
        {
            borderColor = (Color)palette[NexaColorRole.InputBorder].Value;
            fill = _style == NexaComboBoxStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (HasError)
        {
            borderColor = (Color)palette[NexaColorRole.Danger].Value;
            fill = (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (HasSuccess)
        {
            borderColor = (Color)palette[NexaColorRole.Success].Value;
            fill = (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else if (hasFocus)
        {
            borderColor = (Color)palette[NexaColorRole.Primary].Value;
            fill = _style == NexaComboBoxStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : (Color)palette[NexaColorRole.InputBackground].Value;
        }
        else
        {
            borderColor = (Color)palette[NexaColorRole.InputBorder].Value;
            fill = _style == NexaComboBoxStyle.Filled
                ? (Color)palette[NexaColorRole.SurfaceVariant].Value
                : _style == NexaComboBoxStyle.Flat
                    ? Color.Transparent
                    : (Color)palette[NexaColorRole.InputBackground].Value;
        }

        var radius = NexaDpi.Scale(2, dpi);

        if (_style == NexaComboBoxStyle.Flat)
        {
            using var pen = new Pen(borderColor, NexaDpi.Scale(1, dpi));
            g.DrawLine(pen, 0, rect.Bottom - 1, rect.Width, rect.Bottom - 1);
            if (hasFocus)
            {
                using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(1, dpi));
                g.DrawLine(focusPen, 0, rect.Bottom + 1, rect.Width, rect.Bottom + 1);
            }
        }
        else
        {
            var inflated = Rectangle.Inflate(rect, -1, -1);
            using (var path = CreateRoundedPath(inflated, radius))
            {
                if (fill.A > 0)
                {
                    using var fillBrush = new SolidBrush(fill);
                    g.FillPath(fillBrush, path);
                }
                using var pen = new Pen(borderColor, NexaDpi.Scale(1, dpi));
                g.DrawPath(pen, path);
            }

            if (hasFocus)
            {
                var focusRect = Rectangle.Inflate(inflated, -NexaDpi.Scale(2, dpi), -NexaDpi.Scale(2, dpi));
                using var focusPath = CreateRoundedPath(focusRect, Math.Max(0, radius - NexaDpi.Scale(2, dpi)));
                using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(1, dpi));
                g.DrawPath(focusPen, focusPath);
            }
        }

        var textColor = Enabled
            ? (Color)palette[NexaColorRole.TextPrimary].Value
            : (Color)palette[NexaColorRole.TextDisabled].Value;
        var placeholderColor = (Color)palette[NexaColorRole.InputPlaceholder].Value;
        var sidePad = NexaDpi.Scale(10, dpi);
        var arrowWidth = NexaDpi.Scale(28, dpi);

        var arrowRect = new Rectangle(rect.Right - arrowWidth, 0, arrowWidth, rect.Height);
        var textRect = new Rectangle(rect.Left + sidePad, 0, rect.Width - sidePad - arrowWidth, rect.Height);

        using var font = typography.ToFont(NexaTypographyRole.Body, dpi);
        var displayText = Text;
        var showPlaceholder = !DesignMode
            && string.IsNullOrEmpty(displayText)
            && SelectedIndex < 0
            && !string.IsNullOrEmpty(_placeholderText);

        if (showPlaceholder)
        {
            TextRenderer.DrawText(g, _placeholderText, font, textRect, placeholderColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }
        else if (!string.IsNullOrEmpty(displayText))
        {
            TextRenderer.DrawText(g, displayText, font, textRect, textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }

        var arrowColor = Enabled
            ? (Color)palette[NexaColorRole.TextSecondary].Value
            : (Color)palette[NexaColorRole.TextDisabled].Value;
        var arrowSize = NexaDpi.Scale(10, dpi);
        var arrowBmp = NexaIconProvider.ToBitmap(NexaIconKind.ChevronDown, new Size(arrowSize, arrowSize), arrowColor);
        if (arrowBmp is not null)
        {
            var ax = arrowRect.Left + (arrowRect.Width - arrowBmp.Width) / 2;
            var ay = arrowRect.Top + (arrowRect.Height - arrowBmp.Height) / 2;
            g.DrawImage(arrowBmp, ax, ay);
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
