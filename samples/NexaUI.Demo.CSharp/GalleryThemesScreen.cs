using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

public sealed class GalleryThemesScreen : UserControl
{
    private readonly TableLayoutPanel _root;
    private readonly FlowLayoutPanel _themeSwitcher;
    private readonly NexaUI.Controls.NexaButton _lightBtn;
    private readonly NexaUI.Controls.NexaButton _darkBtn;
    private readonly TableLayoutPanel _colorSection;
    private readonly TableLayoutPanel _stateSection;
    private readonly TableLayoutPanel _typographySection;
    private readonly TableLayoutPanel _spacingSection;
    private readonly Label _status;

    public GalleryThemesScreen()
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
            Text = "Theme System",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 4)
        };
        var intro = new Label
        {
            Text = "Semantic colors, control states, typography and spacing for the active theme.",
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };

        _themeSwitcher = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0, 0, 0, 24)
        };
        _lightBtn = new NexaUI.Controls.NexaButton { Text = "Light", AutoSize = true, Margin = new Padding(0, 0, 8, 0) };
        _darkBtn = new NexaUI.Controls.NexaButton { Text = "Dark", AutoSize = true };
        _lightBtn.Click += (_, _) => ThemeManager.SetTheme(new LightTheme());
        _darkBtn.Click += (_, _) => ThemeManager.SetTheme(new DarkTheme());
        _themeSwitcher.Controls.Add(_lightBtn);
        _themeSwitcher.Controls.Add(_darkBtn);

        _colorSection = BuildSectionGrid();
        _stateSection = BuildSectionGrid();
        _typographySection = BuildSingleColumnSection();
        _spacingSection = BuildSingleColumnSection();

        _status = new Label
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            Margin = new Padding(0, 16, 0, 0)
        };

        _root.Controls.Add(header);
        _root.Controls.Add(intro);
        _root.Controls.Add(_themeSwitcher);
        _root.Controls.Add(SectionTitle("Semantic Colors"));
        _root.Controls.Add(_colorSection);
        _root.Controls.Add(SectionTitle("Control States"));
        _root.Controls.Add(_stateSection);
        _root.Controls.Add(SectionTitle("Typography"));
        _root.Controls.Add(_typographySection);
        _root.Controls.Add(SectionTitle("Spacing"));
        _root.Controls.Add(_spacingSection);
        _root.Controls.Add(_status);

        Controls.Add(_root);

        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChangedStatic;
        ThemeManager.ThemeChanged += OnThemeChangedStatic;

        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnThemeChangedStatic(object? sender, ThemeChangedEventArgs e) { /* no-op; instance handler covers it */ }

    private static Label SectionTitle(string title) => new()
    {
        Text = title,
        Dock = DockStyle.Top,
        AutoSize = true,
        Margin = new Padding(0, 8, 0, 8)
    };

    private static TableLayoutPanel BuildSectionGrid() => new()
    {
        Dock = DockStyle.Top,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        ColumnCount = 4,
        Padding = new Padding(0)
    };

    private static TableLayoutPanel BuildSingleColumnSection() => new()
    {
        Dock = DockStyle.Top,
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        ColumnCount = 1,
        Padding = new Padding(0)
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
        var spacing = theme.Spacing;
        var dpi = NexaFormsDpi.CurrentDpi(this);

        BackColor = (Color)palette[NexaColorRole.Background].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;

        // Style title + intro + section titles + status
        foreach (Control c in _root.Controls)
        {
            if (c is Label l)
            {
                var role = (l.Parent == _root && l.Dock == DockStyle.Top && (l.Text == "Theme System"))
                    ? NexaTypographyRole.Heading
                    : (l.Text == "Semantic Colors" || l.Text == "Control States" || l.Text == "Typography" || l.Text == "Spacing")
                        ? NexaTypographyRole.Title
                        : NexaTypographyRole.Body;
                l.Font = typography.ToFont(role, dpi);
                l.ForeColor = (Color)(role == NexaTypographyRole.Title
                    ? palette[NexaColorRole.TextPrimary].Value
                    : palette[NexaColorRole.TextSecondary].Value);
            }
        }
        // Override specific labels
        SetLabelStyle(_root.Controls[0], NexaTypographyRole.Heading, NexaColorRole.TextPrimary, typography, dpi);
        SetLabelStyle(_root.Controls[1], NexaTypographyRole.Body, NexaColorRole.TextSecondary, typography, dpi);
        _status.Font = typography.ToFont(NexaTypographyRole.Caption, dpi);
        _status.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        _status.Text = $"Active theme: {theme.Name} · DPI {dpi:F0}";

        StyleThemeButton(_lightBtn, theme, theme.IsDark ? NexaButtonStyle.Outline : NexaButtonStyle.Primary);
        StyleThemeButton(_darkBtn, theme, theme.IsDark ? NexaButtonStyle.Primary : NexaButtonStyle.Outline);

        PopulateColorGrid(_colorSection, palette);
        PopulateStateGrid(_stateSection, theme);
        PopulateTypographyGrid(_typographySection, typography, palette, dpi);
        PopulateSpacingGrid(_spacingSection, spacing, palette, dpi);
    }

    private static void SetLabelStyle(Control c, NexaTypographyRole role, NexaColorRole color, NexaTypography typography, float dpi)
    {
        if (c is Label l)
        {
            l.Font = typography.ToFont(role, dpi);
        }
    }

    private static void StyleThemeButton(NexaUI.Controls.NexaButton btn, ITheme theme, NexaButtonStyle style)
    {
        btn.Style = style;
    }

    private static void PopulateColorGrid(TableLayoutPanel grid, NexaPalette palette)
    {
        grid.Controls.Clear();
        grid.RowStyles.Clear();
        grid.RowCount = 0;

        var roles = new[]
        {
            NexaColorRole.Primary, NexaColorRole.Secondary, NexaColorRole.Background, NexaColorRole.Surface,
            NexaColorRole.SurfaceVariant, NexaColorRole.Border, NexaColorRole.TextPrimary, NexaColorRole.TextSecondary,
            NexaColorRole.TextDisabled, NexaColorRole.Success, NexaColorRole.Warning, NexaColorRole.Danger,
            NexaColorRole.Info, NexaColorRole.Focus
        };

        var row = 0;
        for (var i = 0; i < roles.Length; i += 4)
        {
            grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            grid.RowCount++;
            for (var c = 0; c < 4 && i + c < roles.Length; c++)
            {
                grid.Controls.Add(BuildSwatch(palette[roles[i + c]]), c, row);
            }
            row++;
        }
    }

    private static Control BuildSwatch(NexaSemanticColor semantic)
    {
        var baseColor = (Color)semantic.Value;
        var subtle = (Color)semantic.Subtle;

        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 78,
            Margin = new Padding(4),
            Padding = new Padding(10),
            BackColor = subtle
        };

        var roleLabel = new Label
        {
            Text = baseColor.IsNamedColor ? baseColor.Name : ColorTranslator.ToHtml(baseColor),
            Dock = DockStyle.Top,
            AutoSize = true,
            ForeColor = baseColor,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        };
        var hexLabel = new Label
        {
            Text = $"#{baseColor.R:X2}{baseColor.G:X2}{baseColor.B:X2}",
            Dock = DockStyle.Top,
            AutoSize = true,
            ForeColor = baseColor,
            Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point)
        };
        panel.Controls.Add(hexLabel);
        panel.Controls.Add(roleLabel);
        return panel;
    }

    private static void PopulateStateGrid(TableLayoutPanel grid, ITheme theme)
    {
        grid.Controls.Clear();
        grid.RowStyles.Clear();
        grid.RowCount = 0;

        var states = new[]
        {
            NexaControlState.Normal, NexaControlState.Hover, NexaControlState.Pressed, NexaControlState.Focused,
            NexaControlState.Selected, NexaControlState.Disabled, NexaControlState.Error, NexaControlState.Success
        };

        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowCount++;
        for (var c = 0; c < 4; c++)
        {
            var state = states[c];
            var colors = theme.ControlStates.Get(state);
            grid.Controls.Add(BuildStateChip(theme, state, colors), c, 0);
        }

        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowCount++;
        for (var c = 0; c < 4; c++)
        {
            var state = states[c + 4];
            var colors = theme.ControlStates.Get(state);
            grid.Controls.Add(BuildStateChip(theme, state, colors), c, 1);
        }
    }

    private static Control BuildStateChip(ITheme theme, NexaControlState state, NexaStateColors colors)
    {
        var chip = new Panel
        {
            Dock = DockStyle.Fill,
            Height = 56,
            Margin = new Padding(4),
            Padding = new Padding(10),
            BackColor = colors.Background != Color.Empty ? colors.Background : (Color)theme.Palette[NexaColorRole.Surface].Value,
            ForeColor = colors.Foreground != Color.Empty ? colors.Foreground : (Color)theme.Palette[NexaColorRole.TextPrimary].Value
        };
        chip.Paint += (s, e) =>
        {
            using var borderPen = new Pen(colors.Border != Color.Empty ? colors.Border : (Color)theme.Palette[NexaColorRole.Border].Value, 1F);
            var rect = chip.ClientRectangle;
            rect.Width -= 1; rect.Height -= 1;
            e.Graphics.DrawRectangle(borderPen, rect);
        };
        var label = new Label
        {
            Text = state.ToString(),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = theme.Typography.ToFont(NexaTypographyRole.Label, NexaFormsDpi.CurrentDpi(chip)),
            ForeColor = chip.ForeColor
        };
        chip.Controls.Add(label);
        return chip;
    }

    private static void PopulateTypographyGrid(TableLayoutPanel grid, NexaTypography typography, NexaPalette palette, float dpi)
    {
        grid.Controls.Clear();
        grid.RowStyles.Clear();
        grid.RowCount = 0;

        var roles = new[]
        {
            NexaTypographyRole.Display, NexaTypographyRole.Heading, NexaTypographyRole.Title,
            NexaTypographyRole.BodyStrong, NexaTypographyRole.Body, NexaTypographyRole.Label,
            NexaTypographyRole.Button, NexaTypographyRole.Caption
        };

        var card = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12),
            Margin = new Padding(4)
        };
        card.Paint += (s, e) =>
        {
            using var borderPen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            var rect = card.ClientRectangle;
            rect.Width -= 1; rect.Height -= 1;
            e.Graphics.DrawRectangle(borderPen, rect);
        };
        foreach (var role in roles)
        {
            var style = typography.Get(role);
            var sample = new Label
            {
                Text = $"{role}  ·  {style.FamilyName} {style.SizeInDips}pt",
                Dock = DockStyle.Top,
                AutoSize = true,
                Margin = new Padding(0, 4, 0, 4),
                Font = style.ToFont(dpi),
                ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value
            };
            card.Controls.Add(sample);
        }
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowCount++;
        grid.Controls.Add(card, 0, 0);
    }

    private static void PopulateSpacingGrid(TableLayoutPanel grid, NexaSpacing spacing, NexaPalette palette, float dpi)
    {
        grid.Controls.Clear();
        grid.RowStyles.Clear();
        grid.RowCount = 0;

        var tokens = new[]
        {
            NexaSpacingToken.None, NexaSpacingToken.Xxs, NexaSpacingToken.Xs, NexaSpacingToken.Sm,
            NexaSpacingToken.Md, NexaSpacingToken.Lg, NexaSpacingToken.Xl, NexaSpacingToken.Xxl, NexaSpacingToken.Huge
        };

        var card = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(12),
            Margin = new Padding(4),
            BackColor = (Color)palette[NexaColorRole.Surface].Value
        };
        card.Paint += (s, e) =>
        {
            using var borderPen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            var rect = card.ClientRectangle;
            rect.Width -= 1; rect.Height -= 1;
            e.Graphics.DrawRectangle(borderPen, rect);
        };

        foreach (var token in tokens)
        {
            var row = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                ColumnCount = 3,
                Margin = new Padding(0, 2, 0, 2)
            };
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));
            row.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            var tokenLabel = new Label
            {
                Text = token.ToString(),
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point),
                ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var valueLabel = new Label
            {
                Text = $"{spacing[token]} DIP",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
                ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value,
                TextAlign = ContentAlignment.MiddleLeft
            };
            var bar = new Panel
            {
                Dock = DockStyle.Fill,
                Height = 14,
                BackColor = (Color)palette[NexaColorRole.SurfaceVariant].Value,
                Margin = new Padding(0, 4, 0, 4),
                Padding = new Padding(0)
            };
            var pixels = NexaDpi.Scale(spacing[token], dpi);
            var filled = new Panel
            {
                Dock = DockStyle.Left,
                Width = Math.Min(220, Math.Max(2, pixels)),
                BackColor = (Color)palette[NexaColorRole.Primary].Value
            };
            bar.Controls.Add(filled);

            row.Controls.Add(tokenLabel, 0, 0);
            row.Controls.Add(valueLabel, 1, 0);
            row.Controls.Add(bar, 2, 0);
            card.Controls.Add(row);
        }

        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        grid.RowCount++;
        grid.Controls.Add(card, 0, 0);
    }
}