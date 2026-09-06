using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

/// <summary>
/// Phase 09 gallery: NexaPanel, NexaCard, NexaGroupBox, NexaFlowPanel, NexaTablePanel.
/// Plus a Dashboard composition example using only NexaUI controls.
/// </summary>
public sealed class GalleryLayoutScreen : UserControl
{
    private readonly TableLayoutPanel _root;

    public GalleryLayoutScreen()
    {
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = (Color)ThemeManager.Current.Palette[NexaColorRole.Background].Value;

        _root = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Padding = new Padding(32, 24, 32, 32)
        };
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

        var heading = HeaderLabel("Layout Controls");
        _root.Controls.Add(heading);
        _root.SetColumnSpan(heading, 2);

        var intro = BodyLabel("NexaPanel, NexaCard, NexaGroupBox, NexaFlowPanel, NexaTablePanel. Modern containers that preserve native WinForms layout.");
        _root.Controls.Add(intro);
        _root.SetColumnSpan(intro, 2);

        _root.Controls.Add(SectionTitle("NexaPanel — surface styles"));
        _root.Controls.Add(BuildPanelSurfaceCard());

        _root.Controls.Add(SectionTitle("NexaPanel — bordered, rounded, elevated, transparent"));
        _root.Controls.Add(BuildPanelVariantsCard());

        _root.Controls.Add(SectionTitle("NexaCard — System Information"));
        _root.Controls.Add(BuildSystemInfoCard());

        _root.Controls.Add(SectionTitle("NexaCard — Application Statistics"));
        _root.Controls.Add(BuildAppStatsCard());

        _root.Controls.Add(SectionTitle("NexaCard — Quick Actions"));
        _root.Controls.Add(BuildQuickActionsCard());

        _root.Controls.Add(SectionTitle("NexaCard — Recent Activity"));
        _root.Controls.Add(BuildRecentActivityCard());

        _root.Controls.Add(SectionTitle("NexaGroupBox — Personal / Account / Display"));
        _root.Controls.Add(BuildGroupBoxCard());

        _root.Controls.Add(SectionTitle("NexaFlowPanel — horizontal, vertical, wrapped, auto-scroll"));
        _root.Controls.Add(BuildFlowPanelCard());

        _root.Controls.Add(SectionTitle("NexaTablePanel — realistic form layouts"));
        _root.Controls.Add(BuildTablePanelCard());

        _root.Controls.Add(SectionTitle("Dashboard Layout Example"));
        _root.Controls.Add(BuildDashboardCard());
        _root.SetColumnSpan(BuildDashboardCard(), 2);

