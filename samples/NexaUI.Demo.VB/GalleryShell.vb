Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GalleryShell
    Inherits System.Windows.Forms.Form

    Public Shared Event NavigationRequested(sender As Object, key As String)

    Private Shared ReadOnly Pages As (Key As String, Title As String, Factory As Func(Of UserControl))() = {
        ("getting-started", "Getting Started", Function() New GettingStartedScreen()),
        ("themes", "Themes", Function() New GalleryThemesScreen()),
        ("basic", "Basic Controls", Function() New GalleryBasicControlsScreen()),
        ("input", "Input Controls", Function() New GalleryInputControlsScreen())
    }

    Private Shared ReadOnly NavItems As (Key As String, Title As String)() = {
        ("getting-started", "Getting Started"),
        ("themes", "Themes"),
        ("basic", "Basic Controls"),
        ("input", "Input Controls"),
        ("selection", "Selection Controls"),
        ("layout", "Layout Controls"),
        ("feedback", "Feedback Controls"),
        ("navigation", "Navigation Controls"),
        ("datagrid", "DataGrid"),
        ("dialogs", "Dialogs"),
        ("advanced", "Advanced")
    }

    Private _root As TableLayoutPanel
    Private _topBar As Panel
    Private _brandLabel As Label
    Private _searchHint As Label
    Private _topBarLayout As TableLayoutPanel
    Private _themeToggle As NexaButton
    Private _split As SplitContainer
    Private _navPanel As TableLayoutPanel
    Private _navButtons As New Dictionary(Of String, NexaButton)()
    Private _contentPanel As TableLayoutPanel
    Private _contentCache As New Dictionary(Of String, UserControl)()
    Private _currentKey As String = "getting-started"
    Private _topBarBorderAttached As Boolean

    Public Sub New()
        Text = "NexaUI Gallery — VB.NET"
        StartPosition = FormStartPosition.CenterScreen
        ClientSize = New Size(1280, 820)
        MinimumSize = New Size(960, 640)

        _root = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 1,
            .RowCount = 2
        }
        _root.RowStyles.Add(New RowStyle(SizeType.Absolute, 56.0F))
        _root.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))

        _topBar = New Panel With {.Dock = DockStyle.Fill}
        _topBarLayout = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .ColumnCount = 3,
            .Padding = New Padding(20, 0, 20, 0)
        }
        _topBarLayout.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        _topBarLayout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        _topBarLayout.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))

        _brandLabel = New Label With {
            .Text = "NexaUI",
            .AutoSize = True,
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = New Font("Segoe UI", 12.0F, FontStyle.Bold, GraphicsUnit.Point)
        }
        _searchHint = New Label With {
            .Text = "Search components…",
            .AutoSize = False,
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Padding = New Padding(14, 0, 0, 0),
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Italic, GraphicsUnit.Point)
        }
        _themeToggle = New NexaButton With {
            .Text = "Toggle Theme",
            .IconKind = NexaIconKind.Sun,
            .Style = NexaButtonStyle.Outline,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }
        AddHandler _themeToggle.Click, Sub() ThemeManager.ToggleLightDark()

        _topBarLayout.Controls.Add(_brandLabel, 0, 0)
        _topBarLayout.Controls.Add(_searchHint, 1, 0)
        _topBarLayout.Controls.Add(_themeToggle, 2, 0)
        _topBar.Controls.Add(_topBarLayout)

        _split = New SplitContainer With {
            .Dock = DockStyle.Fill,
            .Orientation = Orientation.Vertical,
            .SplitterDistance = 240,
            .FixedPanel = FixedPanel.Panel1,
            .Panel1MinSize = 180
        }
        _navPanel = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 1,
            .Padding = New Padding(8, 12, 8, 12)
        }
        _navPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        Dim available As New HashSet(Of String)()
        For Each p In Pages
            available.Add(p.Key)
        Next

        For Each item In NavItems
            Dim btn = New NexaButton With {
                .Text = item.Title,
                .Style = NexaButtonStyle.Ghost,
                .Dock = DockStyle.Top,
                .Margin = New Padding(0, 0, 0, 2),
                .Height = 32,
                .TextAlign = ContentAlignment.MiddleLeft,
                .Padding = New Padding(12, 0, 12, 0)
            }
            If Not available.Contains(item.Key) Then
                btn.Enabled = False
                btn.Text = $"{item.Title}  (coming soon)"
            Else
                Dim capturedKey = item.Key
                AddHandler btn.Click, Sub() NavigateTo(capturedKey)
            End If
            _navButtons(item.Key) = btn
            _navPanel.Controls.Add(btn)
        Next

        _split.Panel1.Controls.Add(_navPanel)

        _contentPanel = New TableLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoScroll = True,
            .ColumnCount = 1,
            .RowCount = 1
        }
        _contentPanel.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        _contentPanel.RowStyles.Add(New RowStyle(SizeType.Percent, 100.0F))
        _split.Panel2.Controls.Add(_contentPanel)

        _root.Controls.Add(_topBar, 0, 0)
        _root.Controls.Add(_split, 0, 1)
        Controls.Add(_root)

        AddHandler NavigationRequested, AddressOf HandleNavigationRequested
        AddHandler ThemeManager.ThemeChanged, Sub(s, e) ApplyTheme(e.Current)

        AddHandler HandleCreated, Sub()
                                       ApplyTheme(ThemeManager.Current)
                                       NavigateTo(_currentKey)
                                   End Sub
    End Sub

    Public Shared Sub RequestNavigate(key As String)
        RaiseEvent NavigationRequested(Nothing, key)
    End Sub

    Private Sub HandleNavigationRequested(sender As Object, key As String)
        If InvokeRequired Then
            BeginInvoke(Sub() HandleNavigationRequested(sender, key))
            Return
        End If
        NavigateTo(key)
    End Sub

    Private Sub NavigateTo(key As String)
        Dim found As UserControl = Nothing
        For Each p In Pages
            If p.Key = key Then
                found = p.Factory()
                Exit For
            End If
        Next
        If found Is Nothing Then Return

        If Not _contentCache.ContainsKey(key) Then
            found.Dock = DockStyle.Fill
            _contentCache(key) = found
        End If

        _contentPanel.Controls.Clear()
        _contentPanel.Controls.Add(_contentCache(key), 0, 0)
        _currentKey = key
        UpdateActiveNavButton()
    End Sub

    Private Sub UpdateActiveNavButton()
        For Each kv In _navButtons
            kv.Value.Style = If(kv.Key = _currentKey, NexaButtonStyle.Primary, NexaButtonStyle.Ghost)
        Next
    End Sub

    Private Sub ApplyTheme(theme As ITheme)
        If InvokeRequired Then
            BeginInvoke(Sub() ApplyTheme(theme))
            Return
        End If

        Dim palette = theme.Palette
        Dim typography = theme.Typography
        Dim dpi = NexaFormsDpi.CurrentDpi(Me)

        BackColor = CType(palette(NexaColorRole.Background).Value, Color)

        _topBar.BackColor = CType(palette(NexaColorRole.Surface).Value, Color)
        If Not _topBarBorderAttached Then
            AddHandler _topBar.Paint, AddressOf TopBarBorderPainter
            _topBarBorderAttached = True
        End If

        _brandLabel.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        _brandLabel.Font = typography.ToFont(NexaTypographyRole.Title, dpi)
        _searchHint.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
        _searchHint.Font = typography.ToFont(NexaTypographyRole.Body, dpi)

        _themeToggle.IconKind = If(theme.IsDark, NexaIconKind.Sun, NexaIconKind.Moon)
        _themeToggle.Text = If(theme.IsDark, "Light", "Dark")

        _split.BackColor = CType(palette(NexaColorRole.Border).Value, Color)
        _split.Panel1.BackColor = CType(palette(NexaColorRole.Surface).Value, Color)
        _split.Panel2.BackColor = CType(palette(NexaColorRole.Background).Value, Color)

        _navPanel.BackColor = CType(palette(NexaColorRole.Surface).Value, Color)

        UpdateActiveNavButton()
    End Sub

    Private Sub TopBarBorderPainter(sender As Object, e As PaintEventArgs)
        Dim palette = ThemeManager.Current.Palette
        Using pen = New Pen(CType(palette(NexaColorRole.Border).Value, Color), 1.0F)
            e.Graphics.DrawLine(pen, 0, _topBar.Height - 1, _topBar.Width, _topBar.Height - 1)
        End Using
    End Sub

End Class