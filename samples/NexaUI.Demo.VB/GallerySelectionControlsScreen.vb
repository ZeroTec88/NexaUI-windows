Imports System.Data
Imports System.Drawing
Imports System.Windows.Forms
Imports NexaUI.Controls
Imports NexaUI.Core
Imports NexaUI.Icons
Imports NexaUI.Themes

Public NotInheritable Class GallerySelectionControlsScreen
    Inherits UserControl

    Private _root As TableLayoutPanel
    Private _checkStatus As Label = Nothing
    Private _radioStatus As Label = Nothing
    Private _indeterminateStatus As Label = Nothing
    Private _comboStatus As Label = Nothing
    Private _toggleStatus As Label = Nothing
    Private _submitStatus As Label = Nothing

    Private _agree As NexaCheckBox = Nothing
    Private _newsletter As NexaCheckBox = Nothing
    Private _marketing As NexaCheckBox = Nothing
    Private _disabled As NexaCheckBox = Nothing
    Private _indeterminate As NexaCheckBox = Nothing
    Private _errorCheck As NexaCheckBox = Nothing
    Private _successCheck As NexaCheckBox = Nothing
    Private _acceptTerms As NexaCheckBox = Nothing

    Private _male As NexaRadioButton = Nothing
    Private _female As NexaRadioButton = Nothing
    Private _other As NexaRadioButton = Nothing
    Private _fullTime As NexaRadioButton = Nothing
    Private _partTime As NexaRadioButton = Nothing
    Private _disabledRadio As NexaRadioButton = Nothing

    Private _basicCombo As NexaComboBox = Nothing
    Private _dataBoundCombo As NexaComboBox = Nothing
    Private _filledCombo As NexaComboBox = Nothing
    Private _flatCombo As NexaComboBox = Nothing
    Private _disabledCombo As NexaComboBox = Nothing
    Private _errorCombo As NexaComboBox = Nothing
    Private _successCombo As NexaComboBox = Nothing
    Private _placeholderCombo As NexaComboBox = Nothing
    Private _autoCompleteCombo As NexaComboBox = Nothing
    Private _registrationCourse As NexaComboBox = Nothing

    Private _notif As NexaToggleSwitch = Nothing
    Private _darkMode As NexaToggleSwitch = Nothing
    Private _autoSave As NexaToggleSwitch = Nothing
    Private _smallToggle As NexaToggleSwitch = Nothing
    Private _largeToggle As NexaToggleSwitch = Nothing
    Private _textToggle As NexaToggleSwitch = Nothing
    Private _disabledToggle As NexaToggleSwitch = Nothing
    Private _registerNotifications As NexaToggleSwitch = Nothing

    Private _submit As NexaButton = Nothing

    Public Sub New()
        Dock = DockStyle.Fill
        AutoScroll = True

        _root = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .Padding = New Padding(32, 24, 32, 24)
        }
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))
        _root.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0F))

        Dim heading = HeaderLabel("Selection Controls", NexaTypographyRole.Heading)
        _root.Controls.Add(heading)
        _root.SetColumnSpan(heading, 2)

        Dim intro = BodyLabel("NexaComboBox, NexaCheckBox, NexaRadioButton and NexaToggleSwitch. Sharp corners, theme-aware, fully keyboard accessible.")
        _root.Controls.Add(intro)
        _root.SetColumnSpan(intro, 2)

        _root.Controls.Add(SectionTitle("NexaCheckBox — normal, checked, indeterminate, disabled"))
        _root.Controls.Add(BuildCheckBoxCard())

        _root.Controls.Add(SectionTitle("NexaCheckBox — error and success validation"))
        _root.Controls.Add(BuildCheckBoxValidationCard())

        _root.Controls.Add(SectionTitle("NexaRadioButton — groups (parent-scoped, native)"))
        _root.Controls.Add(BuildRadioCard())

        _root.Controls.Add(SectionTitle("NexaComboBox — basic, filled, flat, placeholder, disabled"))
        _root.Controls.Add(BuildComboBasicCard())

        _root.Controls.Add(SectionTitle("NexaComboBox — DataSource / DisplayMember / ValueMember"))
        _root.Controls.Add(BuildComboDataBoundCard())

        _root.Controls.Add(SectionTitle("NexaComboBox — error, success, AutoComplete"))
        _root.Controls.Add(BuildComboValidationCard())

        _root.Controls.Add(SectionTitle("NexaToggleSwitch — basic, sizes, with text, disabled"))
        _root.Controls.Add(BuildToggleCard())

        _root.Controls.Add(SectionTitle("Registration — controls working together"))
        _root.Controls.Add(BuildRegistrationCard())

        Dim summary = HeaderLabel("Live status", NexaTypographyRole.Title)
        _root.Controls.Add(summary)
        _root.SetColumnSpan(summary, 2)

        Dim statusRow = BuildStatusRow()
        _root.Controls.Add(statusRow)
        _root.SetColumnSpan(statusRow, 2)

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

    Private Function BuildCheckBoxCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Standard two-state and tri-state check boxes. Space toggles, Tab moves focus."))

        _agree = New NexaCheckBox With {.Text = "I agree to the terms and conditions", .Checked = True, .Margin = New Padding(0, 4, 0, 0)}
        _newsletter = New NexaCheckBox With {.Text = "Subscribe to newsletter", .Margin = New Padding(0, 8, 0, 0)}
        _marketing = New NexaCheckBox With {.Text = "Receive marketing emails", .Margin = New Padding(0, 8, 0, 0)}
        _indeterminate = New NexaCheckBox With {
            .Text = "Select all permissions",
            .ThreeState = True,
            .IsIndeterminate = True,
            .Margin = New Padding(0, 8, 0, 0)
        }
        _disabled = New NexaCheckBox With {
            .Text = "Locked option (disabled)",
            .Checked = True,
            .Enabled = False,
            .Margin = New Padding(0, 8, 0, 0)
        }

        AddHandler _agree.CheckedChanged, Sub() UpdateCheckStatus()
        AddHandler _newsletter.CheckedChanged, Sub() UpdateCheckStatus()
        AddHandler _marketing.CheckedChanged, Sub() UpdateCheckStatus()
        AddHandler _indeterminate.CheckedChanged, Sub() UpdateIndeterminateStatus()

        card.Controls.Add(_agree)
        card.Controls.Add(_newsletter)
        card.Controls.Add(_marketing)
        card.Controls.Add(_indeterminate)
        card.Controls.Add(_disabled)
        Return card
    End Function

    Private Function BuildCheckBoxValidationCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("ValidationState drives the indicator color and the helper text."))

        _errorCheck = New NexaCheckBox With {
            .Text = "Required acknowledgement",
            .Checked = True,
            .ValidationState = NexaTextValidationState.Error,
            .ErrorText = "You must accept before continuing",
            .Margin = New Padding(0, 4, 0, 0)
        }
        _successCheck = New NexaCheckBox With {
            .Text = "Email verified",
            .Checked = True,
            .ValidationState = NexaTextValidationState.Success,
            .HelperText = "Looks good.",
            .Margin = New Padding(0, 8, 0, 0)
        }

        card.Controls.Add(_errorCheck)
        card.Controls.Add(_successCheck)
        Return card
    End Function

    Private Function BuildRadioCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Two native parent-scoped groups. Space selects."))

        Dim genderGroup = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 4, 0, 0)
        }
        Dim genderTitle = New Label With {.Text = "Gender", .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)}
        genderGroup.Controls.Add(genderTitle)
        _male = New NexaRadioButton With {.Text = "Male", .Checked = True, .Margin = New Padding(0, 0, 0, 4)}
        _female = New NexaRadioButton With {.Text = "Female", .Margin = New Padding(0, 0, 0, 4)}
        _other = New NexaRadioButton With {.Text = "Other", .Margin = New Padding(0, 0, 0, 4)}
        genderGroup.Controls.Add(_male)
        genderGroup.Controls.Add(_female)
        genderGroup.Controls.Add(_other)
        card.Controls.Add(genderGroup)

        Dim courseGroup = New FlowLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 8, 0, 0)
        }
        Dim courseTitle = New Label With {.Text = "Course Type", .AutoSize = True, .Margin = New Padding(0, 0, 0, 4)}
        courseGroup.Controls.Add(courseTitle)
        _fullTime = New NexaRadioButton With {.Text = "Full Time", .Checked = True, .Margin = New Padding(0, 0, 0, 4)}
        _partTime = New NexaRadioButton With {.Text = "Part Time", .Margin = New Padding(0, 0, 0, 4)}
        _disabledRadio = New NexaRadioButton With {.Text = "Distance (unavailable)", .Enabled = False, .Margin = New Padding(0, 0, 0, 4)}
        courseGroup.Controls.Add(_fullTime)
        courseGroup.Controls.Add(_partTime)
        courseGroup.Controls.Add(_disabledRadio)
        card.Controls.Add(courseGroup)

        AddHandler _male.CheckedChanged, Sub() UpdateRadioStatus()
        AddHandler _female.CheckedChanged, Sub() UpdateRadioStatus()
        AddHandler _other.CheckedChanged, Sub() UpdateRadioStatus()
        AddHandler _fullTime.CheckedChanged, Sub() UpdateRadioStatus()
        AddHandler _partTime.CheckedChanged, Sub() UpdateRadioStatus()

        Return card
    End Function

    Private Function BuildComboBasicCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Three styles + placeholder + disabled. Items are added through the Items collection."))

        _basicCombo = New NexaComboBox With {
            .Width = 240,
            .Style = NexaComboBoxStyle.Outlined,
            .Margin = New Padding(0, 4, 0, 0)
        }
        _basicCombo.Items.AddRange(New Object() {"ICT", "Software Engineering", "Graphic Design", "Networking", "Business Management"})

        _filledCombo = New NexaComboBox With {
            .Width = 240,
            .Style = NexaComboBoxStyle.Filled,
            .Margin = New Padding(0, 8, 0, 0)
        }
        _filledCombo.Items.AddRange(New Object() {"Light", "Dark", "System"})

        _flatCombo = New NexaComboBox With {
            .Width = 240,
            .Style = NexaComboBoxStyle.Flat,
            .Margin = New Padding(0, 8, 0, 0)
        }
        _flatCombo.Items.AddRange(New Object() {"English", "Spanish", "French", "Sinhala", "Tamil"})

        _placeholderCombo = New NexaComboBox With {
            .Width = 240,
            .PlaceholderText = "Select a department…",
            .Margin = New Padding(0, 8, 0, 0)
        }
        _placeholderCombo.Items.AddRange(New Object() {"Engineering", "Design", "Marketing", "Finance"})

        _disabledCombo = New NexaComboBox With {
            .Width = 240,
            .Enabled = False,
            .Text = "Locked choice",
            .Margin = New Padding(0, 8, 0, 0)
        }
        _disabledCombo.Items.AddRange(New Object() {"Read-only option"})

        AddHandler _basicCombo.SelectedIndexChanged, Sub() UpdateComboStatus()
        AddHandler _filledCombo.SelectedIndexChanged, Sub() UpdateComboStatus()
        AddHandler _flatCombo.SelectedIndexChanged, Sub() UpdateComboStatus()
        AddHandler _placeholderCombo.SelectedIndexChanged, Sub() UpdateComboStatus()

        card.Controls.Add(_basicCombo)
        card.Controls.Add(_filledCombo)
        card.Controls.Add(_flatCombo)
        card.Controls.Add(_placeholderCombo)
        card.Controls.Add(_disabledCombo)
        Return card
    End Function

    Private Function BuildComboDataBoundCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("DataSource / DisplayMember / ValueMember. SelectedValue reflects the bound model."))

        _dataBoundCombo = New NexaComboBox With {.Width = 240, .Margin = New Padding(0, 4, 0, 0)}
        Dim table = New DataTable()
        table.Columns.Add("Id", GetType(Integer))
        table.Columns.Add("Name", GetType(String))
        table.Rows.Add(1, "Mathematics")
        table.Rows.Add(2, "Physics")
        table.Rows.Add(3, "Chemistry")
        table.Rows.Add(4, "Biology")
        _dataBoundCombo.DataSource = table
        _dataBoundCombo.DisplayMember = "Name"
        _dataBoundCombo.ValueMember = "Id"

        AddHandler _dataBoundCombo.SelectedValueChanged, Sub() UpdateComboStatus()

        Dim status = New Label With {
            .Text = "Bound to DataTable with DisplayMember='Name', ValueMember='Id'.",
            .AutoSize = True,
            .Margin = New Padding(0, 8, 0, 0)
        }

        card.Controls.Add(_dataBoundCombo)
        card.Controls.Add(status)
        Return card
    End Function

    Private Function BuildComboValidationCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Error and success border colors + native AutoComplete Suggest mode."))

        _errorCombo = New NexaComboBox With {
            .Width = 240,
            .Text = "Not a valid option",
            .ValidationState = NexaTextValidationState.Error,
            .ErrorText = "Please pick a value from the list",
            .Margin = New Padding(0, 4, 0, 0)
        }
        _errorCombo.Items.AddRange(New Object() {"Apples", "Oranges", "Pears"})
        _errorCombo.SelectedIndex = -1

        _successCombo = New NexaComboBox With {
            .Width = 240,
            .Text = "Mathematics",
            .ValidationState = NexaTextValidationState.Success,
            .HelperText = "Subject confirmed.",
            .Margin = New Padding(0, 8, 0, 0)
        }
        _successCombo.Items.AddRange(New Object() {"Mathematics", "Physics", "Chemistry"})
        _successCombo.SelectedIndex = 0

        _autoCompleteCombo = New NexaComboBox With {
            .Width = 240,
            .PlaceholderText = "Type to search…",
            .AutoCompleteMode = AutoCompleteMode.Suggest,
            .AutoCompleteSource = AutoCompleteSource.ListItems,
            .Margin = New Padding(0, 8, 0, 0)
        }
        _autoCompleteCombo.Items.AddRange(New Object() {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola",
            "Bangladesh", "Barbados", "Belarus", "Belgium", "Bhutan",
            "Canada", "Chile", "China", "Colombia", "Croatia",
            "Denmark", "Dominica",
            "Ecuador", "Egypt", "Estonia",
            "Sri Lanka", "Sweden", "Switzerland"
        })

        AddHandler _errorCombo.SelectedIndexChanged, Sub() UpdateComboStatus()
        AddHandler _successCombo.SelectedIndexChanged, Sub() UpdateComboStatus()
        AddHandler _autoCompleteCombo.SelectedIndexChanged, Sub() UpdateComboStatus()

        card.Controls.Add(_errorCombo)
        card.Controls.Add(_successCombo)
        card.Controls.Add(_autoCompleteCombo)
        Return card
    End Function

    Private Function BuildToggleCard() As Control
        Dim card = CreateDemoCard()
        card.Controls.Add(DescriptionLabel("Toggles with sizes, with-text, disabled, and animation."))

        _notif = New NexaToggleSwitch With {.Text = "Notifications", .Checked = True, .Margin = New Padding(0, 4, 0, 0)}
        _darkMode = New NexaToggleSwitch With {.Text = "Dark Mode", .Checked = False, .Margin = New Padding(0, 8, 0, 0)}
        _autoSave = New NexaToggleSwitch With {.Text = "Auto Save", .Checked = True, .Margin = New Padding(0, 8, 0, 0)}
        _smallToggle = New NexaToggleSwitch With {.Text = "Small", .Checked = False, .ToggleSize = NexaToggleSize.Small, .Margin = New Padding(0, 8, 0, 0)}
        _largeToggle = New NexaToggleSwitch With {.Text = "Large", .Checked = True, .ToggleSize = NexaToggleSize.Large, .Margin = New Padding(0, 8, 0, 0)}
        _textToggle = New NexaToggleSwitch With {
            .Text = "Show Text",
            .Checked = True,
            .ShowText = True,
            .OnText = "ON",
            .OffText = "OFF",
            .Margin = New Padding(0, 8, 0, 0)
        }
        _disabledToggle = New NexaToggleSwitch With {
            .Text = "Disabled (off)",
            .Checked = False,
            .Enabled = False,
            .Margin = New Padding(0, 8, 0, 0)
        }

        AddHandler _notif.CheckedChanged, Sub() UpdateToggleStatus()
        AddHandler _darkMode.CheckedChanged, Sub() UpdateToggleStatus()
        AddHandler _autoSave.CheckedChanged, Sub() UpdateToggleStatus()
        AddHandler _smallToggle.CheckedChanged, Sub() UpdateToggleStatus()
        AddHandler _largeToggle.CheckedChanged, Sub() UpdateToggleStatus()
        AddHandler _textToggle.CheckedChanged, Sub() UpdateToggleStatus()

        card.Controls.Add(_notif)
        card.Controls.Add(_darkMode)
        card.Controls.Add(_autoSave)
        card.Controls.Add(_smallToggle)
        card.Controls.Add(_largeToggle)
        card.Controls.Add(_textToggle)
        card.Controls.Add(_disabledToggle)
        Return card
    End Function

    Private Function BuildRegistrationCard() As Control
        Dim card = CreateDemoCard()
        AddHelperLabel(card, "Small interaction example. The four control families wired together.")

        Dim form = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 2,
            .RowCount = 6,
            .Padding = New Padding(0, 8, 0, 0)
        }
        form.ColumnStyles.Add(New ColumnStyle(SizeType.Absolute, 120.0F))
        form.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        Dim courseLabel = New Label With {.Text = "Course", .AutoSize = True, .Margin = New Padding(0, 6, 8, 6)}
        _registrationCourse = New NexaComboBox With {.Width = 220, .Style = NexaComboBoxStyle.Outlined, .Dock = DockStyle.Fill}
        _registrationCourse.Items.AddRange(New Object() {"ICT", "Software Engineering", "Graphic Design", "Networking", "Business Management"})
        _registrationCourse.SelectedIndex = 0
        form.Controls.Add(courseLabel, 0, 0)
        form.Controls.Add(_registrationCourse, 1, 0)

        Dim genderLabel = New Label With {.Text = "Gender", .AutoSize = True, .Margin = New Padding(0, 6, 8, 6)}
        Dim genderGroup = New FlowLayoutPanel With {
            .Dock = DockStyle.Fill,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .FlowDirection = FlowDirection.TopDown,
            .WrapContents = False,
            .Padding = New Padding(0, 4, 0, 0)
        }
        _male = New NexaRadioButton With {.Text = "Male", .Checked = True, .Margin = New Padding(0, 0, 0, 4)}
        _female = New NexaRadioButton With {.Text = "Female", .Margin = New Padding(0, 0, 0, 4)}
        _other = New NexaRadioButton With {.Text = "Other", .Margin = New Padding(0, 0, 0, 4)}
        genderGroup.Controls.Add(_male)
        genderGroup.Controls.Add(_female)
        genderGroup.Controls.Add(_other)
        form.Controls.Add(genderLabel, 0, 1)
        form.Controls.Add(genderGroup, 1, 1)

        Dim notifLabel = New Label With {.Text = "Notifications", .AutoSize = True, .Margin = New Padding(0, 6, 8, 6)}
        _registerNotifications = New NexaToggleSwitch With {.Text = "Send me updates", .Checked = True, .Dock = DockStyle.Left}
        form.Controls.Add(notifLabel, 0, 2)
        form.Controls.Add(_registerNotifications, 1, 2)

        Dim termsLabel = New Label With {.Text = "Accept Terms", .AutoSize = True, .Margin = New Padding(0, 6, 8, 6)}
        _acceptTerms = New NexaCheckBox With {.Text = "I have read and accept the terms", .Checked = False, .Margin = New Padding(0, 6, 0, 6)}
        AddHandler _acceptTerms.CheckedChanged, Sub()
                                                    _submit.Enabled = _acceptTerms.Checked
                                                    UpdateSubmitStatus()
                                                End Sub
        form.Controls.Add(termsLabel, 0, 3)
        form.Controls.Add(_acceptTerms, 1, 3)

        _submit = New NexaButton With {.Text = "Submit", .SizeMode = NexaButtonSize.Medium, .Enabled = False, .Margin = New Padding(0, 8, 0, 0)}
        AddHandler _submit.Click, AddressOf OnSubmitClick
        form.Controls.Add(New Label(), 0, 4)
        form.Controls.Add(_submit, 1, 4)

        card.Controls.Add(form)
        Return card
    End Function

    Private Sub OnSubmitClick(sender As Object, e As EventArgs)
        UpdateSubmitStatus()
    End Sub

    Private Shared Sub AddHelperLabel(card As Panel, text As String)
        card.Controls.Add(DescriptionLabel(text))
    End Sub

    Private Function BuildStatusRow() As Control
        Dim panel = New Panel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .Padding = New Padding(0, 16, 0, 0)
        }
        Dim layout = New TableLayoutPanel With {
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .AutoSizeMode = AutoSizeMode.GrowAndShrink,
            .ColumnCount = 1
        }
        layout.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 100.0F))

        _checkStatus = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Check boxes: (none)"}
        _indeterminateStatus = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Indeterminate: (none)"}
        _radioStatus = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Radios: (none)"}
        _comboStatus = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Combos: (none)"}
        _toggleStatus = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 4, 0, 0), .Text = "Toggles: (none)"}
        _submitStatus = New Label With {.Dock = DockStyle.Top, .AutoSize = True, .Margin = New Padding(0, 8, 0, 0), .Text = "Submit status: accept terms to enable submission."}

        layout.Controls.Add(_checkStatus)
        layout.Controls.Add(_indeterminateStatus)
        layout.Controls.Add(_radioStatus)
        layout.Controls.Add(_comboStatus)
        layout.Controls.Add(_toggleStatus)
        layout.Controls.Add(_submitStatus)
        panel.Controls.Add(layout)

        UpdateCheckStatus()
        UpdateIndeterminateStatus()
        UpdateRadioStatus()
        UpdateComboStatus()
        UpdateToggleStatus()
        UpdateSubmitStatus()
        Return panel
    End Function

    Private Sub UpdateCheckStatus()
        If _checkStatus Is Nothing Then Return
        Dim parts = New List(Of String)()
        If _agree.Checked Then parts.Add("agree")
        If _newsletter.Checked Then parts.Add("newsletter")
        If _marketing.Checked Then parts.Add("marketing")
        _checkStatus.Text = $"Check boxes: {If(parts.Count = 0, "(none)", String.Join(" + ", parts))}  ·  error: {If(_errorCheck.Checked, "acknowledged", "required")}  ·  success: {If(_successCheck.Checked, "verified", "—")}"
    End Sub

    Private Sub UpdateIndeterminateStatus()
        If _indeterminateStatus Is Nothing Then Return
        Dim state = _indeterminate.CheckState.ToString()
        _indeterminateStatus.Text = $"Tri-state: {state}"
    End Sub

    Private Sub UpdateRadioStatus()
        If _radioStatus Is Nothing Then Return
        Dim gender = If(_male.Checked, "Male", If(_female.Checked, "Female", If(_other.Checked, "Other", "(none)")))
        Dim course = If(_fullTime.Checked, "Full Time", If(_partTime.Checked, "Part Time", "(none)"))
        _radioStatus.Text = $"Radios: gender = {gender};  course type = {course}"
    End Sub

    Private Sub UpdateComboStatus()
        If _comboStatus Is Nothing Then Return
        Dim parts = New List(Of String)()
        If _basicCombo.SelectedIndex >= 0 Then parts.Add($"basic={_basicCombo.SelectedItem}")
        Dim dv = _dataBoundCombo.SelectedValue
        If dv IsNot Nothing Then parts.Add($"dataBoundId={dv}")
        If _errorCombo.SelectedIndex >= 0 Then parts.Add($"error={_errorCombo.SelectedItem}")
        If _successCombo.SelectedIndex >= 0 Then parts.Add($"success={_successCombo.SelectedItem}")
        If _autoCompleteCombo.SelectedIndex >= 0 Then parts.Add($"autocomplete={_autoCompleteCombo.SelectedItem}")
        _comboStatus.Text = $"Combos: {If(parts.Count = 0, "(none)", String.Join("  ·  ", parts))}"
    End Sub

    Private Sub UpdateToggleStatus()
        If _toggleStatus Is Nothing Then Return
        Dim parts = New List(Of String)()
        If _notif.Checked Then parts.Add("notif ON")
        If _darkMode.Checked Then parts.Add("dark ON")
        If _autoSave.Checked Then parts.Add("auto-save ON")
        If _smallToggle.Checked Then parts.Add("small ON")
        If _largeToggle.Checked Then parts.Add("large ON")
        If _textToggle.Checked Then parts.Add("text ON")
        _toggleStatus.Text = $"Toggles: {If(parts.Count = 0, "(all off)", String.Join("  ·  ", parts))}"
    End Sub

    Private Sub UpdateSubmitStatus()
        If _submitStatus Is Nothing Then Return
        If Not _acceptTerms.Checked Then
            _submitStatus.Text = "Submit status: disabled — accept the terms to enable submission."
            Return
        End If
        Dim course = If(_registrationCourse.SelectedItem?.ToString(), "(none)")
        Dim gender = If(_male.Checked, "Male", If(_female.Checked, "Female", If(_other.Checked, "Other", "(none)")))
        Dim notif = If(_registerNotifications.Checked, "yes", "no")
        _submitStatus.Text = $"Submit status: would register — course = {course}, gender = {gender}, notifications = {notif}."
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

    Private Shared Function DescriptionLabel(text As String) As Label
        Return New Label With {
            .Text = text,
            .Dock = DockStyle.Top,
            .AutoSize = True,
            .Margin = New Padding(0, 0, 0, 4),
            .ForeColor = CType(ThemeManager.Current.Palette(NexaColorRole.TextSecondary).Value, Color)
        }
    End Function

    Private Sub ApplyTheme(theme As ITheme)
        If IsDisposed OrElse Disposing Then Return
        Dim palette = theme.Palette
        Dim typography = theme.Typography
        Dim dpi = NexaFormsDpi.CurrentDpi(Me)

        BackColor = CType(palette(NexaColorRole.Background).Value, Color)
        ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)

        Dim sectionTitles = New String() {
            "NexaCheckBox — normal, checked, indeterminate, disabled",
            "NexaCheckBox — error and success validation",
            "NexaRadioButton — groups (parent-scoped, native)",
            "NexaComboBox — basic, filled, flat, placeholder, disabled",
            "NexaComboBox — DataSource / DisplayMember / ValueMember",
            "NexaComboBox — error, success, AutoComplete",
            "NexaToggleSwitch — basic, sizes, with text, disabled",
            "Registration — controls working together",
            "Live status"
        }
        Dim rootControls = _root.Controls.Cast(Of Control).ToArray()
        For Each c As Control In rootControls
            If TypeOf c Is Label Then
                Dim l = DirectCast(c, Label)
                If l.Text = "Selection Controls" Then
                    l.Font = typography.ToFont(NexaTypographyRole.Heading, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
                ElseIf l.Text.StartsWith("NexaComboBox, NexaCheckBox", StringComparison.Ordinal) Then
                    l.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextSecondary).Value, Color)
                ElseIf Array.IndexOf(sectionTitles, l.Text) >= 0 Then
                    l.Font = typography.ToFont(NexaTypographyRole.Title, dpi)
                    l.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
                End If
            End If
        Next

        If _checkStatus IsNot Nothing Then
            _checkStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            _checkStatus.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If
        If _indeterminateStatus IsNot Nothing Then
            _indeterminateStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            _indeterminateStatus.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If
        If _radioStatus IsNot Nothing Then
            _radioStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            _radioStatus.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If
        If _comboStatus IsNot Nothing Then
            _comboStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            _comboStatus.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If
        If _toggleStatus IsNot Nothing Then
            _toggleStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            _toggleStatus.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If
        If _submitStatus IsNot Nothing Then
            _submitStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi)
            _submitStatus.ForeColor = CType(palette(NexaColorRole.TextPrimary).Value, Color)
        End If

        If _agree IsNot Nothing Then _agree.Invalidate()
        If _newsletter IsNot Nothing Then _newsletter.Invalidate()
        If _marketing IsNot Nothing Then _marketing.Invalidate()
        If _disabled IsNot Nothing Then _disabled.Invalidate()
        If _indeterminate IsNot Nothing Then _indeterminate.Invalidate()
        If _errorCheck IsNot Nothing Then _errorCheck.Invalidate()
        If _successCheck IsNot Nothing Then _successCheck.Invalidate()
        If _acceptTerms IsNot Nothing Then _acceptTerms.Invalidate()
        If _male IsNot Nothing Then _male.Invalidate()
        If _female IsNot Nothing Then _female.Invalidate()
        If _other IsNot Nothing Then _other.Invalidate()
        If _fullTime IsNot Nothing Then _fullTime.Invalidate()
        If _partTime IsNot Nothing Then _partTime.Invalidate()
        If _disabledRadio IsNot Nothing Then _disabledRadio.Invalidate()
        If _basicCombo IsNot Nothing Then _basicCombo.Invalidate()
        If _dataBoundCombo IsNot Nothing Then _dataBoundCombo.Invalidate()
        If _filledCombo IsNot Nothing Then _filledCombo.Invalidate()
        If _flatCombo IsNot Nothing Then _flatCombo.Invalidate()
        If _placeholderCombo IsNot Nothing Then _placeholderCombo.Invalidate()
        If _disabledCombo IsNot Nothing Then _disabledCombo.Invalidate()
        If _errorCombo IsNot Nothing Then _errorCombo.Invalidate()
        If _successCombo IsNot Nothing Then _successCombo.Invalidate()
        If _autoCompleteCombo IsNot Nothing Then _autoCompleteCombo.Invalidate()
        If _registrationCourse IsNot Nothing Then _registrationCourse.Invalidate()
        If _notif IsNot Nothing Then _notif.Invalidate()
        If _darkMode IsNot Nothing Then _darkMode.Invalidate()
        If _autoSave IsNot Nothing Then _autoSave.Invalidate()
        If _smallToggle IsNot Nothing Then _smallToggle.Invalidate()
        If _largeToggle IsNot Nothing Then _largeToggle.Invalidate()
        If _textToggle IsNot Nothing Then _textToggle.Invalidate()
        If _disabledToggle IsNot Nothing Then _disabledToggle.Invalidate()
        If _registerNotifications IsNot Nothing Then _registerNotifications.Invalidate()
    End Sub

End Class
