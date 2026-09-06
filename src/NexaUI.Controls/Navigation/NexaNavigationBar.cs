using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A vertical navigation bar control for application-level navigation.
/// Supports expanded and compact modes, selection, badges, icons, and disabled items.
/// </summary>
[DefaultEvent(nameof(SelectedItemChanged))]
[DefaultProperty(nameof(Items))]
[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
public class NexaNavigationBar : Control
{
    private readonly List<NexaNavigationItem> _items = new();
    private NexaNavigationMode _mode = NexaNavigationMode.Expanded;
    private int _itemHeightDips = 40;
    private int _indentDips = 12;
    private int _iconSizeDips = 20;
    private int _selectedIndex = -1;

    /// <summary>Initializes a new instance of the <see cref="NexaNavigationBar"/> class.</summary>
    public NexaNavigationBar()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        DoubleBuffered = true;
        TabStop = true;
        Width = 240;
        BackColor = Color.Transparent;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Collection of navigation items.</summary>
    [Category("NexaUI")]
    [Description("Collection of navigation items.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<NexaNavigationItem> Items => new(_items);

    /// <summary>Current display mode of the navigation bar.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaNavigationMode.Expanded)]
    [Description("Current display mode of the navigation bar.")]
    public NexaNavigationMode Mode
    {
        get => _mode;
        set
        {
            _mode = value;
            UpdateWidth();
            Invalidate();
        }
    }

    /// <summary>Height of each navigation item in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(40)]
    [Description("Height of each navigation item in device-independent pixels.")]
    public int ItemHeight
    {
        get => _itemHeightDips;
        set { _itemHeightDips = Math.Max(24, value); Invalidate(); }
    }

    /// <summary>Indent from left edge in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(12)]
    [Description("Indent from left edge in device-independent pixels.")]
    public int Indent
    {
        get => _indentDips;
        set { _indentDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Icon size in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(20)]
    [Description("Icon size in device-independent pixels.")]
    public int IconSize
    {
        get => _iconSizeDips;
        set { _iconSizeDips = Math.Max(12, value); Invalidate(); }
    }

    /// <summary>Gets or sets the index of the currently selected item.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (value >= -1 && value < _items.Count)
            {
                SetSelectedIndex(value);
            }
        }
    }

    /// <summary>Gets the currently selected item.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public NexaNavigationItem? SelectedItem => _selectedIndex >= 0 && _selectedIndex < _items.Count ? _items[_selectedIndex] : null;

    /// <summary>Adds a navigation item to the collection.</summary>
    public void AddItem(NexaNavigationItem item)
    {
        if (item == null) return;
        _items.Add(item);
        Invalidate();
    }

    /// <summary>Removes a navigation item from the collection.</summary>
    public bool RemoveItem(NexaNavigationItem item)
    {
        var result = _items.Remove(item);
        if (_selectedIndex >= _items.Count)
        {
            _selectedIndex = _items.Count - 1;
        }
        Invalidate();
        return result;
    }

    /// <summary>Clears all items from the navigation bar.</summary>
    public void ClearItems()
    {
        _items.Clear();
        _selectedIndex = -1;
        Invalidate();
    }

    /// <summary>Toggles between Expanded and Compact mode.</summary>
    public void ToggleMode()
    {
        Mode = _mode == NexaNavigationMode.Expanded ? NexaNavigationMode.Compact : NexaNavigationMode.Expanded;
    }

    private void UpdateWidth()
    {
        var dpi = CurrentDpi();
        var compactWidth = NexaDpi.Scale(_indentDips * 2 + _iconSizeDips, dpi);
        var expandedWidth = NexaDpi.Scale(240, dpi);
        Width = _mode == NexaNavigationMode.Compact ? compactWidth : expandedWidth;
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
        BackColor = (Color)palette[NexaColorRole.Surface].Value;
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

        var itemHeight = NexaDpi.Scale(_itemHeightDips, dpi);
        var indent = NexaDpi.Scale(_indentDips, dpi);
        var iconSize = NexaDpi.Scale(_iconSizeDips, dpi);
        var badgeRadius = NexaDpi.Scale(10, dpi);
        var badgePadding = NexaDpi.Scale(4, dpi);

        var visibleItems = _items.Where(i => i.Visible).ToList();

        for (int i = 0; i < visibleItems.Count; i++)
        {
            var item = visibleItems[i];
            var isSelected = i == _selectedIndex;
            var y = i * itemHeight;

            var itemRect = new Rectangle(0, y, Width, itemHeight);

            // Draw selection background
            if (isSelected && item.Enabled)
            {
                var selRect = new Rectangle(
                    indent / 2,
                    y + 2,
                    Width - indent,
                    itemHeight - 4);
                var radius = NexaDpi.Scale(6, dpi);
                using var path = CreateRoundedPath(selRect, radius);
                using var brush = new SolidBrush((Color)palette[NexaColorRole.Primary].Subtle.Value);
                g.FillPath(brush, path);
            }
            else if (isSelected && !item.Enabled)
            {
                var selRect = new Rectangle(
                    indent / 2,
                    y + 2,
                    Width - indent,
                    itemHeight - 4);
                var radius = NexaDpi.Scale(6, dpi);
                using var path = CreateRoundedPath(selRect, radius);
                using var brush = new SolidBrush((Color)palette[NexaColorRole.SurfaceVariant].Value);
                g.FillPath(brush, path);
            }

            // Draw icon
            var iconX = indent;
            var iconY = y + (itemHeight - iconSize) / 2;

            if (item.IconKind != NexaIconKind.None)
            {
                var iconColor = item.Enabled
                    ? (isSelected
                        ? (Color)palette[NexaColorRole.Primary].Value
                        : (Color)palette[NexaColorRole.TextSecondary].Value)
                    : (Color)palette[NexaColorRole.TextDisabled].Value;

                var iconBmp = NexaIconProvider.ToBitmap(item.IconKind, new Size(iconSize, iconSize), iconColor);
                if (iconBmp != null)
                {
                    g.DrawImage(iconBmp, iconX, iconY, iconSize, iconSize);
                }
            }

            // Draw text (only in Expanded mode)
            if (_mode == NexaNavigationMode.Expanded)
            {
                var textX = indent + iconSize + NexaDpi.Scale(12, dpi);
                var textRect = new Rectangle(
                    textX,
                    y,
                    Width - textX - NexaDpi.Scale(12, dpi),
                    itemHeight);

                var textColor = item.Enabled
                    ? (isSelected
                        ? (Color)palette[NexaColorRole.TextPrimary].Value
                        : (Color)palette[NexaColorRole.TextPrimary].Value)
                    : (Color)palette[NexaColorRole.TextDisabled].Value;

                using var font = typography.ToFont(NexaTypographyRole.Body, dpi);
                TextRenderer.DrawText(g, item.Text, font, textRect, textColor,
                    TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }

            // Draw badge
            if (item.BadgeVisible && !string.IsNullOrEmpty(item.BadgeText))
            {
                var badgeText = item.BadgeText;
                using var font = typography.ToFont(NexaTypographyRole.Caption, dpi);
                var textSize = TextRenderer.MeasureText(g, badgeText, font);
                var badgeWidth = textSize.Width + badgePadding * 2;
                var badgeHeight = Math.Max(textSize.Height + badgePadding / 2, badgeRadius * 2);

                var badgeX = _mode == NexaNavigationMode.Compact
                    ? (Width - badgeWidth) / 2
                    : Width - indent - badgeWidth;

                var badgeY = y + (itemHeight - badgeHeight) / 2;

                var badgeRect = new Rectangle(badgeX, badgeY, badgeWidth, badgeHeight);
                var badgeRadiusPx = Math.Min(badgeHeight / 2, NexaDpi.Scale(10, dpi));

                using var path = CreateRoundedPath(badgeRect, badgeRadiusPx);
                using var brush = new SolidBrush((Color)palette[NexaColorRole.Primary].Value);
                g.FillPath(brush, path);

                var textColor = (Color)palette[NexaColorRole.TextOnAccent].Value;
                var textRect = new RectangleF(badgeRect.X, badgeRect.Y, badgeRect.Width, badgeRect.Height);
                TextRenderer.DrawText(g, item.BadgeText, typography.ToFont(NexaTypographyRole.Caption, dpi),
                    badgeRect, textColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }
        }
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        var dpi = CurrentDpi();
        var itemHeight = NexaDpi.Scale(_itemHeightDips, dpi);
        var visibleItems = _items.Where(i => i.Visible).ToList();

        var index = e.Y / itemHeight;
        if (index >= 0 && index < visibleItems.Count)
        {
            var item = visibleItems[index];
            if (item.Enabled)
            {
                SetSelectedIndex(_items.IndexOf(item));
                Focus();
            }
        }
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        var visibleItems = _items.Where(i => i.Visible).ToList();
        if (visibleItems.Count == 0) return;

        var newIndex = _selectedIndex;
        switch (e.KeyCode)
        {
            case Keys.Down:
                newIndex = Math.Min(_selectedIndex + 1, visibleItems.Count - 1);
                break;
            case Keys.Up:
                newIndex = Math.Max(_selectedIndex - 1, 0);
                break;
            case Keys.Home:
                newIndex = 0;
                break;
            case Keys.End:
                newIndex = visibleItems.Count - 1;
                break;
            case Keys.Enter:
            case Keys.Space:
                if (_selectedIndex >= 0 && _selectedIndex < visibleItems.Count)
                {
                    var item = visibleItems[_selectedIndex];
                    if (item.Enabled)
                    {
                        ItemClick?.Invoke(this, new NexaNavigationItemEventArgs(item));
                    }
                }
                return;
        }

        if (newIndex != _selectedIndex)
        {
            var actualIndex = _items.IndexOf(visibleItems[newIndex]);
            SetSelectedIndex(actualIndex);
            e.Handled = true;
        }
    }

    private void SetSelectedIndex(int index)
    {
        if (index == _selectedIndex) return;

        if (_selectedIndex >= 0 && _selectedIndex < _items.Count)
        {
            _items[_selectedIndex].Selected = false;
        }

        _selectedIndex = index;

        if (_selectedIndex >= 0 && _selectedIndex < _items.Count)
        {
            _items[_selectedIndex].Selected = true;
            SelectedItemChanged?.Invoke(this, new NexaNavigationItemEventArgs(_items[_selectedIndex]));
        }

        Invalidate();
    }

    /// <summary>Raised when the selected item changes.</summary>
    [Category("NexaUI")]
    [Description("Raised when the selected item changes.")]
    public event EventHandler<NexaNavigationItemEventArgs>? SelectedItemChanged;

    /// <summary>Raised when an item is clicked.</summary>
    [Category("NexaUI")]
    [Description("Raised when an item is clicked.")]
    public event EventHandler<NexaNavigationItemEventArgs>? ItemClick;

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

/// <summary>
/// Event arguments for navigation item events.
/// </summary>
public class NexaNavigationItemEventArgs : EventArgs
{
    public NexaNavigationItem Item { get; }

    public NexaNavigationItemEventArgs(NexaNavigationItem item)
    {
        Item = item;
    }
}