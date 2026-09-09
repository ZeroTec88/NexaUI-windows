using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GalleryDataPresentationScreen : UserControl
{
    private readonly TableLayoutPanel _root;

    private NexaDataGridView _grid = null!;
    private NexaListView _listView = null!;
    private NexaPropertyGrid _propertyGrid = null!;
    private NexaTreeView _treeView = null!;

    private Label _selectedLabel = null!;
    private Label _gridStyleLabel = null!;

    public GalleryDataPresentationScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;

        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Padding = new Padding(32, 24, 32, 24)
        };
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _root.Controls.Add(HeaderLabel("Data Presentation Controls"));
        _root.Controls.Add(BodyLabel("NexaDataGridView, NexaListView, NexaPropertyGrid, and NexaTreeView with full native behavior and modern NexaUI styling."));

        _root.Controls.Add(SectionTitle("NexaDataGridView — student management"));
        _root.Controls.Add(BuildDataGridCard());

        _root.Controls.Add(SectionTitle("NexaListView — details view"));
        _root.Controls.Add(BuildListViewCard());

        _root.Controls.Add(SectionTitle("NexaPropertyGrid — settings"));
        _root.Controls.Add(BuildPropertyGridCard());

        _root.Controls.Add(SectionTitle("NexaTreeView — application navigation"));
        _root.Controls.Add(BuildTreeViewCard());

        _root.Controls.Add(SectionTitle("Student Management Grid — advanced"));
        _root.Controls.Add(BuildAdvancedGridCard());

        Controls.Add(_root);

        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);
        Disposed += OnSelfDisposed;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnSelfDisposed(object? sender, EventArgs e) => ThemeManager.ThemeChanged -= OnSelfDisposed;

    private static Label HeaderLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 0, 0, 4)
    };

    private static Label BodyLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 0, 0, 16)
    };

    private static Label SectionTitle(string title) => new()
    {
        Text = title,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 16, 0, 8)
    };

    private Control BuildDataGridCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Native DataGridView with row numbers, alternating rows, sorting, and theme-aware selection."));

        _grid = new NexaDataGridView
        {
            Dock = DockStyle.Top,
            Height = 320,
            ReadOnly = true,
            ShowRowNumbers = true,
            AlternateRowColors = true,
            HeaderHeight = 36,
            RowHeight = 32,
            GridStyle = NexaGridStyle.Default,
            HeaderStyle = NexaHeaderStyle.Standard,
            RowCornerRadius = 0,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false
        };

        _grid.Columns.Add("Id", "Student ID");
        _grid.Columns.Add("Name", "Student Name");
        _grid.Columns.Add("Course", "Course");
        _grid.Columns.Add("Batch", "Batch");
        _grid.Columns.Add("Gender", "Gender");
        _grid.Columns.Add("Telephone", "Telephone");
        _grid.Columns.Add("Status", "Status");
        _grid.Columns.Add("Attendance", "Attendance");

        var students = GenerateStudents();
        foreach (var s in students)
        {
            _grid.Rows.Add(s.Id, s.Name, s.Course, s.Batch, s.Gender, s.Telephone, s.Status, s.Attendance);
        }

        _grid.SelectionChanged += (_, _) =>
        {
            if (_grid.SelectedRows.Count > 0)
            {
                var row = _grid.SelectedRows[0];
                _selectedLabel.Text = $"Selected: {row.Cells[1].Value}";
            }
        };

        var controls = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 8)
        };

        var styleCombo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 140 };
        styleCombo.Items.AddRange(new object[] { "Default", "Compact", "Comfortable" });
        styleCombo.SelectedIndex = 0;
        styleCombo.SelectedIndexChanged += (_, _) =>
        {
            _grid.GridStyle = styleCombo.SelectedIndex switch
            {
                0 => NexaGridStyle.Default,
                1 => NexaGridStyle.Compact,
                2 => NexaGridStyle.Comfortable,
                _ => NexaGridStyle.Default
            };
            _gridStyleLabel.Text = $"GridStyle: {_grid.GridStyle}";
        };

        var toggleRowNumbers = new NexaButton { Text = "Toggle Row Numbers", Style = NexaButtonStyle.Secondary, AutoSize = true };
        toggleRowNumbers.Click += (_, _) => _grid.ShowRowNumbers = !_grid.ShowRowNumbers;

        _selectedLabel = new Label { Text = "Selected: None", AutoSize = true, Margin = new Padding(12, 0, 0, 0) };
        _gridStyleLabel = new Label { Text = "GridStyle: Default", AutoSize = true, Margin = new Padding(12, 0, 0, 0) };

        controls.Controls.Add(new Label { Text = "Style:", AutoSize = true, Margin = new Padding(0, 4, 4, 0) });
        controls.Controls.Add(styleCombo);
        controls.Controls.Add(toggleRowNumbers);
        controls.Controls.Add(_selectedLabel);
        controls.Controls.Add(_gridStyleLabel);

        card.Controls.Add(_grid);
        card.Controls.Add(controls);
        return card;
    }

    private Control BuildListViewCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Native ListView in Details view with themed column headers and row selection."));

        _listView = new NexaListView
        {
            Dock = DockStyle.Top,
            Height = 220,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true,
            ListStyle = NexaListStyle.Default
        };

        _listView.Columns.Add("Name", 220);
        _listView.Columns.Add("Type", 120);
        _listView.Columns.Add("Status", 100);

        _listView.Items.Add(new ListViewItem(new[] { "Student List", "Data", "Ready" }));
        _listView.Items.Add(new ListViewItem(new[] { "Course List", "Data", "Ready" }));
        _listView.Items.Add(new ListViewItem(new[] { "Reports", "Document", "Ready" }));
        _listView.Items.Add(new ListViewItem(new[] { "Settings", "System", "Ready" }));
        _listView.Items.Add(new ListViewItem(new[] { "Attendance", "Data", "Processing" }));

        card.Controls.Add(_listView);
        return card;
    }

    private Control BuildPropertyGridCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Native PropertyGrid with NexaUI category, help, and command area colors."));

        _propertyGrid = new NexaPropertyGrid
        {
            Dock = DockStyle.Top,
            Height = 260,
            GridStyle = NexaPropertyGridStyle.Default,
            HelpVisible = true,
            CommandsVisibleIfAvailable = true,
            ToolbarVisible = true,
            PropertySort = PropertySort.Categorized
        };

        _propertyGrid.SelectedObject = new DemoAppSettings();

        card.Controls.Add(_propertyGrid);
        return card;
    }

    private Control BuildTreeViewCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Native TreeView with themed nodes, lines, expand/collapse, and checkboxes."));

        _treeView = new NexaTreeView
        {
            Dock = DockStyle.Top,
            Height = 240,
            TreeStyle = NexaTreeStyle.Default,
            ShowRootLines = true,
            ShowNodeLines = true,
            CheckBoxes = true,
            ShowLines = true,
            ShowPlusMinus = true,
            FullRowSelect = true,
            HotTracking = true
        };

        var root = _treeView.Nodes.Add("NexaUI Application");
        var dashboard = root.Nodes.Add("Dashboard");
        var students = root.Nodes.Add("Students");
        students.Nodes.Add("All Students");
        students.Nodes.Add("Active Students");
        students.Nodes.Add("Graduated Students");
        var courses = root.Nodes.Add("Courses");
        courses.Nodes.Add("ICT");
        courses.Nodes.Add("English");
        courses.Nodes.Add("Business");
        var assessments = root.Nodes.Add("Assessments");
        assessments.Nodes.Add("Continuous Assessment");
        assessments.Nodes.Add("Final Assessment");
        var reports = root.Nodes.Add("Reports");
        reports.Nodes.Add("Student Reports");
        reports.Nodes.Add("Attendance Reports");
        reports.Nodes.Add("Course Reports");
        var settings = root.Nodes.Add("Settings");

        root.Expand();
        card.Controls.Add(_treeView);
        return card;
    }

    private Control BuildAdvancedGridCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Student Management Grid — search, selection, and status feedback."));

        var advancedGrid = new NexaDataGridView
        {
            Dock = DockStyle.Top,
            Height = 280,
            ReadOnly = true,
            ShowRowNumbers = true,
            AlternateRowColors = true,
            HeaderHeight = 36,
            RowHeight = 32,
            GridStyle = NexaGridStyle.Default,
            HeaderStyle = NexaHeaderStyle.Standard,
            RowCornerRadius = 0,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false
        };

        advancedGrid.Columns.Add("Id", "Student ID");
        advancedGrid.Columns.Add("Name", "Student Name");
        advancedGrid.Columns.Add("Course", "Course");
        advancedGrid.Columns.Add("Batch", "Batch");
        advancedGrid.Columns.Add("Status", "Status");
        advancedGrid.Columns.Add("Attendance", "Attendance");

        var students = GenerateStudents();
        foreach (var s in students)
        {
            advancedGrid.Rows.Add(s.Id, s.Name, s.Course, s.Batch, s.Status, s.Attendance);
        }

        var topBar = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            Margin = new Padding(0, 0, 0, 8)
        };
        topBar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        topBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        topBar.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var searchBox = new TextBox { Width = 220, Margin = new Padding(0, 0, 8, 0) };
        searchBox.TextChanged += (_, _) =>
        {
            var term = searchBox.Text.Trim().ToLowerInvariant();
            foreach (DataGridViewRow row in advancedGrid.Rows)
            {
                if (row.IsNewRow) continue;
                var match = string.IsNullOrEmpty(term) ||
                            (row.Cells[1].Value?.ToString() ?? string.Empty).ToLowerInvariant().Contains(term) ||
                            (row.Cells[2].Value?.ToString() ?? string.Empty).ToLowerInvariant().Contains(term);
                row.Visible = match;
            }
        };

        var addButton = new NexaButton { Text = "Add Student", Style = NexaButtonStyle.Primary, AutoSize = true };
        addButton.Click += (_, _) =>
        {
            advancedGrid.Rows.Add(students.Count + 1, "New Student", "ICT", "2025-A", "Active", "0%");
        };

        topBar.Controls.Add(searchBox, 0, 0);
        topBar.Controls.Add(addButton, 1, 0);

        card.Controls.Add(topBar);
        card.Controls.Add(advancedGrid);

        var statusRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 0)
        };
        statusRow.Controls.Add(new Label { Text = "Status: Ready", AutoSize = true });
        statusRow.Controls.Add(new NexaBadge { Text = "Live", BadgeStyle = NexaBadgeStyle.Success, BadgeSize = NexaBadgeSize.Small });
        card.Controls.Add(statusRow);

        return card;
    }

    private static List<Student> GenerateStudents()
    {
        return new List<Student>
        {
            new Student { Id = "STU-001", Name = "Kasun Perera", Course = "ICT", Batch = "2024-A", Gender = "Male", Telephone = "077-1234567", Status = "Active", Attendance = "92%" },
            new Student { Id = "STU-002", Name = "Nimal Silva", Course = "ICT", Batch = "2024-A", Gender = "Male", Telephone = "077-7654321", Status = "Active", Attendance = "88%" },
            new Student { Id = "STU-003", Name = "Amal Fernando", Course = "English", Batch = "2024-B", Gender = "Male", Telephone = "071-2345678", Status = "Completed", Attendance = "95%" },
            new Student { Id = "STU-004", Name = "Saman Kumara", Course = "ICT", Batch = "2024-A", Gender = "Male", Telephone = "072-3456789", Status = "Active", Attendance = "78%" },
            new Student { Id = "STU-005", Name = "Malini Fernando", Course = "Business", Batch = "2024-B", Gender = "Female", Telephone = "073-4567890", Status = "Active", Attendance = "91%" },
            new Student { Id = "STU-006", Name = "Dilshani Perera", Course = "English", Batch = "2024-C", Gender = "Female", Telephone = "074-5678901", Status = "Completed", Attendance = "97%" },
            new Student { Id = "STU-007", Name = "Ruwan Jayasinghe", Course = "ICT", Batch = "2024-A", Gender = "Male", Telephone = "075-6789012", Status = "Active", Attendance = "85%" },
            new Student { Id = "STU-008", Name = "Thilini Wickrama", Course = "Business", Batch = "2024-B", Gender = "Female", Telephone = "076-7890123", Status = "Active", Attendance = "89%" },
            new Student { Id = "STU-009", Name = "Chamara Dias", Course = "ICT", Batch = "2024-C", Gender = "Male", Telephone = "077-8901234", Status = "On Leave", Attendance = "45%" },
            new Student { Id = "STU-010", Name = "Nadeeka Liyanage", Course = "English", Batch = "2024-A", Gender = "Female", Telephone = "078-9012345", Status = "Active", Attendance = "93%" },
            new Student { Id = "STU-011", Name = "Buddhika Herath", Course = "Business", Batch = "2024-C", Gender = "Male", Telephone = "079-0123456", Status = "Active", Attendance = "81%" },
            new Student { Id = "STU-012", Name = "Shalika Ekanayake", Course = "ICT", Batch = "2024-B", Gender = "Female", Telephone = "070-1234567", Status = "Completed", Attendance = "96%" },
            new Student { Id = "STU-013", Name = "Mahesh Randeniya", Course = "English", Batch = "2024-B", Gender = "Male", Telephone = "071-2345679", Status = "Active", Attendance = "74%" },
            new Student { Id = "STU-014", Name = "Kusum Seneviratne", Course = "Business", Batch = "2024-A", Gender = "Female", Telephone = "072-3456780", Status = "Active", Attendance = "90%" },
            new Student { Id = "STU-015", Name = "Ravindra Bandara", Course = "ICT", Batch = "2024-C", Gender = "Male", Telephone = "073-4567891", Status = "Active", Attendance = "87%" }
        };
    }

    private void ApplyTheme(ITheme theme)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyTheme(theme));
            return;
        }

        var palette = theme.Palette;
        BackColor = (Color)palette[NexaColorRole.Background].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        foreach (Control c in _root.Controls)
        {
            if (c is Label l)
            {
                if (l.Text == "Data Presentation Controls")
                {
                    l.Font = theme.Typography.ToFont(NexaTypographyRole.Heading, NexaFormsDpi.CurrentDpi(this));
                    l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
                }
                else if (l.Text.StartsWith("NexaDataGridView", StringComparison.Ordinal) ||
                         l.Text.StartsWith("NexaListView", StringComparison.Ordinal) ||
                         l.Text.StartsWith("NexaPropertyGrid", StringComparison.Ordinal) ||
                         l.Text.StartsWith("NexaTreeView", StringComparison.Ordinal) ||
                         l.Text.StartsWith("Student Management", StringComparison.Ordinal))
                {
                    l.Font = theme.Typography.ToFont(NexaTypographyRole.Title, NexaFormsDpi.CurrentDpi(this));
                    l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
                }
                else if (l.Text.StartsWith("Native ", StringComparison.Ordinal) || l.Text.StartsWith("Theme-aware", StringComparison.Ordinal))
                {
                    l.Font = theme.Typography.ToFont(NexaTypographyRole.Body, NexaFormsDpi.CurrentDpi(this));
                    l.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
                }
            }
        }

        _grid?.Invalidate();
        _listView?.Invalidate();
        _propertyGrid?.Invalidate();
        _treeView?.Invalidate();
    }

    private Panel CreateDemoCard()
    {
        var palette = ThemeManager.Current.Palette;
        var card = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 12),
            BackColor = (Color)palette[NexaColorRole.Surface].Value
        };
        card.Paint += (s, e) =>
        {
            using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            var r = card.ClientRectangle;
            r.Width -= 1;
            r.Height -= 1;
            e.Graphics.DrawRectangle(pen, r);
        };
        return card;
    }

    private static NexaLabel DescriptionLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Muted,
        Margin = new Padding(0, 0, 0, 8)
    };

    private sealed class Student
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Course { get; set; } = string.Empty;
        public string Batch { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Attendance { get; set; } = string.Empty;
    }

    private sealed class DemoAppSettings
    {
        [Category("Appearance")]
        [DisplayName("Theme")]
        public string Theme { get; set; } = "Light";

        [Category("Appearance")]
        [DisplayName("Accent Color")]
        public string AccentColor { get; set; } = "Blue";

        [Category("Appearance")]
        [DisplayName("Font Size")]
        public int FontSize { get; set; } = 12;

        [Category("Application")]
        [DisplayName("Application Name")]
        public string AppName { get; set; } = "NexaUI Demo";

        [Category("Application")]
        [DisplayName("Auto Save")]
        public bool AutoSave { get; set; } = true;

        [Category("Application")]
        [DisplayName("Start Minimized")]
        public bool StartMinimized { get; set; } = false;

        [Category("Notifications")]
        [DisplayName("Enable Notifications")]
        public bool EnableNotifications { get; set; } = true;

        [Category("Notifications")]
        [DisplayName("Notification Duration (ms)")]
        public int NotificationDuration { get; set; } = 3000;

        [Category("Notifications")]
        [DisplayName("Sound Enabled")]
        public bool SoundEnabled { get; set; } = true;
    }
}
