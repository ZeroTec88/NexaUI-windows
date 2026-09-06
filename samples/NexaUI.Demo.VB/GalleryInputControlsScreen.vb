Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
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

        _root.Controls.Add(HeaderLabel("Input Controls", NexaTypographyRole.Heading))
        _root.Controls.Add(BodyLabel("Bootstrap-inspired form controls. Label, helper text, validation states, sizes, icons, password, counter, search."))

        _root.Controls.Add(SectionTitle("Form layout — label, placeholder, helper, error, success"))
        _root.Controls.Add(BuildFormExampleCard())

        _root.Controls.Add(SectionTitle("Sizes — Small, Medium, Large"))
        _root.Controls.Add(BuildSizesCard())

        _root.Controls.Add(SectionTitle("Styles — Outline, Filled, Underline"))
        _root.Controls.Add(BuildStylesCard())

        _root.Controls.Add(SectionTitle("Icons, clear button, password, character counter"))
        _root.Controls.Add(BuildAddOnsCard())

        _root.Controls.Add(SectionTitle("Search — debounced, clearable, with results"))
        _root.Controls.Add(BuildSearchBoxCard())

        _root.Controls.Add(SectionTitle("Masked input — phone, date, postal code"))
        _root.Controls.Add(BuildMaskedCard())

        _status = New Label With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 16, 0, 0),
            .Text = "Switch the theme in the top bar — every control repaints."
        }
        _root.Controls.Add(_status)

        Controls.Add(_root)

        AddHandler ThemeManager.ThemeChanged, AddressOf OnThemeChangedHandler
        AddHandler Disposed, AddressOf OnSelfDisposed
        AddHandler HandleCreated, Sub() ApplyTheme(ThemeManager.Current)
    End Sub

    Private Sub OnThemeChangedHandler(sender As Object, e As ThemeChangedEventArgs)
        ApplyTheme(e.Current)
    End Sub

    Private Sub OnSelfDisposed(sender As Object, e As EventArgs)
        RemoveHandler ThemeManager.ThemeChanged, AddressOf OnThemeChangedHandler
    End Sub

    Private Shared Function HeaderLabel(text As String, role As NexaTypographyRole) As Label
        Return New Label With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 4)
        }
    End Function

    Private Shared Function BodyLabel(text As String) As Label
        Return New Label With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 16)
        }
    End Function

    Private Shared Function SectionTitle(title As String) As Label
        Return New Label With {
            .Text = title,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 16, 0, 8)
        }
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
        Dim rootControls = _root.Controls.Cast(Of Control).ToArray()
        For Each c As Control In rootControls
            If TypeOf c Is Label Then
                Dim l = DirectCast(c, Label)
                If l.Text.StartsWith("Sizes", StringComparison.Ordinal) OrElse
                   l.Text.StartsWith("Styles") OrElse
                   l.Text.StartsWith("Form") OrElse
                   l.Text.StartsWith("Icons") OrElse
                   l.Text.StartsWith("Search") OrElse
                   l.Text.StartsWith("Masked") Then
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

    Private Function BuildFormExampleCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Each input has a label, an optional placeholder, and either helper text or an error message."))

        Dim nameBox = New NexaTextBox With {
            .Width = 320,
            .Label = "Full name",
            .PlaceholderText = "Your full name",
            .HelperText = "First and last name as it appears on your ID.",
            .Margin = New Padding(0, 4, 0, 0)
        }
        Dim email = New NexaTextBox With {
            .Width = 320,
            .Label = "Email address",
            .PlaceholderText = "you@example.com",
            .Icon = NexaIconKind.User,
            .ShowClearButton = True,
            .HelperText = "We'll never share your email.",
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim success = New NexaTextBox With {
            .Width = 320,
            .Label = "Username",
            .Text = "nuwandave",
            .ValidationState = NexaTextValidationState.Success,
            .HelperText = "This username is available.",
            .Icon = NexaIconKind.Check,
            .ShowClearButton = True,
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim errorBox = New NexaTextBox With {
            .Width = 320,
            .Label = "Password",
            .Text = "abc",
            .UseSystemPasswordChar = True,
            .ValidationState = NexaTextValidationState.Error,
            .ErrorText = "Password must be at least 8 characters.",
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim readonlyBox = New NexaTextBox With {
            .Width = 320,
            .Label = "Student ID (read-only)",
            .Text = "STU-2024-0001",
            .ReadOnly = True,
            .HelperText = "Assigned by the registrar.",
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim disabled = New NexaTextBox With {
            .Width = 320,
            .Label = "Country (disabled)",
            .Text = "Sri Lanka",
            .Enabled = False,
            .Margin = New Padding(0, 12, 0, 0)
        }

        card.Controls.Add(nameBox)
        card.Controls.Add(email)
        card.Controls.Add(success)
        card.Controls.Add(errorBox)
        card.Controls.Add(readonlyBox)
        card.Controls.Add(disabled)
        Return card
    End Function

    Private Function BuildSizesCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Small (32px), Medium (40px), Large (48px)."))

        Dim small = New NexaTextBox With {
            .Width = 320,
            .InputSize = NexaInputSize.Small,
            .Label = "Small",
            .PlaceholderText = "Compact input",
            .Margin = New Padding(0, 4, 0, 0)
        }
        Dim medium = New NexaTextBox With {
            .Width = 320,
            .InputSize = NexaInputSize.Medium,
            .Label = "Medium",
            .PlaceholderText = "Default input",
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim large = New NexaTextBox With {
            .Width = 320,
            .InputSize = NexaInputSize.Large,
            .Label = "Large",
            .PlaceholderText = "Hero input",
            .Margin = New Padding(0, 12, 0, 0)
        }

        card.Controls.Add(small)
        card.Controls.Add(medium)
        card.Controls.Add(large)
        Return card
    End Function

    Private Function BuildStylesCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Outline (default), Filled, and Underline."))

        Dim outline = New NexaTextBox With {
            .Width = 320,
            .Style = NexaInputStyle.Outline,
            .Label = "Outline",
            .PlaceholderText = "Bordered with rounded corners",
            .Margin = New Padding(0, 4, 0, 0)
        }
        Dim filled = New NexaTextBox With {
            .Width = 320,
            .Style = NexaInputStyle.Filled,
            .Label = "Filled",
            .PlaceholderText = "Solid background, subtle border",
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim underline = New NexaTextBox With {
            .Width = 320,
            .Style = NexaInputStyle.Underline,
            .Label = "Underline",
            .PlaceholderText = "Bottom border only",
            .Margin = New Padding(0, 12, 0, 0)
        }

        card.Controls.Add(outline)
        card.Controls.Add(filled)
        card.Controls.Add(underline)
        Return card
    End Function

    Private Function BuildAddOnsCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Icons, clear button, password, and character counter."))

        Dim searchBox = New NexaTextBox With {
            .Width = 320,
            .Label = "Search",
            .PlaceholderText = "Type to search…",
            .Icon = NexaIconKind.Search,
            .ShowClearButton = True,
            .Margin = New Padding(0, 4, 0, 0)
        }
        Dim info = New NexaTextBox With {
            .Width = 320,
            .Label = "Website",
            .Text = "nexaui.dev",
            .Icon = NexaIconKind.Info,
            .ShowClearButton = True,
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim pwd = New NexaTextBox With {
            .Width = 320,
            .Label = "Password",
            .UseSystemPasswordChar = True,
            .PlaceholderText = "Enter a strong password",
            .Icon = NexaIconKind.Settings,
            .ShowClearButton = True,
            .Margin = New Padding(0, 12, 0, 0)
        }
        Dim counter = New NexaTextBox With {
            .Width = 320,
            .Label = "Bio",
            .MaxLength = 160,
            .PlaceholderText = "Tell us about yourself",
            .ShowCounter = True,
            .Multiline = True,
            .Margin = New Padding(0, 12, 0, 0),
            .Height = 90
        }

        card.Controls.Add(searchBox)
        card.Controls.Add(info)
        card.Controls.Add(pwd)
        card.Controls.Add(counter)
        Return card
    End Function

    Private Function BuildSearchBoxCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Debounced search with clear button and Escape-to-clear."))

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
        card.Controls.Add(DescriptionLabel("Themed masked inputs with built-in labels and validation icons."))

        Dim phone = New NexaMaskedTextBox With {.Width = 240, .Mask = "(000) 000-0000", .Label = "Phone number"}
        Dim dateBox = New NexaMaskedTextBox With {.Width = 240, .Mask = "00/00/0000", .Label = "Date of birth"}
        Dim postal = New NexaMaskedTextBox With {.Width = 240, .Mask = "00000", .Label = "Postal code"}

        Dim phoneValue = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Value: (empty)"}
        Dim dateValue = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Value: (empty)"}
        Dim postalValue = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Value: (empty)"}

        AddHandler phone.TextChanged, Sub() phoneValue.Text = $"Value: {If(String.IsNullOrEmpty(phone.Text), "(empty)", phone.Text)}  ·  Completed: {phone.MaskCompleted}"
        AddHandler dateBox.TextChanged, Sub() dateValue.Text = $"Value: {If(String.IsNullOrEmpty(dateBox.Text), "(empty)", dateBox.Text)}  ·  Completed: {dateBox.MaskCompleted}"
        AddHandler postal.TextChanged, Sub() postalValue.Text = $"Value: {If(String.IsNullOrEmpty(postal.Text), "(empty)", postal.Text)}  ·  Completed: {postal.MaskCompleted}"

        Dim phoneStack = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 8, 0, 0)
        }
        phoneStack.Controls.Add(phone)
        phoneStack.Controls.Add(phoneValue)

        Dim dateStack = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 8, 0, 0)
        }
        dateStack.Controls.Add(dateBox)
        dateStack.Controls.Add(dateValue)

        Dim postalStack = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 8, 0, 0)
        }
        postalStack.Controls.Add(postal)
        postalStack.Controls.Add(postalValue)

        card.Controls.Add(phoneStack)
        card.Controls.Add(dateStack)
        card.Controls.Add(postalStack)
        Return card
    End Function

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
