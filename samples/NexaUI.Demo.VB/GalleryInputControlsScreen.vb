Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Themes

Public NotInheritable Class GalleryInputControlsScreen
    Inherits UserControl

    Private Shared ReadOnly SampleStudents As String() = {
        "Amal Perera", "Amal Silva", "Amal Fernando",
        "Nimal Jayasuriya", "Nimal Bandara",
        "Kamal Rathnayake", "Kamal Wijesinghe",
        "Sunil Perera", "Ruwan Dias", "Saman Kumara"
    }

    Private _root As TableLayoutPanel
    Private _status As Label
    Private _search As NexaSearchBox = Nothing
    Private _searchResults As ListBox = Nothing

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

        Dim header = New Label With {.Text = "Input Controls", .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)}
        Dim intro = New Label With {.Text = "NexaTextBox, NexaSearchBox, NexaMaskedTextBox — themed wrappers around native WinForms editors.", .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 0, 0, 16)}

        _root.Controls.Add(header)
        _root.Controls.Add(intro)
        _root.Controls.Add(SectionTitle("NexaTextBox — styles"))
        _root.Controls.Add(BuildTextBoxStylesCard())
        _root.Controls.Add(SectionTitle("NexaTextBox — placeholder, clear, helper, error, success"))
        _root.Controls.Add(BuildTextBoxStatesCard())
        _root.Controls.Add(SectionTitle("NexaTextBox — password and character counter"))
        _root.Controls.Add(BuildPasswordAndCounterCard())
        _root.Controls.Add(SectionTitle("NexaSearchBox — debounced search"))
        _root.Controls.Add(BuildSearchBoxCard())
        _root.Controls.Add(SectionTitle("NexaMaskedTextBox — phone, date, postal code"))
        _root.Controls.Add(BuildMaskedCard())

        _status = New Label With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 16, 0, 0),
            .Text = "Try the controls — they all respond to light/dark theme switching."
        }
        _root.Controls.Add(_status)

        Controls.Add(_root)

        AddHandler ThemeManager.ThemeChanged, Sub(s, e) ApplyTheme(e.Current)

        AddHandler HandleCreated, Sub() ApplyTheme(ThemeManager.Current)
    End Sub

    Private Shared Function SectionTitle(title As String) As Label
        Return New Label With {.Text = title, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 16, 0, 8)}
    End Function

    Private Sub ApplyTheme(theme As ITheme)
        If IsDisposed OrElse Disposing Then Return
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
        Dim sectionTitles = New String() {
            "NexaTextBox — styles",
            "NexaTextBox — placeholder, clear, helper, error, success",
            "NexaTextBox — password and character counter",
            "NexaSearchBox — debounced search",
            "NexaMaskedTextBox — phone, date, postal code"
        }
        Dim rootControls = _root.Controls.Cast(Of Control).ToArray()
        For Each c As Control In rootControls
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

        If _searchResults IsNot Nothing Then
            _searchResults.BackColor = CType(palette(NexaColorRole.Surface).Value, Color)
        End If
    End Sub

    Private Function BuildTextBoxStylesCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Default, Filled, Outlined, and Flat styles use the active theme."))
        Dim stack = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 8, 0, 0)
        }
        Dim styles = New NexaTextBoxStyle() {NexaTextBoxStyle.Default, NexaTextBoxStyle.Filled, NexaTextBoxStyle.Outlined, NexaTextBoxStyle.Flat}
        For Each s In styles
            stack.Controls.Add(New NexaTextBox With {.Text = $"Style: {s}", .Style = s, .Width = 360, .Margin = New Padding(0, 4, 0, 4)})
        Next
        card.Controls.Add(stack)
        Return card
    End Function

    Private Function BuildTextBoxStatesCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Validation states, placeholder, helper, and error text are all theme-aware."))

        Dim nameBox = New NexaTextBox With {.Width = 360, .PlaceholderText = "Enter student name", .Margin = New Padding(0, 8, 0, 0), .HelperText = "First name and last name."}
        Dim emailBox = New NexaTextBox With {.Width = 360, .PlaceholderText = "example@email.com", .Margin = New Padding(0, 8, 0, 0), .ShowClearButton = True}
        Dim successBox = New NexaTextBox With {.Width = 360, .Text = "Available username", .Margin = New Padding(0, 8, 0, 0), .ValidationState = NexaTextValidationState.Success, .HelperText = "Looks good."}
        Dim errorBox = New NexaTextBox With {.Width = 360, .Text = "Invalid email", .Margin = New Padding(0, 8, 0, 0), .ValidationState = NexaTextValidationState.Error, .ErrorText = "Email address is not valid."}
        Dim disabledBox = New NexaTextBox With {.Width = 360, .Text = "Disabled field", .Margin = New Padding(0, 8, 0, 0), .Enabled = False}
        Dim readonlyBox = New NexaTextBox With {.Width = 360, .Text = "Read-only field", .Margin = New Padding(0, 8, 0, 0), .ReadOnly = True}

        card.Controls.Add(nameBox)
        card.Controls.Add(emailBox)
        card.Controls.Add(successBox)
        card.Controls.Add(errorBox)
        card.Controls.Add(disabledBox)
        card.Controls.Add(readonlyBox)
        Return card
    End Function

    Private Function BuildPasswordAndCounterCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Password mode and live character counter."))

        Dim pwd = New NexaTextBox With {
            .Width = 360,
            .UseSystemPasswordChar = True,
            .PlaceholderText = "Enter password",
            .ShowClearButton = True,
            .Margin = New Padding(0, 8, 0, 0)
        }
        Dim counter = New NexaTextBox With {
            .Width = 360,
            .MaxLength = 500,
            .PlaceholderText = "Type to see the counter",
            .CharacterCounterEnabled = True,
            .Margin = New Padding(0, 8, 0, 0)
        }
        card.Controls.Add(pwd)
        card.Controls.Add(counter)
        Return card
    End Function

    Private Function BuildSearchBoxCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Search students... typing updates results. Clear button, Escape, and debounce all work."))

        Dim search = New NexaSearchBox With {
            .Width = 380,
            .PlaceholderText = "Search students...",
            .SearchDelayMs = 200
        }
        Dim list = New ListBox With {
            .Dock = DockStyle.Top,
            .Height = 160,
            .IntegralHeight = False,
            .Margin = New Padding(0, 8, 0, 0),
            .BorderStyle = BorderStyle.None
        }
        _search = search
        _searchResults = list

        AddHandler search.SearchChanged, Sub(s, e) RunSearch(search.SearchText)
        AddHandler search.Cleared, Sub() list.Items.Clear()

        RunSearch(String.Empty)

        card.Controls.Add(search)
        card.Controls.Add(list)
        Return card
    End Function

    Private Sub RunSearch(query As String)
        If _searchResults Is Nothing Then Return
        _searchResults.Items.Clear()
        If String.IsNullOrWhiteSpace(query) Then
            _searchResults.Items.Add("Type to search the local sample list.")
            Return
        End If
        Dim q = query.Trim()
        Dim hits = 0
        Dim names = SampleStudents
        For Each name As String In names
            If name.Contains(q, StringComparison.OrdinalIgnoreCase) Then
                _searchResults.Items.Add(name)
                hits += 1
            End If
        Next
        If hits = 0 Then _searchResults.Items.Add("No results.")
    End Sub

    Private Function BuildMaskedCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Native masked input with themed border and live validation status."))

        Dim phone = New NexaMaskedTextBox With {.Width = 220, .Mask = "(000) 000-0000", .Margin = New Padding(0, 8, 0, 4)}
        Dim dateBox = New NexaMaskedTextBox With {.Width = 220, .Mask = "00/00/0000", .Margin = New Padding(0, 8, 0, 4)}
        Dim postal = New NexaMaskedTextBox With {.Width = 220, .Mask = "00000", .Margin = New Padding(0, 8, 0, 4)}

        Dim phoneValue = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Text = "Value: (empty)"}
        Dim dateValue = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Text = "Value: (empty)"}
        Dim postalValue = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Text = "Value: (empty)"}

        AddHandler phone.TextChanged, Sub() phoneValue.Text = $"Value: {If(String.IsNullOrEmpty(phone.Text), "(empty)", phone.Text)}  ·  Completed: {phone.MaskCompleted}"
        AddHandler dateBox.TextChanged, Sub() dateValue.Text = $"Value: {If(String.IsNullOrEmpty(dateBox.Text), "(empty)", dateBox.Text)}  ·  Completed: {dateBox.MaskCompleted}"
        AddHandler postal.TextChanged, Sub() postalValue.Text = $"Value: {If(String.IsNullOrEmpty(postal.Text), "(empty)", postal.Text)}  ·  Completed: {postal.MaskCompleted}"

        card.Controls.Add(LabeledRow("Phone Number", phone, phoneValue))
        card.Controls.Add(LabeledRow("Date", dateBox, dateValue))
        card.Controls.Add(LabeledRow("Postal Code", postal, postalValue))
        Return card
    End Function

    Private Shared Function LabeledRow(label As String, input As Control, valueLabel As Control) As Control
        Dim wrap = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 1
        }
        wrap.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))
        Dim lbl = New Label With {.Text = label, .Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 8, 0, 4)}
        wrap.Controls.Add(lbl)
        wrap.Controls.Add(input)
        wrap.Controls.Add(valueLabel)
        Return wrap
    End Function

    Private Function CreateDemoCard() As Panel
        Dim palette = ThemeManager.Current.Palette
        Dim card = New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(16, 16, 16, 16),
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

    Private Shared Function DescriptionLabel(text As String) As Label
        Return New Label With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 4),
            .ForeColor = CType(ThemeManager.Current.Palette(NexaColorRole.TextSecondary).Value, Color)
        }
    End Function

End Class