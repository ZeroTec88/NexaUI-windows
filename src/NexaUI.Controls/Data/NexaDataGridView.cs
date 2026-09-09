using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A feature-rich, theme-aware DataGridView that preserves all native functionality
/// while adding modern NexaUI visual styling and productivity features:
/// filter row, search panel, column chooser, summary footer, CSV export,
/// context menu, column pinning, and multi-column sort.
/// </summary>
[DefaultEvent(nameof(DataGridView.CellContentClick))]
[DefaultProperty(nameof(DataSource))]
[ToolboxBitmap(typeof(DataGridView))]
public class NexaDataGridView : DataGridView
{
    #region Fields

    private NexaGridStyle _gridStyle = NexaGridStyle.Default;
    private NexaHeaderStyle _headerStyle = NexaHeaderStyle.Standard;
    private bool _showRowNumbers = false;
    private bool _showHorizontalGridLines = true;
    private bool _showVerticalGridLines = true;
    private bool _alternateRowColors = true;
    private int _rowCornerRadius = 0;
    private int _headerHeight = 32;
    private int _rowHeight = 32;

    private bool _showFilterRow = false;
    private bool _showSearchPanel = false;
    private bool _showSummaryFooter = false;
    private bool _showContextMenu = true;
    private bool _enableColumnPinning = true;
    private bool _enableMultiColumnSort = true;

    private Panel? _filterPanel;
    private Dictionary<DataGridViewColumn, TextBox>? _filterBoxes;
    private Panel? _searchPanel;
    private TextBox? _searchBox;
    private Panel? _summaryPanel;
    private ContextMenuStrip? _contextMenu;
    private DataGridViewSummaryItemCollection? _summaryItems;

    #endregion

    #region Constructors

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

    #endregion

    #region Public Properties

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
                RowHeadersWidth = 4;
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

