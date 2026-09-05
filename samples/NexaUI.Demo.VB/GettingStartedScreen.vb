Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GettingStartedScreen
    Inherits UserControl

    Public Sub New()
        Dock = DockStyle.Fill
        AutoScroll = True

        Dim root = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 1,
            .Padding = New Padding(32, 28, 32, 28)
        }
        root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        Dim title = New Label With {
            .Text = "Welcome to NexaUI",
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 8)
        }
        Dim subtitle = New Label With {
            .Text = "A modern Windows Forms component library for .NET 10.",
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 24)
        }
        Dim body = New Label With {
            .Text = "NexaUI ships a coherent design system — colors, typography, spacing, " &
                    "corner radii, borders, and elevation — exposed through the ThemeManager. " &
                    "Use the navigation on the left to explore available pages." & Environment.NewLine & Environment.NewLine &
                    "This gallery is itself a NexaUI consumer: every page is rendered with " &
                    "themed controls, including the navigation and the theme toggle at the top right.",
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .MaximumSize = New Size(900, 0),
            .Margin = New Padding(0, 0, 0, 24)
        }
        Dim actions = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 3,
            .Margin = New Padding(0, 0, 0, 16)
        }
        For i = 0 To 2
            actions.ColumnStyles.Add(New ColumnStyle(SizeType.AutoSize))
        Next

        Dim exploreBtn = New NexaButton With {
            .Text = "Explore Themes",
            .IconKind = NexaIconKind.Sun,
            .Style = NexaButtonStyle.Primary,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 0, 12, 0)
        }
        Dim buttonsBtn = New NexaButton With {
            .Text = "See Buttons",
            .IconKind = NexaIconKind.ChevronRight,
            .IconPosition = NexaButtonIconPosition.Right,
            .Style = NexaButtonStyle.Outline,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Margin = New Padding(0, 0, 12, 0)
        }
        Dim loadingBtn = New NexaButton With {
            .Text = "Try Loading",
            .Style = NexaButtonStyle.Secondary,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink
        }
        AddHandler loadingBtn.Click, Sub()
                                          loadingBtn.Loading = True
                                          Dim captured = loadingBtn
                                          Dim t = New Timer With {.Interval = 1500}
                                          AddHandler t.Tick, Sub(s, e)
                                                                  t.Stop()
                                                                  t.Dispose()
                                                                  captured.Loading = False
                                                              End Sub
                                          t.Start()
                                      End Sub

        AddHandler exploreBtn.Click, Sub() GalleryShell.RequestNavigate("themes")
        AddHandler buttonsBtn.Click, Sub() GalleryShell.RequestNavigate("basic")

        actions.Controls.Add(exploreBtn, 0, 0)
        actions.Controls.Add(buttonsBtn, 1, 0)
        actions.Controls.Add(loadingBtn, 2, 0)

        Dim footer = New Label With {
            .Text = "Target: net10.0-windows  ·  Languages: C# and VB.NET",
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 8, 0, 0)
        }

        root.Controls.Add(title)
        root.Controls.Add(subtitle)
        root.Controls.Add(body)
        root.Controls.Add(actions)
        root.Controls.Add(footer)

        Controls.Add(root)

        AddHandler ThemeManager.ThemeChanged, Sub(s, e) ApplyTheme(e.Current)
        AddHandler Disposed, Sub() RemoveHandler ThemeManager.ThemeChanged, AddressOf OnThemeChangedStatic
        AddHandler ThemeManager.ThemeChanged, AddressOf OnThemeChangedStatic

        AddHandler HandleCreated, Sub() ApplyTheme(ThemeManager.Current)
    End Sub

    Private Sub OnThemeChangedStatic(sender As Object, e As ThemeChangedEventArgs)
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
        ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)

        For Each c As Control In Controls
            If TypeOf c Is Label Then
                Dim l = DirectCast(c, Label)
                l.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
                l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
            End If
        Next

        If Controls(0) IsNot Nothing AndAlso TypeOf Controls(0) Is TableLayoutPanel Then
            Dim root = DirectCast(Controls(0), TableLayoutPanel)
            If root.Controls.Count >= 2 Then
                If TypeOf root.Controls(0) Is Label Then
                    Dim l = DirectCast(root.Controls(0), Label)
                    l.Font = typography.ToFont(NexaTypographyRole.Display, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
                End If
                If TypeOf root.Controls(1) Is Label Then
                    Dim l = DirectCast(root.Controls(1), Label)
                    l.Font = typography.ToFont(NexaTypographyRole.Title, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
                End If
                Dim last = root.Controls(root.Controls.Count - 1)
                If TypeOf last Is Label Then
                    Dim l = DirectCast(last, Label)
                    l.Font = typography.ToFont(NexaTypographyRole.Caption, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
                End If
            End If
        End If
    End Sub

End Class