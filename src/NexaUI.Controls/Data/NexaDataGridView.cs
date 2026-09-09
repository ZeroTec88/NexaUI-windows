using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware DataGridView that preserves all native functionality while adding
/// modern NexaUI visual styling. Inherits from the native <see cref="DataGridView"/>
/// so columns, rows, data binding, sorting, editing, virtual mode, and all native
/// behaviors work exactly as expected.
/// </summary>
[DefaultEvent(nameof(DataGridView.CellContentClick))]
[DefaultProperty(nameof(DataSource))]
[ToolboxBitmap(typeof(DataGridView))]
public class NexaDataGridView : DataGridView
{
    private NexaGridStyle _gridStyle = NexaGridStyle.Default;
    private NexaHeaderStyle _headerStyle = NexaHeaderStyle.Standard;
    private bool _showRowNumbers = false;
    private bool _showHorizontalGridLines = true;
    private bool _showVerticalGridLines = true;
    private bool _alternateRowColors = true;
    private int _rowCornerRadius = 0;
    private int _headerHeight = 32;
    private int _rowHeight = 32;
    private bool _autoSizeRowsOnPaint = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="NexaDataGridView"/> class.
    /// </summary>
    public NexaDataGridView()
    {
        EnableHeadersVisualStyles = false;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        ApplyDefaultSettings();
        RefreshTheme();
    }

    /// <summary>
    /// Gets or sets the visual density style of the grid.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(NexaGridStyle.Default)]
    [Description("Visual density style of the grid.")]
    public NexaGridStyle GridStyle
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

