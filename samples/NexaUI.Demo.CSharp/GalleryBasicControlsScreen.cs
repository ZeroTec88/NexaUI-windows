using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GalleryBasicControlsScreen : UserControl
{
    private readonly TableLayoutPanel _root;
    private readonly Label _status;

    public GalleryBasicControlsScreen()
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
            Text = "Basic Controls",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        };
        var intro = new Label
        {
            Text = "NexaButton — styles, sizes, icons, loading, and disabled states.",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };

        _root.Controls.Add(header);
        _root.Controls.Add(intro);
        _root.Controls.Add(SectionTitle("Styles"));
        _root.Controls.Add(BuildStylesGrid());
        _root.Controls.Add(SectionTitle("Sizes"));
        _root.Controls.Add(BuildSizesGrid());
        _root.Controls.Add(SectionTitle("Icons & loading"));
        _root.Controls.Add(BuildIconAndLoadingGrid());
        _root.Controls.Add(SectionTitle("Disabled"));
        _root.Controls.Add(BuildDisabledGrid());

        _status = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 16, 0, 0),
            Text = "Click any button to see the click handler in action."
        };
        _root.Controls.Add(_status);

        Controls.Add(_root);

        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChangedStatic;
        ThemeManager.ThemeChanged += OnThemeChangedStatic;

        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnThemeChangedStatic(object? sender, ThemeChangedEventArgs e) { }

    private static Label SectionTitle(string title) => new()
    {
        Text = title,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 16, 0, 8)
    };

    private void ApplyTheme(ITheme theme)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyTheme(theme));
            return;
        }
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = NexaFormsDpi.CurrentDpi(this);

        BackColor = (Color)palette[NexaColorRole.Background].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        // Header + intro styling
        if (_root.Controls[0] is Label headerLabel)
        {
            headerLabel.Font = typography.ToFont(NexaTypographyRole.Heading, dpi);
            headerLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        }
        if (_root.Controls[1] is Label introLabel)
        {
            introLabel.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
            introLabel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        }
        // Section titles + status
        foreach (Control c in _root.Controls)
        {
            if (c is Label l && (l.Text == "Styles" || l.Text == "Sizes" || l.Text == "Icons & loading" || l.Text == "Disabled"))
            {
                l.Font = typography.ToFont(NexaTypographyRole.Title, dpi);
                l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
            }
        }
        _status.Font = typography.ToFont(NexaTypographyRole.Caption, dpi);
        _status.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
    }

    private Control BuildStylesGrid()
    {
        var wrap = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 4,
            Padding = new Padding(0)
        };
        for (var i = 0; i < 4; i++) wrap.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

        var styles = new[] { NexaButtonStyle.Primary, NexaButtonStyle.Secondary, NexaButtonStyle.Outline, NexaButtonStyle.Ghost,
                             NexaButtonStyle.Success, NexaButtonStyle.Warning, NexaButtonStyle.Danger };

        var row = 0;
        for (var i = 0; i < styles.Length; i += 4)
        {
            wrap.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            wrap.RowCount++;
            for (var c = 0; c < 4 && i + c < styles.Length; c++)
            {
                var style = styles[i + c];
                var btn = new NexaButton
                {
                    Text = style.ToString(),
                    Style = style,
                    Dock = DockStyle.Fill,
                    Margin = new Padding(4),
                    AutoSize = true,
                    AutoSizeMode = AutoSizeMode.GrowAndShrink
                };
                btn.Click += (_, _) => NotifyClick(style.ToString());
                wrap.Controls.Add(btn, c, row);
            }
            row++;
        }
        return wrap;
    }

    private Control BuildSizesGrid()
    {
        var wrap = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        var small = new NexaButton { Text = "Small", SizeMode = NexaButtonSize.Small, AutoSize = true };
        var medium = new NexaButton { Text = "Medium", SizeMode = NexaButtonSize.Medium, AutoSize = true };
        var large = new NexaButton { Text = "Large", SizeMode = NexaButtonSize.Large, AutoSize = true };

        small.Click += (_, _) => NotifyClick("Small");
        medium.Click += (_, _) => NotifyClick("Medium");
        large.Click += (_, _) => NotifyClick("Large");

        wrap.Controls.AddRange(new Control[] { small, medium, large });
        return wrap;
    }

    private Control BuildIconAndLoadingGrid()
    {
        var wrap = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            Padding = new Padding(0)
        };
        for (var i = 0; i < 3; i++) wrap.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

        var iconLeft = new NexaButton { Text = "Save", IconKind = NexaIconKind.Check, IconPosition = NexaButtonIconPosition.Left, Style = NexaButtonStyle.Primary, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(4) };
        var iconRight = new NexaButton { Text = "Next", IconKind = NexaIconKind.ChevronRight, IconPosition = NexaButtonIconPosition.Right, Style = NexaButtonStyle.Secondary, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(4) };
        var ghostSearch = new NexaButton { Text = "Search", IconKind = NexaIconKind.Search, Style = NexaButtonStyle.Ghost, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(4) };

        var loading = new NexaButton { Text = "Submit", Style = NexaButtonStyle.Primary, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(4) };
        var loadingSecondary = new NexaButton { Text = "Process", Style = NexaButtonStyle.Secondary, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(4) };
        var loadingWithIcon = new NexaButton { Text = "Sync", IconKind = NexaIconKind.Settings, Style = NexaButtonStyle.Outline, AutoSize = true, Dock = DockStyle.Fill, Margin = new Padding(4) };

        loading.Click += (_, _) => StartLoading(loading, "Submitting...", 1500);
        loadingSecondary.Click += (_, _) => StartLoading(loadingSecondary, "Processing...", 1200);
        loadingWithIcon.Click += (_, _) => StartLoading(loadingWithIcon, "Syncing...", 1400);

        iconLeft.Click += (_, _) => NotifyClick("Save");
        iconRight.Click += (_, _) => NotifyClick("Next");
        ghostSearch.Click += (_, _) => NotifyClick("Search");

        wrap.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        wrap.RowCount++;
        wrap.Controls.Add(iconLeft, 0, 0);
        wrap.Controls.Add(iconRight, 1, 0);
        wrap.Controls.Add(ghostSearch, 2, 0);

        wrap.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        wrap.RowCount++;
        wrap.Controls.Add(loading, 0, 1);
        wrap.Controls.Add(loadingSecondary, 1, 1);
        wrap.Controls.Add(loadingWithIcon, 2, 1);

        return wrap;
    }

    private static void StartLoading(NexaButton btn, string loadingText, int durationMs)
    {
        btn.Loading = true;
        btn.LoadingText = loadingText;
        var captured = btn;
        var timer = new System.Windows.Forms.Timer { Interval = durationMs };
        timer.Tick += (s, e) =>
        {
            timer.Stop();
            timer.Dispose();
            captured.Loading = false;
        };
        timer.Start();
    }

    private Control BuildDisabledGrid()
    {
        var wrap = new FlowLayoutPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };

        var styles = new[] { NexaButtonStyle.Primary, NexaButtonStyle.Outline, NexaButtonStyle.Ghost, NexaButtonStyle.Danger };
        foreach (var style in styles)
        {
            var btn = new NexaButton
            {
                Text = $"{style} (disabled)",
                Style = style,
                Enabled = false,
                AutoSize = true,
                Margin = new Padding(4)
            };
            wrap.Controls.Add(btn);
        }
        return wrap;
    }

    private void NotifyClick(string caption)
    {
        _status.Text = $"Clicked: {caption} at {DateTime.Now:HH:mm:ss}";
    }
}