        Controls.Add(_root);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => OnThemeChanged(sender, e));
            return;
        }
        BackColor = (Color)e.Current.Palette[NexaColorRole.Background].Value;
    }

    // ---------- Cards ----------

    private Control BuildPanelSurfaceCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Default, Surface, Elevated, Transparent surface styles."));

        var row = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Default, "Default"));
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Surface, "Surface"));
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Elevated, "Elevated"));
        row.Controls.Add(MakePanelChip(NexaPanelSurfaceStyle.Transparent, "Transparent"));
        card.Controls.Add(row);
        return card;
    }

    private static NexaPanel MakePanelChip(NexaPanelSurfaceStyle style, string label)
    {
        return new NexaPanel
        {
            SurfaceStyle = style,
            BorderStyleEx = NexaBorderStyleEx.Solid,
            CornerRadius = 2,
            Size = new Size(140, 72),
            Margin = new Padding(0, 4, 12, 4)
        }.WithOverlayLabel(label);
    }

    private Control BuildPanelVariantsCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Bordered, rounded, elevated, shadow, and transparent panels."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            RowCount = 2
        };
        for (var i = 0; i < 3; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        for (var i = 0; i < 2; i++) grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Surface, NexaBorderStyleEx.Solid, 0, false, "Bordered"), 0, 0);
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Surface, NexaBorderStyleEx.Solid, 8, false, "Rounded"), 1, 0);
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Elevated, NexaBorderStyleEx.None, 4, true, "Shadow"), 2, 0);
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Surface, NexaBorderStyleEx.Solid, 4, false, "Bordered + Rounded"), 0, 1);
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Elevated, NexaBorderStyleEx.Solid, 8, true, "Shadow + Rounded"), 1, 1);
        grid.Controls.Add(MakePanel(NexaPanelSurfaceStyle.Transparent, NexaBorderStyleEx.Solid, 4, false, "Transparent"), 2, 1);
        card.Controls.Add(grid);
        return card;
    }

    private static NexaPanel MakePanel(NexaPanelSurfaceStyle surface, NexaBorderStyleEx border, int corner, bool shadow, string label)
    {
        return new NexaPanel
        {
            SurfaceStyle = surface,
            BorderStyleEx = border,
            CornerRadius = corner,
            ShadowEnabled = shadow,
            ShadowDepth = shadow ? 4 : 0,
            Size = new Size(160, 64),
            Margin = new Padding(0, 4, 8, 4),
            Dock = DockStyle.Top
        }.WithOverlayLabel(label);
    }

    private Control BuildSystemInfoCard()
    {
        var card = new NexaCard
        {
            Title = "System Information",
            Subtitle = "Current computer status",
            Dock = DockStyle.Top,
            CornerRadius = 4,
            ShadowEnabled = true,
            ShadowDepth = 4,
            Margin = new Padding(0, 0, 0, 12)
        };
        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        AddReadOnlyRow(grid, "OS", "Windows 11 Pro");
        AddReadOnlyRow(grid, "Version", "23H2");
        AddReadOnlyRow(grid, "Architecture", "x64");
        AddReadOnlyRow(grid, "Hostname", "DESKTOP-NEXAUI");
        card.ContentPanel.Controls.Add(grid);
        return card;
    }

    private static void AddReadOnlyRow(TableLayoutPanel grid, string label, string value)
    {
        var lbl = new NexaLabel { Text = label, LabelStyle = NexaLabelStyle.Muted, Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 2, 0, 2) };
        var val = new NexaLabel { Text = value, Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 2, 0, 2) };
        grid.Controls.Add(lbl);
        grid.Controls.Add(val);
    }

    private Control BuildAppStatsCard()
    {
        var card = new NexaCard
        {
            Title = "Application Statistics",
            Subtitle = "Runtime metrics",
            Dock = DockStyle.Top,
            CornerRadius = 4,
            Margin = new Padding(0, 0, 0, 12)
        };
        var row = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        row.Controls.Add(MakeStat("42%", "CPU"));
        row.Controls.Add(MakeStat("68%", "RAM"));
        row.Controls.Add(MakeStat("71%", "Storage"));
        row.Controls.Add(MakeStat("1.2k", "Requests/s"));
        card.ContentPanel.Controls.Add(row);
        return card;
    }

    private static NexaPanel MakeStat(string value, string label)
    {
        var stat = new NexaPanel
        {
            SurfaceStyle = NexaPanelSurfaceStyle.Elevated,
            BorderStyleEx = NexaBorderStyleEx.Solid,
            CornerRadius = 3,
            Size = new Size(96, 72),
            Margin = new Padding(0, 0, 8, 0)
        };
        stat.Controls.Add(new NexaLabel
        {
            Text = value,
            LabelStyle = NexaLabelStyle.Heading,
            AutoSize = true,
            Location = new Point(12, 8)
        });
        stat.Controls.Add(new NexaLabel
        {
            Text = label,
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Location = new Point(12, 44)
        });
        return stat;
    }

    private Control BuildQuickActionsCard()
    {
        var card = new NexaCard
        {
            Title = "Quick Actions",
            Subtitle = "Common tasks",
            Dock = DockStyle.Top,
            CornerRadius = 4,
            Margin = new Padding(0, 0, 0, 12)
        };
        var col = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        col.Controls.Add(new NexaButton { Text = "Refresh now", Style = NexaButtonStyle.Primary, Width = 200, Margin = new Padding(0, 0, 0, 8) });
        col.Controls.Add(new NexaButton { Text = "Open settings", Style = NexaButtonStyle.Secondary, Width = 200, Margin = new Padding(0, 0, 0, 8) });
        col.Controls.Add(new NexaButton { Text = "Export data", Style = NexaButtonStyle.Outline, Width = 200, Margin = new Padding(0, 0, 0, 8) });
        col.Controls.Add(new NexaButton { Text = "View logs", Style = NexaButtonStyle.Ghost, Width = 200 });
        card.ContentPanel.Controls.Add(col);
        return card;
    }

    private Control BuildRecentActivityCard()
    {
        var card = new NexaCard
        {
            Title = "Recent Activity",
            Subtitle = "Last 24 hours",
            Dock = DockStyle.Top,
            CornerRadius = 4,
            Margin = new Padding(0, 0, 0, 12)
        };
        var list = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        list.Controls.Add(MakeActivityItem("Build succeeded", "12 minutes ago", NexaLabelStyle.Success));
        list.Controls.Add(MakeActivityItem("Tests passed (98/98)", "1 hour ago", NexaLabelStyle.Success));
        list.Controls.Add(MakeActivityItem("Dependency update", "3 hours ago", NexaLabelStyle.Info));
        list.Controls.Add(MakeActivityItem("Cache cleared", "Yesterday", NexaLabelStyle.Muted));
        card.ContentPanel.Controls.Add(list);
        return card;
    }

    private static NexaPanel MakeActivityItem(string title, string when, NexaLabelStyle status)
    {
        var item = new NexaPanel
        {
            SurfaceStyle = NexaPanelSurfaceStyle.Transparent,
            BorderStyleEx = NexaBorderStyleEx.None,
            Size = new Size(260, 44),
            Margin = new Padding(0, 0, 0, 4)
        };
        item.Controls.Add(new NexaLabel
        {
            Text = title,
            AutoSize = true,
            Location = new Point(0, 2)
        });
        item.Controls.Add(new NexaLabel
        {
            Text = when,
            LabelStyle = status,
            AutoSize = true,
            Location = new Point(0, 22)
        });
        return item;
    }

    private Control BuildGroupBoxCard()
    {
        var host = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        host.Controls.Add(MakeGroupBox("Personal Information", new (string, Control)[]
        {
            ("Name", MakeTextBox("Your full name")),
            ("Email", MakeTextBox("name@example.com"))
        }));
        host.Controls.Add(MakeGroupBox("Account Settings", new (string, Control)[]
        {
            ("Username", MakeTextBox("nexa_user")),
            ("Password", MakeTextBox("••••••••"))
        }));
        host.Controls.Add(MakeGroupBox("Display Settings", new (string, Control)[]
        {
            ("Theme", MakeThemeCombo()),
            ("Density", MakeDensityCombo())
        }));
        return host;
    }

    private static NexaGroupBox MakeGroupBox(string title, (string Label, Control Editor)[] rows)
    {
        var gb = new NexaGroupBox
        {
            Text = title,
            Width = 260,
            Height = 120,
            Margin = new Padding(0, 0, 12, 0)
        };
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Padding = new Padding(8, 4, 8, 8)
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        for (var r = 0; r < rows.Length; r++)
        {
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.Controls.Add(new NexaLabel { Text = rows[r].Label, LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Margin = new Padding(0, 6, 6, 4) }, 0, r);
            tbl.Controls.Add(rows[r].Editor, 1, r);
        }
        gb.Controls.Add(tbl);
        return gb;
    }

    private static NexaTextBox MakeTextBox(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        Margin = new Padding(0, 4, 0, 4)
    };

    private static NexaComboBox MakeThemeCombo()
    {
        var combo = new NexaComboBox
        {
            Dock = DockStyle.Top,
            Margin = new Padding(0, 4, 0, 4)
        };
        combo.Items.AddRange(new object[] { "System", "Light", "Dark" });
        return combo;
    }

    private static NexaComboBox MakeDensityCombo()
    {
        var combo = new NexaComboBox
        {
            Dock = DockStyle.Top,
            Margin = new Padding(0, 4, 0, 4)
        };
        combo.Items.AddRange(new object[] { "Compact", "Comfortable", "Spacious" });
        return combo;
    }

    private Control BuildFlowPanelCard()
    {
        var host = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };

        // Horizontal
        var horiz = new NexaFlowPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BorderEnabled = true,
            CornerRadius = 2,
            Margin = new Padding(0, 0, 0, 12)
        };
        horiz.Controls.AddRange(new Control[] {
            new NexaButton { Text = "One", Width = 80 },
            new NexaButton { Text = "Two", Width = 80 },
            new NexaButton { Text = "Three", Width = 80 },
            new NexaButton { Text = "Four", Width = 80 }
        });
        var horizLabel = new NexaLabel { Text = "Horizontal (LeftToRight, no wrap):", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        host.Controls.Add(horizLabel);
        host.Controls.Add(horiz);

        // Vertical
        var vert = new NexaFlowPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BorderEnabled = true,
            CornerRadius = 2,
            Width = 200,
            Height = 160,
            Margin = new Padding(0, 0, 0, 12)
        };
        vert.Controls.AddRange(new Control[] {
            new NexaLabel { Text = "Line 1", AutoSize = true },
            new NexaLabel { Text = "Line 2", AutoSize = true },
            new NexaLabel { Text = "Line 3", AutoSize = true },
            new NexaLabel { Text = "Line 4", AutoSize = true }
        });
        var vertLabel = new NexaLabel { Text = "Vertical (TopDown, fixed height):", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        host.Controls.Add(vertLabel);
        host.Controls.Add(vert);

        // Wrapped
        var wrap = new NexaFlowPanel
        {
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BorderEnabled = true,
            CornerRadius = 2,
            Width = 280,
            Margin = new Padding(0, 0, 0, 12)
        };
        for (var i = 0; i < 12; i++)
        {
            wrap.Controls.Add(new NexaButton { Text = $"Tag {i + 1}", Width = 64, Margin = new Padding(2) });
        }
        var wrapLabel = new NexaLabel { Text = "Wrapped (LeftToRight, WrapContents=true):", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        host.Controls.Add(wrapLabel);
        host.Controls.Add(wrap);

        // Auto-scroll
        var scroll = new NexaFlowPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BorderEnabled = true,
            CornerRadius = 2,
            Width = 200,
            Height = 120
        };
        for (var i = 0; i < 20; i++)
        {
            scroll.Controls.Add(new NexaLabel { Text = $"Item {i + 1}", AutoSize = true, Margin = new Padding(2) });
        }
        var scrollLabel = new NexaLabel { Text = "AutoScroll (TopDown, scrollable):", AutoSize = true, Margin = new Padding(0, 0, 0, 4) };
        host.Controls.Add(scrollLabel);
        host.Controls.Add(scroll);

        return host;
    }

    private Control BuildTablePanelCard()
    {
        var host = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };
        host.Controls.Add(MakeFormTable("Label + TextBox", new (string, Control)[]
        {
            ("Full name", new NexaTextBox { Text = "Ada Lovelace", Width = 200 }),
            ("Email", new NexaTextBox { Text = "ada@example.com", Width = 200 })
        }));
        host.Controls.Add(MakeFormTable("Label + ComboBox", new (string, Control)[]
        {
            ("Country", new NexaComboBox { Width = 200 }.WithItems("Sri Lanka", "India", "USA", "UK")),
            ("Language", new NexaComboBox { Width = 200 }.WithItems("English", "Sinhala", "Tamil"))
        }));
        host.Controls.Add(MakeFormTable("Label + CheckBox", new (string, Control)[]
        {
            ("Notify", new NexaCheckBox { Text = "Email me about updates", Checked = true }),
            ("", new NexaCheckBox { Text = "I agree to the terms" })
        }));
        host.Controls.Add(MakeFormTable("Label + ToggleSwitch", new (string, Control)[]
        {
            ("Dark mode", new NexaToggleSwitch { Text = "Enabled", Checked = true }),
            ("Auto-save", new NexaToggleSwitch { Text = "Enabled", Checked = false })
        }));
        return host;
    }

    private static NexaPanel MakeFormTable(string title, (string Label, Control Editor)[] rows)
    {
        var tbl = new NexaTablePanel
        {
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 12, 0),
            BorderEnabled = false
        };
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
        tbl.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        for (var r = 0; r < rows.Length; r++)
        {
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tbl.Controls.Add(new NexaLabel { Text = rows[r].Label, AutoSize = true, Margin = new Padding(0, 6, 6, 4) }, 0, r);
            tbl.Controls.Add(rows[r].Editor, 1, r);
        }
        var wrapper = new NexaPanel
        {
            SurfaceStyle = NexaPanelSurfaceStyle.Surface,
            BorderStyleEx = NexaBorderStyleEx.Solid,
            CornerRadius = 3,
            Padding = new Padding(10),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 12, 0)
        };
        var titleLabel = new NexaLabel { Text = title, LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Dock = DockStyle.Top };
        wrapper.Controls.Add(titleLabel);
        wrapper.Controls.Add(tbl);
        return wrapper;
    }

    private Control BuildDashboardCard()
    {
        var outer = new NexaPanel
        {
            SurfaceStyle = NexaPanelSurfaceStyle.Surface,
            BorderStyleEx = NexaBorderStyleEx.Solid,
            CornerRadius = 4,
            ShadowEnabled = true,
            ShadowDepth = 6,
            Padding = new Padding(16),
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 0)
        };

        var header = new NexaLabel
        {
            Text = "Dashboard",
            LabelStyle = NexaLabelStyle.Heading,
            AutoSize = true,
            Dock = DockStyle.Top
        };
        outer.Controls.Add(header);

        // Top row: three stat cards
        var topRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 12, 0, 12)
        };
        topRow.Controls.Add(MakeStatTile("CPU", "42%", "2 cores active", NexaLabelStyle.Success));
        topRow.Controls.Add(MakeStatTile("RAM", "68%", "5.4 GB / 8.0 GB", NexaLabelStyle.Warning));
        topRow.Controls.Add(MakeStatTile("Storage", "71%", "713 GB / 1.0 TB", NexaLabelStyle.Warning));
        outer.Controls.Add(topRow);

        // Bottom row: activity + actions
        var bottom = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            ColumnCount = 2,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));
        bottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));

        var activity = new NexaCard
        {
            Title = "Recent Activity",
            Subtitle = "Last hour",
            CornerRadius = 3,
            Margin = new Padding(0, 0, 8, 0)
        };
        var actList = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        actList.Controls.Add(MakeActivityItem("Build succeeded", "12 min ago", NexaLabelStyle.Success));
        actList.Controls.Add(MakeActivityItem("Tests passed (98/98)", "1 hr ago", NexaLabelStyle.Success));
        actList.Controls.Add(MakeActivityItem("Deployment finished", "2 hr ago", NexaLabelStyle.Info));
        activity.ContentPanel.Controls.Add(actList);
        bottom.Controls.Add(activity, 0, 0);

        var actions = new NexaCard
        {
            Title = "Quick Actions",
            Subtitle = "Common tasks",
            CornerRadius = 3
        };
        var actCol = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink, FlowDirection = FlowDirection.TopDown, WrapContents = false };
        actCol.Controls.Add(new NexaButton { Text = "Refresh now", Style = NexaButtonStyle.Primary, Width = 200, Margin = new Padding(0, 0, 0, 6) });
        actCol.Controls.Add(new NexaButton { Text = "Open settings", Width = 200, Margin = new Padding(0, 0, 0, 6) });
        actCol.Controls.Add(new NexaToggleSwitch { Text = "Auto-refresh every 5s", Checked = true });
        actions.ContentPanel.Controls.Add(actCol);
        bottom.Controls.Add(actions, 1, 0);

        outer.Controls.Add(bottom);
        return outer;
    }

    private static NexaPanel MakeStatTile(string label, string value, string detail, NexaLabelStyle status)
    {
        var tile = new NexaPanel
        {
            SurfaceStyle = NexaPanelSurfaceStyle.Elevated,
            BorderStyleEx = NexaBorderStyleEx.Solid,
            CornerRadius = 3,
            Width = 200,
            Height = 96,
            Margin = new Padding(0, 0, 8, 0),
            Padding = new Padding(12)
        };
        tile.Controls.Add(new NexaLabel { Text = label, LabelStyle = NexaLabelStyle.Caption, AutoSize = true, Location = new Point(12, 8) });
        tile.Controls.Add(new NexaLabel { Text = value, LabelStyle = NexaLabelStyle.Heading, AutoSize = true, Location = new Point(12, 28) });
        tile.Controls.Add(new NexaLabel { Text = detail, LabelStyle = status, AutoSize = true, Location = new Point(12, 64) });
        return tile;
    }

    // ---------- helpers ----------

    private static NexaLabel HeaderLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Heading,
        Margin = new Padding(0, 0, 0, 8)
    };

    private static NexaLabel BodyLabel(string text) => new()
    {
        Text = text,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Muted,
        Margin = new Padding(0, 0, 0, 16)
    };

    private static NexaLabel SectionTitle(string title) => new()
    {
        Text = title,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Subheading,
        Margin = new Padding(0, 8, 0, 8)
    };

    private static Panel CreateDemoCard()
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

    private static NexaLabel DescriptionLabel(string text) => new()
    {
        Text = text,
        Dock = DockStyle.Top,
        AutoSize = true,
        LabelStyle = NexaLabelStyle.Muted,
        Margin = new Padding(0, 0, 0, 8)
    };
}

internal static class NexaLayoutDemoExtensions
{
    public static T WithOverlayLabel<T>(this T control, string text) where T : Control
    {
        control.Controls.Add(new NexaLabel
        {
            Text = text,
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Location = new Point(8, 8)
        });
        return control;
    }

    public static NexaComboBox WithItems(this NexaComboBox combo, params string[] items)
    {
        combo.Items.AddRange(items);
        return combo;
    }
}
