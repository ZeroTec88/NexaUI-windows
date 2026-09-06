using System.Drawing;
using System.Windows.Forms;
using NexaUI.Controls;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Demo.CSharp;

/// <summary>
/// Gallery page for Phase 11 Navigation Controls.
/// Demonstrates NexaTabControl, NexaNavigationBar, NexaBreadcrumb, NexaStepper,
/// and an Application Shell example combining multiple navigation controls.
/// </summary>
public sealed class GalleryNavigationScreen : UserControl
{
    private readonly TableLayoutPanel _root;

    public GalleryNavigationScreen()
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

        var heading = new NexaLabel
        {
            Text = "Navigation Controls",
            LabelStyle = NexaLabelStyle.Heading,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        _root.Controls.Add(heading);
        _root.SetColumnSpan(heading, 2);

        var intro = new NexaLabel
        {
            Text = "NexaTabControl, NexaTabPage, NexaNavigationBar, NexaBreadcrumb, NexaStepper. Complete navigation system for desktop applications.",
            LabelStyle = NexaLabelStyle.Muted,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 16)
        };
        _root.Controls.Add(intro);
        _root.SetColumnSpan(intro, 2);

        // Tab Control
        _root.Controls.Add(SectionTitle("NexaTabControl — Styles"));
        _root.Controls.Add(BuildTabStyleCard());

        _root.Controls.Add(SectionTitle("NexaTabControl — With NexaTabPage (Icons & Badges)"));
        _root.Controls.Add(BuildTabPageCard());

        _root.Controls.Add(SectionTitle("NexaTabControl — Interactive"));
        _root.Controls.Add(BuildInteractiveTabCard());

        // Navigation Bar
        _root.Controls.Add(SectionTitle("NexaNavigationBar — Expanded Mode"));
        _root.Controls.Add(BuildNavBarExpandedCard());

        _root.Controls.Add(SectionTitle("NexaNavigationBar — Compact Mode"));
        _root.Controls.Add(BuildNavBarCompactCard());

        // Breadcrumb
        _root.Controls.Add(SectionTitle("NexaBreadcrumb — Path Navigation"));
        _root.Controls.Add(BuildBreadcrumbCard());

        // Stepper
        _root.Controls.Add(SectionTitle("NexaStepper — Horizontal Workflow"));
        _root.Controls.Add(BuildStepperHorizontalCard());

        _root.Controls.Add(SectionTitle("NexaStepper — Vertical Workflow"));
        _root.Controls.Add(BuildStepperVerticalCard());

        // Application Shell
        _root.Controls.Add(SectionTitle("Application Shell Example"));
        var shellCard = BuildApplicationShellCard();
        _root.Controls.Add(shellCard);
        _root.SetColumnSpan(shellCard, 2);

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

    // ---------- Tab Control Cards ----------

    private Control BuildTabStyleCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Three visual styles: Default (underline), Underline (thin line), Pill (filled background)."));

