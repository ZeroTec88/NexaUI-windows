using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GalleryShell : Form
{
    public static event System.EventHandler<string>? NavigationRequested;

    private static readonly (string Key, string Title, Func<UserControl> Factory)[] Pages =
    {
        ("getting-started", "Getting Started", () => new GettingStartedScreen()),
        ("themes", "Themes", () => new GalleryThemesScreen()),
        ("basic", "Basic Controls", () => new GalleryBasicControlsScreen()),
        ("input", "Input Controls", () => new GalleryInputControlsScreen()),
        ("selection", "Selection Controls", () => new GallerySelectionControlsScreen()),
        ("layout", "Layout Controls", () => new GalleryLayoutScreen()),
        ("feedback", "Feedback Controls", () => new GalleryFeedbackScreen()),
        ("navigation", "Navigation Controls", () => new GalleryNavigationScreen()),
        ("dialogs", "Dialogs", () => new GalleryDialogsScreen()),
    };

    private static readonly (string Key, string Title)[] NavItems =
    {
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
        ("advanced", "Advanced"),
    };

    private readonly TableLayoutPanel _root;
    private readonly Panel _topBar;
    private readonly Label _brandLabel;
    private readonly Label _searchHint;
    private readonly TableLayoutPanel _topBarLayout;
    private readonly NexaButton _themeToggle;
    private readonly SplitContainer _split;
    private readonly TableLayoutPanel _navPanel;
    private readonly Dictionary<string, NexaButton> _navButtons = new();
    private readonly TableLayoutPanel _contentPanel;
    private readonly Dictionary<string, UserControl> _contentCache = new();
    private string _currentKey = "getting-started";
    private bool _isDark;

    public GalleryShell()
    {
        Text = "NexaUI Gallery — C#";
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(1280, 820);
        MinimumSize = new Size(960, 640);

        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2
        };
        _root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
        _root.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _topBar = new Panel { Dock = DockStyle.Fill };
        _topBarLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            Padding = new Padding(20, 0, 20, 0)
        };
        _topBarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _topBarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _topBarLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        _brandLabel = new Label
        {
            Text = "NexaUI",
            AutoSize = true,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point)
        };

        _searchHint = new Label
        {
            Text = "Search components…",
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(14, 0, 0, 0),
            Font = new Font("Segoe UI", 9F, FontStyle.Italic, GraphicsUnit.Point)
        };

        _themeToggle = new NexaButton
        {
            Text = "Toggle Theme",
            IconKind = NexaIconKind.Sun,
            Style = NexaButtonStyle.Outline,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        _themeToggle.Click += (_, _) => ThemeManager.ToggleLightDark();

        _topBarLayout.Controls.Add(_brandLabel, 0, 0);
        _topBarLayout.Controls.Add(_searchHint, 1, 0);
        _topBarLayout.Controls.Add(_themeToggle, 2, 0);
        _topBar.Controls.Add(_topBarLayout);

        _split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 240,
            FixedPanel = FixedPanel.Panel1,
            Panel1MinSize = 180
        };

        _navPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Padding = new Padding(8, 12, 8, 12)
        };
        _navPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        foreach (var (key, title) in NavItems)
        {
            var btn = new NexaButton
            {
                Text = title,
                Style = NexaButtonStyle.Ghost,
                Dock = DockStyle.Top,
                Margin = new Padding(0, 0, 0, 2),
                Height = 32,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 12, 0)
            };
            if (!Pages.Any(p => p.Key == key))
            {
                btn.Enabled = false;
                btn.Text = $"{title}  (coming soon)";
            }
            else
            {
                var capturedKey = key;
                btn.Click += (_, _) => NavigateTo(capturedKey);
            }
            _navButtons[key] = btn;
            _navPanel.Controls.Add(btn);
        }
        _split.Panel1.Controls.Add(_navPanel);

        _contentPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            ColumnCount = 1,
            RowCount = 1
        };
        _contentPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _split.Panel2.Controls.Add(_contentPanel);

        _root.Controls.Add(_topBar, 0, 0);
        _root.Controls.Add(_split, 0, 1);
        Controls.Add(_root);

        NavigationRequested += HandleNavigationRequested;
        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);

        HandleCreated += (_, _) =>
        {
            ApplyTheme(ThemeManager.Current);
            NavigateTo(_currentKey);
        };
    }

    private void HandleNavigationRequested(object? sender, string key)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => HandleNavigationRequested(sender, key));
            return;
        }
        NavigateTo(key);
    }

    public static void RequestNavigate(string key)
    {
        NavigationRequested?.Invoke(null, key);
    }

    private void NavigateTo(string key)
    {
        if (!Pages.Any(p => p.Key == key)) return;
        if (!_contentCache.TryGetValue(key, out var page))
        {
            page = Pages.First(p => p.Key == key).Factory();
            page.Dock = DockStyle.Fill;
            _contentCache[key] = page;
        }

        _contentPanel.Controls.Clear();
        _contentPanel.Controls.Add(page, 0, 0);
        _currentKey = key;
        UpdateActiveNavButton();
    }

    private void UpdateActiveNavButton()
    {
        foreach (var (key, btn) in _navButtons)
        {
            btn.Style = (key == _currentKey) ? NexaButtonStyle.Primary : NexaButtonStyle.Ghost;
        }
    }

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
        _isDark = theme.IsDark;

        BackColor = (Color)palette[NexaColorRole.Background].Value;

        _topBar.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        _topBar.Paint -= TopBarBorderPainter;
        _topBar.Paint += TopBarBorderPainter;

        _brandLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        _brandLabel.Font = typography.ToFont(NexaTypographyRole.Title, dpi);
        _searchHint.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        _searchHint.Font = typography.ToFont(NexaTypographyRole.Body, dpi);

        _themeToggle.IconKind = theme.IsDark ? NexaIconKind.Sun : NexaIconKind.Moon;
        _themeToggle.Text = theme.IsDark ? "Light" : "Dark";

        _split.BackColor = (Color)palette[NexaColorRole.Border].Value;
        _split.Panel1.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        _split.Panel2.BackColor = (Color)palette[NexaColorRole.Background].Value;

        _navPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;

        UpdateActiveNavButton();

        // Re-paint cached pages by raising a ThemeChanged already done — they subscribe themselves
    }

    private void TopBarBorderPainter(object? sender, PaintEventArgs e)
    {
        var palette = ThemeManager.Current.Palette;
        using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
        e.Graphics.DrawLine(pen, 0, _topBar.Height - 1, _topBar.Width, _topBar.Height - 1);
    }
}