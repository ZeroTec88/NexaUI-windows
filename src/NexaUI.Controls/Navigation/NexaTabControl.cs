using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed tab control inheriting from the native <see cref="TabControl"/>.
/// Preserves all native behavior (tab pages, selection, keyboard/mouse navigation,
/// ImageList, ItemSize, Padding, etc.) while adding NexaUI theming capabilities
/// with multiple visual styles.
/// </summary>
[DefaultEvent(nameof(SelectedIndexChanged))]
[DefaultProperty(nameof(TabPages))]
[Designer("System.Windows.Forms.Design.TabControlDesigner, System.Design")]
public class NexaTabControl : TabControl
{
    private NexaTabStyle _tabStyle = NexaTabStyle.Default;
    private int _tabHeaderHeightDips = 40;
    private int _activeIndicatorThicknessDips = 3;
    private int _activeIndicatorLengthDips = 0; // 0 = full width
    private int _tabSpacingDips = 8;
    private bool _showCloseButton = false;

    /// <summary>Initializes a new instance of the <see cref="NexaTabControl"/> class.</summary>
    public NexaTabControl()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        DoubleBuffered = true;
        DrawMode = TabDrawMode.OwnerDrawFixed;
        Alignment = TabAlignment.Top;
        HotTrack = true;
        Padding = new Point(12, 8);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Visual style of the tabs.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaTabStyle.Default)]
    [Description("Visual style of the tabs (Default, Underline, Pill).")]
    public NexaTabStyle TabStyle
    {
        get => _tabStyle;
        set { _tabStyle = value; Invalidate(); UpdatePadding(); }
    }

    /// <summary>Height of the tab header area in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(40)]
    [Description("Height of the tab header area in device-independent pixels.")]
    public int TabHeaderHeight
    {
        get => _tabHeaderHeightDips;
        set { _tabHeaderHeightDips = Math.Max(20, value); UpdateItemSize(); }
    }

    /// <summary>Thickness of the active indicator line in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(3)]
    [Description("Thickness of the active indicator line in device-independent pixels.")]
    public int ActiveIndicatorThickness
    {
        get => _activeIndicatorThicknessDips;
        set { _activeIndicatorThicknessDips = Math.Max(1, value); Invalidate(); }
    }

    /// <summary>Length of the active indicator in DIPs. 0 = full tab width.</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Length of the active indicator in device-independent pixels. 0 = full tab width.")]
    public int ActiveIndicatorLength
    {
        get => _activeIndicatorLengthDips;
        set { _activeIndicatorLengthDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Spacing between tabs in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(8)]
    [Description("Spacing between tabs in device-independent pixels.")]
    public int TabSpacing
    {
        get => _tabSpacingDips;
        set { _tabSpacingDips = Math.Max(0, value); UpdateItemSize(); }
    }

    /// <summary>Whether to show a close button on tabs (when implemented).</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether to show a close button on tabs.")]
    public bool ShowCloseButton
    {
        get => _showCloseButton;
        set { _showCloseButton = value; Invalidate(); }
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

        BackColor = (Color)theme.Palette[NexaColorRole.Background].Value;
        Invalidate();
    }

    private void UpdateItemSize()
    {
        if (IsHandleCreated)
        {
            var dpi = CurrentDpi();
            var height = NexaDpi.Scale(_tabHeaderHeightDips, CurrentDpi());
            ItemSize = new Size(0, height);
        }
    }

    private void UpdatePadding()
    {
        // Pill style needs more padding for rounded look
        if (_tabStyle == NexaTabStyle.Pill)
        {
            Padding = new Point(16, 10);
        }
        else
        {
            Padding = new Point(12, 8);
        }
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateItemSize();
        UpdatePadding();
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

        var tabHeight = NexaDpi.Scale(_tabHeaderHeightDips, dpi);
        var indicatorThickness = NexaDpi.Scale(_activeIndicatorThicknessDips, dpi);
        var indicatorLength = _activeIndicatorLengthDips > 0
            ? NexaDpi.Scale(_activeIndicatorLengthDips, dpi)
            : 0;
        var tabSpacing = NexaDpi.Scale(_tabSpacingDips, dpi);

        // Draw tab header background
        var headerRect = new Rectangle(0, 0, Width, tabHeight);
        using (var bgBrush = new SolidBrush((Color)palette[NexaColorRole.Surface].Value))
        {
            g.FillRectangle(bgBrush, headerRect);
        }

        // Draw bottom border line
        using (var borderPen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F))
        {
            g.DrawLine(borderPen, 0, tabHeight - 1, Width, tabHeight - 1);
        }

        // Draw each tab
        for (int i = 0; i < TabCount; i++)
        {
            var tabRect = GetTabRect(i);
            var isSelected = i == SelectedIndex;
            var isEnabled = TabPages[i]?.Enabled ?? true;

            var textColor = isEnabled
                ? (isSelected
                    ? (Color)palette[NexaColorRole.TextPrimary].Value
                    : (Color)palette[NexaColorRole.TextSecondary].Value)
                : (Color)palette[NexaColorRole.TextDisabled].Value;

            var backColor = isSelected
                ? (Color)palette[NexaColorRole.Surface].Value
                : Color.Transparent;

            // Draw tab background for Pill style
            if (_tabStyle == NexaTabStyle.Pill && isSelected)
            {
                var pillRect = new Rectangle(
                    tabRect.X + 2,
                    tabRect.Y + 4,
                    tabRect.Width - 4,
                    tabRect.Height - 8);
                var radius = Math.Min(pillRect.Height / 2, 12);
                using var path = CreateRoundedPath(pillRect, radius);
                using var pillBrush = new SolidBrush((Color)palette[NexaColorRole.Primary].Subtle.Value);
                g.FillPath(pillBrush, path);
            }

            // Draw active indicator
            if (isSelected && indicatorThickness > 0)
            {
                var indicatorColor = (Color)palette[NexaColorRole.Primary].Value;
                var indicatorY = tabRect.Bottom - indicatorThickness;

                int indicatorX, indicatorW;
                if (indicatorLength > 0)
                {
                    indicatorX = tabRect.X + (tabRect.Width - indicatorLength) / 2;
                    indicatorW = Math.Min(indicatorLength, tabRect.Width);
                }
                else
                {
                    indicatorX = tabRect.X;
                    indicatorW = tabRect.Width;
                }

                var indicatorRect = new Rectangle(indicatorX, indicatorY, indicatorW, indicatorThickness);

                if (_tabStyle == NexaTabStyle.Pill)
                {
                    // Rounded indicator for pill style
                    var radius = indicatorThickness / 2;
                    using var path = CreateRoundedPath(indicatorRect, radius);
                    using var brush = new SolidBrush(indicatorColor);
                    g.FillPath(brush, path);
                }
                else
                {
                    // Straight line indicator for Default/Underline
                    using var brush = new SolidBrush(indicatorColor);
                    g.FillRectangle(brush, indicatorRect);
                }
            }

            // Draw tab text
            var tabPage = TabPages[i];
            var text = tabPage?.Text ?? string.Empty;
            if (!string.IsNullOrEmpty(text))
            {
                using var font = typography.ToFont(NexaTypographyRole.Label, dpi);
                var textRect = new RectangleF(tabRect.X, tabRect.Y, tabRect.Width, tabRect.Height);
                TextRenderer.DrawText(g, text, font, Rectangle.Round(textRect), textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }
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