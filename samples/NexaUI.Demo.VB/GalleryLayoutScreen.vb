Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Themes

Public NotInheritable Class GalleryLayoutScreen
    Inherits UserControl

    Private ReadOnly _root As TableLayoutPanel

    Public Sub New()
        Dock = DockStyle.Fill
        AutoScroll = True
        BackColor = CType(ThemeManager.Current.Palette(NexaColorRole.Background).Value, Color)

        _root = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .Padding = New Padding(32, 24, 32, 32)
        }
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))

        Dim heading = HeaderLabel("Layout Controls")
        _root.Controls.Add(heading)
        _root.SetColumnSpan(heading, 2)

        Dim intro = BodyLabel("NexaPanel, NexaCard, NexaGroupBox, NexaFlowPanel, NexaTablePanel. Modern containers that preserve native WinForms layout.")
        _root.Controls.Add(intro)
        _root.SetColumnSpan(intro, 2)

        _root.Controls.Add(SectionTitle("NexaPanel — surface styles"))
        _root.Controls.Add(BuildPanelSurfaceCard())

        _root.Controls.Add(SectionTitle("NexaPanel — bordered, rounded, elevated, shadow, transparent"))
        _root.Controls.Add(BuildPanelVariantsCard())

        _root.Controls.Add(SectionTitle("NexaCard — System Information"))
        _root.Controls.Add(BuildSystemInfoCard())

        _root.Controls.Add(SectionTitle("NexaCard — Application Statistics"))
        _root.Controls.Add(BuildAppStatsCard())

        _root.Controls.Add(SectionTitle("NexaCard — Quick Actions"))
        _root.Controls.Add(BuildQuickActionsCard())

        _root.Controls.Add(SectionTitle("NexaCard — Recent Activity"))
        _root.Controls.Add(BuildRecentActivityCard())

        _root.Controls.Add(SectionTitle("NexaGroupBox — Personal / Account / Display"))
        _root.Controls.Add(BuildGroupBoxCard())

        _root.Controls.Add(SectionTitle("NexaFlowPanel — horizontal, vertical, wrapped, auto-scroll"))
        _root.Controls.Add(BuildFlowPanelCard())

        _root.Controls.Add(SectionTitle("NexaTablePanel — realistic form layouts"))
        _root.Controls.Add(BuildTablePanelCard())

        _root.Controls.Add(SectionTitle("Dashboard Layout Example"))
        Dim dashboard = BuildDashboardCard()
        _root.Controls.Add(dashboard)
        _root.SetColumnSpan(dashboard, 2)

        Controls.Add(_root)

        AddHandler ThemeManager.ThemeChanged, AddressOf OnThemeChanged
        AddHandler Disposed, Sub() RemoveHandler ThemeManager.ThemeChanged, AddressOf OnThemeChanged
    End Sub

    Private Sub OnThemeChanged(sender As Object, e As ThemeChangedEventArgs)
        If IsDisposed OrElse Disposing Then Return
        If InvokeRequired Then
            BeginInvoke(Sub() OnThemeChanged(sender, e))
            Return
        End If
        BackColor = CType(e.Current.Palette(NexaColorRole.Background).Value, Color)
    End Sub

    ' ---------- Cards ----------

    Private Function BuildPanelSurfaceCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Default, Surface, Elevated, Transparent surface styles."))

        Dim row = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Default, "Default"))
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Surface, "Surface"))
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Elevated, "Elevated"))
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Transparent, "Transparent"))
        card.Controls.Add(row)
        Return card
    End Function

    Private Shared Function MakePanelChip(style As NexaPanelSurfaceStyle, label As String) As NexaPanel
        Dim panel As New NexaPanel With {
            .SurfaceStyle = style,
            .BorderStyleEx = NexaBorderStyleEx.Solid,
            .CornerRadius = 2,
            .Size = New Size(140, 72),
            .Margin = New Padding(0, 4, 12, 4)
        }
        panel.Controls.Add(New NexaLabel With {
            .Text = label,
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Location = New Point(8, 8)
        })
        Return panel
    End Function

    Private Function BuildPanelVariantsCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Bordered, rounded, elevated, shadow, and transparent panels."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 3,
            .RowCount = 2
        }
        For i = 0 To 2
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
        Next
        For i = 0 To 1
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        Next

        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Surface, NexaBorderStyleEx.Solid, 0, False, "Bordered"), 0, 0)
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Surface, NexaBorderStyleEx.Solid, 8, False, "Rounded"), 1, 0)
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Elevated, NexaBorderStyleEx.None, 4, True, "Shadow"), 2, 0)
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Surface, NexaBorderStyleEx.Solid, 4, False, "Bordered + Rounded"), 0, 1)
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Elevated, NexaBorderStyleEx.Solid, 8, True, "Shadow + Rounded"), 1, 1)
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Transparent, NexaBorderStyleEx.Solid, 4, False, "Transparent"), 2, 1)
        card.Controls.Add(grid)
        Return card
    End Function

    Private Shared Function MakePanel(surface As NexaPanelSurfaceStyle, border As NexaBorderStyleEx, corner As Integer, shadow As Boolean, label As String) As NexaPanel
        Dim panel As New NexaPanel With {
            .SurfaceStyle = surface,
            .BorderStyleEx = border,
            .CornerRadius = corner,
            .ShadowEnabled = shadow,
            .ShadowDepth = If(shadow, 4, 0),
            .Size = New Size(160, 64),
            .Margin = New Padding(0, 4, 8, 4),
            .Dock = DockStyle.Top
        }
        panel.Controls.Add(New NexaLabel With {
            .Text = label,
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Location = New Point(8, 8)
        })
        Return panel
    End Function

    Private Function BuildSystemInfoCard() As Control
        Dim card As New NexaCard With {
            .Title = "System Information",
            .Subtitle = "Current computer status",
            .Dock = DockStyle.Top,
            .CornerRadius = 4,
            .ShadowEnabled = True,
            .ShadowDepth = 4,
            .Margin = New Padding(0, 0, 0, 12)
        }
        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35.0F))
        grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65.0F))
        AddReadOnlyRow(grid, "OS", "Windows 11 Pro")
        AddReadOnlyRow(grid, "Version", "23H2")
        AddReadOnlyRow(grid, "Architecture", "x64")
        AddReadOnlyRow(grid, "Hostname", "DESKTOP-NEXAUI")
        card.ContentPanel.Controls.Add(grid)
        Return card
    End Function

    Private Shared Sub AddReadOnlyRow(grid As TableLayoutPanel, label As String, value As String)
        grid.Controls.Add(New NexaLabel With {
            .Text = label,
            .LabelStyle = NexaLabelStyle.Muted,
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .Margin = New Padding(0, 2, 0, 2)
        })
        grid.Controls.Add(New NexaLabel With {
            .Text = value,
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .Margin = New Padding(0, 2, 0, 2)
        })
    End Sub

    Private Function BuildAppStatsCard() As Control
        Dim card As New NexaCard With {
            .Title = "Application Statistics",
            .Subtitle = "Runtime metrics",
            .Dock = DockStyle.Top,
            .CornerRadius = 4,
            .Margin = New Padding(0, 0, 0, 12)
        }
        Dim row = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        row.Controls.Add(MakeStat("42%", "CPU"))
        row.Controls.Add(MakeStat("68%", "RAM"))
        row.Controls.Add(MakeStat("71%", "Storage"))
        row.Controls.Add(MakeStat("1.2k", "Requests/s"))
        card.ContentPanel.Controls.Add(row)
        Return card
    End Function

    Private Shared Function MakeStat(value As String, label As String) As NexaPanel
        Dim stat As New NexaPanel With {
            .SurfaceStyle = NexaPanelSurfaceStyle.Elevated,
            .BorderStyleEx = NexaBorderStyleEx.Solid,
            .CornerRadius = 3,
            .Size = New Size(96, 72),
            .Margin = New Padding(0, 0, 8, 0)
        }
        stat.Controls.Add(New NexaLabel With {
            .Text = value,
            .LabelStyle = NexaLabelStyle.Heading,
            .AutoSize = True,
            .Location = New Point(12, 8)
        })
        stat.Controls.Add(New NexaLabel With {
            .Text = label,
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Location = New Point(12, 44)
        })
        Return stat
    End Function

    Private Function BuildQuickActionsCard() As Control
        Dim card As New NexaCard With {
            .Title = "Quick Actions",
            .Subtitle = "Common tasks",
            .Dock = DockStyle.Top,
            .CornerRadius = 4,
            .Margin = New Padding(0, 0, 0, 12)
        }
        Dim col = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        col.Controls.Add(New NexaButton With {.Text = "Refresh now", .Style = NexaButtonStyle.Primary, .Width = 200, .Margin = New Padding(0, 0, 0, 8)})
        col.Controls.Add(New NexaButton With {.Text = "Open settings", .Style = NexaButtonStyle.Secondary, .Width = 200, .Margin = New Padding(0, 0, 0, 8)})
        col.Controls.Add(New NexaButton With {.Text = "Export data", .Style = NexaButtonStyle.Outline, .Width = 200, .Margin = New Padding(0, 0, 0, 8)})
        col.Controls.Add(New NexaButton With {.Text = "View logs", .Style = NexaButtonStyle.Ghost, .Width = 200})
        card.ContentPanel.Controls.Add(col)
        Return card
    End Function

    Private Function BuildRecentActivityCard() As Control
        Dim card As New NexaCard With {
            .Title = "Recent Activity",
            .Subtitle = "Last 24 hours",
            .Dock = DockStyle.Top,
            .CornerRadius = 4,
            .Margin = New Padding(0, 0, 0, 12)
        }
        Dim list = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        list.Controls.Add(MakeActivityItem("Build succeeded", "12 minutes ago", NexaLabelStyle.Success))
        list.Controls.Add(MakeActivityItem("Tests passed (98/98)", "1 hour ago", NexaLabelStyle.Success))
        list.Controls.Add(MakeActivityItem("Dependency update", "3 hours ago", NexaLabelStyle.Info))
        list.Controls.Add(MakeActivityItem("Cache cleared", "Yesterday", NexaLabelStyle.Muted))
        card.ContentPanel.Controls.Add(list)
        Return card
    End Function

    Private Shared Function MakeActivityItem(title As String, [when] As String, status As NexaLabelStyle) As NexaPanel
        Dim item As New NexaPanel With {
            .SurfaceStyle = NexaPanelSurfaceStyle.Transparent,
            .BorderStyleEx = NexaBorderStyleEx.None,
            .Size = New Size(260, 44),
            .Margin = New Padding(0, 0, 0, 4)
        }
        item.Controls.Add(New NexaLabel With {
            .Text = title,
            .AutoSize = True,
            .Location = New Point(0, 2)
        })
        item.Controls.Add(New NexaLabel With {
            .Text = [when],
            .LabelStyle = status,
            .AutoSize = True,
            .Location = New Point(0, 22)
        })
        Return item
    End Function

    Private Function BuildGroupBoxCard() As Control
        Dim host = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        host.Controls.Add(MakeGroupBox("Personal Information", {
            ("Name", MakeTextBox("Your full name")),
            ("Email", MakeTextBox("name@example.com"))
        }))
        host.Controls.Add(MakeGroupBox("Account Settings", {
            ("Username", MakeTextBox("nexa_user")),
            ("Password", MakeTextBox("••••••••"))
        }))
        host.Controls.Add(MakeGroupBox("Display Settings", {
            ("Theme", MakeThemeCombo()),
            ("Density", MakeDensityCombo())
        }))
        Return host
    End Function

    Private Shared Function MakeGroupBox(title As String, rows As (Label As String, Editor As Control)()) As NexaGroupBox
        Dim gb As New NexaGroupBox With {
            .Text = title,
            .Width = 260,
            .Height = 120,
            .Margin = New Padding(0, 0, 12, 0)
        }
        Dim tbl = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 2,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(8, 4, 8, 8)
        }
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 35.0F))
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 65.0F))
        For r = 0 To rows.Length - 1
            tbl.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            tbl.Controls.Add(New NexaLabel With {
                .Text = rows(r).Label,
                .LabelStyle = NexaLabelStyle.Muted,
                .AutoSize = True,
                .Margin = New Padding(0, 6, 6, 4)
            }, 0, r)
            tbl.Controls.Add(rows(r).Editor, 1, r)
        Next
        gb.Controls.Add(tbl)
        Return gb
    End Function

    Private Shared Function MakeTextBox(text As String) As NexaTextBox
        Return New NexaTextBox With {
            .Text = text,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 4, 0, 4)
        }
    End Function

    Private Shared Function MakeThemeCombo() As NexaComboBox
        Dim combo As New NexaComboBox With {
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 4, 0, 4)
        }
        combo.Items.AddRange(New Object() {"System", "Light", "Dark"})
        Return combo
    End Function

    Private Shared Function MakeDensityCombo() As NexaComboBox
        Dim combo As New NexaComboBox With {
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 4, 0, 4)
        }
        combo.Items.AddRange(New Object() {"Compact", "Comfortable", "Spacious"})
        Return combo
    End Function

    Private Function BuildFlowPanelCard() As Control
        Dim host = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }

        ' Horizontal
        Dim horiz = New NexaFlowPanel With {
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .BorderEnabled = True,
            .CornerRadius = 2,
            .Margin = New Padding(0, 0, 0, 12)
        }
        horiz.Controls.AddRange(New Control() {
            New NexaButton With {.Text = "One", .Width = 80},
            New NexaButton With {.Text = "Two", .Width = 80},
            New NexaButton With {.Text = "Three", .Width = 80},
            New NexaButton With {.Text = "Four", .Width = 80}
        })
        host.Controls.Add(New NexaLabel With {.Text = "Horizontal (LeftToRight, no wrap):", .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)})
        host.Controls.Add(horiz)

        ' Vertical
        Dim vert = New NexaFlowPanel With {
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .BorderEnabled = True,
            .CornerRadius = 2,
            .Width = 200,
            .Height = 160,
            .Margin = New Padding(0, 0, 0, 12)
        }
        vert.Controls.AddRange(New Control() {
            New NexaLabel With {.Text = "Line 1", .AutoSize = True},
            New NexaLabel With {.Text = "Line 2", .AutoSize = True},
            New NexaLabel With {.Text = "Line 3", .AutoSize = True},
            New NexaLabel With {.Text = "Line 4", .AutoSize = True}
        })
        host.Controls.Add(New NexaLabel With {.Text = "Vertical (TopDown, fixed height):", .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)})
        host.Controls.Add(vert)

        ' Wrapped
        Dim wrap = New NexaFlowPanel With {
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .BorderEnabled = True,
            .CornerRadius = 2,
            .Width = 280,
            .Margin = New Padding(0, 0, 0, 12)
        }
        For i = 0 To 11
            wrap.Controls.Add(New NexaButton With {.Text = $"Tag {i + 1}", .Width = 64, .Margin = New Padding(2)})
        Next
        host.Controls.Add(New NexaLabel With {.Text = "Wrapped (LeftToRight, WrapContents=True):", .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)})
        host.Controls.Add(wrap)

        ' Auto-scroll
        Dim scroll = New NexaFlowPanel With {
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .AutoScroll = True,
            .BorderEnabled = True,
            .CornerRadius = 2,
            .Width = 200,
            .Height = 120
        }
        For i = 0 To 19
            scroll.Controls.Add(New NexaLabel With {.Text = $"Item {i + 1}", .AutoSize = True, .Margin = New Padding(2)})
        Next
        host.Controls.Add(New NexaLabel With {.Text = "AutoScroll (TopDown, scrollable):", .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)})
        host.Controls.Add(scroll)

        Return host
    End Function

    Private Function BuildTablePanelCard() As Control
        Dim host = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True
        }
        host.Controls.Add(MakeFormTable("Label + TextBox", {
            ("Full name", New NexaTextBox With {.Text = "Ada Lovelace", .Width = 200}),
            ("Email", New NexaTextBox With {.Text = "ada@example.com", .Width = 200})
        }))
        host.Controls.Add(MakeFormTable("Label + ComboBox", {
            ("Country", MakeComboWithItems(200, "Sri Lanka", "India", "USA", "UK")),
            ("Language", MakeComboWithItems(200, "English", "Sinhala", "Tamil"))
        }))
        host.Controls.Add(MakeFormTable("Label + CheckBox", {
            ("Notify", New NexaCheckBox With {.Text = "Email me about updates", .Checked = True}),
            ("", New NexaCheckBox With {.Text = "I agree to the terms"})
        }))
        host.Controls.Add(MakeFormTable("Label + ToggleSwitch", {
            ("Dark mode", New NexaToggleSwitch With {.Text = "Enabled", .Checked = True}),
            ("Auto-save", New NexaToggleSwitch With {.Text = "Enabled", .Checked = False})
        }))
        Return host
    End Function

    Private Shared Function MakeComboWithItems(width As Integer, ParamArray items As String()) As NexaComboBox
        Dim combo As New NexaComboBox With {.Width = width}
        combo.Items.AddRange(items)
        Return combo
    End Function

    Private Shared Function MakeFormTable(title As String, rows As (Label As String, Editor As Control)()) As NexaPanel
        Dim tbl = New NexaTablePanel With {
            .ColumnCount = 2,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 0, 12, 0),
            .BorderEnabled = False
        }
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 100.0F))
        tbl.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        For r = 0 To rows.Length - 1
            tbl.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            tbl.Controls.Add(New NexaLabel With {
                .Text = rows(r).Label,
                .AutoSize = True,
                .Margin = New Padding(0, 6, 6, 4)
            }, 0, r)
            tbl.Controls.Add(rows(r).Editor, 1, r)
        Next
        Dim wrapper As New NexaPanel With {
            .SurfaceStyle = NexaPanelSurfaceStyle.Surface,
            .BorderStyleEx = NexaBorderStyleEx.Solid,
            .CornerRadius = 3,
            .Padding = New Padding(10),
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 0, 12, 0)
        }
        wrapper.Controls.Add(New NexaLabel With {
            .Text = title,
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Dock = DockStyle.Top
        })
        wrapper.Controls.Add(tbl)
        Return wrapper
    End Function

    Private Function BuildDashboardCard() As Control
        Dim outer As New NexaPanel With {
            .SurfaceStyle = NexaPanelSurfaceStyle.Surface,
            .BorderStyleEx = NexaBorderStyleEx.Solid,
            .CornerRadius = 4,
            .ShadowEnabled = True,
            .ShadowDepth = 6,
            .Padding = New Padding(16),
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 8, 0, 0)
        }
        outer.Controls.Add(New NexaLabel With {
            .Text = "Dashboard",
            .LabelStyle = NexaLabelStyle.Heading,
            .AutoSize = True,
            .Dock = DockStyle.Top
        })

        Dim topRow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .Margin = New Padding(0, 12, 0, 12)
        }
        topRow.Controls.Add(MakeStatTile("CPU", "42%", "2 cores active", NexaLabelStyle.Success))
        topRow.Controls.Add(MakeStatTile("RAM", "68%", "5.4 GB / 8.0 GB", NexaLabelStyle.Warning))
        topRow.Controls.Add(MakeStatTile("Storage", "71%", "713 GB / 1.0 TB", NexaLabelStyle.Warning))
        outer.Controls.Add(topRow)

        Dim bottom = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .ColumnCount = 2,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }
        bottom.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 60.0F))
        bottom.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 40.0F))

        Dim activity As New NexaCard With {
            .Title = "Recent Activity",
            .Subtitle = "Last hour",
            .CornerRadius = 3,
            .Margin = New Padding(0, 0, 8, 0)
        }
        Dim actList = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        actList.Controls.Add(MakeActivityItem("Build succeeded", "12 min ago", NexaLabelStyle.Success))
        actList.Controls.Add(MakeActivityItem("Tests passed (98/98)", "1 hr ago", NexaLabelStyle.Success))
        actList.Controls.Add(MakeActivityItem("Deployment finished", "2 hr ago", NexaLabelStyle.Info))
        activity.ContentPanel.Controls.Add(actList)
        bottom.Controls.Add(activity, 0, 0)

        Dim actions As New NexaCard With {
            .Title = "Quick Actions",
            .Subtitle = "Common tasks",
            .CornerRadius = 3
        }
        Dim actCol = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False
        }
        actCol.Controls.Add(New NexaButton With {.Text = "Refresh now", .Style = NexaButtonStyle.Primary, .Width = 200, .Margin = New Padding(0, 0, 0, 6)})
        actCol.Controls.Add(New NexaButton With {.Text = "Open settings", .Width = 200, .Margin = New Padding(0, 0, 0, 6)})
        actCol.Controls.Add(New NexaToggleSwitch With {.Text = "Auto-refresh every 5s", .Checked = True})
        actions.ContentPanel.Controls.Add(actCol)
        bottom.Controls.Add(actions, 1, 0)

        outer.Controls.Add(bottom)
        Return outer
    End Function

    Private Shared Function MakeStatTile(label As String, value As String, detail As String, status As NexaLabelStyle) As NexaPanel
        Dim tile As New NexaPanel With {
            .SurfaceStyle = NexaPanelSurfaceStyle.Elevated,
            .BorderStyleEx = NexaBorderStyleEx.Solid,
            .CornerRadius = 3,
            .Width = 200,
            .Height = 96,
            .Margin = New Padding(0, 0, 8, 0),
            .Padding = New Padding(12)
        }
        tile.Controls.Add(New NexaLabel With {.Text = label, .LabelStyle = NexaLabelStyle.Caption, .AutoSize = True, .Location = New Point(12, 8)})
        tile.Controls.Add(New NexaLabel With {.Text = value, .LabelStyle = NexaLabelStyle.Heading, .AutoSize = True, .Location = New Point(12, 28)})
        tile.Controls.Add(New NexaLabel With {.Text = detail, .LabelStyle = status, .AutoSize = True, .Location = New Point(12, 64)})
        Return tile
    End Function

    ' ---------- helpers ----------

    Private Shared Function HeaderLabel(text As String) As NexaLabel
        Return New NexaLabel With {
            .Text = text,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Heading,
            .Margin = New Padding(0, 0, 0, 8)
        }
    End Function

    Private Shared Function BodyLabel(text As String) As NexaLabel
        Return New NexaLabel With {
            .Text = text,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Muted,
            .Margin = New Padding(0, 0, 0, 16)
        }
    End Function

    Private Shared Function SectionTitle(title As String) As NexaLabel
        Return New NexaLabel With {
            .Text = title,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Subheading,
            .Margin = New Padding(0, 8, 0, 8)
        }
    End Function

    Private Shared Function CreateDemoCard() As Panel
        Dim palette = ThemeManager.Current.Palette
        Dim card As New Panel With {
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
        Return New NexaLabel With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Muted,
            .Margin = New Padding(0, 0, 0, 8)
        }
    End Function
End Class
