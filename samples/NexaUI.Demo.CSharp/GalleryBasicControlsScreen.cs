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

        _root.Controls.Add(SectionTitle("NexaLabel — styles"));
        _root.Controls.Add(BuildLabelStylesCard());

        _root.Controls.Add(SectionTitle("NexaLabel — alignment & disabled"));
        _root.Controls.Add(BuildLabelAlignmentCard());

        _root.Controls.Add(SectionTitle("NexaLinkLabel"));
        _root.Controls.Add(BuildLinkLabelCard());

        _root.Controls.Add(SectionTitle("NexaSeparator"));
        _root.Controls.Add(BuildSeparatorCard());

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
            if (c is Label l && (l.Text == "Styles" || l.Text == "Sizes" || l.Text == "Icons & loading" || l.Text == "Disabled"
                || l.Text.StartsWith("NexaLabel") || l.Text == "NexaLinkLabel" || l.Text == "NexaSeparator"))
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

    private Control BuildLabelStylesCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Modern themed text display control."));
        card.Controls.Add(Spacer());

        var styles = new[]
        {
            NexaLabelStyle.Default, NexaLabelStyle.Heading, NexaLabelStyle.Subheading,
            NexaLabelStyle.Caption, NexaLabelStyle.Muted, NexaLabelStyle.Success,
            NexaLabelStyle.Warning, NexaLabelStyle.Danger, NexaLabelStyle.Info
        };
        var samples = new[]
        {
            "Default Label", "Heading Label", "Subheading Label", "Caption Label",
            "Muted label text", "Success label", "Warning label", "Danger label", "Info label"
        };

        for (var i = 0; i < styles.Length; i++)
        {
            var row = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                WrapContents = false
            };
            var tag = new Label
            {
                Text = $"{styles[i],-12}",
                Dock = DockStyle.Left,
                Width = 140,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = (Color)ThemeManager.Current.Palette[NexaColorRole.TextSecondary].Value
            };
            var lbl = new NexaLabel
            {
                Text = samples[i],
                LabelStyle = styles[i],
                AutoSize = true,
                Dock = DockStyle.Left
            };
            row.Controls.Add(tag);
            row.Controls.Add(lbl);
            card.Controls.Add(row);
        }
        return card;
    }

    private Control BuildLabelAlignmentCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Alignment and disabled rendering."));
        card.Controls.Add(Spacer());

        var left = new NexaLabel { Text = "Left aligned", LabelStyle = NexaLabelStyle.Default, TextAlign = ContentAlignment.MiddleLeft, AutoSize = false, Width = 360, Height = 28, Dock = DockStyle.Top };
        var center = new NexaLabel { Text = "Center aligned", LabelStyle = NexaLabelStyle.Heading, TextAlign = ContentAlignment.MiddleCenter, AutoSize = false, Width = 360, Height = 36, Dock = DockStyle.Top };
        var right = new NexaLabel { Text = "Right aligned", LabelStyle = NexaLabelStyle.Subheading, TextAlign = ContentAlignment.MiddleRight, AutoSize = false, Width = 360, Height = 32, Dock = DockStyle.Top };
        var disabled = new NexaLabel { Text = "Disabled label (muted, no interaction)", LabelStyle = NexaLabelStyle.Default, Enabled = false, AutoSize = true, Dock = DockStyle.Top };

        card.Controls.Add(left);
        card.Controls.Add(center);
        card.Controls.Add(right);
        card.Controls.Add(Spacer());
        card.Controls.Add(disabled);
        return card;
    }

    private Control BuildLinkLabelCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Theme-aware hyperlinks. The demo does not launch external sites; LinkClicked is delegated to the consumer."));
        card.Controls.Add(Spacer());

        var docs = new NexaLinkLabel { Text = "Open Documentation", AutoSize = true, Dock = DockStyle.Top };
        docs.LinkClicked += (_, _) => _status.Text = "Docs link clicked.";

        var web = new NexaLinkLabel { Text = "Visit Website (demo)", AutoSize = true, Dock = DockStyle.Top };
        web.LinkClicked += (_, _) => _status.Text = "Website link clicked.";

        var another = new NexaLinkLabel { Text = "Another Link", AutoSize = true, Dock = DockStyle.Top };
        another.LinkClicked += (_, _) => _status.Text = "Another link clicked.";

        var visited = new NexaLinkLabel { Text = "Visited Link", AutoSize = true, Dock = DockStyle.Top, Visited = true };
        visited.LinkClicked += (_, _) => { visited.Visited = true; _status.Text = "Visited link clicked."; };

        var disabled = new NexaLinkLabel { Text = "Disabled Link", AutoSize = true, Dock = DockStyle.Top, Enabled = false };

        card.Controls.Add(docs);
        card.Controls.Add(web);
        card.Controls.Add(another);
        card.Controls.Add(visited);
        card.Controls.Add(disabled);
        return card;
    }

    private Control BuildSeparatorCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Horizontal and vertical themed dividers. Switch the global theme to verify they update."));
        card.Controls.Add(Spacer());

        var row = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2
        };
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
        row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));

        var leftCol = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        leftCol.Controls.Add(new NexaLabel { Text = "Above the separator", LabelStyle = NexaLabelStyle.Default, AutoSize = true });
        leftCol.Controls.Add(new NexaSeparator { Dock = DockStyle.Top, Width = 360, Margin = new Padding(0, 6, 0, 6) });
        leftCol.Controls.Add(new NexaLabel { Text = "Below the separator", LabelStyle = NexaLabelStyle.Default, AutoSize = true });
        leftCol.Controls.Add(new NexaSeparator { Dock = DockStyle.Top, Width = 360, Margin = new Padding(0, 12, 0, 6), Style = NexaSeparatorStyle.Dashed });
        leftCol.Controls.Add(new NexaLabel { Text = "After a 2-DIP thick divider", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(0, 4, 0, 0) });
        leftCol.Controls.Add(new NexaSeparator { Dock = DockStyle.Top, Width = 360, ThicknessInDips = 2, Margin = new Padding(0, 6, 0, 6) });
        leftCol.Controls.Add(new NexaLabel { Text = "After a 3-DIP thick divider", LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Margin = new Padding(0, 4, 0, 0) });
        leftCol.Controls.Add(new NexaSeparator { Dock = DockStyle.Top, Width = 360, ThicknessInDips = 3, Margin = new Padding(0, 6, 0, 0) });

        var rightCol = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(0, 0, 0, 0)
        };
        rightCol.Controls.Add(new NexaLabel { Text = "Left", LabelStyle = NexaLabelStyle.Default, AutoSize = true });
        rightCol.Controls.Add(new NexaSeparator { Orientation = NexaSeparatorOrientation.Vertical, Height = 120, Margin = new Padding(8, 0, 8, 0) });
        rightCol.Controls.Add(new NexaLabel { Text = "Right", LabelStyle = NexaLabelStyle.Default, AutoSize = true });
        rightCol.Controls.Add(new NexaSeparator { Orientation = NexaSeparatorOrientation.Vertical, Height = 120, ThicknessInDips = 2, Margin = new Padding(8, 0, 8, 0) });
        rightCol.Controls.Add(new NexaLabel { Text = "Far", LabelStyle = NexaLabelStyle.Default, AutoSize = true });

        row.Controls.Add(leftCol, 0, 0);
        row.Controls.Add(rightCol, 1, 0);
        card.Controls.Add(row);
        return card;
    }

    private Panel CreateDemoCard()
    {
        var card = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(16),
            Margin = new Padding(0, 0, 0, 12),
            BackColor = (Color)ThemeManager.Current.Palette[NexaColorRole.Surface].Value
        };
        var palette = ThemeManager.Current.Palette;
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
        Margin = new Padding(0, 0, 0, 8),
        ForeColor = (Color)ThemeManager.Current.Palette[NexaColorRole.TextSecondary].Value
    };

    private static Panel Spacer() => new() { Dock = DockStyle.Top, Height = 8 };
}