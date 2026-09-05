using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GalleryInputControlsScreen : UserControl
{
    private static readonly string[] SampleStudents =
    {
        "Amal Perera", "Amal Silva", "Amal Fernando",
        "Nimal Jayasuriya", "Nimal Bandara",
        "Kamal Rathnayake", "Kamal Wijesinghe",
        "Sunil Perera", "Ruwan Dias", "Saman Kumara"
    };

    private readonly TableLayoutPanel _root;
    private readonly Label _status;

    private NexaSearchBox _search = null!;
    private ListBox _searchResults = null!;

    public GalleryInputControlsScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;

        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Padding = new Padding(32, 24, 32, 24)
        };
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var header = new Label
        {
            Text = "Input Controls",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        };
        var intro = new Label
        {
            Text = "NexaTextBox, NexaSearchBox, NexaMaskedTextBox — themed wrappers around native WinForms editors.",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };

        _root.Controls.Add(header);
        _root.Controls.Add(intro);

        _root.Controls.Add(SectionTitle("NexaTextBox — styles"));
        _root.Controls.Add(BuildTextBoxStylesCard());

        _root.Controls.Add(SectionTitle("NexaTextBox — placeholder, clear, helper, error, success"));
        _root.Controls.Add(BuildTextBoxStatesCard());

        _root.Controls.Add(SectionTitle("NexaTextBox — password and character counter"));
        _root.Controls.Add(BuildPasswordAndCounterCard());

        _root.Controls.Add(SectionTitle("NexaSearchBox — debounced search"));
        _root.Controls.Add(BuildSearchBoxCard());

        _root.Controls.Add(SectionTitle("NexaMaskedTextBox — phone, date, postal code"));
        _root.Controls.Add(BuildMaskedCard());

        _status = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 16, 0, 0),
            Text = "Try the controls — they all respond to light/dark theme switching."
        };
        _root.Controls.Add(_status);

        Controls.Add(_root);

        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);
        Disposed += OnSelfDisposed;

        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnSelfDisposed(object? sender, System.EventArgs e) => ThemeManager.ThemeChanged -= OnSelfDisposed;

    private static Label SectionTitle(string title) => new()
    {
        Text = title,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 16, 0, 8)
    };

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = NexaFormsDpi.CurrentDpi(this);

        BackColor = (Color)palette[NexaColorRole.Background].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        if (_root.Controls.Count > 0 && _root.Controls[0] is Label l0)
        {
            l0.Font = typography.ToFont(NexaTypographyRole.Heading, dpi);
            l0.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_root.Controls.Count > 1 && _root.Controls[1] is Label l1)
        {
            l1.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            l1.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        }
        var sectionTitles = new[]
        {
            "NexaTextBox — styles",
            "NexaTextBox — placeholder, clear, helper, error, success",
            "NexaTextBox — password and character counter",
            "NexaSearchBox — debounced search",
            "NexaMaskedTextBox — phone, date, postal code"
        };
        foreach (Control c in _root.Controls)
        {
            if (c is Label l && Array.IndexOf(sectionTitles, l.Text) >= 0)
            {
                l.Font = typography.ToFont(NexaTypographyRole.Title, dpi);
                l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
            }
        }
        _status.Font = typography.ToFont(NexaTypographyRole.Caption, dpi);
        _status.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;

        if (_searchResults is not null)
            _searchResults.BackColor = (Color)palette[NexaColorRole.Surface].Value;
    }

    private Control BuildTextBoxStylesCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Default, Filled, Outlined, and Flat styles use the active theme."));

        var stack = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        var styles = new[] { NexaTextBoxStyle.Default, NexaTextBoxStyle.Filled, NexaTextBoxStyle.Outlined, NexaTextBoxStyle.Flat };
        foreach (var s in styles)
        {
            var box = new NexaTextBox { Text = $"Style: {s}", Style = s, Width = 360, Margin = new Padding(0, 4, 0, 4) };
            stack.Controls.Add(box);
        }
        card.Controls.Add(stack);
        return card;
    }

    private Control BuildTextBoxStatesCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Validation states, placeholder, helper, and error text are all theme-aware."));

        var nameBox = new NexaTextBox { Width = 360, PlaceholderText = "Enter student name", Margin = new Padding(0, 8, 0, 0), HelperText = "First name and last name." };
        var emailBox = new NexaTextBox { Width = 360, PlaceholderText = "example@email.com", Margin = new Padding(0, 8, 0, 0), ShowClearButton = true };
        var successBox = new NexaTextBox { Width = 360, Text = "Available username", Margin = new Padding(0, 8, 0, 0), ValidationState = NexaTextValidationState.Success, HelperText = "Looks good." };
        var errorBox = new NexaTextBox
        {
            Width = 360,
            Text = "Invalid email",
            Margin = new Padding(0, 8, 0, 0),
            ValidationState = NexaTextValidationState.Error,
            ErrorText = "Email address is not valid."
        };
        var disabledBox = new NexaTextBox { Width = 360, Text = "Disabled field", Margin = new Padding(0, 8, 0, 0), Enabled = false };
        var readonlyBox = new NexaTextBox { Width = 360, Text = "Read-only field", Margin = new Padding(0, 8, 0, 0), ReadOnly = true };

        card.Controls.Add(nameBox);
        card.Controls.Add(emailBox);
        card.Controls.Add(successBox);
        card.Controls.Add(errorBox);
        card.Controls.Add(disabledBox);
        card.Controls.Add(readonlyBox);

        return card;
    }

    private Control BuildPasswordAndCounterCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Password mode and live character counter."));

        var pwd = new NexaTextBox
        {
            Width = 360,
            UseSystemPasswordChar = true,
            PlaceholderText = "Enter password",
            ShowClearButton = true,
            Margin = new Padding(0, 8, 0, 0)
        };
        var counter = new NexaTextBox
        {
            Width = 360,
            MaxLength = 500,
            PlaceholderText = "Type to see the counter",
            CharacterCounterEnabled = true,
            Margin = new Padding(0, 8, 0, 0)
        };
        card.Controls.Add(pwd);
        card.Controls.Add(counter);
        return card;
    }

    private Control BuildSearchBoxCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Search students... typing updates results. Clear button, Escape, and debounce all work."));

        var search = new NexaSearchBox
        {
            Width = 380,
            PlaceholderText = "Search students...",
            SearchDelayMs = 200
        };
        var list = new ListBox
        {
            Dock = DockStyle.Top,
            Height = 160,
            IntegralHeight = false,
            Margin = new Padding(0, 8, 0, 0),
            BorderStyle = BorderStyle.None
        };
        _search = search;
        _searchResults = list;

        search.SearchChanged += (_, _) => RunSearch(search.SearchText);
        search.Cleared += (_, _) => { list.Items.Clear(); };
        search.SearchBoxKeyDown += (_, _) => { };

        RunSearch(string.Empty);

        card.Controls.Add(search);
        card.Controls.Add(list);
        return card;
    }

    private void RunSearch(string query)
    {
        if (_searchResults is null) return;
        _searchResults.Items.Clear();
        if (string.IsNullOrWhiteSpace(query))
        {
            _searchResults.Items.Add("Type to search the local sample list.");
            return;
        }
        var q = query.Trim();
        var hits = 0;
        foreach (var name in SampleStudents)
        {
            if (name.Contains(q, StringComparison.OrdinalIgnoreCase))
            {
                _searchResults.Items.Add(name);
                hits++;
            }
        }
        if (hits == 0) _searchResults.Items.Add("No results.");
    }

    private Control BuildMaskedCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Native masked input with themed border and live validation status."));

        var phone = new NexaMaskedTextBox { Width = 220, Mask = "(000) 000-0000", Margin = new Padding(0, 8, 0, 4) };
        var date = new NexaMaskedTextBox { Width = 220, Mask = "00/00/0000", Margin = new Padding(0, 8, 0, 4) };
        var postal = new NexaMaskedTextBox { Width = 220, Mask = "00000", Margin = new Padding(0, 8, 0, 4) };

        var phoneValue = new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Value: (empty)" };
        var dateValue = new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Value: (empty)" };
        var postalValue = new Label { Dock = DockStyle.Top, AutoSize = true, Text = "Value: (empty)" };

        phone.TextChanged += (_, _) =>
        {
            phoneValue.Text = $"Value: {(string.IsNullOrEmpty(phone.Text) ? "(empty)" : phone.Text)}  ·  Completed: {phone.MaskCompleted}";
        };
        date.TextChanged += (_, _) =>
        {
            dateValue.Text = $"Value: {(string.IsNullOrEmpty(date.Text) ? "(empty)" : date.Text)}  ·  Completed: {date.MaskCompleted}";
        };
        postal.TextChanged += (_, _) =>
        {
            postalValue.Text = $"Value: {(string.IsNullOrEmpty(postal.Text) ? "(empty)" : postal.Text)}  ·  Completed: {postal.MaskCompleted}";
        };

        card.Controls.Add(LabeledRow("Phone Number", phone, phoneValue));
        card.Controls.Add(LabeledRow("Date", date, dateValue));
        card.Controls.Add(LabeledRow("Postal Code", postal, postalValue));
        return card;
    }

    private static Control LabeledRow(string label, Control input, Control valueLabel)
    {
        var wrap = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1
        };
        wrap.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        var lbl = new Label { Text = label, Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 8, 0, 4) };
        wrap.Controls.Add(lbl);
        wrap.Controls.Add(input);
        wrap.Controls.Add(valueLabel);
        return wrap;
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
}