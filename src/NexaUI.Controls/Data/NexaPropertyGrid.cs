using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware PropertyGrid that preserves all native functionality while adding
/// modern NexaUI visual styling. Inherits from the native <see cref="PropertyGrid"/>
/// so SelectedObject, SelectedObjects, property tabs, categorization, sorting,
/// help area, commands, and all native behaviors work exactly as expected.
/// </summary>
[DefaultEvent(nameof(PropertyValueChanged))]
[DefaultProperty(nameof(SelectedObject))]
[ToolboxBitmap(typeof(PropertyGrid))]
public class NexaPropertyGrid : PropertyGrid
{
    private NexaPropertyGridStyle _gridStyle = NexaPropertyGridStyle.Default;
    private bool _themeApplied = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NexaPropertyGrid"/> class.
    /// </summary>
    public NexaPropertyGrid()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable,
            true);

        DoubleBuffered = true;
        ToolbarVisible = true;
        PropertySort = PropertySort.Categorized;
        HelpVisible = true;
        CommandsVisibleIfAvailable = true;
        LineColor = Color.Empty;
        CategoryForeColor = Color.Empty;
        CategorySplitterColor = Color.Empty;
        ViewBackColor = Color.Empty;
        ViewForeColor = Color.Empty;
        ViewBorderColor = Color.Empty;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        RefreshTheme();
    }

    /// <summary>
    /// Gets or sets the visual density style of the property grid.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(NexaPropertyGridStyle.Default)]
    [Description("Visual density style of the property grid.")]
    public NexaPropertyGridStyle GridStyle
    {
        get => _gridStyle;
        set
        {
            if (_gridStyle == value) return;
            _gridStyle = value;
            ApplyGridStyle();
            Invalidate();
        }
    }

    private void ApplyGridStyle()
    {
        var dpi = GetCurrentDpi();
        var padding = _gridStyle switch
        {
            NexaPropertyGridStyle.Compact => NexaDpi.Scale(2, dpi),
            NexaPropertyGridStyle.Comfortable => NexaDpi.Scale(8, dpi),
            _ => NexaDpi.Scale(4, dpi)
        };

        var grid = GetType().GetProperty("GridEntries", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
    }

    private void RefreshTheme()
    {
        if (IsDisposed || Disposing) return;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = GetCurrentDpi();

        BackColor = (Color)palette[NexaColorRole.Surface].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        ViewBackColor = (Color)palette[NexaColorRole.Surface].Value;
        ViewForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        ViewBorderColor = (Color)palette[NexaColorRole.Border].Value;

        CategoryForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        CategorySplitterColor = (Color)palette[NexaColorRole.Border].Value;

        LineColor = (Color)palette[NexaColorRole.Border].Value;

        HelpBackColor = (Color)palette[NexaColorRole.SurfaceVariant].Value;
        HelpForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        HelpBorderColor = (Color)palette[NexaColorRole.Border].Value;

        CommandsBackColor = (Color)palette[NexaColorRole.Surface].Value;
        CommandsForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        CommandsBorderColor = (Color)palette[NexaColorRole.Border].Value;
        CommandsActiveLinkColor = (Color)palette[NexaColorRole.Primary].Value;
        CommandsDisabledLinkColor = (Color)palette[NexaColorRole.TextDisabled].Value;
        CommandsLinkColor = (Color)palette[NexaColorRole.Primary].Value;

        DisabledItemForeColor = (Color)palette[NexaColorRole.TextDisabled].Value;
        SelectedItemWithFocusForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        var font = theme.Typography.ToFont(NexaTypographyRole.Body, dpi);
        Font = font;

        ApplyGridStyle();
        _themeApplied = true;
        Invalidate();
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => { if (!IsDisposed && !Disposing) RefreshTheme(); });
            return;
        }
        RefreshTheme();
    }

    private float GetCurrentDpi()
    {
        if (!IsHandleCreated || DesignMode) return NexaDpi.BaseDpi;
        using var g = Graphics.FromHwnd(Handle);
        return g.DpiY;
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RefreshTheme();
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }

    protected override void OnSelectedObjectsChanged(EventArgs e)
    {
        base.OnSelectedObjectsChanged(e);
        Invalidate();
    }
}

/// <summary>
/// Visual density styles for the NexaPropertyGrid.
/// </summary>
public enum NexaPropertyGridStyle
{
    /// <summary>Standard density with normal spacing.</summary>
    Default,
    /// <summary>Compact density with minimal padding.</summary>
    Compact,
    /// <summary>Comfortable density with generous padding.</summary>
    Comfortable
}