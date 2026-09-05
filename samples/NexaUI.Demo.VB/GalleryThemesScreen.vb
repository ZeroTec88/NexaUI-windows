Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GalleryThemesScreen
    Inherits UserControl

    Private _root As TableLayoutPanel
    Private _themeSwitcher As FlowLayoutPanel
    Private _lightBtn As NexaButton
    Private _darkBtn As NexaButton
    Private _colorSection As TableLayoutPanel
    Private _stateSection As TableLayoutPanel
    Private _typographySection As TableLayoutPanel
    Private _spacingSection As TableLayoutPanel
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

        Dim header = New Label With {.Text = "Theme System", .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)}
        Dim intro = New Label With {.Text = "Semantic colors, control states, typography and spacing for the active theme.", .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 16)}

        _themeSwitcher = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .WrapContents = False,
            .FlowDirection = FlowDirection.LeftToRight,
            .Margin = New Padding(0, 0, 0, 24)
        }
        _lightBtn = New NexaButton With {.Text = "Light", .AutoSize = True, .Margin = New Padding(0, 0, 8, 0)}
        _darkBtn = New NexaButton With {.Text = "Dark", .AutoSize = True}
        AddHandler _lightBtn.Click, Sub() ThemeManager.SetTheme(New LightTheme())
        AddHandler _darkBtn.Click, Sub() ThemeManager.SetTheme(New DarkTheme())
        _themeSwitcher.Controls.Add(_lightBtn)
        _themeSwitcher.Controls.Add(_darkBtn)

        _colorSection = BuildSectionGrid()
        _stateSection = BuildSectionGrid()
        _typographySection = BuildSingleColumnSection()
        _spacingSection = BuildSingleColumnSection()

        _status = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 16, 0, 0)}

        _root.Controls.Add(header)
        _root.Controls.Add(intro)
        _root.Controls.Add(_themeSwitcher)
        _root.Controls.Add(SectionTitle("Semantic Colors"))
        _root.Controls.Add(_colorSection)
        _root.Controls.Add(SectionTitle("Control States"))
        _root.Controls.Add(_stateSection)
        _root.Controls.Add(SectionTitle("Typography"))
        _root.Controls.Add(_typographySection)
        _root.Controls.Add(SectionTitle("Spacing"))
        _root.Controls.Add(_spacingSection)
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
        Return New Label With {.Text = title, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 8, 0, 8)}
    End Function

    Private Shared Function BuildSectionGrid() As TableLayoutPanel
        Return New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 4,
            .Padding = New Padding(0)
        }
    End Function

    Private Shared Function BuildSingleColumnSection() As TableLayoutPanel
        Return New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 1,
            .Padding = New Padding(0)
        }
    End Function

    Private Sub ApplyTheme(theme As ITheme)
        If InvokeRequired Then
            BeginInvoke(Sub() ApplyTheme(theme))
            Return
        End If

        Dim palette = theme.Palette
        Dim typography = theme.Typography
        Dim spacing = theme.Spacing
        Dim dpi = NexaFormsDpi.CurrentDpi(Me)

        BackColor = CType(palette(NexaColorRole.Background).Value, Color)
        ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)

        ' Header + intro + section titles
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

        Dim sectionTitles = New String() {"Semantic Colors", "Control States", "Typography", "Spacing"}
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
        _status.Text = $"Active theme: {theme.Name} · DPI {dpi:F0}"

        _lightBtn.Style = If(theme.IsDark, NexaButtonStyle.Outline, NexaButtonStyle.Primary)
        _darkBtn.Style = If(theme.IsDark, NexaButtonStyle.Primary, NexaButtonStyle.Outline)

        PopulateColorGrid(_colorSection, palette)
        PopulateStateGrid(_stateSection, theme)
        PopulateTypographyGrid(_typographySection, typography, palette, dpi)
        PopulateSpacingGrid(_spacingSection, spacing, palette, dpi)
    End Sub

    Private Shared Sub PopulateColorGrid(grid As TableLayoutPanel, palette As NexaPalette)
        grid.Controls.Clear()
        grid.RowStyles.Clear()
        grid.RowCount = 0

        Dim roles = New NexaColorRole() {
            NexaColorRole.Primary, NexaColorRole.Secondary, NexaColorRole.Background, NexaColorRole.Surface,
            NexaColorRole.SurfaceVariant, NexaColorRole.Border, NexaColorRole.TextPrimary, NexaColorRole.TextSecondary,
            NexaColorRole.TextDisabled, NexaColorRole.Success, NexaColorRole.Warning, NexaColorRole.Danger,
            NexaColorRole.Info, NexaColorRole.Focus
        }

        Dim row = 0
        For i = 0 To roles.Length - 1 Step 4
            grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
            grid.RowCount += 1
            For c = 0 To 3
                If i + c < roles.Length Then
                    grid.Controls.Add(BuildSwatch(palette(roles(i + c))), c, row)
                End If
            Next
            row += 1
        Next
    End Sub

    Private Shared Function BuildSwatch(semantic As NexaSemanticColor) As Control
        Dim baseColor = CType(semantic.Value, Color)
        Dim subtle = CType(semantic.Subtle, Color)

        Dim panel = New Panel With {
            .Dock = DockStyle.Fill,
            .Height = 78,
            .Margin = New Padding(4),
            .Padding = New Padding(10),
            .BackColor = subtle
        }
        Dim roleLabel = New Label With {
            .Text = If(baseColor.IsNamedColor, baseColor.Name, ColorTranslator.ToHtml(baseColor)),
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .ForeColor = baseColor,
            .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point)
        }
        Dim hexLabel = New Label With {
            .Text = $"#{baseColor.R:X2}{baseColor.G:X2}{baseColor.B:X2}",
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .ForeColor = baseColor,
            .Font = New Font("Segoe UI", 8.0F, FontStyle.Regular, GraphicsUnit.Point)
        }
        panel.Controls.Add(hexLabel)
        panel.Controls.Add(roleLabel)
        Return panel
    End Function

    Private Shared Sub PopulateStateGrid(grid As TableLayoutPanel, theme As ITheme)
        grid.Controls.Clear()
        grid.RowStyles.Clear()
        grid.RowCount = 0

        Dim states = New NexaControlState() {
            NexaControlState.Normal, NexaControlState.Hover, NexaControlState.Pressed, NexaControlState.Focused,
            NexaControlState.Selected, NexaControlState.Disabled, NexaControlState.Error, NexaControlState.Success
        }

        grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        grid.RowCount += 1
        For c = 0 To 3
            Dim state = states(c)
            Dim colors = theme.ControlStates.Get(state)
            grid.Controls.Add(BuildStateChip(theme, state, colors), c, 0)
        Next

        grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        grid.RowCount += 1
        For c = 0 To 3
            Dim state = states(c + 4)
            Dim colors = theme.ControlStates.Get(state)
            grid.Controls.Add(BuildStateChip(theme, state, colors), c, 1)
        Next
    End Sub

    Private Shared Function BuildStateChip(theme As ITheme, state As NexaControlState, colors As NexaStateColors) As Control
        Dim chip As New Panel With {
            .Dock = DockStyle.Fill,
            .Height = 56,
            .Margin = New Padding(4),
            .Padding = New Padding(10)
        }
        If colors.Background <> Color.Empty Then
            chip.BackColor = colors.Background
        Else
            chip.BackColor = CType(theme.Palette(NexaColorRole.Surface).Value, Color)
        End If
        If colors.Foreground <> Color.Empty Then
            chip.ForeColor = colors.Foreground
        Else
            chip.ForeColor = CType(theme.Palette(NexaColorRole.TextPrimary).Value, Color)
        End If

        AddHandler chip.Paint, Sub(s, e)
                                    Dim bdColor = If(colors.Border <> Color.Empty, colors.Border, CType(theme.Palette(NexaColorRole.Border).Value, Color))
                                    Using borderPen = New Pen(bdColor, 1.0F)
                                        Dim rect = chip.ClientRectangle
                                        rect.Width -= 1
                                        rect.Height -= 1
                                        e.Graphics.DrawRectangle(borderPen, rect)
                                    End Using
                                End Sub

        Dim label = New Label With {
            .Text = state.ToString(),
            .Dock = DockStyle.Fill,
            .TextAlign = ContentAlignment.MiddleLeft,
            .Font = theme.Typography.ToFont(NexaTypographyRole.Label, NexaFormsDpi.CurrentDpi(chip)),
            .ForeColor = chip.ForeColor
        }
        chip.Controls.Add(label)
        Return chip
    End Function

    Private Shared Sub PopulateTypographyGrid(grid As TableLayoutPanel, typography As NexaTypography, palette As NexaPalette, dpi As Single)
        grid.Controls.Clear()
        grid.RowStyles.Clear()
        grid.RowCount = 0

        Dim roles = New NexaTypographyRole() {
            NexaTypographyRole.Display, NexaTypographyRole.Heading, NexaTypographyRole.Title,
            NexaTypographyRole.BodyStrong, NexaTypographyRole.Body, NexaTypographyRole.Label,
            NexaTypographyRole.Button, NexaTypographyRole.Caption
        }

        Dim card = New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(12),
            .Margin = New Padding(4)
        }
        AddHandler card.Paint, Sub(s, e)
                                   Using borderPen = New Pen(CType(palette(NexaColorRole.Border).Value, Color), 1.0F)
                                       Dim rect = card.ClientRectangle
                                       rect.Width -= 1
                                       rect.Height -= 1
                                       e.Graphics.DrawRectangle(borderPen, rect)
                                   End Using
                               End Sub
        For Each role In roles
            Dim style = typography.Get(role)
            Dim sample = New Label With {
                .Text = $"{role}  ·  {style.FamilyName} {style.SizeInDips}pt",
                .Dock = DockStyle.Top,
                .AutoSize = True,
                .Margin = New Padding(0, 4, 0, 4),
                .Font = style.ToFont(dpi),
                .ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
            }
            card.Controls.Add(sample)
        Next
        grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        grid.RowCount += 1
        grid.Controls.Add(card, 0, 0)
    End Sub

    Private Shared Sub PopulateSpacingGrid(grid As TableLayoutPanel, spacing As NexaSpacing, palette As NexaPalette, dpi As Single)
        grid.Controls.Clear()
        grid.RowStyles.Clear()
        grid.RowCount = 0

        Dim tokens = New NexaSpacingToken() {
            NexaSpacingToken.None, NexaSpacingToken.Xxs, NexaSpacingToken.Xs, NexaSpacingToken.Sm,
            NexaSpacingToken.Md, NexaSpacingToken.Lg, NexaSpacingToken.Xl, NexaSpacingToken.Xxl, NexaSpacingToken.Huge
        }

        Dim card = New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(12),
            .Margin = New Padding(4),
            .BackColor = CType(palette(NexaColorRole.Surface).Value, Color)
        }
        AddHandler card.Paint, Sub(s, e)
                                   Using borderPen = New Pen(CType(palette(NexaColorRole.Border).Value, Color), 1.0F)
                                       Dim rect = card.ClientRectangle
                                       rect.Width -= 1
                                       rect.Height -= 1
                                       e.Graphics.DrawRectangle(borderPen, rect)
                                   End Using
                               End Sub

        For Each token In tokens
            Dim row = New TableLayoutPanel With {
                .Dock = DockStyle.Top,
                .AutoSize = True,
                .AutoSizeMode = AutoSizeMode.GrowAndShrink,
                .ColumnCount = 3,
                .Margin = New Padding(0, 2, 0, 2)
            }
            row.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 80.0F))
            row.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 80.0F))
            row.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

            Dim tokenLabel = New Label With {
                .Text = token.ToString(),
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Bold, GraphicsUnit.Point),
                .ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color),
                .TextAlign = ContentAlignment.MiddleLeft
            }
            Dim valueLabel = New Label With {
                .Text = $"{spacing(token)} DIP",
                .Dock = DockStyle.Fill,
                .Font = New Font("Segoe UI", 9.0F, FontStyle.Regular, GraphicsUnit.Point),
                .ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color),
                .TextAlign = ContentAlignment.MiddleLeft
            }
            Dim bar = New Panel With {
                .Dock = DockStyle.Fill,
                .Height = 14,
                .BackColor = CType(palette(NexaColorRole.SurfaceVariant).Value, Color),
                .Margin = New Padding(0, 4, 0, 4),
                .Padding = New Padding(0)
            }
            Dim pixels = NexaDpi.Scale(spacing(token), dpi)
            Dim filled = New Panel With {
                .Dock = DockStyle.Left,
                .Width = Math.Min(220, Math.Max(2, pixels)),
                .BackColor = CType(palette(NexaColorRole.Primary).Value, Color)
            }
            bar.Controls.Add(filled)

            row.Controls.Add(tokenLabel, 0, 0)
            row.Controls.Add(valueLabel, 1, 0)
            row.Controls.Add(bar, 2, 0)
            card.Controls.Add(row)
        Next

        grid.RowStyles.Add(New RowStyle(SizeType.AutoSize))
        grid.RowCount += 1
        grid.Controls.Add(card, 0, 0)
    End Sub

End Class