Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GalleryNavigationScreen
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

        Dim heading = New NexaLabel With {
            .Text = "Navigation Controls",
            .LabelStyle = NexaLabelStyle.Heading,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 8)
        }
        _root.Controls.Add(heading)
        _root.SetColumnSpan(heading, 2)

        Dim intro = New NexaLabel With {
            .Text = "NexaTabControl, NexaTabPage, NexaNavigationBar, NexaBreadcrumb, NexaStepper. Complete navigation system for desktop applications.",
            .LabelStyle = NexaLabelStyle.Muted,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 16)
        }
        _root.Controls.Add(intro)
        _root.SetColumnSpan(intro, 2)

        ' Tab Control
        _root.Controls.Add(SectionTitle("NexaTabControl — Styles"))
        _root.Controls.Add(BuildTabStyleCard())

        _root.Controls.Add(SectionTitle("NexaTabControl — With NexaTabPage (Icons & Badges)"))
        _root.Controls.Add(BuildTabPageCard())

        _root.Controls.Add(SectionTitle("NexaTabControl — Interactive"))
        _root.Controls.Add(BuildInteractiveTabCard())

        ' Navigation Bar
        _root.Controls.Add(SectionTitle("NexaNavigationBar — Expanded Mode"))
        _root.Controls.Add(BuildNavBarExpandedCard())

        _root.Controls.Add(SectionTitle("NexaNavigationBar — Compact Mode"))
        _root.Controls.Add(BuildNavBarCompactCard())

        ' Breadcrumb
        _root.Controls.Add(SectionTitle("NexaBreadcrumb — Path Navigation"))
        _root.Controls.Add(BuildBreadcrumbCard())

        ' Stepper
        _root.Controls.Add(SectionTitle("NexaStepper — Horizontal Workflow"))
        _root.Controls.Add(BuildStepperHorizontalCard())

        _root.Controls.Add(SectionTitle("NexaStepper — Vertical Workflow"))
        _root.Controls.Add(BuildStepperVerticalCard())

        ' Application Shell
        _root.Controls.Add(SectionTitle("Application Shell Example"))
        Dim shellCard = BuildApplicationShellCard()
        _root.Controls.Add(shellCard)
        _root.SetColumnSpan(shellCard, 2)

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

    ' ---------- Tab Control Cards ----------

    Private Function BuildTabStyleCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Three visual styles: Default (underline), Underline (thin line), Pill (filled background)."))

        Dim grid = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 3,
            .RowCount = 1
        }
        For i = 0 To 2
            grid.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
        Next
        grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))

        grid.Controls.Add(BuildTabStyleDemo(NexaTabStyle.Default, "Default"), 0, 0)
        grid.Controls.Add(BuildTabStyleDemo(NexaTabStyle.Underline, "Underline"), 1, 0)
        grid.Controls.Add(BuildTabStyleDemo(NexaTabStyle.Pill, "Pill"), 2, 0)

        card.Controls.Add(grid)
        Return card
    End Function

    Private Function BuildTabStyleDemo(style As NexaTabStyle, label As String) As Control
        Dim panel = New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 0, 12, 0),
            .Padding = New Padding(16)
        }
        AddHandler panel.Paint, Sub(s, e)
                                    Using pen = New Pen(CType(ThemeManager.Current.Palette(NexaColorRole.Border).Value, Color), 1.0F)
                                        Dim r = panel.ClientRectangle
                                        r.Width -= 1
                                        r.Height -= 1
                                        e.Graphics.DrawRectangle(pen, r)
                                    End Using
                                End Sub

        Dim tabControl = New NexaTabControl With {
            .TabStyle = style,
            .Width = 300,
            .Height = 180,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 8, 0, 0)
        }
        tabControl.TabPages.Add(New TabPage With {.Text = "Overview"})
        tabControl.TabPages.Add(New TabPage With {.Text = "Details"})
        tabControl.TabPages.Add(New TabPage With {.Text = "Settings"})
        tabControl.TabPages.Add(New TabPage With {.Text = "Advanced"})

        Dim titleLabel = New NexaLabel With {
            .Text = label,
            .LabelStyle = NexaLabelStyle.Subheading,
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 0, 0, 8)
        }

        panel.Controls.Add(tabControl)
        panel.Controls.Add(titleLabel)
        Return panel
    End Function

    Private Function BuildTabPageCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("NexaTabPage supports icons and badges on individual tabs."))

        Dim tabControl = New NexaTabControl With {
            .TabStyle = NexaTabStyle.Default,
            .Width = 400,
            .Height = 200,
            .Dock = DockStyle.Top
        }

        Dim page1 = New NexaTabPage With {.Text = "Inbox", .IconKind = NexaIconKind.Check, .BadgeText = "5", .BadgeVisible = True}
        page1.Controls.Add(New NexaLabel With {.Text = "Inbox content here", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(16)})

        Dim page2 = New NexaTabPage With {.Text = "Drafts", .IconKind = NexaIconKind.Check, .BadgeText = "2", .BadgeVisible = True}
        page2.Controls.Add(New NexaLabel With {.Text = "Drafts content here", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(16)})

        Dim page3 = New NexaTabPage With {.Text = "Sent", .IconKind = NexaIconKind.Cross}
        page3.Controls.Add(New NexaLabel With {.Text = "Sent items content", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(16)})

        Dim page4 = New NexaTabPage With {.Text = "Archived", .BadgeText = "99+", .BadgeVisible = True}
        page4.Controls.Add(New NexaLabel With {.Text = "Archived items", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(16)})

        tabControl.TabPages.AddRange(New TabPage() {page1, page2, page3, page4})

        card.Controls.Add(tabControl)
        Return card
    End Function

    Private Function BuildInteractiveTabCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("TabControl with dynamic tab management and selection events."))

        Dim tabControl = New NexaTabControl With {
            .TabStyle = NexaTabStyle.Underline,
            .Width = 400,
            .Height = 160,
            .Dock = DockStyle.Top
        }
        tabControl.TabPages.Add(New TabPage With {.Text = "Tab 1"})
        tabControl.TabPages.Add(New TabPage With {.Text = "Tab 2"})
        tabControl.TabPages.Add(New TabPage With {.Text = "Tab 3"})

        Dim status = New NexaLabel With {
            .Text = "Selected: Tab 1 (Index 0)",
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 8, 0, 8)
        }

        AddHandler tabControl.SelectedIndexChanged, Sub()
                                                        status.Text = $"Selected: {tabControl.SelectedTab?.Text} (Index {tabControl.SelectedIndex})"
                                                    End Sub

        Dim btnRow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True,
            .Margin = New Padding(0, 0, 0, 8)
        }
        Dim btnAdd = New NexaButton With {.Text = "Add Tab", .Style = NexaButtonStyle.Primary, .Width = 100, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnRemove = New NexaButton With {.Text = "Remove Tab", .Style = NexaButtonStyle.Danger, .Width = 110, .Margin = New Padding(0, 0, 8, 0)}
        Dim count As Integer = 3
        AddHandler btnAdd.Click, Sub()
                                     count += 1
                                     Dim newTab = New TabPage With {.Text = $"Tab {count}"}
                                     newTab.Controls.Add(New NexaLabel With {.Text = $"Content of Tab {count}", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(16)})
                                     tabControl.TabPages.Add(newTab)
                                 End Sub
        AddHandler btnRemove.Click, Sub()
                                        If tabControl.TabCount > 1 Then
                                            tabControl.TabPages.RemoveAt(tabControl.TabCount - 1)
                                        End If
                                    End Sub
        btnRow.Controls.Add(btnAdd)
        btnRow.Controls.Add(btnRemove)

        card.Controls.Add(tabControl)
        card.Controls.Add(status)
        card.Controls.Add(btnRow)
        Return card
    End Function

    ' ---------- Navigation Bar Cards ----------

    Private Function BuildNavBarExpandedCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Expanded mode shows icons and text labels. Supports selection, badges, and disabled items."))

        Dim navBar = New NexaNavigationBar With {
            .Mode = NexaNavigationMode.Expanded,
            .Width = 260,
            .Height = 320,
            .Dock = DockStyle.Top,
            .ItemHeight = 40,
            .Indent = 16,
            .IconSize = 20
        }
        navBar.Items.Add(New NexaNavigationItem With {.Key = "dashboard", .Text = "Dashboard", .IconKind = NexaIconKind.User})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "projects", .Text = "Projects", .IconKind = NexaIconKind.User, .BadgeText = "3", .BadgeVisible = True})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "students", .Text = "Students", .IconKind = NexaIconKind.User, .BadgeText = "12", .BadgeVisible = True})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "reports", .Text = "Reports", .IconKind = NexaIconKind.Info})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "settings", .Text = "Settings", .IconKind = NexaIconKind.Settings})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "help", .Text = "Help", .IconKind = NexaIconKind.Info})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "disabled", .Text = "Disabled", .IconKind = NexaIconKind.Settings, .Enabled = False})
        navBar.SelectedIndex = 0

        Dim status = New NexaLabel With {
            .Text = "Selected: Dashboard",
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 8, 0, 0)
        }

        AddHandler navBar.SelectedItemChanged, Sub(s, e) status.Text = $"Selected: {e.Item.Text}"

        card.Controls.Add(navBar)
        card.Controls.Add(status)
        Return card
    End Function

    Private Function BuildNavBarCompactCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Compact mode shows only icons. Click to expand or use ToggleMode()."))

        Dim navBar = New NexaNavigationBar With {
            .Mode = NexaNavigationMode.Compact,
            .Width = 60,
            .Height = 320,
            .Dock = DockStyle.Top,
            .ItemHeight = 40,
            .Indent = 10,
            .IconSize = 24
        }
        navBar.Items.Add(New NexaNavigationItem With {.Key = "dashboard", .Text = "Dashboard", .IconKind = NexaIconKind.User})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "projects", .Text = "Projects", .IconKind = NexaIconKind.User, .BadgeText = "3", .BadgeVisible = True})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "students", .Text = "Students", .IconKind = NexaIconKind.User, .BadgeText = "12", .BadgeVisible = True})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "reports", .Text = "Reports", .IconKind = NexaIconKind.Info})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "settings", .Text = "Settings", .IconKind = NexaIconKind.Settings})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "help", .Text = "Help", .IconKind = NexaIconKind.Info})
        navBar.SelectedIndex = 0

        Dim toggle = New NexaButton With {
            .Text = "Toggle Expanded",
            .Style = NexaButtonStyle.Primary,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 8, 0, 0)
        }
        AddHandler toggle.Click, Sub() navBar.ToggleMode()

        Dim status = New NexaLabel With {
            .Text = "Mode: Compact",
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 8, 0, 0)
        }

        AddHandler navBar.SelectedItemChanged, Sub(s, e)
                                                   If navBar.SelectedItem IsNot Nothing Then
                                                       status.Text = $"Selected: {navBar.SelectedItem.Text} | Mode: {navBar.Mode}"
                                                   End If
                                               End Sub

        card.Controls.Add(navBar)
        card.Controls.Add(toggle)
        card.Controls.Add(status)
        Return card
    End Function

    ' ---------- Breadcrumb Card ----------

    Private Function BuildBreadcrumbCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Hierarchical path navigation with customizable separators and clickable items."))

        Dim breadcrumb = New NexaBreadcrumb With {
            .Dock = DockStyle.Top,
            .Height = 44,
            .PaddingDips = 16,
            .ItemSpacing = 8,
            .Separator = NexaBreadcrumbSeparator.Chevron
        }
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "home", .Text = "Home", .Enabled = True})
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "education", .Text = "Education", .Enabled = True})
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "courses", .Text = "Courses", .Enabled = True})
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "ict", .Text = "ICT", .Enabled = True})
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "students", .Text = "Students", .Enabled = True})

        Dim status = New NexaLabel With {
            .Text = "Click a breadcrumb item...",
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Dock = DockStyle.Top,
            .Margin = New Padding(0, 8, 0, 0)
        }

        AddHandler breadcrumb.ItemClick, Sub(s, e) status.Text = $"Clicked: {e.Item.Text} (Key: {e.Item.Key})"

        ' Separator options demo
        Dim sepRow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True,
            .Margin = New Padding(0, 16, 0, 0)
        }
        Dim separators = {
            ("Chevron", NexaBreadcrumbSeparator.Chevron),
            ("Slash", NexaBreadcrumbSeparator.Slash),
            ("Greater Than", NexaBreadcrumbSeparator.GreaterThan),
            ("Custom", NexaBreadcrumbSeparator.Custom)
        }
        For Each pair In separators
            Dim name = pair.Item1
            Dim sep = pair.Item2
            Dim btn = New NexaButton With {.Text = name, .Width = 100, .Margin = New Padding(0, 0, 8, 0)}
            Dim s = sep
            AddHandler btn.Click, Sub()
                                      Dim demo = New NexaBreadcrumb With {
                                          .Dock = DockStyle.Top,
                                          .Height = 36,
                                          .Separator = s
                                      }
                                      demo.Items.Add(New NexaBreadcrumbItem With {.Key = "a", .Text = "Home"})
                                      demo.Items.Add(New NexaBreadcrumbItem With {.Key = "b", .Text = "Section"})
                                      demo.Items.Add(New NexaBreadcrumbItem With {.Key = "c", .Text = "Page"})
                                      If s = NexaBreadcrumbSeparator.Custom Then demo.CustomSeparatorText = "|"
                                      Dim popup = New Form With {.Text = $"Separator: {name}", .Width = 400, .Height = 120, .StartPosition = FormStartPosition.CenterParent}
                                      popup.Controls.Add(demo)
                                      popup.ShowDialog()
                                  End Sub
            sepRow.Controls.Add(btn)
        Next

        card.Controls.Add(breadcrumb)
        card.Controls.Add(status)
        card.Controls.Add(sepRow)
        Return card
    End Function

    ' ---------- Stepper Cards ----------

    Private Function BuildStepperHorizontalCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Horizontal stepper with step states (Pending, Current, Completed, Error) and navigation."))

        Dim stepper = New NexaStepper With {
            .Orientation = NexaOrientation.Horizontal,
            .Dock = DockStyle.Top,
            .Height = 120,
            .CircleDiameter = 36,
            .ItemSpacing = 60,
            .ShowDescriptions = True,
            .ShowStepNumbers = True,
            .AllowNavigation = True
        }
        stepper.Steps.Add(New NexaStep With {.Key = "account", .Title = "Account", .Description = "Create your account", .State = NexaStepState.Completed})
        stepper.Steps.Add(New NexaStep With {.Key = "profile", .Title = "Profile", .Description = "Set up your profile", .State = NexaStepState.Current})
        stepper.Steps.Add(New NexaStep With {.Key = "confirm", .Title = "Confirm", .Description = "Verify your details", .State = NexaStepState.Pending})
        stepper.Steps.Add(New NexaStep With {.Key = "complete", .Title = "Complete", .Description = "All done!", .State = NexaStepState.Pending})

        Dim btnRow = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = True,
            .Margin = New Padding(0, 8, 0, 0)
        }
        Dim btnPrev = New NexaButton With {.Text = "Previous", .Style = NexaButtonStyle.Outline, .Width = 100, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnNext = New NexaButton With {.Text = "Next", .Style = NexaButtonStyle.Primary, .Width = 100, .Margin = New Padding(0, 0, 8, 0)}
        Dim btnReset = New NexaButton With {.Text = "Reset", .Style = NexaButtonStyle.Outline, .Width = 100}
        AddHandler btnPrev.Click, Sub() stepper.Previous()
        AddHandler btnNext.Click, Sub() stepper.Next()
        AddHandler btnReset.Click, Sub() stepper.Reset()
        btnRow.Controls.AddRange(New Control() {btnPrev, btnNext, btnReset})

        card.Controls.Add(stepper)
        card.Controls.Add(btnRow)
        Return card
    End Function

    Private Function BuildStepperVerticalCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Vertical stepper showing different step states including Error and Disabled."))

        Dim stepper = New NexaStepper With {
            .Orientation = NexaOrientation.Vertical,
            .Dock = DockStyle.Top,
            .Height = 300,
            .CircleDiameter = 32,
            .ItemSpacing = 24,
            .ShowDescriptions = True,
            .ShowStepNumbers = True,
            .AllowNavigation = False
        }
        stepper.Steps.Add(New NexaStep With {.Key = "step1", .Title = "Account Setup", .Description = "Enter your account information", .State = NexaStepState.Completed})
        stepper.Steps.Add(New NexaStep With {.Key = "step2", .Title = "Profile", .Description = "Add profile details", .State = NexaStepState.Completed})
        stepper.Steps.Add(New NexaStep With {.Key = "step3", .Title = "Verification", .Description = "Verify email address", .State = NexaStepState.Error})
        stepper.Steps.Add(New NexaStep With {.Key = "step4", .Title = "Preferences", .Description = "Set your preferences", .State = NexaStepState.Pending, .Enabled = False})
        stepper.Steps.Add(New NexaStep With {.Key = "step5", .Title = "Complete", .Description = "Finish setup", .State = NexaStepState.Pending})

        card.Controls.Add(stepper)
        Return card
    End Function

    ' ---------- Application Shell ----------

    Private Function BuildApplicationShellCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Complete application shell combining Navigation Bar, Breadcrumb, TabControl, and content area."))

        Dim split = New SplitContainer With {
            .Dock = DockStyle.Fill,
            .Orientation = Orientation.Vertical,
            .SplitterDistance = 500,
            .FixedPanel = FixedPanel.Panel1,
            .Panel1MinSize = 300
        }

        ' Left: Navigation Bar
        Dim navBar = New NexaNavigationBar With {
            .Mode = NexaNavigationMode.Expanded,
            .Dock = DockStyle.Fill,
            .ItemHeight = 44,
            .Indent = 16,
            .IconSize = 22
        }
        navBar.Items.Add(New NexaNavigationItem With {.Key = "dashboard", .Text = "Dashboard", .IconKind = NexaIconKind.User})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "students", .Text = "Students", .IconKind = NexaIconKind.User, .BadgeText = "24", .BadgeVisible = True})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "courses", .Text = "Courses", .IconKind = NexaIconKind.User, .BadgeText = "8", .BadgeVisible = True})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "assessments", .Text = "Assessments", .IconKind = NexaIconKind.Info})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "reports", .Text = "Reports", .IconKind = NexaIconKind.Info})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "settings", .Text = "Settings", .IconKind = NexaIconKind.Settings})
        navBar.Items.Add(New NexaNavigationItem With {.Key = "help", .Text = "Help", .IconKind = NexaIconKind.Info})
        navBar.SelectedIndex = 0

        ' Content area with breadcrumb + tab control
        Dim contentPanel = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2,
            .Padding = New Padding(24, 16, 24, 24)
        }
        contentPanel.RowStyles.Add(New RowStyle(SizeType.Absolute, 44.0F))
        contentPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        Dim breadcrumb = New NexaBreadcrumb With {
            .Dock = DockStyle.Fill,
            .Height = 40,
            .PaddingDips = 0,
            .ItemSpacing = 8,
            .Separator = NexaBreadcrumbSeparator.Chevron
        }
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "home", .Text = "Home", .Enabled = True})
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "students", .Text = "Students", .Enabled = True})
        breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "batch2026", .Text = "Batch 2026", .Enabled = True})

        Dim tabControl = New NexaTabControl With {
            .TabStyle = NexaTabStyle.Default,
            .Dock = DockStyle.Fill
        }
        Dim tabStudents = New TabPage With {.Text = "Students List"}
        tabStudents.Controls.Add(New NexaLabel With {.Text = "Students list content would go here", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(24)})
        Dim tabDetails = New TabPage With {.Text = "Student Details"}
        tabDetails.Controls.Add(New NexaLabel With {.Text = "Student detail view", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(24)})
        Dim tabEnroll = New TabPage With {.Text = "Enrollment"}
        tabEnroll.Controls.Add(New NexaLabel With {.Text = "Enrollment management", .LabelStyle = NexaLabelStyle.Muted, .AutoSize = True, .Dock = DockStyle.Top, .Margin = New Padding(24)})
        tabControl.TabPages.AddRange(New TabPage() {tabStudents, tabDetails, tabEnroll})

        contentPanel.Controls.Add(breadcrumb, 0, 0)
        contentPanel.Controls.Add(tabControl, 0, 1)

        split.Panel1.Controls.Add(navBar)
        split.Panel2.Controls.Add(contentPanel)

        Dim statusLabel = New NexaLabel With {
            .Text = "Select a navigation item to change the view",
            .LabelStyle = NexaLabelStyle.Caption,
            .AutoSize = True,
            .Dock = DockStyle.Bottom,
            .Margin = New Padding(0, 8, 0, 0)
        }

        AddHandler navBar.SelectedItemChanged, Sub(s, e)
                                                   Dim key = If(e.Item?.Key, "")
                                                   breadcrumb.Items.Clear()
                                                   breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = "home", .Text = "Home", .Enabled = True})
                                                   breadcrumb.Items.Add(New NexaBreadcrumbItem With {.Key = key, .Text = If(e.Item?.Text, ""), .Enabled = True})
                                                   statusLabel.Text = $"Viewing: {e.Item?.Text} (Key: {key})"
                                               End Sub

        Dim outerPanel = New Panel With {
            .Dock = DockStyle.Fill,
            .Padding = New Padding(8)
        }
        outerPanel.Controls.Add(split)
        outerPanel.Controls.Add(statusLabel)

        ' Make the card larger for this demo
        card.Height = 600
        card.Controls.Add(outerPanel)

        Return card
    End Function

    ' ---------- Helpers ----------

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
        Return New NexaLabel With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .LabelStyle = NexaLabelStyle.Muted,
            .Margin = New Padding(0, 0, 0, 8)
        }
    End Function
End Class