        var grid = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 3,
            RowCount = 1
        };
        for (var i = 0; i < 3; i++) grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        grid.Controls.Add(BuildTabStyleDemo(NexaTabStyle.Default, "Default"), 0, 0);
        grid.Controls.Add(BuildTabStyleDemo(NexaTabStyle.Underline, "Underline"), 1, 0);
        grid.Controls.Add(BuildTabStyleDemo(NexaTabStyle.Pill, "Pill"), 2, 0);

        card.Controls.Add(grid);
        return card;
    }

    private Control BuildTabStyleDemo(NexaTabStyle style, string label)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 0, 12, 0),
            Padding = new Padding(16)
        };
        panel.Paint += (s, e) =>
        {
            using var pen = new Pen((Color)ThemeManager.Current.Palette[NexaColorRole.Border].Value, 1F);
            var r = panel.ClientRectangle;
            r.Width -= 1; r.Height -= 1;
            e.Graphics.DrawRectangle(pen, r);
        };

        var tabControl = new NexaTabControl
        {
            TabStyle = style,
            Width = 300,
            Height = 180,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 8, 0, 0)
        };
        tabControl.TabPages.Add(new TabPage { Text = "Overview" });
        tabControl.TabPages.Add(new TabPage { Text = "Details" });
        tabControl.TabPages.Add(new TabPage { Text = "Settings" });
        tabControl.TabPages.Add(new TabPage { Text = "Advanced" });

        var titleLabel = new NexaLabel
        {
            Text = label,
            LabelStyle = NexaLabelStyle.Subheading,
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 8)
        };

        panel.Controls.Add(tabControl);
        panel.Controls.Add(titleLabel);
        return panel;
    }

    private Control BuildTabPageCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("NexaTabPage supports icons and badges on individual tabs."));

        var tabControl = new NexaTabControl
        {
            TabStyle = NexaTabStyle.Default,
            Width = 400,
            Height = 200,
            Dock = DockStyle.Top
        };

        var page1 = new NexaTabPage { Text = "Inbox", IconKind = NexaIconKind.Check, BadgeText = "5", BadgeVisible = true };
        page1.Controls.Add(new NexaLabel { Text = "Inbox content here", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(16) });

        var page2 = new NexaTabPage { Text = "Drafts", IconKind = NexaIconKind.Check, BadgeText = "2", BadgeVisible = true };
        page2.Controls.Add(new NexaLabel { Text = "Drafts content here", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(16) });

        var page3 = new NexaTabPage { Text = "Sent", IconKind = NexaIconKind.Cross };
        page3.Controls.Add(new NexaLabel { Text = "Sent items content", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(16) });

        var page4 = new NexaTabPage { Text = "Archived", BadgeText = "99+", BadgeVisible = true };
        page4.Controls.Add(new NexaLabel { Text = "Archived items", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(16) });

        tabControl.TabPages.AddRange(new TabPage[] { page1, page2, page3, page4 });

        card.Controls.Add(tabControl);
        return card;
    }

    private Control BuildInteractiveTabCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("TabControl with dynamic tab management and selection events."));

        var tabControl = new NexaTabControl
        {
            TabStyle = NexaTabStyle.Underline,
            Width = 400,
            Height = 160,
            Dock = DockStyle.Top
        };
        tabControl.TabPages.Add(new TabPage { Text = "Tab 1" });
        tabControl.TabPages.Add(new TabPage { Text = "Tab 2" });
        tabControl.TabPages.Add(new TabPage { Text = "Tab 3" });

        var status = new NexaLabel
        {
            Text = "Selected: Tab 1 (Index 0)",
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 8, 0, 8)
        };

        tabControl.SelectedIndexChanged += (_, _) =>
        {
            status.Text = $"Selected: {tabControl.SelectedTab?.Text} (Index {tabControl.SelectedIndex})";
        };

        var btnRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 8)
        };
        var btnAdd = new NexaButton { Text = "Add Tab", Style = NexaButtonStyle.Primary, Width = 100, Margin = new Padding(0, 0, 8, 0) };
        var btnRemove = new NexaButton { Text = "Remove Tab", Style = NexaButtonStyle.Danger, Width = 110, Margin = new Padding(0, 0, 8, 0) };
        var count = 3;
        btnAdd.Click += (_, _) =>
        {
            count++;
            var newTab = new TabPage { Text = $"Tab {count}" };
            newTab.Controls.Add(new NexaLabel { Text = $"Content of Tab {count}", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(16) });
            tabControl.TabPages.Add(newTab);
        };
        btnRemove.Click += (_, _) =>
        {
            if (tabControl.TabCount > 1)
            {
                tabControl.TabPages.RemoveAt(tabControl.TabCount - 1);
            }
        };
        btnRow.Controls.Add(btnAdd);
        btnRow.Controls.Add(btnRemove);

        card.Controls.Add(tabControl);
        card.Controls.Add(status);
        card.Controls.Add(btnRow);
        return card;
    }

    // ---------- Navigation Bar Cards ----------

    private Control BuildNavBarExpandedCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Expanded mode shows icons and text labels. Supports selection, badges, and disabled items."));

        var navBar = new NexaNavigationBar
        {
            Mode = NexaNavigationMode.Expanded,
            Width = 260,
            Height = 320,
            Dock = DockStyle.Top,
            ItemHeight = 40,
            Indent = 16,
            IconSize = 20
        };
        navBar.Items.Add(new NexaNavigationItem { Key = "dashboard", Text = "Dashboard", IconKind = NexaIconKind.User });
        navBar.Items.Add(new NexaNavigationItem { Key = "projects", Text = "Projects", IconKind = NexaIconKind.User, BadgeText = "3", BadgeVisible = true });
        navBar.Items.Add(new NexaNavigationItem { Key = "students", Text = "Students", IconKind = NexaIconKind.User, BadgeText = "12", BadgeVisible = true });
        navBar.Items.Add(new NexaNavigationItem { Key = "reports", Text = "Reports", IconKind = NexaIconKind.Info });
        navBar.Items.Add(new NexaNavigationItem { Key = "settings", Text = "Settings", IconKind = NexaIconKind.Settings });
        navBar.Items.Add(new NexaNavigationItem { Key = "help", Text = "Help", IconKind = NexaIconKind.Info });
        navBar.Items.Add(new NexaNavigationItem { Key = "disabled", Text = "Disabled", IconKind = NexaIconKind.Settings, Enabled = false });
        navBar.SelectedIndex = 0;

        var status = new NexaLabel
        {
            Text = "Selected: Dashboard",
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 8, 0, 0)
        };

        navBar.SelectedItemChanged += (_, e) => status.Text = $"Selected: {e.Item.Text}";

        card.Controls.Add(navBar);
        card.Controls.Add(status);
        return card;
    }

    private Control BuildNavBarCompactCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Compact mode shows only icons. Click to expand or use ToggleMode()."));

        var navBar = new NexaNavigationBar
        {
            Mode = NexaNavigationMode.Compact,
            Width = 60,
            Height = 320,
            Dock = DockStyle.Top,
            ItemHeight = 40,
            Indent = 10,
            IconSize = 24
        };
        navBar.Items.Add(new NexaNavigationItem { Key = "dashboard", Text = "Dashboard", IconKind = NexaIconKind.User });
        navBar.Items.Add(new NexaNavigationItem { Key = "projects", Text = "Projects", IconKind = NexaIconKind.User, BadgeText = "3", BadgeVisible = true });
        navBar.Items.Add(new NexaNavigationItem { Key = "students", Text = "Students", IconKind = NexaIconKind.User, BadgeText = "12", BadgeVisible = true });
        navBar.Items.Add(new NexaNavigationItem { Key = "reports", Text = "Reports", IconKind = NexaIconKind.Info });
        navBar.Items.Add(new NexaNavigationItem { Key = "settings", Text = "Settings", IconKind = NexaIconKind.Settings });
        navBar.Items.Add(new NexaNavigationItem { Key = "help", Text = "Help", IconKind = NexaIconKind.Info });
        navBar.SelectedIndex = 0;

        var toggle = new NexaButton
        {
            Text = "Toggle Expanded",
            Style = NexaButtonStyle.Primary,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(0, 8, 0, 0)
        };
        toggle.Click += (_, _) => navBar.ToggleMode();

        var status = new NexaLabel
        {
            Text = "Mode: Compact",
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 8, 0, 0)
        };

        navBar.SelectedItemChanged += (_, e) =>
        {
            if (navBar.SelectedItem != null)
            {
                status.Text = $"Selected: {navBar.SelectedItem.Text} | Mode: {navBar.Mode}";
            }
        };

        card.Controls.Add(navBar);
        card.Controls.Add(toggle);
        card.Controls.Add(status);
        return card;
    }

    // ---------- Breadcrumb Card ----------

    private Control BuildBreadcrumbCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Hierarchical path navigation with customizable separators and clickable items."));

        var breadcrumb = new NexaBreadcrumb
        {
            Dock = DockStyle.Top,
            Height = 44,
            PaddingDips = 16,
            ItemSpacing = 8,
            Separator = NexaBreadcrumbSeparator.Chevron
        };
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "home", Text = "Home", Enabled = true });
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "education", Text = "Education", Enabled = true });
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "courses", Text = "Courses", Enabled = true });
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "ict", Text = "ICT", Enabled = true });
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "students", Text = "Students", Enabled = true });

        var status = new NexaLabel
        {
            Text = "Click a breadcrumb item...",
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 8, 0, 0)
        };

        breadcrumb.ItemClick += (_, e) => status.Text = $"Clicked: {e.Item.Text} (Key: {e.Item.Key})";

        // Separator options demo
        var sepRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 16, 0, 0)
        };
        var separators = new (string Name, NexaBreadcrumbSeparator Sep)[]
        {
            ("Chevron", NexaBreadcrumbSeparator.Chevron),
            ("Slash", NexaBreadcrumbSeparator.Slash),
            ("Greater Than", NexaBreadcrumbSeparator.GreaterThan),
            ("Custom", NexaBreadcrumbSeparator.Custom)
        };
        foreach (var (name, separator) in separators)
        {
            var btn = new NexaButton { Text = name, Width = 100, Margin = new Padding(0, 0, 8, 0) };
            var sep = separator; // capture
            btn.Click += (_, _) =>
            {
                var demo = new NexaBreadcrumb
                {
                    Dock = DockStyle.Top,
                    Height = 36,
                    Items = { new NexaBreadcrumbItem { Key = "a", Text = "Home" }, new NexaBreadcrumbItem { Key = "b", Text = "Section" }, new NexaBreadcrumbItem { Key = "c", Text = "Page" } },
                    Separator = sep
                };
                if (sep == NexaBreadcrumbSeparator.Custom) demo.CustomSeparatorText = "|";
                var popup = new Form { Text = $"Separator: {name}", Width = 400, Height = 120, StartPosition = FormStartPosition.CenterParent };
                popup.Controls.Add(demo);
                popup.ShowDialog();
            };
            sepRow.Controls.Add(btn);
        }

        card.Controls.Add(breadcrumb);
        card.Controls.Add(status);
        card.Controls.Add(sepRow);
        return card;
    }

    // ---------- Stepper Cards ----------

    private Control BuildStepperHorizontalCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Horizontal stepper with step states (Pending, Current, Completed, Error) and navigation."));

        var stepper = new NexaStepper
        {
            Orientation = NexaOrientation.Horizontal,
            Dock = DockStyle.Top,
            Height = 120,
            CircleDiameter = 36,
            ItemSpacing = 60,
            ShowDescriptions = true,
            ShowStepNumbers = true,
            AllowNavigation = true
        };
        stepper.Steps.Add(new NexaStep { Key = "account", Title = "Account", Description = "Create your account", State = NexaStepState.Completed });
        stepper.Steps.Add(new NexaStep { Key = "profile", Title = "Profile", Description = "Set up your profile", State = NexaStepState.Current });
        stepper.Steps.Add(new NexaStep { Key = "confirm", Title = "Confirm", Description = "Verify your details", State = NexaStepState.Pending });
        stepper.Steps.Add(new NexaStep { Key = "complete", Title = "Complete", Description = "All done!", State = NexaStepState.Pending });

        var btnRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 8, 0, 0)
        };
        var btnPrev = new NexaButton { Text = "Previous", Style = NexaButtonStyle.Outline, Width = 100, Margin = new Padding(0, 0, 8, 0) };
        var btnNext = new NexaButton { Text = "Next", Style = NexaButtonStyle.Primary, Width = 100, Margin = new Padding(0, 0, 8, 0) };
        var btnReset = new NexaButton { Text = "Reset", Style = NexaButtonStyle.Outline, Width = 100 };
        btnPrev.Click += (_, _) => stepper.Previous();
        btnNext.Click += (_, _) => stepper.Next();
        btnReset.Click += (_, _) => stepper.Reset();
        btnRow.Controls.AddRange(new Control[] { btnPrev, btnNext, btnReset });

        card.Controls.Add(stepper);
        card.Controls.Add(btnRow);
        return card;
    }

    private Control BuildStepperVerticalCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Vertical stepper showing different step states including Error and Disabled."));

        var stepper = new NexaStepper
        {
            Orientation = NexaOrientation.Vertical,
            Dock = DockStyle.Top,
            Height = 300,
            CircleDiameter = 32,
            ItemSpacing = 24,
            ShowDescriptions = true,
            ShowStepNumbers = true,
            AllowNavigation = false
        };
        stepper.Steps.Add(new NexaStep { Key = "step1", Title = "Account Setup", Description = "Enter your account information", State = NexaStepState.Completed });
        stepper.Steps.Add(new NexaStep { Key = "step2", Title = "Profile", Description = "Add profile details", State = NexaStepState.Completed });
        stepper.Steps.Add(new NexaStep { Key = "step3", Title = "Verification", Description = "Verify email address", State = NexaStepState.Error });
        stepper.Steps.Add(new NexaStep { Key = "step4", Title = "Preferences", Description = "Set your preferences", State = NexaStepState.Pending, Enabled = false });
        stepper.Steps.Add(new NexaStep { Key = "step5", Title = "Complete", Description = "Finish setup", State = NexaStepState.Pending });

        card.Controls.Add(stepper);
        return card;
    }

    // ---------- Application Shell ----------

    private Control BuildApplicationShellCard()
    {
        var card = CreateDemoCard();
        card.Controls.Add(DescriptionLabel("Complete application shell combining Navigation Bar, Breadcrumb, TabControl, and content area."));

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Vertical,
            SplitterDistance = 500,
            FixedPanel = FixedPanel.Panel1,
            Panel1MinSize = 300
        };

        // Left: Navigation Bar
        var navBar = new NexaNavigationBar
        {
            Mode = NexaNavigationMode.Expanded,
            Dock = DockStyle.Fill,
            ItemHeight = 44,
            Indent = 16,
            IconSize = 22
        };
        navBar.Items.Add(new NexaNavigationItem { Key = "dashboard", Text = "Dashboard", IconKind = NexaIconKind.User });
        navBar.Items.Add(new NexaNavigationItem { Key = "students", Text = "Students", IconKind = NexaIconKind.User, BadgeText = "24", BadgeVisible = true });
        navBar.Items.Add(new NexaNavigationItem { Key = "courses", Text = "Courses", IconKind = NexaIconKind.User, BadgeText = "8", BadgeVisible = true });
        navBar.Items.Add(new NexaNavigationItem { Key = "assessments", Text = "Assessments", IconKind = NexaIconKind.Info });
        navBar.Items.Add(new NexaNavigationItem { Key = "reports", Text = "Reports", IconKind = NexaIconKind.Info });
        navBar.Items.Add(new NexaNavigationItem { Key = "settings", Text = "Settings", IconKind = NexaIconKind.Settings });
        navBar.Items.Add(new NexaNavigationItem { Key = "help", Text = "Help", IconKind = NexaIconKind.Info });
        navBar.SelectedIndex = 0;

        // Content area with breadcrumb + tab control
        var contentPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Padding = new Padding(24, 16, 24, 24)
        };
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        contentPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var breadcrumb = new NexaBreadcrumb
        {
            Dock = DockStyle.Fill,
            Height = 40,
            PaddingDips = 0,
            ItemSpacing = 8,
            Separator = NexaBreadcrumbSeparator.Chevron
        };
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "home", Text = "Home", Enabled = true });
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "students", Text = "Students", Enabled = true });
        breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "batch2026", Text = "Batch 2026", Enabled = true });

        var tabControl = new NexaTabControl
        {
            TabStyle = NexaTabStyle.Default,
            Dock = DockStyle.Fill
        };
        var tabStudents = new TabPage { Text = "Students List" };
        tabStudents.Controls.Add(new NexaLabel { Text = "Students list content would go here", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(24) });
        var tabDetails = new TabPage { Text = "Student Details" };
        tabDetails.Controls.Add(new NexaLabel { Text = "Student detail view", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(24) });
        var tabEnroll = new TabPage { Text = "Enrollment" };
        tabEnroll.Controls.Add(new NexaLabel { Text = "Enrollment management", LabelStyle = NexaLabelStyle.Muted, AutoSize = true, Dock = DockStyle.Top, Margin = new Padding(24) });
        tabControl.TabPages.AddRange(new TabPage[] { tabStudents, tabDetails, tabEnroll });

        contentPanel.Controls.Add(breadcrumb, 0, 0);
        contentPanel.Controls.Add(tabControl, 0, 1);

        split.Panel1.Controls.Add(navBar);
        split.Panel2.Controls.Add(contentPanel);

        var statusLabel = new NexaLabel
        {
            Text = "Select a navigation item to change the view",
            LabelStyle = NexaLabelStyle.Caption,
            AutoSize = true,
            Dock = DockStyle.Bottom,
            Margin = new Padding(0, 8, 0, 0)
        };

        navBar.SelectedItemChanged += (_, e) =>
        {
            var key = e.Item?.Key ?? "";
            breadcrumb.Items.Clear();
            breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = "home", Text = "Home", Enabled = true });
            breadcrumb.Items.Add(new NexaBreadcrumbItem { Key = key, Text = e.Item?.Text ?? "", Enabled = true });
            statusLabel.Text = $"Viewing: {e.Item?.Text} (Key: {key})";
        };

        var outerPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8)
        };
        outerPanel.Controls.Add(split);
        outerPanel.Controls.Add(statusLabel);

        // Make the card larger for this demo
        card.Height = 600;
        card.Controls.Add(outerPanel);

        return card;
    }

    // ---------- Helpers ----------

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
