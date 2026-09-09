Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Themes

Public NotInheritable Class GalleryDataPresentationScreen
    Inherits UserControl

    Private _root As TableLayoutPanel

    Private _grid As NexaDataGridView = Nothing
    Private _listView As NexaListView = Nothing
    Private _propertyGrid As NexaPropertyGrid = Nothing
    Private _treeView As NexaTreeView = Nothing

    Private _selectedLabel As Label = Nothing
    Private _gridStyleLabel As Label = Nothing

    Public Sub New()
        Dock = DockStyle.Fill
        AutoScroll = True

        _root = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 1,
            .Padding = New Padding(32, 24, 32, 24)
        }
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        _root.Controls.Add(HeaderLabel("Data Presentation Controls"))
        _root.Controls.Add(BodyLabel("NexaDataGridView, NexaListView, NexaPropertyGrid, and NexaTreeView with full native behavior and modern NexaUI styling."))

        _root.Controls.Add(SectionTitle("NexaDataGridView — student management"))
        _root.Controls.Add(BuildDataGridCard())

        _root.Controls.Add(SectionTitle("Auto-Generated Columns from API Data"))
        _root.Controls.Add(BuildAutoGridCard())

        _root.Controls.Add(SectionTitle("Manual Column Configuration"))
        _root.Controls.Add(BuildManualGridCard())

        _root.Controls.Add(SectionTitle("NexaListView — details view"))
        _root.Controls.Add(BuildListViewCard())

        _root.Controls.Add(SectionTitle("NexaPropertyGrid — settings"))
        _root.Controls.Add(BuildPropertyGridCard())

        _root.Controls.Add(SectionTitle("NexaTreeView — application navigation"))
        _root.Controls.Add(BuildTreeViewCard())

        _root.Controls.Add(SectionTitle("Student Management Grid — advanced"))
        _root.Controls.Add(BuildAdvancedGridCard())

        Controls.Add(_root)

        AddHandler ThemeManager.ThemeChanged, Sub(s, e) ApplyTheme(e.Current)
        AddHandler Disposed, AddressOf OnSelfDisposed
        AddHandler HandleCreated, Sub() ApplyTheme(ThemeManager.Current)
    End Sub

    Private Sub OnSelfDisposed(sender As Object, e As EventArgs)
        RemoveHandler ThemeManager.ThemeChanged, AddressOf OnSelfDisposed
    End Sub

    Private Shared Function HeaderLabel(text As String) As Label
        Return New Label With {.Text = text, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)}
    End Function

    Private Shared Function BodyLabel(text As String) As Label
        Return New Label With {.Text = text, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 16)}
    End Function

    Private Shared Function SectionTitle(title As String) As Label
        Return New Label With {.Text = title, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 16, 0, 8)}
    End Function

    Private Function BuildDataGridCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Native DataGridView with row numbers, alternating rows, sorting, and theme-aware selection."))

        _grid = New NexaDataGridView With {
            .Dock = DockStyle.Top,
            .Height = 320,
            .ReadOnly = True,
            .ShowRowNumbers = False,
            .AlternateRowColors = True,
            .HeaderHeight = 36,
            .RowHeight = 32,
            .GridStyle = NexaGridStyle.Default,
            .HeaderStyle = NexaHeaderStyle.Standard,
            .RowCornerRadius = 0,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = True,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False
        }

        _grid.Columns.Add("Id", "Student ID")
        _grid.Columns.Add("Name", "Student Name")
        _grid.Columns.Add("Course", "Course")
        _grid.Columns.Add("Batch", "Batch")
        _grid.Columns.Add("Gender", "Gender")
        _grid.Columns.Add("Telephone", "Telephone")
        _grid.Columns.Add("Status", "Status")
        _grid.Columns.Add("Attendance", "Attendance")

        Dim students = GenerateStudents()
        For Each s In students
            _grid.Rows.Add(s.Id, s.Name, s.Course, s.Batch, s.Gender, s.Telephone, s.Status, s.Attendance)
        Next

        AddHandler _grid.SelectionChanged, Sub(sender, e)
                                                If _grid.SelectedRows.Count > 0 Then
                                                    Dim row = _grid.SelectedRows(0)
                                                    _selectedLabel.Text = "Selected: " & row.Cells(1).Value.ToString()
                                                End If
                                            End Sub

        Dim controls = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 8, 0, 8)
        }

        Dim styleCombo = New ComboBox With {.DropDownStyle = ComboBoxStyle.DropDownList, .Width = 140}
        styleCombo.Items.AddRange(New Object() {"Default", "Compact", "Comfortable"})
        styleCombo.SelectedIndex = 0
        AddHandler styleCombo.SelectedIndexChanged, Sub(sender, e)
                                                            _grid.GridStyle = If(styleCombo.SelectedIndex = 2, NexaGridStyle.Comfortable, If(styleCombo.SelectedIndex = 1, NexaGridStyle.Compact, NexaGridStyle.Default))
                                                            _gridStyleLabel.Text = "GridStyle: " & _grid.GridStyle.ToString()
                                                        End Sub

        Dim toggleRowNumbers = New NexaButton With {.Text = "Toggle Row Numbers", .Style = NexaButtonStyle.Secondary, .AutoSize = True}
        AddHandler toggleRowNumbers.Click, Sub(sender, e) _grid.ShowRowNumbers = Not _grid.ShowRowNumbers

        Dim toggleFilter = New NexaButton With {.Text = "Toggle Filter Row", .Style = NexaButtonStyle.Secondary, .AutoSize = True}
        AddHandler toggleFilter.Click, Sub(sender, e) _grid.ShowFilterRow = Not _grid.ShowFilterRow

        Dim toggleSearch = New NexaButton With {.Text = "Toggle Search", .Style = NexaButtonStyle.Secondary, .AutoSize = True}
        AddHandler toggleSearch.Click, Sub(sender, e) _grid.ShowSearchPanel = Not _grid.ShowSearchPanel

        Dim toggleSummary = New NexaButton With {.Text = "Toggle Summary", .Style = NexaButtonStyle.Secondary, .AutoSize = True}
        AddHandler toggleSummary.Click, Sub(sender, e)
                                                  _grid.ShowSummaryFooter = Not _grid.ShowSummaryFooter
                                                  If _grid.ShowSummaryFooter Then
                                                      _grid.SummaryItems.Clear()
                                                      _grid.SummaryItems.Add("Attendance", DataGridViewSummaryAggregate.Avg, "Avg: {0}%")
                                                  End If
                                              End Sub

        Dim chooserBtn = New NexaButton With {.Text = "Column Chooser", .Style = NexaButtonStyle.Secondary, .AutoSize = True}
        AddHandler chooserBtn.Click, Sub(sender, e) _grid.ShowColumnChooser()

        Dim exportBtn = New NexaButton With {.Text = "Export CSV", .Style = NexaButtonStyle.Primary, .AutoSize = True}
        AddHandler exportBtn.Click, Sub(sender, e)
                                            Using sfd = New SaveFileDialog With {.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*", .FileName = "students.csv"}
                                                If sfd.ShowDialog() = DialogResult.OK Then
                                                    _grid.ExportToCsv(sfd.FileName)
                                                    MessageBox.Show("Exported successfully.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information)
                                                End If
                                            End Using
                                        End Sub

        _selectedLabel = New Label With {.Text = "Selected: None", .AutoSize = True, .Margin = New Padding(12, 0, 0, 0)}
        _gridStyleLabel = New Label With {.Text = "GridStyle: Default", .AutoSize = True, .Margin = New Padding(12, 0, 0, 0)}

        controls.Controls.Add(New Label With {.Text = "Style:", .AutoSize = True, .Margin = New Padding(0, 4, 4, 0)})
        controls.Controls.Add(styleCombo)
        controls.Controls.Add(toggleRowNumbers)
        controls.Controls.Add(toggleFilter)
        controls.Controls.Add(toggleSearch)
        controls.Controls.Add(toggleSummary)
        controls.Controls.Add(chooserBtn)
        controls.Controls.Add(exportBtn)
        controls.Controls.Add(_selectedLabel)
        controls.Controls.Add(_gridStyleLabel)

        card.Controls.Add(_grid)
        card.Controls.Add(controls)
        Return card
    End Function

    Private Function BuildAutoGridCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Columns are auto-generated from the API DTO using reflection. Ideal for dynamic data sources."))

        Dim apiGrid = New NexaDataGridView With {
            .Dock = DockStyle.Top,
            .Height = 320,
            .ReadOnly = True,
            .AlternateRowColors = True,
            .HeaderHeight = 36,
            .RowHeight = 32,
            .GridStyle = NexaGridStyle.Default,
            .HeaderStyle = NexaHeaderStyle.Standard,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False
        }

        Dim apiData = GenerateApiCourses()
        apiGrid.AutoGenerateColumnsFromType(apiData, "Course")

        card.Controls.Add(apiGrid)
        Return card
    End Function

    Private Function BuildManualGridCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Columns are defined manually with custom widths, formats, and behavior — full control like DevExpress."))

        Dim manualGrid = New NexaDataGridView With {
            .Dock = DockStyle.Top,
            .Height = 320,
            .ReadOnly = True,
            .AlternateRowColors = True,
            .HeaderHeight = 36,
            .RowHeight = 32,
            .GridStyle = NexaGridStyle.Default,
            .HeaderStyle = NexaHeaderStyle.Standard,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False
        }

        Dim colId = New DataGridViewTextBoxColumn With {.DataPropertyName = "Id", .HeaderText = "ID", .Width = 80, .ReadOnly = True}
        Dim colName = New DataGridViewTextBoxColumn With {.DataPropertyName = "Name", .HeaderText = "Employee Name", .Width = 180}
        Dim colDept = New DataGridViewComboBoxColumn With {.DataPropertyName = "Department", .HeaderText = "Department", .Width = 140}
        colDept.Items.AddRange("ICT", "English", "Business", "HR", "Finance")
        Dim colSalary = New DataGridViewTextBoxColumn With {.DataPropertyName = "Salary", .HeaderText = "Salary", .Width = 100}
        colSalary.DefaultCellStyle.Format = "C2"
        Dim colActive = New DataGridViewCheckBoxColumn With {.DataPropertyName = "IsActive", .HeaderText = "Active", .Width = 70}
        Dim colDate = New DataGridViewTextBoxColumn With {.DataPropertyName = "Joined", .HeaderText = "Joined", .Width = 120}
        colDate.DefaultCellStyle.Format = "yyyy-MM-dd"

        manualGrid.Columns.AddRange(colId, colName, colDept, colSalary, colActive, colDate)

        Dim employees = GenerateEmployees()
        manualGrid.DataSource = New BindingList(Of Employee)(employees)

        card.Controls.Add(manualGrid)
        Return card
    End Function

    Private Shared Function GenerateApiCourses() As List(Of CourseDto)
        Return New List(Of CourseDto) From {
            New CourseDto With {.Code = "ICT-101", .Title = "Introduction to Computing", .Credits = 3, .Level = "Beginner", .Instructor = "Dr. Perera", .Capacity = 40, .Enrolled = 38},
            New CourseDto With {.Code = "ICT-201", .Title = "Data Structures", .Credits = 4, .Level = "Intermediate", .Instructor = "Prof. Silva", .Capacity = 35, .Enrolled = 35},
            New CourseDto With {.Code = "ENG-101", .Title = "English for Academic Purposes", .Credits = 2, .Level = "Beginner", .Instructor = "Ms. Fernando", .Capacity = 50, .Enrolled = 45},
            New CourseDto With {.Code = "BUS-301", .Title = "Business Strategy", .Credits = 3, .Level = "Advanced", .Instructor = "Dr. Kumara", .Capacity = 30, .Enrolled = 28},
            New CourseDto With {.Code = "ICT-301", .Title = "Machine Learning Basics", .Credits = 4, .Level = "Advanced", .Instructor = "Dr. Dias", .Capacity = 25, .Enrolled = 25},
            New CourseDto With {.Code = "ENG-201", .Title = "Business English", .Credits = 2, .Level = "Intermediate", .Instructor = "Ms. Liyanage", .Capacity = 40, .Enrolled = 30}
        }
    End Function

    Private Class CourseDto
        Public Property Code As String = String.Empty
        Public Property Title As String = String.Empty
        Public Property Credits As Integer
        Public Property Level As String = String.Empty
        Public Property Instructor As String = String.Empty
        Public Property Capacity As Integer
        Public Property Enrolled As Integer
    End Class

    Private Shared Function GenerateEmployees() As List(Of Employee)
        Return New List(Of Employee) From {
            New Employee With {.Id = "EMP-001", .Name = "Kasun Perera", .Department = "ICT", .Salary = 120000D, .IsActive = True, .Joined = New DateTime(2021, 5, 10)},
            New Employee With {.Id = "EMP-002", .Name = "Nimal Silva", .Department = "English", .Salary = 95000D, .IsActive = True, .Joined = New DateTime(2022, 2, 15)},
            New Employee With {.Id = "EMP-003", .Name = "Amal Fernando", .Department = "Business", .Salary = 110000D, .IsActive = True, .Joined = New DateTime(2020, 9, 1)},
            New Employee With {.Id = "EMP-004", .Name = "Saman Kumara", .Department = "ICT", .Salary = 130000D, .IsActive = False, .Joined = New DateTime(2019, 3, 22)},
            New Employee With {.Id = "EMP-005", .Name = "Malini Fernando", .Department = "HR", .Salary = 85000D, .IsActive = True, .Joined = New DateTime(2023, 1, 8)},
            New Employee With {.Id = "EMP-006", .Name = "Dilshani Perera", .Department = "Finance", .Salary = 105000D, .IsActive = True, .Joined = New DateTime(2021, 11, 30)}
        }
    End Function

    Private Class Employee
        Public Property Id As String = String.Empty
        Public Property Name As String = String.Empty
        Public Property Department As String = String.Empty
        Public Property Salary As Decimal
        Public Property IsActive As Boolean
        Public Property Joined As DateTime
    End Class

    Private Function BuildListViewCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Native ListView in Details view with themed column headers and row selection."))

        _listView = New NexaListView With {
            .Dock = DockStyle.Top,
            .Height = 220,
            .View = View.Details,
            .FullRowSelect = True,
            .GridLines = True,
            .ListStyle = NexaListStyle.Default
        }

        _listView.Columns.Add("Name", 220)
        _listView.Columns.Add("Type", 120)
        _listView.Columns.Add("Status", 100)

        _listView.Items.Add(New ListViewItem(New String() {"Student List", "Data", "Ready"}))
        _listView.Items.Add(New ListViewItem(New String() {"Course List", "Data", "Ready"}))
        _listView.Items.Add(New ListViewItem(New String() {"Reports", "Document", "Ready"}))
        _listView.Items.Add(New ListViewItem(New String() {"Settings", "System", "Ready"}))
        _listView.Items.Add(New ListViewItem(New String() {"Attendance", "Data", "Processing"}))

        card.Controls.Add(_listView)
        Return card
    End Function

    Private Function BuildPropertyGridCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Native PropertyGrid with NexaUI category, help, and command area colors."))

        _propertyGrid = New NexaPropertyGrid With {
            .Dock = DockStyle.Top,
            .Height = 260,
            .GridStyle = NexaPropertyGridStyle.Default,
            .HelpVisible = True,
            .CommandsVisibleIfAvailable = True,
            .ToolbarVisible = True,
            .PropertySort = PropertySort.Categorized
        }

        _propertyGrid.SelectedObject = New DemoAppSettings()

        card.Controls.Add(_propertyGrid)
        Return card
    End Function

    Private Function BuildTreeViewCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Native TreeView with themed nodes, lines, expand/collapse, and checkboxes."))

        _treeView = New NexaTreeView With {
            .Dock = DockStyle.Top,
            .Height = 240,
            .TreeStyle = NexaTreeStyle.Default,
            .ShowRootLines = True,
            .ShowNodeLines = True,
            .CheckBoxes = True,
            .ShowLines = True,
            .ShowPlusMinus = True,
            .FullRowSelect = True,
            .HotTracking = True
        }

        Dim root = _treeView.Nodes.Add("NexaUI Application")
        Dim dashboard = root.Nodes.Add("Dashboard")
        Dim students = root.Nodes.Add("Students")
        students.Nodes.Add("All Students")
        students.Nodes.Add("Active Students")
        students.Nodes.Add("Graduated Students")
        Dim courses = root.Nodes.Add("Courses")
        courses.Nodes.Add("ICT")
        courses.Nodes.Add("English")
        courses.Nodes.Add("Business")
        Dim assessments = root.Nodes.Add("Assessments")
        assessments.Nodes.Add("Continuous Assessment")
        assessments.Nodes.Add("Final Assessment")
        Dim reports = root.Nodes.Add("Reports")
        reports.Nodes.Add("Student Reports")
        reports.Nodes.Add("Attendance Reports")
        reports.Nodes.Add("Course Reports")
        Dim settingsNode = root.Nodes.Add("Settings")

        root.Expand()
        card.Controls.Add(_treeView)
        Return card
    End Function

    Private Function BuildAdvancedGridCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Student Management Grid — search, selection, and status feedback."))

        Dim advancedGrid = New NexaDataGridView With {
            .Dock = DockStyle.Top,
            .Height = 300,
            .ReadOnly = False,
            .ShowRowNumbers = False,
            .AlternateRowColors = True,
            .HeaderHeight = 36,
            .RowHeight = 32,
            .GridStyle = NexaGridStyle.Default,
            .HeaderStyle = NexaHeaderStyle.Standard,
            .RowCornerRadius = 0,
            .AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            .SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            .MultiSelect = False,
            .AllowUserToAddRows = False,
            .AllowUserToDeleteRows = False,
            .ShowFilterRow = True,
            .ShowSearchPanel = True,
            .ShowSummaryFooter = True
        }

        advancedGrid.Columns.Add("Id", "Student ID")
        advancedGrid.Columns.Add("Name", "Student Name")
        advancedGrid.Columns.Add("Course", "Course")
        advancedGrid.Columns.Add("Batch", "Batch")
        advancedGrid.Columns.Add("Status", "Status")
        Dim attendanceCol = New DataGridViewTextBoxColumn With {.DataPropertyName = "Attendance", .HeaderText = "Attendance", .Name = "Attendance"}
        attendanceCol.DefaultCellStyle.Format = "P0"
        advancedGrid.Columns.Add(attendanceCol)

        advancedGrid.SummaryItems.Add("Attendance", DataGridViewSummaryAggregate.Avg, "Avg: {0:P0}")

        Dim students = GenerateStudents()
        For Each s In students
            Dim attendanceValue = If(s.Attendance = 0D, 0D, s.Attendance / 100D)
            advancedGrid.Rows.Add(s.Id, s.Name, s.Course, s.Batch, s.Status, attendanceValue)
        Next

        Dim topBar = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 3,
            .Margin = New Padding(0, 0, 0, 8)
        }
        topBar.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        topBar.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        topBar.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))

        Dim searchBox = New TextBox With {.Width = 220, .Margin = New Padding(0, 0, 8, 0)}
        AddHandler searchBox.TextChanged, Sub(sender, e)
                                              Dim term = searchBox.Text.Trim().ToLowerInvariant()
                                              For Each row As DataGridViewRow In advancedGrid.Rows
                                                  If row.IsNewRow Then Continue For
                                                  Dim v1 = If(row.Cells(1).Value, String.Empty).ToString()
                                                  Dim v2 = If(row.Cells(2).Value, String.Empty).ToString()
                                                  Dim match = String.IsNullOrEmpty(term) OrElse
                                                              v1.ToLowerInvariant().Contains(term) OrElse
                                                              v2.ToLowerInvariant().Contains(term)
                                                  row.Visible = match
                                              Next
                                          End Sub

        Dim addButton = New NexaButton With {.Text = "Add Student", .Style = NexaButtonStyle.Primary, .AutoSize = True}
        AddHandler addButton.Click, Sub(sender, e)
                                         advancedGrid.Rows.Add((students.Count + 1).ToString(), "New Student", "ICT", "2025-A", "Active", 0D)
                                     End Sub

        topBar.Controls.Add(searchBox, 0, 0)
        topBar.Controls.Add(addButton, 1, 0)

        card.Controls.Add(topBar)
        card.Controls.Add(advancedGrid)

        Dim statusRow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 8, 0, 0)
        }
        statusRow.Controls.Add(New Label With {.Text = "Status: Ready", .AutoSize = True})
        statusRow.Controls.Add(New NexaBadge With {.Text = "Live", .BadgeStyle = NexaBadgeStyle.Success, .BadgeSize = NexaBadgeSize.Small})
        card.Controls.Add(statusRow)

        Return card
    End Function

    Private Shared Function GenerateStudents() As List(Of Student)
        Return New List(Of Student) From {
            New Student With {.Id = "STU-001", .Name = "Kasun Perera", .Course = "ICT", .Batch = "2024-A", .Gender = "Male", .Telephone = "077-1234567", .Status = "Active", .Attendance = 0.92D},
            New Student With {.Id = "STU-002", .Name = "Nimal Silva", .Course = "ICT", .Batch = "2024-A", .Gender = "Male", .Telephone = "077-7654321", .Status = "Active", .Attendance = 0.88D},
            New Student With {.Id = "STU-003", .Name = "Amal Fernando", .Course = "English", .Batch = "2024-B", .Gender = "Male", .Telephone = "071-2345678", .Status = "Completed", .Attendance = 0.95D},
            New Student With {.Id = "STU-004", .Name = "Saman Kumara", .Course = "ICT", .Batch = "2024-A", .Gender = "Male", .Telephone = "072-3456789", .Status = "Active", .Attendance = 0.78D},
            New Student With {.Id = "STU-005", .Name = "Malini Fernando", .Course = "Business", .Batch = "2024-B", .Gender = "Female", .Telephone = "073-4567890", .Status = "Active", .Attendance = 0.91D},
            New Student With {.Id = "STU-006", .Name = "Dilshani Perera", .Course = "English", .Batch = "2024-C", .Gender = "Female", .Telephone = "074-5678901", .Status = "Completed", .Attendance = 0.97D},
            New Student With {.Id = "STU-007", .Name = "Ruwan Jayasinghe", .Course = "ICT", .Batch = "2024-A", .Gender = "Male", .Telephone = "075-6789012", .Status = "Active", .Attendance = 0.85D},
            New Student With {.Id = "STU-008", .Name = "Thilini Wickrama", .Course = "Business", .Batch = "2024-B", .Gender = "Female", .Telephone = "076-7890123", .Status = "Active", .Attendance = 0.89D},
            New Student With {.Id = "STU-009", .Name = "Chamara Dias", .Course = "ICT", .Batch = "2024-C", .Gender = "Male", .Telephone = "077-8901234", .Status = "On Leave", .Attendance = 0.45D},
            New Student With {.Id = "STU-010", .Name = "Nadeeka Liyanage", .Course = "English", .Batch = "2024-A", .Gender = "Female", .Telephone = "078-9012345", .Status = "Active", .Attendance = 0.93D},
            New Student With {.Id = "STU-011", .Name = "Buddhika Herath", .Course = "Business", .Batch = "2024-C", .Gender = "Male", .Telephone = "079-0123456", .Status = "Active", .Attendance = 0.81D},
            New Student With {.Id = "STU-012", .Name = "Shalika Ekanayake", .Course = "ICT", .Batch = "2024-B", .Gender = "Female", .Telephone = "070-1234567", .Status = "Completed", .Attendance = 0.96D},
            New Student With {.Id = "STU-013", .Name = "Mahesh Randeniya", .Course = "English", .Batch = "2024-B", .Gender = "Male", .Telephone = "071-2345679", .Status = "Active", .Attendance = 0.74D},
            New Student With {.Id = "STU-014", .Name = "Kusum Seneviratne", .Course = "Business", .Batch = "2024-A", .Gender = "Female", .Telephone = "072-3456780", .Status = "Active", .Attendance = 0.90D},
            New Student With {.Id = "STU-015", .Name = "Ravindra Bandara", .Course = "ICT", .Batch = "2024-C", .Gender = "Male", .Telephone = "073-4567891", .Status = "Active", .Attendance = 0.87D}
        }
    End Function

    Private Sub ApplyTheme(theme As ITheme)
        If IsDisposed OrElse Disposing Then Return

        Dim palette = theme.Palette
        BackColor = CType(palette(NexaColorRole.Background).Value, Color)
        ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)

        For Each c As Control In _root.Controls
            If TypeOf c Is Label Then
                Dim l = DirectCast(c, Label)
                If l.Text = "Data Presentation Controls" Then
                    l.Font = theme.Typography.ToFont(NexaTypographyRole.Heading, NexaFormsDpi.CurrentDpi(Me))
                    l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
                ElseIf l.Text.StartsWith("NexaDataGridView", StringComparison.Ordinal) OrElse
                        l.Text.StartsWith("NexaListView", StringComparison.Ordinal) OrElse
                        l.Text.StartsWith("NexaPropertyGrid", StringComparison.Ordinal) OrElse
                        l.Text.StartsWith("NexaTreeView", StringComparison.Ordinal) OrElse
                        l.Text.StartsWith("Student Management", StringComparison.Ordinal) Then
                    l.Font = theme.Typography.ToFont(NexaTypographyRole.Title, NexaFormsDpi.CurrentDpi(Me))
                    l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
                ElseIf l.Text.StartsWith("Native ", StringComparison.Ordinal) OrElse l.Text.StartsWith("Theme-aware", StringComparison.Ordinal) Then
                    l.Font = theme.Typography.ToFont(NexaTypographyRole.Body, NexaFormsDpi.CurrentDpi(Me))
                    l.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
                End If
            End If
        Next

        If _grid IsNot Nothing Then _grid.Invalidate()
        If _listView IsNot Nothing Then _listView.Invalidate()
        If _propertyGrid IsNot Nothing Then _propertyGrid.Invalidate()
        If _treeView IsNot Nothing Then _treeView.Invalidate()
    End Sub

    Private Function CreateDemoCard() As Panel
        Dim palette = ThemeManager.Current.Palette
        Dim card = New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(16),
            .Margin = New Padding(0, 0, 0, 12),
            .BackColor = CType(palette(NexaColorRole.Surface).Value, Color)
        }
        AddHandler card.Paint, Sub(s, e)
                                   Using pen = New Pen(CType(palette(NexaColorRole.Border).Value, Color), 1.0F)
                                       Dim r = card.ClientRectangle
                                       r.Width -= 1
                                       r.Height -= 1
                                       e.Graphics.DrawRectangle(pen, r)
                                   End Using
                               End Sub
        Return card
    End Function

    Private Shared Function DescriptionLabel(text As String) As NexaLabel
        Return New NexaLabel With {.Text = text, .Dock = DockStyle.Top, .AutoSize = True, .LabelStyle = NexaLabelStyle.Muted, .Margin = New Padding(0, 0, 0, 8)}
    End Function

    Private Class Student
        Public Property Id As String = String.Empty
        Public Property Name As String = String.Empty
        Public Property Course As String = String.Empty
        Public Property Batch As String = String.Empty
        Public Property Gender As String = String.Empty
        Public Property Telephone As String = String.Empty
        Public Property Status As String = String.Empty
        Public Property Attendance As Decimal
    End Class

    Private Class DemoAppSettings
        <Category("Appearance")>
        <DisplayName("Theme")>
        Public Property Theme As String = "Light"

        <Category("Appearance")>
        <DisplayName("Accent Color")>
        Public Property AccentColor As String = "Blue"

        <Category("Appearance")>
        <DisplayName("Font Size")>
        Public Property FontSize As Integer = 12

        <Category("Application")>
        <DisplayName("Application Name")>
        Public Property AppName As String = "NexaUI Demo"

        <Category("Application")>
        <DisplayName("Auto Save")>
        Public Property AutoSave As Boolean = True

        <Category("Application")>
        <DisplayName("Start Minimized")>
        Public Property StartMinimized As Boolean = False

        <Category("Notifications")>
        <DisplayName("Enable Notifications")>
        Public Property EnableNotifications As Boolean = True

        <Category("Notifications")>
        <DisplayName("Notification Duration (ms)")>
        Public Property NotificationDuration As Integer = 3000

        <Category("Notifications")>
        <DisplayName("Sound Enabled")>
        Public Property SoundEnabled As Boolean = True
    End Class
End Class