    /// <summary>
    /// Gets or sets the header visual style.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(NexaHeaderStyle.Standard)]
    [Description("Visual style of the column headers.")]
    public NexaHeaderStyle HeaderStyle
    {
        get => _headerStyle;
        set
        {
            if (_headerStyle == value) return;
            _headerStyle = value;
            ApplyHeaderStyle();
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether row numbers are displayed in the row header.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Show row numbers in the row header column.")]
    public bool ShowRowNumbers
    {
        get => _showRowNumbers;
        set
        {
            if (_showRowNumbers == value) return;
            _showRowNumbers = value;
            RowHeadersVisible = value;
            if (value)
            {
                RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing;
                RefreshRowNumbers();
            }
            else
            {
                foreach (DataGridViewRow row in Rows)
                {
                    if (!row.IsNewRow)
                        row.HeaderCell.Value = null;
                }
                RowHeadersWidth = 0;
            }
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether horizontal grid lines are shown.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Show horizontal grid lines between rows.")]
    public bool ShowHorizontalGridLines
    {
        get => _showHorizontalGridLines;
        set
        {
            if (_showHorizontalGridLines == value) return;
            _showHorizontalGridLines = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether vertical grid lines are shown.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Show vertical grid lines between columns.")]
    public bool ShowVerticalGridLines
    {
        get => _showVerticalGridLines;
        set
        {
            if (_showVerticalGridLines == value) return;
            _showVerticalGridLines = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether alternating row colors are used.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Use alternating background colors for rows.")]
    public bool AlternateRowColors
    {
        get => _alternateRowColors;
        set
        {
            if (_alternateRowColors == value) return;
            _alternateRowColors = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the corner radius for row rendering in DIPs.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Corner radius for row cells in DIPs. Use 0 for square corners.")]
    public int RowCornerRadius
    {
        get => _rowCornerRadius;
        set
        {
            _rowCornerRadius = Math.Max(0, value);
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets the column header height in DIPs.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(32)]
    [Description("Height of the column header row in DIPs.")]
    public int HeaderHeight
    {
        get => _headerHeight;
        set
        {
            _headerHeight = Math.Max(20, value);
            ApplyHeaderHeight();
        }
    }

    /// <summary>
    /// Gets or sets the default row height in DIPs.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(32)]
    [Description("Default height of data rows in DIPs.")]
    public int RowHeight
    {
        get => _rowHeight;
        set
        {
            _rowHeight = Math.Max(20, value);
            ApplyRowHeight();
        }
    }

    private void ApplyDefaultSettings()
    {
        AllowUserToAddRows = false;
        AllowUserToDeleteRows = false;
        AllowUserToResizeRows = true;
        AllowUserToResizeColumns = true;
        MultiSelect = true;
        SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        ReadOnly = true;
        BorderStyle = BorderStyle.None;
        CellBorderStyle = DataGridViewCellBorderStyle.None;
        RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
    }

    private void ApplyGridStyle()
    {
        var dpi = GetCurrentDpi();
        var padding = _gridStyle switch
        {
            NexaGridStyle.Compact => new Padding(0),
            NexaGridStyle.Comfortable => new Padding(NexaDpi.Scale(4, dpi)),
            _ => new Padding(NexaDpi.Scale(2, dpi))
        };
        CellBorderStyle = _gridStyle == NexaGridStyle.Default ? DataGridViewCellBorderStyle.SingleHorizontal : DataGridViewCellBorderStyle.None;
    }

    private void ApplyHeaderStyle()
    {
        EnableHeadersVisualStyles = false;
    }

    private void ApplyHeaderHeight()
    {
        var dpi = GetCurrentDpi();
        ColumnHeadersHeight = NexaDpi.Scale(_headerHeight, dpi);
    }

    private void ApplyRowHeight()
    {
        var dpi = GetCurrentDpi();
        var height = NexaDpi.Scale(_rowHeight, dpi);
        if (!AutoSizeRowsMode.Equals(DataGridViewAutoSizeRowsMode.None))
        {
            foreach (DataGridViewRow row in Rows)
            {
                if (!row.IsNewRow)
                    row.MinimumHeight = height;
            }
        }
        else
        {
            RowTemplate.MinimumHeight = height;
        }
    }

    private void RefreshTheme()
    {
        if (IsDisposed || Disposing) return;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;

        BackgroundColor = (Color)palette[NexaColorRole.Background].Value;
        DefaultCellStyle.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        DefaultCellStyle.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        DefaultCellStyle.SelectionBackColor = (Color)palette[NexaColorRole.Primary].Value;
        DefaultCellStyle.SelectionForeColor = (Color)palette[NexaColorRole.TextOnAccent].Value;

        AlternatingRowsDefaultCellStyle.BackColor = _alternateRowColors
            ? (Color)palette[NexaColorRole.SurfaceVariant].Value
            : (Color)palette[NexaColorRole.Surface].Value;
        AlternatingRowsDefaultCellStyle.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        AlternatingRowsDefaultCellStyle.SelectionBackColor = (Color)palette[NexaColorRole.Primary].Value;
        AlternatingRowsDefaultCellStyle.SelectionForeColor = (Color)palette[NexaColorRole.TextOnAccent].Value;

        ColumnHeadersDefaultCellStyle.BackColor = _headerStyle == NexaHeaderStyle.Minimal
            ? Color.Transparent
            : (Color)palette[NexaColorRole.Surface].Value;
        ColumnHeadersDefaultCellStyle.ForeColor = _headerStyle == NexaHeaderStyle.Emphasized
            ? (Color)palette[NexaColorRole.Primary].Value
            : (Color)palette[NexaColorRole.TextPrimary].Value;
        ColumnHeadersDefaultCellStyle.SelectionBackColor = ColumnHeadersDefaultCellStyle.BackColor;
        ColumnHeadersDefaultCellStyle.SelectionForeColor = ColumnHeadersDefaultCellStyle.ForeColor;
        ColumnHeadersDefaultCellStyle.Font = theme.Typography.ToFont(NexaTypographyRole.BodyStrong, GetCurrentDpi());

        RowHeadersDefaultCellStyle.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        RowHeadersDefaultCellStyle.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        RowHeadersDefaultCellStyle.SelectionBackColor = (Color)palette[NexaColorRole.Primary].Value;
        RowHeadersDefaultCellStyle.SelectionForeColor = (Color)palette[NexaColorRole.TextOnAccent].Value;

        GridColor = _showHorizontalGridLines || _showVerticalGridLines
            ? (Color)palette[NexaColorRole.Border].Value
            : Color.Empty;

        ApplyHeaderHeight();
        ApplyRowHeight();
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

    protected override void OnRowsAdded(DataGridViewRowsAddedEventArgs e)
    {
        base.OnRowsAdded(e);
        if (_showRowNumbers) RefreshRowNumbers();
    }

    protected override void OnRowsRemoved(DataGridViewRowsRemovedEventArgs e)
    {
        base.OnRowsRemoved(e);
        if (_showRowNumbers) RefreshRowNumbers();
    }

    protected override void OnSorted(EventArgs e)
    {
        base.OnSorted(e);
        if (_showRowNumbers) RefreshRowNumbers();
    }

    protected override void OnRowHeaderMouseClick(DataGridViewCellMouseEventArgs e)
    {
        base.OnRowHeaderMouseClick(e);
        if (_showRowNumbers) RefreshRowNumbers();
    }

    private void RefreshRowNumbers()
    {
        if (!_showRowNumbers || IsDisposed || Disposing || !IsHandleCreated) return;

        var dpi = GetCurrentDpi();
        var maxText = Rows.Count.ToString();
        var width = TextRenderer.MeasureText(maxText, RowHeadersDefaultCellStyle.Font).Width +
                    NexaDpi.Scale(16, dpi);
        RowHeadersWidth = width;

        foreach (DataGridViewRow row in Rows)
        {
            if (!row.IsNewRow)
            {
                row.HeaderCell.Value = (row.Index + 1).ToString();
            }
        }
    }

    protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
    {
        base.OnRowPostPaint(e);
        if (_showRowNumbers && e.RowIndex >= 0 && !Rows[e.RowIndex].IsNewRow)
        {
            var headerRect = RowHeadersVisible ? GetCellDisplayRectangle(-1, e.RowIndex, true) : Rectangle.Empty;
            if (!headerRect.IsEmpty)
            {
                var numberText = (e.RowIndex + 1).ToString();
                var font = RowHeadersDefaultCellStyle.Font;
                var textColor = RowHeadersDefaultCellStyle.ForeColor;
                var textFormat = TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter | TextFormatFlags.SingleLine;
                TextRenderer.DrawText(e.Graphics, numberText, font, headerRect, textColor, textFormat);
            }
        }
    }

    protected override void OnColumnHeadersHeightChanged(EventArgs e)
    {
        base.OnColumnHeadersHeightChanged(e);
        Invalidate();
    }

    protected override void OnRowHeightChanged(DataGridViewRowEventArgs e)
    {
        base.OnRowHeightChanged(e);
        if (_showRowNumbers) RefreshRowNumbers();
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
/// Visual density styles for the NexaDataGridView.
/// </summary>
public enum NexaGridStyle
{
    /// <summary>Standard density with normal spacing.</summary>
    Default,
    /// <summary>Compact density with minimal padding.</summary>
    Compact,
    /// <summary>Comfortable density with generous padding.</summary>
    Comfortable
}

/// <summary>
/// Header visual styles for the NexaDataGridView.
/// </summary>
public enum NexaHeaderStyle
{
    /// <summary>Standard header with surface background.</summary>
    Standard,
    /// <summary>Emphasized header with primary accent color text.</summary>
    Emphasized,
    /// <summary>Minimal header with transparent background.</summary>
    Minimal
}