    /// <summary>
    /// Gets or sets whether the filter row is visible.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Show a filter row above the data for quick text filtering.")]
    public bool ShowFilterRow
    {
        get => _showFilterRow;
        set
        {
            if (_showFilterRow == value) return;
            _showFilterRow = value;
            EnsureFilterPanel();
            _filterPanel.Visible = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether the search panel is visible.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Show a search panel above the grid for quick text search with highlighting.")]
    public bool ShowSearchPanel
    {
        get => _showSearchPanel;
        set
        {
            if (_showSearchPanel == value) return;
            _showSearchPanel = value;
            EnsureSearchPanel();
            _searchPanel.Visible = value;
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether the summary footer is visible.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Show a summary footer row with aggregate values.")]
    public bool ShowSummaryFooter
    {
        get => _showSummaryFooter;
        set
        {
            if (_showSummaryFooter == value) return;
            _showSummaryFooter = value;
            EnsureSummaryPanel();
            _summaryPanel.Visible = value;
            RefreshSummary();
            Invalidate();
        }
    }

    /// <summary>
    /// Gets or sets whether the context menu is enabled.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Show the built-in context menu on right-click.")]
    public bool ShowContextMenu
    {
        get => _showContextMenu;
        set
        {
            if (_showContextMenu == value) return;
            _showContextMenu = value;
            if (!value && _contextMenu != null)
                ContextMenuStrip = null;
            else if (value)
                EnsureContextMenu();
        }
    }

    /// <summary>
    /// Gets or sets whether column pinning is enabled in the context menu.
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Allow pinning/unpinning columns via the context menu.")]
    public bool EnableColumnPinning
    {
        get => _enableColumnPinning;
        set => _enableColumnPinning = value;
    }

    /// <summary>
    /// Gets or sets whether multi-column sort is enabled (Shift+Click on headers).
    /// </summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Allow multi-column sorting by Shift+Clicking column headers.")]
    public bool EnableMultiColumnSort
    {
        get => _enableMultiColumnSort;
        set => _enableMultiColumnSort = value;
    }

    /// <summary>
    /// Gets the collection of summary items for the footer.
    /// </summary>
    [Category("NexaUI")]
    [Description("Summary items displayed in the footer when ShowSummaryFooter is true.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public DataGridViewSummaryItemCollection SummaryItems
    {
        get
        {
            EnsureSummaryPanel();
            return _summaryItems;
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Exports the visible data to a CSV file.
    /// </summary>
    public void ExportToCsv(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path must not be empty.", nameof(filePath));

        var lines = new List<string>();

        if (Columns.Count == 0)
            return;

        var headerLine = string.Join(",", Columns.Cast<DataGridViewColumn>()
            .Where(c => c.Visible)
            .Select(c => $"\"{c.HeaderText.Replace("\"", "\"\"")}\""));
        lines.Add(headerLine);

        foreach (DataGridViewRow row in Rows)
        {
            if (row.IsNewRow) continue;
            var cells = row.Cells.Cast<DataGridViewCell>()
                .Where(c => c.ColumnIndex >= 0 && c.ColumnIndex < Columns.Count && Columns[c.ColumnIndex].Visible)
                .Select(c => $"\"{(c.Value?.ToString() ?? string.Empty).Replace("\"", "\"\"")}\"");
            lines.Add(string.Join(",", cells));
        }

        File.WriteAllLines(filePath, lines, System.Text.Encoding.UTF8);
    }

    /// <summary>
    /// Shows the column chooser popup.
    /// </summary>
    public void ShowColumnChooser()
    {
        using var chooser = new Form
        {
            Text = "Column Chooser",
            Size = new Size(240, 320),
            StartPosition = FormStartPosition.CenterParent,
            MinimizeBox = false,
            MaximizeBox = false,
            ShowIcon = false
        };

        var list = new CheckedListBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None
        };

        foreach (DataGridViewColumn col in Columns)
        {
            list.Items.Add(col.HeaderText, col.Visible);
        }

        list.ItemCheck += (_, e) =>
        {
            Columns[e.Index].Visible = e.NewValue == CheckState.Checked;
        };

        chooser.Controls.Add(list);
        chooser.ShowDialog(this);
    }

    /// <summary>
    /// Auto-generates columns from the public properties of a data type and binds the provided data.
    /// </summary>
    /// <typeparam name="T">The data model type.</typeparam>
    /// <param name="data">The collection of data to display.</param>
    /// <param name="columnHeaderPrefix">Optional prefix for auto-generated column header text.</param>
    public void AutoGenerateColumnsFromType<T>(IEnumerable<T> data, string? columnHeaderPrefix = null)
    {
        if (data == null) return;

        Columns.Clear();
        AutoGenerateColumns = false;

        var bindingList = new BindingList<T>(data.ToList());
        DataSource = bindingList;

        var props = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        foreach (var prop in props)
        {
            var col = new DataGridViewTextBoxColumn
            {
                DataPropertyName = prop.Name,
                HeaderText = string.IsNullOrWhiteSpace(columnHeaderPrefix) ? prop.Name : $"{columnHeaderPrefix} {prop.Name}",
                Name = prop.Name,
                SortMode = DataGridViewColumnSortMode.Automatic
            };
            Columns.Add(col);
        }
    }

    #endregion

    #region Overrides

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        RefreshTheme();
        EnsureFilterPanel();
        EnsureSearchPanel();
        EnsureSummaryPanel();
        EnsureContextMenu();
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        if (Parent != null)
        {
            EnsureFilterPanel();
            EnsureSearchPanel();
            EnsureSummaryPanel();
            EnsureContextMenu();
        }
    }

    protected override void OnColumnHeaderMouseClick(DataGridViewCellMouseEventArgs e)
    {
        var isMultiSort = _enableMultiColumnSort &&
                          e.Button == MouseButtons.Left &&
                          (ModifierKeys & Keys.Shift) == Keys.Shift &&
                          Columns[e.ColumnIndex].SortMode != DataGridViewColumnSortMode.NotSortable;

        if (!isMultiSort)
        {
            base.OnColumnHeaderMouseClick(e);
        }

        if (isMultiSort)
        {
            var col = Columns[e.ColumnIndex];
            if (col.HeaderCell.SortGlyphDirection == SortOrder.Ascending)
            {
                col.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                Sort(col, ListSortDirection.Descending);
            }
            else
            {
                col.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                Sort(col, ListSortDirection.Ascending);
            }
        }
    }

    protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
    {
        base.OnCellMouseDown(e);

        if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
        {
            if (CurrentCell != null)
            {
                CurrentCell = Rows[e.RowIndex].Cells[e.ColumnIndex];
            }
        }
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

    protected override void OnColumnDisplayIndexChanged(DataGridViewColumnEventArgs e)
    {
        base.OnColumnDisplayIndexChanged(e);
        if (_showFilterRow)
        {
            RebuildFilterPanel();
        }
    }

    protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
    {
        base.OnDataBindingComplete(e);
        if (_showRowNumbers) RefreshRowNumbers();
        if (_showSummaryFooter) RefreshSummary();
    }

    protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
    {
        base.OnRowPostPaint(e);
        if (_showRowNumbers && e.RowIndex >= 0 && Rows.Count > e.RowIndex && !Rows[e.RowIndex].IsNewRow)
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

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);

        if (_showSearchPanel && _searchBox != null && !string.IsNullOrEmpty(_searchBox.Text))
        {
            HighlightSearchResults(e.Graphics, _searchBox.Text);
        }
    }

    #endregion

    #region Private Methods

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
        CellBorderStyle = _gridStyle == NexaGridStyle.Compact ? DataGridViewCellBorderStyle.None :
                          _gridStyle == NexaGridStyle.Comfortable ? DataGridViewCellBorderStyle.SingleHorizontal :
                          DataGridViewCellBorderStyle.SingleHorizontal;
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
        ApplyFilterTheme();
        ApplySearchTheme();
        ApplySummaryTheme();
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

    #region Row Numbers

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

    #endregion

    #region Filter Row

    private void EnsureFilterPanel()
    {
        if (_filterPanel != null) return;

        _filterPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = NexaDpi.Scale(36, GetCurrentDpi()),
            Padding = new Padding(4),
            Margin = new Padding(0)
        };

        _filterBoxes = new Dictionary<DataGridViewColumn, TextBox>();
        RebuildFilterPanel();
        Parent?.Controls.Add(_filterPanel);
        if (_filterPanel.Parent != null)
            _filterPanel.BringToFront();
        _filterPanel.Visible = _showFilterRow;
    }

    private void RebuildFilterPanel()
    {
        if (_filterPanel == null) return;

        _filterPanel.Controls.Clear();
        _filterBoxes?.Clear();

        if (!_showFilterRow || Columns.Count == 0) return;

        var flow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        foreach (DataGridViewColumn col in Columns)
        {
            if (!col.Visible) continue;

            var itemPanel = new Panel
            {
                Width = NexaDpi.Scale(140, GetCurrentDpi()),
                Height = NexaDpi.Scale(28, GetCurrentDpi()),
                Margin = new Padding(2),
                Padding = new Padding(0)
            };

            var label = new Label
            {
                Text = col.HeaderText,
                Dock = DockStyle.Top,
                Height = NexaDpi.Scale(14, GetCurrentDpi()),
                Margin = new Padding(0),
                Padding = new Padding(2, 0, 0, 0)
            };

            var textBox = new TextBox
            {
                Dock = DockStyle.Bottom,
                Height = NexaDpi.Scale(20, GetCurrentDpi()),
                Margin = new Padding(0),
                Padding = new Padding(2, 0, 0, 0)
            };
            textBox.TextChanged += (_, __) => ApplyFilter(col, textBox.Text);

            itemPanel.Controls.Add(textBox);
            itemPanel.Controls.Add(label);
            flow.Controls.Add(itemPanel);

            if (_filterBoxes != null)
                _filterBoxes[col] = textBox;
        }

        _filterPanel.Controls.Add(flow);
        ApplyFilterTheme();
    }

    private void ApplyFilterTheme()
    {
        if (_filterPanel == null || IsDisposed || Disposing) return;
        var palette = ThemeManager.Current.Palette;
        _filterPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;

        foreach (Control ctrl in _filterPanel.Controls)
        {
            if (ctrl is FlowLayoutPanel flow)
            {
                foreach (Control item in flow.Controls)
                {
                    if (item is Panel p)
                    {
                        foreach (Control inner in p.Controls)
                        {
                            if (inner is TextBox tb)
                            {
                                tb.BackColor = (Color)palette[NexaColorRole.InputBackground].Value;
                                tb.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
                            }
                            else if (inner is Label lbl)
                            {
                                lbl.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
                            }
                        }
                    }
                }
            }
        }
    }

    private void ApplyFilter(DataGridViewColumn column, string filterText)
    {
        if (string.IsNullOrWhiteSpace(filterText))
        {
            foreach (DataGridViewRow row in Rows)
            {
                if (!row.IsNewRow)
                    row.Visible = true;
            }
            return;
        }

        var term = filterText.Trim().ToLowerInvariant();
        foreach (DataGridViewRow row in Rows)
        {
            if (row.IsNewRow) continue;
            var cellValue = row.Cells[column.Index].Value?.ToString() ?? string.Empty;
            row.Visible = cellValue.ToLowerInvariant().Contains(term);
        }
    }

    #endregion

    #region Search Panel

    private void EnsureSearchPanel()
    {
        if (_searchPanel != null) return;

        _searchPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = NexaDpi.Scale(36, GetCurrentDpi()),
            Padding = new Padding(8, 4, 8, 4),
            Margin = new Padding(0)
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _searchBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };
        _searchBox.TextChanged += (_, __) => Invalidate();

        var clearBtn = new Label
        {
            Text = "✕",
            Dock = DockStyle.Right,
            AutoSize = true,
            Margin = new Padding(4, 0, 0, 0),
            Cursor = Cursors.Hand,
            TextAlign = ContentAlignment.MiddleCenter
        };
        clearBtn.Click += (_, __) =>
        {
            _searchBox.Text = string.Empty;
            Invalidate();
        };

        layout.Controls.Add(_searchBox, 0, 0);
        layout.Controls.Add(clearBtn, 1, 0);
        _searchPanel.Controls.Add(layout);

        Parent?.Controls.Add(_searchPanel);
        if (_searchPanel.Parent != null)
            _searchPanel.BringToFront();
        _searchPanel.Visible = _showSearchPanel;
        ApplySearchTheme();
    }

