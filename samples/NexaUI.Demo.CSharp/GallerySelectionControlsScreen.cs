using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GallerySelectionControlsScreen : UserControl
{
    private readonly TableLayoutPanel _root;

    private Label _checkStatus = null!;
    private Label _radioStatus = null!;
    private Label _indeterminateStatus = null!;
    private Label _comboStatus = null!;
    private Label _toggleStatus = null!;
    private Label _submitStatus = null!;

    private NexaCheckBox _agree = null!;
    private NexaCheckBox _newsletter = null!;
    private NexaCheckBox _marketing = null!;
    private NexaCheckBox _disabled = null!;
    private NexaCheckBox _indeterminate = null!;
    private NexaCheckBox _errorCheck = null!;
    private NexaCheckBox _successCheck = null!;
    private NexaCheckBox _acceptTerms = null!;

    private NexaRadioButton _male = null!;
    private NexaRadioButton _female = null!;
    private NexaRadioButton _other = null!;
    private NexaRadioButton _fullTime = null!;
    private NexaRadioButton _partTime = null!;
    private NexaRadioButton _disabledRadio = null!;

    private NexaComboBox _basicCombo = null!;
    private NexaComboBox _dataBoundCombo = null!;
    private NexaComboBox _filledCombo = null!;
    private NexaComboBox _flatCombo = null!;
    private NexaComboBox _disabledCombo = null!;
    private NexaComboBox _errorCombo = null!;
    private NexaComboBox _successCombo = null!;
    private NexaComboBox _placeholderCombo = null!;
    private NexaComboBox _autoCompleteCombo = null!;
    private NexaComboBox _registrationCourse = null!;

    private NexaToggleSwitch _notif = null!;
    private NexaToggleSwitch _darkMode = null!;
    private NexaToggleSwitch _autoSave = null!;
    private NexaToggleSwitch _smallToggle = null!;
    private NexaToggleSwitch _largeToggle = null!;
    private NexaToggleSwitch _textToggle = null!;
    private NexaToggleSwitch _disabledToggle = null!;
    private NexaToggleSwitch _registerNotifications = null!;

    private NexaButton _submit = null!;

    public GallerySelectionControlsScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;

        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Padding = new Padding(32, 24, 32, 24)
        };
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        var heading = HeaderLabel("Selection Controls", NexaTypographyRole.Heading);
        _root.Controls.Add(heading);
        _root.SetColumnSpan(heading, 2);

        var intro = BodyLabel("NexaComboBox, NexaCheckBox, NexaRadioButton and NexaToggleSwitch. Sharp corners, theme-aware, fully keyboard accessible.");
        _root.Controls.Add(intro);
        _root.SetColumnSpan(intro, 2);

        _root.Controls.Add(SectionTitle("NexaCheckBox — normal, checked, indeterminate, disabled"));
        _root.Controls.Add(BuildCheckBoxCard());

        _root.Controls.Add(SectionTitle("NexaCheckBox — error and success validation"));
        _root.Controls.Add(BuildCheckBoxValidationCard());

        _root.Controls.Add(SectionTitle("NexaRadioButton — groups (parent-scoped, native)"));
        _root.Controls.Add(BuildRadioCard());

        _root.Controls.Add(SectionTitle("NexaComboBox — basic, filled, flat, placeholder, disabled"));
        _root.Controls.Add(BuildComboBasicCard());

        _root.Controls.Add(SectionTitle("NexaComboBox — DataSource / DisplayMember / ValueMember"));
        _root.Controls.Add(BuildComboDataBoundCard());

        _root.Controls.Add(SectionTitle("NexaComboBox — error, success, AutoComplete"));
        _root.Controls.Add(BuildComboValidationCard());

        _root.Controls.Add(SectionTitle("NexaToggleSwitch — basic, sizes, with text, disabled"));
        _root.Controls.Add(BuildToggleCard());

        _root.Controls.Add(SectionTitle("Registration — controls working together"));
        _root.Controls.Add(BuildRegistrationCard());

        var summary = HeaderLabel("Live status", NexaTypographyRole.Title);
        _root.Controls.Add(summary);
        _root.SetColumnSpan(summary, 2);

        var statusRow = BuildStatusRow();
        _root.Controls.Add(statusRow);
        _root.SetColumnSpan(statusRow, 2);

        Controls.Add(_root);

        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);
        Disposed += OnSelfDisposed;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnSelfDisposed(object? sender, System.EventArgs e) => ThemeManager.ThemeChanged -= OnSelfDisposed;

    private static Label HeaderLabel(string text, NexaTypographyRole role) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 0, 0, 4)
    };

    private static Label BodyLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 0, 0, 16)
    };

    private static Label SectionTitle(string title) => new()
    {
        Text = title,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 16, 0, 8)
    };

    private Control BuildCheckBoxCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Standard two-state and tri-state check boxes. Space toggles, Tab moves focus."));

        _agree = new NexaCheckBox { Text = "I agree to the terms and conditions", Checked = true, Margin = new Padding(0, 4, 0, 0) };
        _newsletter = new NexaCheckBox { Text = "Subscribe to newsletter", Margin = new Padding(0, 8, 0, 0) };
        _marketing = new NexaCheckBox { Text = "Receive marketing emails", Margin = new Padding(0, 8, 0, 0) };
        _indeterminate = new NexaCheckBox
        {
            Text = "Select all permissions",
            ThreeState = true,
            IsIndeterminate = true,
            Margin = new Padding(0, 8, 0, 0)
        };
        _disabled = new NexaCheckBox
        {
            Text = "Locked option (disabled)",
            Checked = true,
            Enabled = false,
            Margin = new Padding(0, 8, 0, 0)
        };

        _agree.CheckedChanged += (_, _) => UpdateCheckStatus();
        _newsletter.CheckedChanged += (_, _) => UpdateCheckStatus();
        _marketing.CheckedChanged += (_, _) => UpdateCheckStatus();
        _indeterminate.CheckedChanged += (_, _) => UpdateIndeterminateStatus();

        card.Controls.Add(_agree);
        card.Controls.Add(_newsletter);
        card.Controls.Add(_marketing);
        card.Controls.Add(_indeterminate);
        card.Controls.Add(_disabled);
        return card;
    }

    private Control BuildCheckBoxValidationCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("ValidationState drives the indicator color and the helper text."));

        _errorCheck = new NexaCheckBox
        {
            Text = "Required acknowledgement",
            Checked = true,
            ValidationState = NexaTextValidationState.Error,
            ErrorText = "You must accept before continuing",
            Margin = new Padding(0, 4, 0, 0)
        };
        _successCheck = new NexaCheckBox
        {
            Text = "Email verified",
            Checked = true,
            ValidationState = NexaTextValidationState.Success,
            HelperText = "Looks good.",
            Margin = new Padding(0, 8, 0, 0)
        };

        card.Controls.Add(_errorCheck);
        card.Controls.Add(_successCheck);
        return card;
    }

    private Control BuildRadioCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Two native parent-scoped groups. Space selects."));

        var genderGroup = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 4, 0, 0)
        };
        var genderTitle = new Label { Text = "Gender", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        genderGroup.Controls.Add(genderTitle);
        _male = new NexaRadioButton { Text = "Male", Checked = true, Margin = new Padding(0, 0, 0, 4) };
        _female = new NexaRadioButton { Text = "Female", Margin = new Padding(0, 0, 0, 4) };
        _other = new NexaRadioButton { Text = "Other", Margin = new Padding(0, 0, 0, 4) };
        genderGroup.Controls.Add(_male);
        genderGroup.Controls.Add(_female);
        genderGroup.Controls.Add(_other);
        card.Controls.Add(genderGroup);

        var courseGroup = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var courseTitle = new Label { Text = "Course Type", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        courseGroup.Controls.Add(courseTitle);
        _fullTime = new NexaRadioButton { Text = "Full Time", Checked = true, Margin = new Padding(0, 0, 0, 4) };
        _partTime = new NexaRadioButton { Text = "Part Time", Margin = new Padding(0, 0, 0, 4) };
        _disabledRadio = new NexaRadioButton { Text = "Distance (unavailable)", Enabled = false, Margin = new Padding(0, 0, 0, 4) };
        courseGroup.Controls.Add(_fullTime);
        courseGroup.Controls.Add(_partTime);
        courseGroup.Controls.Add(_disabledRadio);
        card.Controls.Add(courseGroup);

        _male.CheckedChanged += (_, _) => UpdateRadioStatus();
        _female.CheckedChanged += (_, _) => UpdateRadioStatus();
        _other.CheckedChanged += (_, _) => UpdateRadioStatus();
        _fullTime.CheckedChanged += (_, _) => UpdateRadioStatus();
        _partTime.CheckedChanged += (_, _) => UpdateRadioStatus();

        return card;
    }

    private Control BuildComboBasicCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Three styles + placeholder + disabled. Items are added through the Items collection."));

        _basicCombo = new NexaComboBox
        {
            Width = 240,
            Style = NexaComboBoxStyle.Outlined,
            Margin = new Padding(0, 4, 0, 0)
        };
        _basicCombo.Items.AddRange(new object[]
        {
            "ICT",
            "Software Engineering",
            "Graphic Design",
            "Networking",
            "Business Management"
        });

        _filledCombo = new NexaComboBox
        {
            Width = 240,
            Style = NexaComboBoxStyle.Filled,
            Margin = new Padding(0, 8, 0, 0)
        };
        _filledCombo.Items.AddRange(new object[] { "Light", "Dark", "System" });

        _flatCombo = new NexaComboBox
        {
            Width = 240,
            Style = NexaComboBoxStyle.Flat,
            Margin = new Padding(0, 8, 0, 0)
        };
        _flatCombo.Items.AddRange(new object[] { "English", "Spanish", "French", "Sinhala", "Tamil" });

        _placeholderCombo = new NexaComboBox
        {
            Width = 240,
            PlaceholderText = "Select a department…",
            Margin = new Padding(0, 8, 0, 0)
        };
        _placeholderCombo.Items.AddRange(new object[] { "Engineering", "Design", "Marketing", "Finance" });

        _disabledCombo = new NexaComboBox
        {
            Width = 240,
            Enabled = false,
            Text = "Locked choice",
            Margin = new Padding(0, 8, 0, 0)
        };
        _disabledCombo.Items.AddRange(new object[] { "Read-only option" });

        _basicCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();
        _filledCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();
        _flatCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();
        _placeholderCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();

        card.Controls.Add(_basicCombo);
        card.Controls.Add(_filledCombo);
        card.Controls.Add(_flatCombo);
        card.Controls.Add(_placeholderCombo);
        card.Controls.Add(_disabledCombo);
        return card;
    }

    private Control BuildComboDataBoundCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("DataSource / DisplayMember / ValueMember. SelectedValue reflects the bound model."));

        _dataBoundCombo = new NexaComboBox
        {
            Width = 240,
            Margin = new Padding(0, 4, 0, 0)
        };
        var table = new DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "Mathematics");
        table.Rows.Add(2, "Physics");
        table.Rows.Add(3, "Chemistry");
        table.Rows.Add(4, "Biology");
        _dataBoundCombo.DataSource = table;
        _dataBoundCombo.DisplayMember = "Name";
        _dataBoundCombo.ValueMember = "Id";
        _dataBoundCombo.SelectedValueChanged += (_, _) => UpdateComboStatus();

        var status = new Label
        {
            Text = "Bound to DataTable with DisplayMember='Name', ValueMember='Id'.",
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 0)
        };

        card.Controls.Add(_dataBoundCombo);
        card.Controls.Add(status);
        return card;
    }

    private Control BuildComboValidationCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Error and success border colors + native AutoComplete Suggest mode."));

        _errorCombo = new NexaComboBox
        {
            Width = 240,
            Text = "Not a valid option",
            ValidationState = NexaTextValidationState.Error,
            ErrorText = "Please pick a value from the list",
            Margin = new Padding(0, 4, 0, 0)
        };
        _errorCombo.Items.AddRange(new object[] { "Apples", "Oranges", "Pears" });
        _errorCombo.SelectedIndex = -1;

        _successCombo = new NexaComboBox
        {
            Width = 240,
            Text = "Mathematics",
            ValidationState = NexaTextValidationState.Success,
            HelperText = "Subject confirmed.",
            Margin = new Padding(0, 8, 0, 0)
        };
        _successCombo.Items.AddRange(new object[] { "Mathematics", "Physics", "Chemistry" });
        _successCombo.SelectedIndex = 0;

        _autoCompleteCombo = new NexaComboBox
        {
            Width = 240,
            PlaceholderText = "Type to search…",
            AutoCompleteMode = AutoCompleteMode.Suggest,
            AutoCompleteSource = AutoCompleteSource.ListItems,
            Margin = new Padding(0, 8, 0, 0)
        };
        _autoCompleteCombo.Items.AddRange(new object[]
        {
            "Afghanistan", "Albania", "Algeria", "Andorra", "Angola",
            "Bangladesh", "Barbados", "Belarus", "Belgium", "Bhutan",
            "Canada", "Chile", "China", "Colombia", "Croatia",
            "Denmark", "Dominica",
            "Ecuador", "Egypt", "Estonia",
            "Sri Lanka", "Sweden", "Switzerland"
        });

        _errorCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();
        _successCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();
        _autoCompleteCombo.SelectedIndexChanged += (_, _) => UpdateComboStatus();

        card.Controls.Add(_errorCombo);
        card.Controls.Add(_successCombo);
        card.Controls.Add(_autoCompleteCombo);
        return card;
    }

    private Control BuildToggleCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Toggles with sizes, with-text, disabled, and animation."));

        _notif = new NexaToggleSwitch { Text = "Notifications", Checked = true, Margin = new Padding(0, 4, 0, 0) };
        _darkMode = new NexaToggleSwitch { Text = "Dark Mode", Checked = false, Margin = new Padding(0, 8, 0, 0) };
        _autoSave = new NexaToggleSwitch { Text = "Auto Save", Checked = true, Margin = new Padding(0, 8, 0, 0) };
        _smallToggle = new NexaToggleSwitch { Text = "Small", Checked = false, ToggleSize = NexaToggleSize.Small, Margin = new Padding(0, 8, 0, 0) };
        _largeToggle = new NexaToggleSwitch { Text = "Large", Checked = true, ToggleSize = NexaToggleSize.Large, Margin = new Padding(0, 8, 0, 0) };
        _textToggle = new NexaToggleSwitch
        {
            Text = "Show Text",
            Checked = true,
            ShowText = true,
            OnText = "ON",
            OffText = "OFF",
            Margin = new Padding(0, 8, 0, 0)
        };
        _disabledToggle = new NexaToggleSwitch
        {
            Text = "Disabled (off)",
            Checked = false,
            Enabled = false,
            Margin = new Padding(0, 8, 0, 0)
        };

        _notif.CheckedChanged += (_, _) => UpdateToggleStatus();
        _darkMode.CheckedChanged += (_, _) => UpdateToggleStatus();
        _autoSave.CheckedChanged += (_, _) => UpdateToggleStatus();
        _smallToggle.CheckedChanged += (_, _) => UpdateToggleStatus();
        _largeToggle.CheckedChanged += (_, _) => UpdateToggleStatus();
        _textToggle.CheckedChanged += (_, _) => UpdateToggleStatus();

        card.Controls.Add(_notif);
        card.Controls.Add(_darkMode);
        card.Controls.Add(_autoSave);
        card.Controls.Add(_smallToggle);
        card.Controls.Add(_largeToggle);
        card.Controls.Add(_textToggle);
        card.Controls.Add(_disabledToggle);
        return card;
    }

    private Control BuildRegistrationCard()
    {
        var card = CreateDemoCard();
        AddHelperLabel(card, "Small interaction example. The four control families wired together.");

        var form = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 6,
            Padding = new Padding(0, 8, 0, 0)
        };
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));
        form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var courseLabel = new Label { Text = "Course", AutoSize = true, Margin = new Padding(0, 6, 8, 6) };
        _registrationCourse = new NexaComboBox { Width = 220, Style = NexaComboBoxStyle.Outlined, Dock = DockStyle.Fill };
        _registrationCourse.Items.AddRange(new object[]
        {
            "ICT", "Software Engineering", "Graphic Design", "Networking", "Business Management"
        });
        _registrationCourse.SelectedIndex = 0;
        form.Controls.Add(courseLabel, 0, 0);
        form.Controls.Add(_registrationCourse, 1, 0);

        var genderLabel = new Label { Text = "Gender", AutoSize = true, Margin = new Padding(0, 6, 8, 6) };
        var genderGroup = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 4, 0, 0)
        };
        _male = new NexaRadioButton { Text = "Male", Checked = true, Margin = new Padding(0, 0, 0, 4) };
        _female = new NexaRadioButton { Text = "Female", Margin = new Padding(0, 0, 0, 4) };
        _other = new NexaRadioButton { Text = "Other", Margin = new Padding(0, 0, 0, 4) };
        genderGroup.Controls.Add(_male);
        genderGroup.Controls.Add(_female);
        genderGroup.Controls.Add(_other);
        form.Controls.Add(genderLabel, 0, 1);
        form.Controls.Add(genderGroup, 1, 1);

        var notifLabel = new Label { Text = "Notifications", AutoSize = true, Margin = new Padding(0, 6, 8, 6) };
        _registerNotifications = new NexaToggleSwitch { Text = "Send me updates", Checked = true, Dock = DockStyle.Left };
        form.Controls.Add(notifLabel, 0, 2);
        form.Controls.Add(_registerNotifications, 1, 2);

        var termsLabel = new Label { Text = "Accept Terms", AutoSize = true, Margin = new Padding(0, 6, 8, 6) };
        _acceptTerms = new NexaCheckBox { Text = "I have read and accept the terms", Checked = false, Margin = new Padding(0, 6, 0, 6) };
        _acceptTerms.CheckedChanged += (_, _) => { _submit.Enabled = _acceptTerms.Checked; UpdateSubmitStatus(); };
        form.Controls.Add(termsLabel, 0, 3);
        form.Controls.Add(_acceptTerms, 1, 3);

        _submit = new NexaButton { Text = "Submit", SizeMode = NexaButtonSize.Medium, Enabled = false, Margin = new Padding(0, 8, 0, 0) };
        _submit.Click += OnSubmitClick;
        form.Controls.Add(new Label(), 0, 4);
        form.Controls.Add(_submit, 1, 4);

        card.Controls.Add(form);
        return card;
    }

    private static Label AddHelperLabel(Panel card, string text)
    {
        var l = DescriptionLabel(text);
        card.Controls.Add(l);
        return l;
    }

    private void OnSubmitClick(object? sender, System.EventArgs e)
    {
        UpdateSubmitStatus();
    }

    private Control BuildStatusRow()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(0, 16, 0, 0)
        };
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _checkStatus = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Check boxes: (none)" };
        _indeterminateStatus = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Indeterminate: (none)" };
        _radioStatus = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Radios: (none)" };
        _comboStatus = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Combos: (none)" };
        _toggleStatus = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Toggles: (none)" };
        _submitStatus = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 8, 0, 0), Text = "Submit status: accept terms to enable submission." };

        layout.Controls.Add(_checkStatus);
        layout.Controls.Add(_indeterminateStatus);
        layout.Controls.Add(_radioStatus);
        layout.Controls.Add(_comboStatus);
        layout.Controls.Add(_toggleStatus);
        layout.Controls.Add(_submitStatus);
        panel.Controls.Add(layout);

        UpdateCheckStatus();
        UpdateIndeterminateStatus();
        UpdateRadioStatus();
        UpdateComboStatus();
        UpdateToggleStatus();
        UpdateSubmitStatus();
        return panel;
    }

    private void UpdateCheckStatus()
    {
        if (_checkStatus is null) return;
        var parts = new List<string>();
        if (_agree.Checked) parts.Add("agree");
        if (_newsletter.Checked) parts.Add("newsletter");
        if (_marketing.Checked) parts.Add("marketing");
        _checkStatus.Text = $"Check boxes: {(parts.Count == 0 ? "(none)" : string.Join(" + ", parts))}  ·  error: {(_errorCheck.Checked ? "acknowledged" : "required")}  ·  success: {(_successCheck.Checked ? "verified" : "—")}";
    }

    private void UpdateIndeterminateStatus()
    {
        if (_indeterminateStatus is null) return;
        var state = _indeterminate.CheckState switch
        {
            CheckState.Checked => "Checked (all permissions)",
            CheckState.Indeterminate => "Indeterminate (mixed)",
            _ => "Unchecked (no permissions)"
        };
        _indeterminateStatus.Text = $"Tri-state: {state}";
    }

    private void UpdateRadioStatus()
    {
        if (_radioStatus is null) return;
        var gender = _male.Checked ? "Male" : _female.Checked ? "Female" : _other.Checked ? "Other" : "(none)";
        var course = _fullTime.Checked ? "Full Time" : _partTime.Checked ? "Part Time" : "(none)";
        _radioStatus.Text = $"Radios: gender = {gender};  course type = {course}";
    }

    private void UpdateComboStatus()
    {
        if (_comboStatus is null) return;
        var parts = new List<string>();
        if (_basicCombo.SelectedIndex >= 0) parts.Add($"basic={_basicCombo.SelectedItem}");
        if (_dataBoundCombo.SelectedValue is int id) parts.Add($"dataBoundId={id}");
        if (_errorCombo.SelectedIndex >= 0) parts.Add($"error={_errorCombo.SelectedItem}");
        if (_successCombo.SelectedIndex >= 0) parts.Add($"success={_successCombo.SelectedItem}");
        if (_autoCompleteCombo.SelectedIndex >= 0) parts.Add($"autocomplete={_autoCompleteCombo.SelectedItem}");
        _comboStatus.Text = $"Combos: {(parts.Count == 0 ? "(none)" : string.Join("  ·  ", parts))}";
    }

    private void UpdateToggleStatus()
    {
        if (_toggleStatus is null) return;
        var parts = new List<string>();
        if (_notif.Checked) parts.Add("notif ON");
        if (_darkMode.Checked) parts.Add("dark ON");
        if (_autoSave.Checked) parts.Add("auto-save ON");
        if (_smallToggle.Checked) parts.Add("small ON");
        if (_largeToggle.Checked) parts.Add("large ON");
        if (_textToggle.Checked) parts.Add("text ON");
        _toggleStatus.Text = $"Toggles: {(parts.Count == 0 ? "(all off)" : string.Join("  ·  ", parts))}";
    }

    private void UpdateSubmitStatus()
    {
        if (_submitStatus is null) return;
        if (!_acceptTerms.Checked)
        {
            _submitStatus.Text = "Submit status: disabled — accept the terms to enable submission.";
            return;
        }
        var course = _registrationCourse.SelectedItem?.ToString() ?? "(none)";
        var gender = _male.Checked ? "Male" : _female.Checked ? "Female" : _other.Checked ? "Other" : "(none)";
        var notif = _registerNotifications.Checked ? "yes" : "no";
        _submitStatus.Text = $"Submit status: would register — course = {course}, gender = {gender}, notifications = {notif}.";
    }

    private Panel CreateDemoCard()
    {
        var palette = ThemeManager.Current.Palette;
        var card = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 12),
            BackColor = (Color)palette[NexaColorRole.Surface].Value
        };
        card.Paint += (s, e) =>
        {
            using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            var r = card.ClientRectangle;
            r.Width -= 1; r.Height -= 1;
            e.Graphics.DrawRectangle(pen, r);
        };
        return card;
    }

    private static Label DescriptionLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 0, 0, 4),
        ForeColor = (Color)ThemeManager.Current.Palette[NexaColorRole.TextSecondary].Value
    };

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = NexaFormsDpi.CurrentDpi(this);

        BackColor = (Color)palette[NexaColorRole.Background].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        var sectionTitles = new[]
        {
            "NexaCheckBox — normal, checked, indeterminate, disabled",
            "NexaCheckBox — error and success validation",
            "NexaRadioButton — groups (parent-scoped, native)",
            "NexaComboBox — basic, filled, flat, placeholder, disabled",
            "NexaComboBox — DataSource / DisplayMember / ValueMember",
            "NexaComboBox — error, success, AutoComplete",
            "NexaToggleSwitch — basic, sizes, with text, disabled",
            "Registration — controls working together",
            "Live status"
        };
        foreach (Control c in _root.Controls)
        {
            if (c is Label l)
            {
                if (l.Text == "Selection Controls")
                {
                    l.Font = typography.ToFont(NexaTypographyRole.Heading, dpi);
                    l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
                }
                else if (l.Text.StartsWith("NexaComboBox, NexaCheckBox", StringComparison.Ordinal))
                {
                    l.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
                    l.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
                }
                else if (Array.IndexOf(sectionTitles, l.Text) >= 0)
                {
                    l.Font = typography.ToFont(NexaTypographyRole.Title, dpi);
                    l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
                }
            }
        }

        if (_checkStatus is not null)
        {
            _checkStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            _checkStatus.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_indeterminateStatus is not null)
        {
            _indeterminateStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            _indeterminateStatus.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_radioStatus is not null)
        {
            _radioStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            _radioStatus.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_comboStatus is not null)
        {
            _comboStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            _comboStatus.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_toggleStatus is not null)
        {
            _toggleStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            _toggleStatus.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_submitStatus is not null)
        {
            _submitStatus.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            _submitStatus.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }

        // Force-repaint custom-painted controls
        _agree?.Invalidate();
        _newsletter?.Invalidate();
        _marketing?.Invalidate();
        _disabled?.Invalidate();
        _indeterminate?.Invalidate();
        _errorCheck?.Invalidate();
        _successCheck?.Invalidate();
        _acceptTerms?.Invalidate();
        _male?.Invalidate();
        _female?.Invalidate();
        _other?.Invalidate();
        _fullTime?.Invalidate();
        _partTime?.Invalidate();
        _disabledRadio?.Invalidate();
        _basicCombo?.Invalidate();
        _dataBoundCombo?.Invalidate();
        _filledCombo?.Invalidate();
        _flatCombo?.Invalidate();
        _placeholderCombo?.Invalidate();
        _disabledCombo?.Invalidate();
        _errorCombo?.Invalidate();
        _successCombo?.Invalidate();
        _autoCompleteCombo?.Invalidate();
        _registrationCourse?.Invalidate();
        _notif?.Invalidate();
        _darkMode?.Invalidate();
        _autoSave?.Invalidate();
        _smallToggle?.Invalidate();
        _largeToggle?.Invalidate();
        _textToggle?.Invalidate();
        _disabledToggle?.Invalidate();
        _registerNotifications?.Invalidate();
    }
}
