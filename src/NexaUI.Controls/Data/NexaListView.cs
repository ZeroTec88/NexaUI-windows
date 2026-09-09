using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware ListView that preserves all native functionality while adding
/// modern NexaUI visual styling. Inherits from the native <see cref="ListView"/>
/// so items, groups, columns, views, selection, checkboxes, image lists, and
/// all native behaviors work exactly as expected.
/// </summary>
[DefaultEvent(nameof(SelectedIndexChanged))]
[DefaultProperty(nameof(Items))]
[ToolboxBitmap(typeof(ListView))]
public class NexaListView : ListView
{
    private NexaListStyle _listStyle = NexaListStyle.Default;
    private bool _ownerDrawInitialized = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NexaListView"/> class.
    /// </summary>
    public NexaListView()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable,
            true);

        DoubleBuffered = true;
        FullRowSelect = true;
        HideSelection = false;
        BorderStyle = BorderStyle.None;
        HeaderStyle = ColumnHeaderStyle.Nonclickable;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        RefreshTheme();
    }

    /// <summary>
    /// Gets or sets the visual density style of the list view.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(NexaListStyle.Default)]
    [Description("Visual density style of the list view.")]
    public NexaListStyle ListStyle
    {
        get => _listStyle;
        set
        {
            if (_listStyle == value) return;
            _listStyle = value;
            ApplyListStyle();
            Invalidate();
        }
    }

    private void ApplyListStyle()
    {
        var dpi = GetCurrentDpi();
        var padding = _listStyle switch
        {
            NexaListStyle.Compact => NexaDpi.Scale(2, dpi),
            NexaListStyle.Spacious => NexaDpi.Scale(10, dpi),
            _ => NexaDpi.Scale(6, dpi)
        };

        if (View == View.Details)
        {
            foreach (ColumnHeader col in Columns)
            {
                col.Width += padding * 2;
            }
        }
    }

    private void RefreshTheme()
    {
        if (IsDisposed || Disposing) return;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = GetCurrentDpi();

        BackColor = (Color)palette[NexaColorRole.Surface].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        if (View == View.Details)
        {
            OwnerDraw = true;
            if (!_ownerDrawInitialized)
            {
                DrawColumnHeader += OnDrawColumnHeader;
                DrawItem += OnDrawItem;
                DrawSubItem += OnDrawSubItem;
                _ownerDrawInitialized = true;
            }
        }
        else
        {
            OwnerDraw = false;
        }

        Invalidate();
    }

    private void OnDrawColumnHeader(object? sender, DrawListViewColumnHeaderEventArgs e)
    {
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = GetCurrentDpi();

        e.Graphics.FillRectangle(
            new SolidBrush((Color)palette[NexaColorRole.Surface].Value),
            e.Bounds);

        using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1);
        e.Graphics.DrawLine(pen, e.Bounds.Left, e.Bounds.Bottom - 1, e.Bounds.Right, e.Bounds.Bottom - 1);

        var textRect = new Rectangle(
            e.Bounds.Left + NexaDpi.Scale(12, dpi),
            e.Bounds.Top,
            e.Bounds.Width - NexaDpi.Scale(24, dpi),
            e.Bounds.Height);

        using var font = theme.Typography.ToFont(NexaTypographyRole.BodyStrong, dpi);
        TextRenderer.DrawText(
            e.Graphics,
            e.Header.Text,
            font,
            textRect,
            (Color)palette[NexaColorRole.TextPrimary].Value,
            TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
    }

    private void OnDrawItem(object? sender, DrawListViewItemEventArgs e)
    {
        if (View != View.Details)
        {
            e.DrawDefault = true;
            return;
        }
        e.DrawDefault = false;
    }

    private void OnDrawSubItem(object? sender, DrawListViewSubItemEventArgs e)
    {
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = GetCurrentDpi();

        var item = e.Item;
        var bounds = e.Bounds;
        var isSelected = item.Selected && Focused;
        var isHot = item.Selected && !Focused;

        Color bg;
        Color fg;

        if (isSelected)
        {
            bg = (Color)palette[NexaColorRole.Primary].Value;
            fg = (Color)palette[NexaColorRole.TextOnAccent].Value;
        }
        else if (isHot)
        {
            bg = (Color)palette[NexaColorRole.Primary].Subtle.Value;
            fg = (Color)palette[NexaColorRole.Primary].Value;
        }
        else if (e.ItemIndex % 2 == 1)
        {
            bg = (Color)palette[NexaColorRole.SurfaceVariant].Value;
            fg = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        else
        {
            bg = (Color)palette[NexaColorRole.Surface].Value;
            fg = (Color)palette[NexaColorRole.TextPrimary].Value;
        }

        using (var brush = new SolidBrush(bg))
        {
            e.Graphics.FillRectangle(brush, bounds);
        }

        var textRect = new Rectangle(
            bounds.Left + NexaDpi.Scale(12, dpi),
            bounds.Top,
            bounds.Width - NexaDpi.Scale(24, dpi),
            bounds.Height);

        if (e.ColumnIndex == 0 && SmallImageList != null && item.ImageIndex >= 0 && item.ImageIndex < SmallImageList.Images.Count)
        {
            var img = SmallImageList.Images[item.ImageIndex];
            var imgSize = NexaDpi.Scale(16, dpi);
            var imgRect = new Rectangle(
                bounds.Left + NexaDpi.Scale(8, dpi),
                bounds.Top + (bounds.Height - imgSize) / 2,
                imgSize, imgSize);
            e.Graphics.DrawImage(img, imgRect);
            textRect.X = imgRect.Right + NexaDpi.Scale(8, dpi);
            textRect.Width = bounds.Right - textRect.X - NexaDpi.Scale(12, dpi);
        }

        if (CheckBoxes && e.ColumnIndex == 0)
        {
            var checkSize = NexaDpi.Scale(16, dpi);
            var checkRect = new Rectangle(
                bounds.Left + NexaDpi.Scale(8, dpi),
                bounds.Top + (bounds.Height - checkSize) / 2,
                checkSize, checkSize);

            DrawCheckBox(e.Graphics, checkRect, item.Checked, fg);
            textRect.X = checkRect.Right + NexaDpi.Scale(8, dpi);
            textRect.Width = bounds.Right - textRect.X - NexaDpi.Scale(12, dpi);
        }

        using var font = theme.Typography.ToFont(NexaTypographyRole.Body, dpi);
        TextRenderer.DrawText(
            e.Graphics,
            e.SubItem.Text,
            font,
            textRect,
            fg,
            TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);

        if (isSelected && Focused && ShowFocusCues)
        {
            using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(1, dpi));
            var focusRect = Rectangle.Inflate(bounds, -NexaDpi.Scale(2, dpi), -NexaDpi.Scale(2, dpi));
            e.Graphics.DrawRectangle(focusPen, focusRect);
        }
    }

    private void DrawCheckBox(Graphics g, Rectangle rect, bool checked_, Color color)
    {
        using var pen = new Pen(color, NexaDpi.Scale(2, GetCurrentDpi()));
        var r = rect;
        r.Width -= 1; r.Height -= 1;
        var radius = NexaDpi.Scale(2, GetCurrentDpi());

        using var path = CreateRoundedRect(r, radius);
        g.DrawPath(pen, path);

        if (checked_)
        {
            var cx = rect.X + rect.Width / 2;
            var cy = rect.Y + rect.Height / 2;
            var checkPoints = new Point[]
            {
                new(cx - rect.Width / 4, cy),
                new(cx - rect.Width / 8, cy + rect.Height / 4),
                new(cx + rect.Width / 3, cy - rect.Height / 4)
            };
            g.DrawLines(pen, checkPoints);
        }
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

    private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
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
/// Visual density styles for the NexaListView.
/// </summary>
public enum NexaListStyle
{
    /// <summary>Standard density with normal spacing.</summary>
    Default,
    /// <summary>Compact density with minimal padding.</summary>
    Compact,
    /// <summary>Spacious density with generous padding.</summary>
    Spacious
}