    private void ApplySearchTheme()
    {
        if (_searchPanel == null || IsDisposed || Disposing) return;
        var palette = ThemeManager.Current.Palette;
        _searchPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;

        if (_searchBox != null)
        {
            _searchBox.BackColor = (Color)palette[NexaColorRole.InputBackground].Value;
            _searchBox.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
    }

    private void HighlightSearchResults(Graphics g, string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText) || Rows.Count == 0) return;

        var term = searchText.Trim().ToLowerInvariant();
        if (term.Length == 0) return;

        foreach (DataGridViewRow row in Rows)
        {
            if (row.IsNewRow || !row.Visible) continue;
            foreach (DataGridViewCell cell in row.Cells)
            {
                if (!Columns[cell.ColumnIndex].Visible) continue;
                var value = cell.Value?.ToString() ?? string.Empty;
                if (value.ToLowerInvariant().Contains(term))
                {
                    var bounds = GetCellDisplayRectangle(cell.ColumnIndex, cell.RowIndex, true);
                    if (!bounds.IsEmpty)
                    {
                        using var brush = new SolidBrush(Color.FromArgb(80, ThemeManager.Current.Palette[NexaColorRole.Primary].Value));
                        g.FillRectangle(brush, bounds);
                    }
                }
            }
        }
    }

    #endregion

    #region Summary Footer

    private void EnsureSummaryPanel()
    {
        if (_summaryPanel != null) return;

        _summaryPanel = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = NexaDpi.Scale(32, GetCurrentDpi()),
            Padding = new Padding(8, 4, 8, 4),
            Margin = new Padding(0)
        };

        _summaryItems = new DataGridViewSummaryItemCollection(this);
        RefreshSummary();

        Parent?.Controls.Add(_summaryPanel);
        if (_summaryPanel.Parent != null)
            _summaryPanel.BringToFront();
        _summaryPanel.Visible = _showSummaryFooter;
    }

    private void RefreshSummary()
    {
        if (_summaryPanel == null || _summaryItems == null) return;

        _summaryPanel.Controls.Clear();

        if (!_showSummaryFooter || Columns.Count == 0) return;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = Columns.Count,
            RowCount = 1,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        foreach (DataGridViewColumn col in Columns)
        {
            if (!col.Visible) continue;
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / Columns.Count));
        }

        int colIndex = 0;
        foreach (DataGridViewColumn col in Columns)
        {
            if (!col.Visible) continue;

            var item = _summaryItems.GetSummaryForColumn(col.Name);
            var text = item != null ? item.GetDisplayText() : string.Empty;
            if (string.IsNullOrEmpty(text) && item != null && item.Aggregate == DataGridViewSummaryAggregate.Count)
            {
                text = $"Count: {Rows.Count}";
            }

            var label = new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(0)
            };
            layout.Controls.Add(label, colIndex, 0);
            colIndex++;
        }

        _summaryPanel.Controls.Add(layout);
        ApplySummaryTheme();
    }

    private void ApplySummaryTheme()
    {
        if (_summaryPanel == null || IsDisposed || Disposing) return;
        var palette = ThemeManager.Current.Palette;
        _summaryPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;

        foreach (Control ctrl in _summaryPanel.Controls)
        {
            if (ctrl is TableLayoutPanel layout)
            {
                foreach (Control child in layout.Controls)
                {
                    if (child is Label lbl)
                    {
                        lbl.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
                        lbl.Font = ThemeManager.Current.Typography.ToFont(NexaTypographyRole.Caption, GetCurrentDpi());
                    }
                }
            }
        }
    }

    #endregion

    #region Context Menu

    private void EnsureContextMenu()
    {
        if (_contextMenu != null) return;

        _contextMenu = new ContextMenuStrip();

        var copyItem = new ToolStripMenuItem("Copy Selection");
        copyItem.Click += (_, __) => CopySelectionToClipboard();
        _contextMenu.Items.Add(copyItem);

        _contextMenu.Items.Add(new ToolStripSeparator());

        var exportItem = new ToolStripMenuItem("Export to CSV...");
        exportItem.Click += (_, __) => ExportToCsvPrompt();
        _contextMenu.Items.Add(exportItem);

        _contextMenu.Items.Add(new ToolStripSeparator());

        var chooserItem = new ToolStripMenuItem("Column Chooser");
        chooserItem.Click += (_, __) => ShowColumnChooser();
        _contextMenu.Items.Add(chooserItem);

        if (_enableColumnPinning)
        {
            var pinItem = new ToolStripMenuItem("Pin Column");
            pinItem.Click += (_, __) => PinCurrentColumn();
            _contextMenu.Items.Add(pinItem);

            var unpinItem = new ToolStripMenuItem("Unpin Column");
            unpinItem.Click += (_, __) => UnpinCurrentColumn();
            _contextMenu.Items.Add(unpinItem);
        }

        _contextMenu.Items.Add(new ToolStripSeparator());

        var filterItem = new ToolStripMenuItem("Show Filter Row");
        filterItem.Checked = _showFilterRow;
        filterItem.Click += (_, __) => ShowFilterRow = !ShowFilterRow;
        _contextMenu.Items.Add(filterItem);

        var summaryItem = new ToolStripMenuItem("Show Summary Footer");
        summaryItem.Checked = _showSummaryFooter;
        summaryItem.Click += (_, __) => ShowSummaryFooter = !ShowSummaryFooter;
        _contextMenu.Items.Add(summaryItem);

        var searchItem = new ToolStripMenuItem("Show Search Panel");
        searchItem.Checked = _showSearchPanel;
        searchItem.Click += (_, __) => ShowSearchPanel = !ShowSearchPanel;
        _contextMenu.Items.Add(searchItem);

        ContextMenuStrip = _contextMenu;
    }

    private void CopySelectionToClipboard()
    {
        if (SelectedCells.Count == 0) return;

        var text = new System.Text.StringBuilder();
        foreach (DataGridViewCell cell in SelectedCells)
        {
            if (cell.RowIndex >= 0 && cell.ColumnIndex >= 0 && cell.ColumnIndex < Columns.Count && Columns[cell.ColumnIndex].Visible)
            {
                text.Append(cell.Value?.ToString() ?? string.Empty);
                text.Append("\t");
            }
        }
        Clipboard.SetText(text.ToString());
    }

    private void ExportToCsvPrompt()
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*",
            FileName = "export.csv"
        };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            ExportToCsv(sfd.FileName);
        }
    }

    private void PinCurrentColumn()
    {
        if (CurrentCell == null || CurrentCell.ColumnIndex < 0) return;
        Columns[CurrentCell.ColumnIndex].Frozen = true;
    }

    private void UnpinCurrentColumn()
    {
        if (CurrentCell == null || CurrentCell.ColumnIndex < 0) return;
        Columns[CurrentCell.ColumnIndex].Frozen = false;
    }

    #endregion

    #endregion

    #region Helper Classes

    /// <summary>
    /// Represents a summary item for a column in the footer.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class DataGridViewSummaryItem
    {
        private readonly NexaDataGridView _grid;
        private string _columnName = string.Empty;
        private DataGridViewSummaryAggregate _aggregate = DataGridViewSummaryAggregate.Count;
        private string _format = "{0}";
        private string _displayText = string.Empty;

        internal DataGridViewSummaryItem(NexaDataGridView grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Gets or sets the name of the column this summary applies to.
        /// </summary>
        [Category("Summary")]
        public string ColumnName
        {
            get => _columnName;
            set => _columnName = value;
        }

        /// <summary>
        /// Gets or sets the aggregate function to apply.
        /// </summary>
        [Category("Summary")]
        public DataGridViewSummaryAggregate Aggregate
        {
            get => _aggregate;
            set => _aggregate = value;
        }

        /// <summary>
        /// Gets or sets the display format string.
        /// </summary>
        [Category("Summary")]
        public string Format
        {
            get => _format;
            set => _format = value;
        }

        internal string GetDisplayText()
        {
            if (string.IsNullOrEmpty(_columnName) || _grid.Columns.Contains(_columnName) == false)
                return string.Empty;

            var col = _grid.Columns[_columnName];
            var values = new List<object>();
            foreach (DataGridViewRow row in _grid.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells[col.Index].Value != null)
                    values.Add(row.Cells[col.Index].Value);
            }

            object result;
            if (values.Count == 0)
            {
                result = _aggregate switch
                {
                    DataGridViewSummaryAggregate.Sum => 0m,
                    DataGridViewSummaryAggregate.Avg => 0m,
                    DataGridViewSummaryAggregate.Min => 0m,
                    DataGridViewSummaryAggregate.Max => 0m,
                    _ => 0
                };
            }
            else
            {
                var numericValues = new List<decimal>();
                foreach (var v in values)
                {
                    if (v == null) continue;
                    var s = v.ToString();
                    if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out var d))
                        numericValues.Add(d);
                }

                if (numericValues.Count == 0)
                {
                    result = _aggregate switch
                    {
                        DataGridViewSummaryAggregate.Sum => 0m,
                        DataGridViewSummaryAggregate.Avg => 0m,
                        DataGridViewSummaryAggregate.Min => 0m,
                        DataGridViewSummaryAggregate.Max => 0m,
                        _ => 0
                    };
                }
                else
                {
                    result = _aggregate switch
                    {
                        DataGridViewSummaryAggregate.Sum => numericValues.Sum(),
                        DataGridViewSummaryAggregate.Avg => numericValues.Average(),
                        DataGridViewSummaryAggregate.Min => numericValues.Min(),
                        DataGridViewSummaryAggregate.Max => numericValues.Max(),
                        _ => numericValues.Count
                    };
                }
            }

            try
            {
                return string.Format(CultureInfo.CurrentCulture, _format, result);
            }
            catch
            {
                return result.ToString() ?? string.Empty;
            }
        }

        public override string ToString() => $"{_aggregate}({_columnName})";
    }

    /// <summary>
    /// Collection of summary items for the footer.
    /// </summary>
    public class DataGridViewSummaryItemCollection
    {
        private readonly NexaDataGridView _grid;
        private readonly List<DataGridViewSummaryItem> _items = new();

        internal DataGridViewSummaryItemCollection(NexaDataGridView grid)
        {
            _grid = grid;
        }

        /// <summary>
        /// Gets the number of items in the collection.
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// Adds a summary item.
        /// </summary>
        public DataGridViewSummaryItem Add(string columnName, DataGridViewSummaryAggregate aggregate, string format = "{0}")
        {
            var item = new DataGridViewSummaryItem(_grid)
            {
                ColumnName = columnName,
                Aggregate = aggregate,
                Format = format
            };
            _items.Add(item);
            _grid.RefreshSummary();
            return item;
        }

        /// <summary>
        /// Removes all summary items.
        /// </summary>
        public void Clear()
        {
            _items.Clear();
            _grid.RefreshSummary();
        }

        internal DataGridViewSummaryItem? GetSummaryForColumn(string columnName)
        {
            return _items.FirstOrDefault(i => i.ColumnName == columnName);
        }
    }

    #endregion
}

/// <summary>
/// Aggregate functions for summary items.
/// </summary>
public enum DataGridViewSummaryAggregate
{
    /// <summary>Count of rows.</summary>
    Count,
    /// <summary>Sum of values.</summary>
    Sum,
    /// <summary>Average of values.</summary>
    Avg,
    /// <summary>Minimum value.</summary>
    Min,
    /// <summary>Maximum value.</summary>
    Max
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
