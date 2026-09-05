Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GalleryBasicControlsScreen
    Inherits UserControl

    Private _root As TableLayoutPanel
    Private _status As Label

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

        Dim header = New Label With {.Text = "Basic Controls", .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)}
        Dim intro = New Label With {.Text = "NexaButton — styles, sizes, icons, loading, and disabled states.", .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 16)}

        _root.Controls.Add(header)
        _root.Controls.Add(intro)
        _root.Controls.Add(SectionTitle("Styles"))
        _root.Controls.Add(BuildStylesGrid())
        _root.Controls.Add(SectionTitle("Sizes"))
        _root.Controls.Add(BuildSizesGrid())
        _root.Controls.Add(SectionTitle("Icons & loading"))
        _root.Controls.Add(BuildIconAndLoadingGrid())
        _root.Controls.Add(SectionTitle("Disabled"))
        _root.Controls.Add(BuildDisabledGrid())

        _status = New Label With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 16, 0, 0),
            .Text = "Click any button to see the click handler in action."
        }
        _root.Controls.Add(_status)

        Controls.Add(_root)

        AddHandler ThemeManager.ThemeChanged, Sub(s, e) ApplyTheme(e.Current)
        AddHandler Disposed, Sub() RemoveHandler ThemeManager.ThemeChanged, AddressOf OnThemeChangedStatic
        AddHandler ThemeManager.ThemeChanged, AddressOf OnThemeChangedStatic

        AddHandler HandleCreated, Sub() ApplyTheme(ThemeManager.Current)
    End Sub

    Private Sub OnThemeChangedStatic(sender As Object, e As ThemeChangedEventArgs)
    End Sub

    Private Shared Function SectionTitle(title As String) As Label
        Return New Label With {.Text = title, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 16, 0, 8)}
    End Function

    Private Sub ApplyTheme(theme As ITheme)
        If InvokeRequired Then
            BeginInvoke(Sub() ApplyTheme(theme))
            Return
        End If

        Dim palette = theme.Palette
        Dim typography = theme.Typography
        Dim dpi = NexaFormsDpi.CurrentDpi(Me)

        BackColor = CType(palette(NexaColorRole.Background).Value, Color)
        ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)

        If _root.Controls.Count > 0 AndAlso TypeOf _root.Controls(0) Is Label Then
            Dim l = DirectCast(_root.Controls(0), Label)
            l.Font = typography.ToFont(NexaTypographyRole.Heading, dpi)
            l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If
        If _root.Controls.Count > 1 AndAlso TypeOf _root.Controls(1) Is Label Then
            Dim l = DirectCast(_root.Controls(1), Label)
            l.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            l.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
        End If

        Dim sectionTitles = New String() {"Styles", "Sizes", "Icons & loading", "Disabled"}
        For Each c As Control In _root.Controls
            If TypeOf c Is Label Then
                Dim l = DirectCast(c, Label)
                If Array.IndexOf(sectionTitles, l.Text) >= 0 Then
                    l.Font = typography.ToFont(NexaTypographyRole.Title, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
                End If
            End If
        Next

        _status.Font = typography.ToFont(NexaTypographyRole.Caption, dpi)
        _status.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
    End Sub

    Private Function BuildStylesGrid() As Control
        Dim wrap = New TableLayoutPanel With {
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 4,
            .Padding = New Padding(0)
        }
        For i = 0 To 3
            wrap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 25.0F))
        Next

        Dim styles = New NexaButtonStyle() {
            NexaButtonStyle.Primary, NexaButtonStyle.Secondary, NexaButtonStyle.Outline, NexaButtonStyle.Ghost,
            NexaButtonStyle.Success, NexaButtonStyle.Warning, NexaButtonStyle.Danger
        }

        Dim row = 0
        For i = 0 To styles.Length - 1 Step 4
            wrap.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            wrap.RowCount += 1
            For c = 0 To 3
                If i + c < styles.Length Then
                    Dim style = styles(i + c)
                    Dim btn = New NexaButton With {
                        .Text = style.ToString(),
                        .Style = style,
                        .Dock = DockStyle.Fill,
                        .Margin = New Padding(4),
                        .AutoSize = True,
                        .AutoSizeMode = AutoSizeMode.GrowAndShrink
                    }
                    AddHandler btn.Click, Sub() NotifyClick(style.ToString())
                    wrap.Controls.Add(btn, c, row)
                End If
            Next
            row += 1
        Next
        Return wrap
    End Function

    Private Function BuildSizesGrid() As Control
        Dim wrap = New FlowLayoutPanel With {
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }

        Dim small = New NexaButton With {.Text = "Small", .SizeMode = NexaButtonSize.Small, .AutoSize = True}
        Dim medium = New NexaButton With {.Text = "Medium", .SizeMode = NexaButtonSize.Medium, .AutoSize = True}
        Dim large = New NexaButton With {.Text = "Large", .SizeMode = NexaButtonSize.Large, .AutoSize = True}

        AddHandler small.Click, Sub() NotifyClick("Small")
        AddHandler medium.Click, Sub() NotifyClick("Medium")
        AddHandler large.Click, Sub() NotifyClick("Large")

        wrap.Controls.AddRange(New Control() {small, medium, large})
        Return wrap
    End Function

    Private Function BuildIconAndLoadingGrid() As Control
        Dim wrap = New TableLayoutPanel With {
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 3,
            .Padding = New Padding(0)
        }
        For i = 0 To 2
            wrap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 33.33F))
        Next

        Dim iconLeft = New NexaButton With {.Text = "Save", .IconKind = NexaIconKind.Check, .IconPosition = NexaButtonIconPosition.Left, .Style = NexaButtonStyle.Primary, .AutoSize = True, .Dock = DockStyle.Fill, .Margin = New Padding(4)}
        Dim iconRight = New NexaButton With {.Text = "Next", .IconKind = NexaIconKind.ChevronRight, .IconPosition = NexaButtonIconPosition.Right, .Style = NexaButtonStyle.Secondary, .AutoSize = True, .Dock = DockStyle.Fill, .Margin = New Padding(4)}
        Dim ghostSearch = New NexaButton With {.Text = "Search", .IconKind = NexaIconKind.Search, .Style = NexaButtonStyle.Ghost, .AutoSize = True, .Dock = DockStyle.Fill, .Margin = New Padding(4)}

        Dim loading = New NexaButton With {.Text = "Submit", .Style = NexaButtonStyle.Primary, .AutoSize = True, .Dock = DockStyle.Fill, .Margin = New Padding(4)}
        Dim loadingSecondary = New NexaButton With {.Text = "Process", .Style = NexaButtonStyle.Secondary, .AutoSize = True, .Dock = DockStyle.Fill, .Margin = New Padding(4)}
        Dim loadingWithIcon = New NexaButton With {.Text = "Sync", .IconKind = NexaIconKind.Settings, .Style = NexaButtonStyle.Outline, .AutoSize = True, .Dock = DockStyle.Fill, .Margin = New Padding(4)}

        AddHandler loading.Click, Sub() StartLoading(loading, "Submitting...", 1500)
        AddHandler loadingSecondary.Click, Sub() StartLoading(loadingSecondary, "Processing...", 1200)
        AddHandler loadingWithIcon.Click, Sub() StartLoading(loadingWithIcon, "Syncing...", 1400)

        AddHandler iconLeft.Click, Sub() NotifyClick("Save")
        AddHandler iconRight.Click, Sub() NotifyClick("Next")
        AddHandler ghostSearch.Click, Sub() NotifyClick("Search")

        wrap.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        wrap.RowCount += 1
        wrap.Controls.Add(iconLeft, 0, 0)
        wrap.Controls.Add(iconRight, 1, 0)
        wrap.Controls.Add(ghostSearch, 2, 0)

        wrap.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        wrap.RowCount += 1
        wrap.Controls.Add(loading, 0, 1)
        wrap.Controls.Add(loadingSecondary, 1, 1)
        wrap.Controls.Add(loadingWithIcon, 2, 1)

        Return wrap
    End Function

    Private Shared Sub StartLoading(btn As NexaButton, loadingText As String, durationMs As Integer)
        btn.Loading = True
        btn.LoadingText = loadingText
        Dim captured = btn
        Dim t = New Timer With {.Interval = durationMs}
        AddHandler t.Tick, Sub(s, e)
                                t.Stop()
                                t.Dispose()
                                captured.Loading = False
                            End Sub
        t.Start()
    End Sub

    Private Function BuildDisabledGrid() As Control
        Dim wrap = New FlowLayoutPanel With {
            .FlowDirection = FlowDirection.LeftToRight,
            .WrapContents = False,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }
        Dim styles = New NexaButtonStyle() {NexaButtonStyle.Primary, NexaButtonStyle.Outline, NexaButtonStyle.Ghost, NexaButtonStyle.Danger}
        For Each style In styles
            Dim btn = New NexaButton With {
                .Text = $"{style} (disabled)",
                .Style = style,
                .Enabled = False,
                .AutoSize = True,
                .Margin = New Padding(4)
            }
            wrap.Controls.Add(btn)
        Next
        Return wrap
    End Function

    Private Sub NotifyClick(caption As String)
        _status.Text = $"Clicked: {caption} at {DateTime.Now:HH:mm:ss}"
    End Sub

End Class