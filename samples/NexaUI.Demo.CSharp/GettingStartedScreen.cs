using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GettingStartedScreen : UserControl
{
    public GettingStartedScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = SystemColors.Control;

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Padding = new Padding(32, 28, 32, 28)
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        var title = new Label
        {
            Text = "Welcome to NexaUI",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        var subtitle = new Label
        {
            Text = "A modern Windows Forms component library for .NET 10.",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 24)
        };
        var body = new Label
        {
            Text =
                "NexaUI ships a coherent design system — colors, typography, spacing, " +
                "corner radii, borders, and elevation — exposed through the ThemeManager. " +
                "Use the navigation on the left to explore available pages.\n\n" +
                "This gallery is itself a NexaUI consumer: every page is rendered with " +
                "themed controls, including the navigation and the theme toggle at the top right.",
            Dock = DockStyle.Top,
            AutoSize = true,
            MaximumSize = new Size(900, 0),
            Margin = new Padding(0, 0, 0, 24)
        };
        var actions = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            Margin = new Padding(0, 0, 0, 16)
        };
        for (var i = 0; i < 3; i++)
            actions.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var exploreBtn = new NexaButton
        {
            Text = "Explore Themes",
            IconKind = NexaIconKind.Sun,
            Style = NexaButtonStyle.Primary,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 12, 0)
        };
        var buttonsBtn = new NexaButton
        {
            Text = "See Buttons",
            IconKind = NexaIconKind.ChevronRight,
            IconPosition = NexaButtonIconPosition.Right,
            Style = NexaButtonStyle.Outline,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 12, 0)
        };
        var loadingBtn = new NexaButton
        {
            Text = "Try Loading",
            Style = NexaButtonStyle.Secondary,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        loadingBtn.Click += (_, _) =>
        {
            loadingBtn.Loading = true;
            var captured = loadingBtn;
            var timer = new System.Windows.Forms.Timer { Interval = 1500 };
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                timer.Dispose();
                captured.Loading = false;
            };
            timer.Start();
        };

        exploreBtn.Click += (_, _) => GalleryShell.RequestNavigate("themes");
        buttonsBtn.Click += (_, _) => GalleryShell.RequestNavigate("basic");

        actions.Controls.Add(exploreBtn, 0, 0);
        actions.Controls.Add(buttonsBtn, 1, 0);
        actions.Controls.Add(loadingBtn, 2, 0);

        var footer = new Label
        {
            Text = "Target: net10.0-windows  ·  Languages: C# and VB.NET",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 8, 0, 0)
        };

        root.Controls.Add(title);
        root.Controls.Add(subtitle);
        root.Controls.Add(body);
        root.Controls.Add(actions);
        root.Controls.Add(footer);

        Controls.Add(root);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => ApplyTheme(e.Current);

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

        foreach (Control c in Controls)
        {
            if (c is Label l)
            {
                l.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
                l.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
            }
        }

        // Title gets a larger role
        if (Controls[0] is TableLayoutPanel root && root.Controls.Count >= 2)
        {
            if (root.Controls[0] is Label titleLabel)
            {
                titleLabel.Font = typography.ToFont(NexaTypographyRole.Display, dpi);
                titleLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
            }
            if (root.Controls[1] is Label subtitleLabel)
            {
                subtitleLabel.Font = typography.ToFont(NexaTypographyRole.Title, dpi);
                subtitleLabel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
            }
            // last label is the footer
            var last = root.Controls[root.Controls.Count - 1];
            if (last is Label footerLabel)
            {
                footerLabel.Font = typography.ToFont(NexaTypographyRole.Caption, dpi);
                footerLabel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
            }
        }
    }
}