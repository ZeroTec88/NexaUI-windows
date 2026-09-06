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

        _root.Controls.Add(HeaderLabel("Input Controls", NexaTypographyRole.Heading));
        _root.Controls.Add(BodyLabel("Bootstrap-inspired form controls. Label, helper text, validation states, sizes, icons, password, counter, search."));

        _root.Controls.Add(SectionTitle("Form layout — label, placeholder, helper, error, success"));
        _root.Controls.Add(BuildFormExampleCard());

        _root.Controls.Add(SectionTitle("Sizes — Small, Medium, Large"));
        _root.Controls.Add(BuildSizesCard());

        _root.Controls.Add(SectionTitle("Styles — Outline, Filled, Underline"));
        _root.Controls.Add(BuildStylesCard());

        _root.Controls.Add(SectionTitle("Icons, clear button, password, character counter"));
        _root.Controls.Add(BuildAddOnsCard());

        _root.Controls.Add(SectionTitle("Search — debounced, clearable, with results"));
        _root.Controls.Add(BuildSearchBoxCard());

        _root.Controls.Add(SectionTitle("Masked input — phone, date, postal code"));
        _root.Controls.Add(BuildMaskedCard());

        _status = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 16, 0, 0),
            Text = "Switch the theme in the top bar — every control repaints."
        };
        _root.Controls.Add(_status);

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
        var sectionPrefixes = new[] { "Sizes", "Styles", "Form layout", "Icons", "Search", "Masked" };
        foreach (Control c in _root.Controls)
        {
            if (c is Label l && sectionPrefixes.Any(p => l.Text.StartsWith(p, StringComparison.Ordinal)))
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

    private Control BuildFormExampleCard()
    {
        var card = CreateDemoCard();
        var description = DescriptionLabel("Each input has a label, an optional placeholder, and either helper text or an error message.");
        card.Controls.Add(description);

        var name = new NexaTextBox
        {
            Width = 320,
            Label = "Full name",
            PlaceholderText = "Your full name",
            HelperText = "First and last name as it appears on your ID.",
            Margin = new Padding(0, 4, 0, 0)
        };

        var email = new NexaTextBox
        {
            Width = 320,
            Label = "Email address",
            PlaceholderText = "you@example.com",
            Icon = NexaIconKind.User,
            ShowClearButton = true,
            HelperText = "We'll never share your email.",
            Margin = new Padding(0, 12, 0, 0)
        };

        var success = new NexaTextBox
        {
            Width = 320,
            Label = "Username",
            Text = "nuwandave",
            ValidationState = NexaTextValidationState.Success,
            HelperText = "This username is available.",
            Icon = NexaIconKind.Check,
            ShowClearButton = true,
            Margin = new Padding(0, 12, 0, 0)
        };

        var error = new NexaTextBox
        {
            Width = 320,
            Label = "Password",
            Text = "abc",
            UseSystemPasswordChar = true,
            ValidationState = NexaTextValidationState.Error,
            ErrorText = "Password must be at least 8 characters.",
            Margin = new Padding(0, 12, 0, 0)
        };

        var readonlyBox = new NexaTextBox
        {
            Width = 320,
            Label = "Student ID (read-only)",
            Text = "STU-2024-0001",
            ReadOnly = true,
            HelperText = "Assigned by the registrar.",
            Margin = new Padding(0, 12, 0, 0)
        };

        var disabled = new NexaTextBox
        {
            Width = 320,
            Label = "Country (disabled)",
            Text = "Sri Lanka",
            Enabled = false,
            Margin = new Padding(0, 12, 0, 0)
        };

        card.Controls.Add(name);
        card.Controls.Add(email);
        card.Controls.Add(success);
        card.Controls.Add(error);
        card.Controls.Add(readonlyBox);
        card.Controls.Add(disabled);
        return card;
    }

    private Control BuildSizesCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Small (32px), Medium (40px), Large (48px)."));

        var small = new NexaTextBox
        {
            Width = 320,
            InputSize = NexaInputSize.Small,
            Label = "Small",
            PlaceholderText = "Compact input",
            Margin = new Padding(0, 4, 0, 0)
        };
        var medium = new NexaTextBox
        {
            Width = 320,
            InputSize = NexaInputSize.Medium,
            Label = "Medium",
            PlaceholderText = "Default input",
            Margin = new Padding(0, 12, 0, 0)
        };
        var large = new NexaTextBox
        {
            Width = 320,
            InputSize = NexaInputSize.Large,
            Label = "Large",
            PlaceholderText = "Hero input",
            Margin = new Padding(0, 12, 0, 0)
        };

        card.Controls.Add(small);
        card.Controls.Add(medium);
        card.Controls.Add(large);
        return card;
    }

    private Control BuildStylesCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Outline (default), Filled, and Underline."));

        var outline = new NexaTextBox
        {
            Width = 320,
            Style = NexaInputStyle.Outline,
            Label = "Outline",
            PlaceholderText = "Bordered with rounded corners",
            Margin = new Padding(0, 4, 0, 0)
        };
        var filled = new NexaTextBox
        {
            Width = 320,
            Style = NexaInputStyle.Filled,
            Label = "Filled",
            PlaceholderText = "Solid background, subtle border",
            Margin = new Padding(0, 12, 0, 0)
        };
        var underline = new NexaTextBox
        {
            Width = 320,
            Style = NexaInputStyle.Underline,
            Label = "Underline",
            PlaceholderText = "Bottom border only",
            Margin = new Padding(0, 12, 0, 0)
        };

        card.Controls.Add(outline);
        card.Controls.Add(filled);
        card.Controls.Add(underline);
        return card;
    }

    private Control BuildAddOnsCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Icons, clear button, password, and character counter."));

        var search = new NexaTextBox
        {
            Width = 320,
            Label = "Search",
            PlaceholderText = "Type to search…",
            Icon = NexaIconKind.Search,
            ShowClearButton = true,
            Margin = new Padding(0, 4, 0, 0)
        };
        var info = new NexaTextBox
        {
            Width = 320,
            Label = "Website",
            Text = "nexaui.dev",
            Icon = NexaIconKind.Info,
            ShowClearButton = true,
            Margin = new Padding(0, 12, 0, 0)
        };
        var pwd = new NexaTextBox
        {
            Width = 320,
            Label = "Password",
            UseSystemPasswordChar = true,
            PlaceholderText = "Enter a strong password",
            Icon = NexaIconKind.Settings,
            ShowClearButton = true,
            Margin = new Padding(0, 12, 0, 0)
        };
        var counter = new NexaTextBox
        {
            Width = 320,
            Label = "Bio",
            MaxLength = 160,
            PlaceholderText = "Tell us about yourself",
            ShowCounter = true,
            Multiline = true,
            Margin = new Padding(0, 12, 0, 0)
        };
        counter.Height = 90;

        card.Controls.Add(search);
        card.Controls.Add(info);
        card.Controls.Add(pwd);
        card.Controls.Add(counter);
        return card;
    }

    private Control BuildSearchBoxCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Debounced search with clear button and Escape-to-clear."));

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
        card.Controls.Add(DescriptionLabel("Themed masked inputs with built-in labels and validation icons."));

        var phone = new NexaMaskedTextBox { Width = 240, Mask = "(000) 000-0000", Label = "Phone number" };
        var date = new NexaMaskedTextBox { Width = 240, Mask = "00/00/0000", Label = "Date of birth" };
        var postal = new NexaMaskedTextBox { Width = 240, Mask = "00000", Label = "Postal code" };

        var phoneValue = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Value: (empty)" };
        var dateValue = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Value: (empty)" };
        var postalValue = new Label { Dock = DockStyle.Top, AutoSize = true, Margin = new Padding(0, 4, 0, 0), Text = "Value: (empty)" };

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

        var phoneStack = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        phoneStack.Controls.Add(phone);
        phoneStack.Controls.Add(phoneValue);

        var dateStack = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        dateStack.Controls.Add(date);
        dateStack.Controls.Add(dateValue);

        var postalStack = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            Padding = new Padding(0, 8, 0, 0)
        };
        postalStack.Controls.Add(postal);
        postalStack.Controls.Add(postalValue);

        card.Controls.Add(phoneStack);
        card.Controls.Add(dateStack);
        card.Controls.Add(postalStack);
        return card;
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
