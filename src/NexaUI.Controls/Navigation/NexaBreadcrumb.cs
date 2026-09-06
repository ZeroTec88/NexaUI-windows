using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A horizontal breadcrumb navigation control showing hierarchical navigation path.
/// Supports custom separators, clickable items, and current/active item styling.
/// </summary>
[DefaultEvent(nameof(ItemClick))]
[DefaultProperty(nameof(Items))]
[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
public class NexaBreadcrumb : Control
{
    private readonly List<NexaBreadcrumbItem> _items = new();
    private NexaBreadcrumbSeparator _separator = NexaBreadcrumbSeparator.Chevron;
    private string _customSeparatorText = ">";
    private int _itemSpacingDips = 8;
    private int _paddingDips = 12;
    private int _itemHeightDips = 32;

    /// <summary>Initializes a new instance of the <see cref="NexaBreadcrumb"/> class.</summary>
    public NexaBreadcrumb()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        DoubleBuffered = true;
        TabStop = false;
        Height = 40;
        BackColor = Color.Transparent;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Collection of breadcrumb items.</summary>
    [Category("NexaUI")]
    [Description("Collection of breadcrumb items.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<NexaBreadcrumbItem> Items => new(_items);

    /// <summary>Separator style between items.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaBreadcrumbSeparator.Chevron)]
    [Description("Separator style between items.")]
    public NexaBreadcrumbSeparator Separator
    {
        get => _separator;
        set { _separator = value; Invalidate(); }
    }

    /// <summary>Custom separator text when Separator is Custom.</summary>
    [Category("NexaUI")]
    [DefaultValue(">")]
    [Description("Custom separator text when Separator is Custom.")]
    public string CustomSeparatorText
    {
        get => _customSeparatorText;
        set { _customSeparatorText = value ?? ">"; Invalidate(); }
    }

    /// <summary>Spacing between items in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(8)]
    [Description("Spacing between items in device-independent pixels.")]
    public int ItemSpacing
    {
        get => _itemSpacingDips;
        set { _itemSpacingDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Horizontal padding in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(12)]
    [Description("Horizontal padding in device-independent pixels.")]
    public int PaddingDips
    {
        get => _paddingDips;
        set { _paddingDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Height of each item in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(32)]
    [Description("Height of each item in device-independent pixels.")]
    public int ItemHeight
    {
        get => _itemHeightDips;
        set { _itemHeightDips = Math.Max(20, value); Invalidate(); }
    }

    /// <summary>Gets the text representation of the current separator.</summary>
    private string GetSeparatorText()
    {
        return _separator switch
        {
            NexaBreadcrumbSeparator.Chevron => "›",
            NexaBreadcrumbSeparator.Slash => "/",
            NexaBreadcrumbSeparator.GreaterThan => ">",
            NexaBreadcrumbSeparator.Custom => _customSeparatorText,
            _ => "›"
        };
    }

    /// <summary>Updates the theme colors for the control.</summary>
    public void RefreshColors() => RefreshTheme(ThemeManager.Current);

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => RefreshTheme(e.Current);

    private void RefreshTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => RefreshTheme(theme));
            return;
        }

        var palette = theme.Palette;
        BackColor = Color.Transparent;
        Invalidate();
    }

    /// <inheritdoc />
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

        var padding = NexaDpi.Scale(_paddingDips, dpi);
        var itemSpacing = NexaDpi.Scale(_itemSpacingDips, dpi);
        var itemHeight = NexaDpi.Scale(_itemHeightDips, dpi);
        var separatorText = GetSeparatorText();

        var visibleItems = _items.Where(i => i.Visible).ToList();
        if (visibleItems.Count == 0) return;

        var y = (Height - NexaDpi.Scale(20, dpi)) / 2; // Center vertically roughly
        var x = padding;

        for (int i = 0; i < visibleItems.Count; i++)
        {
            var item = visibleItems[i];
            var isLast = i == visibleItems.Count - 1;
            var isEnabled = item.Enabled;

            var textColor = isEnabled
                ? (isLast
                    ? (Color)palette[NexaColorRole.TextPrimary].Value
                    : (Color)palette[NexaColorRole.TextSecondary].Value)
                : (Color)palette[NexaColorRole.TextDisabled].Value;

            using var font = typography.ToFont(NexaTypographyRole.Body, dpi);
            var textSize = TextRenderer.MeasureText(g, item.Text, font);
            var itemRect = new Rectangle(
                (int)x,
                (Height - textSize.Height) / 2,
                textSize.Width,
                textSize.Height);

            // Draw item text
            TextRenderer.DrawText(g, item.Text, font, itemRect, textColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine |
                TextFormatFlags.NoPadding);

            x += itemRect.Width;

            // Draw separator (except after last item)
            if (!isLast)
            {
                var sepColor = (Color)palette[NexaColorRole.TextSecondary].Value;
                var sepX = x + itemSpacing / 2;
                var sepRect = new Rectangle(
                    (int)sepX,
                    (Height - textSize.Height) / 2,
                    itemSpacing,
                    textSize.Height);

                TextRenderer.DrawText(g, separatorText, typography.ToFont(NexaTypographyRole.Body, dpi),
                    sepRect, sepColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);

                x += itemSpacing;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        var dpi = CurrentDpi();
        var padding = NexaDpi.Scale(_paddingDips, dpi);
        var itemSpacing = NexaDpi.Scale(_itemSpacingDips, dpi);

        var visibleItems = _items.Where(i => i.Visible).ToList();
        if (visibleItems.Count == 0) return;

        var x = padding;
        var typography = ThemeManager.Current.Typography;

        using var font = typography.ToFont(NexaTypographyRole.Body, dpi);
        using var graphics = CreateGraphics();

        for (int i = 0; i < visibleItems.Count; i++)
        {
            var item = visibleItems[i];
            if (!item.Enabled) continue;

            var textSize = TextRenderer.MeasureText(graphics, item.Text, font);
            var itemRect = new Rectangle(
                (int)x,
                (Height - textSize.Height) / 2,
                textSize.Width,
                textSize.Height);

            if (itemRect.Contains(e.Location))
            {
                ItemClick?.Invoke(this, new NexaBreadcrumbItemClickEventArgs(item));
                break;
            }

            x += textSize.Width;

            if (i < visibleItems.Count - 1)
            {
                x += NexaDpi.Scale(_itemSpacingDips, dpi);
            }
        }
    }

    /// <summary>Raised when a breadcrumb item is clicked.</summary>
    [Category("NexaUI")]
    [Description("Raised when a breadcrumb item is clicked.")]
    public event EventHandler<NexaBreadcrumbItemClickEventArgs>? ItemClick;
}

/// <summary>
/// Event arguments for breadcrumb item click events.
/// </summary>
public class NexaBreadcrumbItemClickEventArgs : EventArgs
{
    public NexaBreadcrumbItem Item { get; }

    public NexaBreadcrumbItemClickEventArgs(NexaBreadcrumbItem item)
    {
        Item = item;
    }
}