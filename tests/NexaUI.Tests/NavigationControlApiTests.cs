using System.Windows.Forms;

namespace NexaUI.Tests;

[TestClass]
public sealed class NavigationControlApiTests
{
    // -------- NexaTabControl --------

    [TestMethod]
    public void NexaTabControl_Inherits_From_Native_TabControl()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.TabControl).IsAssignableFrom(typeof(NexaUI.Controls.NexaTabControl)));
    }

    [TestMethod]
    public void NexaTabControl_Default_Properties()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        Assert.AreEqual(NexaUI.Core.NexaTabStyle.Default, tab.TabStyle);
        Assert.AreEqual(40, tab.TabHeaderHeight);
        Assert.AreEqual(3, tab.ActiveIndicatorThickness);
        Assert.AreEqual(0, tab.ActiveIndicatorLength);
        Assert.AreEqual(8, tab.TabSpacing);
        Assert.IsFalse(tab.ShowCloseButton);
        tab.Dispose();
    }

    [TestMethod]
    public void NexaTabControl_TabStyle_Roundtrip()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        tab.TabStyle = NexaUI.Core.NexaTabStyle.Pill;
        Assert.AreEqual(NexaUI.Core.NexaTabStyle.Pill, tab.TabStyle);
        tab.TabStyle = NexaUI.Core.NexaTabStyle.Underline;
        Assert.AreEqual(NexaUI.Core.NexaTabStyle.Underline, tab.TabStyle);
        tab.Dispose();
    }

    [TestMethod]
    public void NexaTabControl_Add_TabPages()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        tab.TabPages.Add(new TabPage { Text = "Page 1" });
        tab.TabPages.Add(new TabPage { Text = "Page 2" });
        tab.TabPages.Add(new TabPage { Text = "Page 3" });
        Assert.AreEqual(3, tab.TabCount);
        tab.Dispose();
    }

    [TestMethod]
    public void NexaTabControl_SelectedIndex_Changed()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        tab.TabPages.Add(new TabPage { Text = "Page 1" });
        tab.TabPages.Add(new TabPage { Text = "Page 2" });
        tab.SelectedIndex = 1;
        Assert.AreEqual(1, tab.SelectedIndex);
        Assert.AreEqual("Page 2", tab.SelectedTab?.Text);
        tab.Dispose();
    }

    [TestMethod]
    public void NexaTabControl_TabStyle_Roundtrip_Multiple()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        tab.TabStyle = NexaUI.Core.NexaTabStyle.Pill;
        Assert.AreEqual(NexaUI.Core.NexaTabStyle.Pill, tab.TabStyle);
        tab.TabStyle = NexaUI.Core.NexaTabStyle.Underline;
        Assert.AreEqual(NexaUI.Core.NexaTabStyle.Underline, tab.TabStyle);
        tab.TabStyle = NexaUI.Core.NexaTabStyle.Default;
        Assert.AreEqual(NexaUI.Core.NexaTabStyle.Default, tab.TabStyle);
        tab.Dispose();
    }

    // -------- NexaTabPage --------

    [TestMethod]
    public void NexaTabPage_Inherits_From_Native_TabPage()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.TabPage).IsAssignableFrom(typeof(NexaUI.Controls.NexaTabPage)));
    }

    [TestMethod]
    public void NexaTabPage_IconAndBadge_Roundtrip()
    {
        var page = new NexaUI.Controls.NexaTabPage
        {
            Text = "Test",
            IconKind = NexaUI.Icons.NexaIconKind.Check,
            BadgeText = "5",
            BadgeVisible = true
        };
        Assert.AreEqual("Test", page.Text);
        Assert.AreEqual(NexaUI.Icons.NexaIconKind.Check, page.IconKind);
        Assert.AreEqual("5", page.BadgeText);
        Assert.IsTrue(page.BadgeVisible);
        page.Dispose();
    }

    // -------- NexaNavigationItem --------

    [TestMethod]
    public void NexaNavigationItem_Properties_Roundtrip()
    {
        var item = new NexaUI.Controls.NexaNavigationItem("key1", "Dashboard")
        {
            IconKind = NexaUI.Icons.NexaIconKind.User,
            Enabled = true,
            Visible = true,
            BadgeText = "3",
            BadgeVisible = true,
            Tag = "user-data"
        };
        Assert.AreEqual("key1", item.Key);
        Assert.AreEqual("Dashboard", item.Text);
        Assert.AreEqual(NexaUI.Icons.NexaIconKind.User, item.IconKind);
        Assert.IsTrue(item.Enabled);
        Assert.IsTrue(item.Visible);
        Assert.AreEqual("3", item.BadgeText);
        Assert.IsTrue(item.BadgeVisible);
        Assert.AreEqual("user-data", item.Tag);
    }

    // -------- NexaNavigationBar --------

    [TestMethod]
    public void NexaNavigationBar_Default_Properties()
    {
        var nav = new NexaUI.Controls.NexaNavigationBar();
        Assert.AreEqual(NexaUI.Core.NexaNavigationMode.Expanded, nav.Mode);
        Assert.AreEqual(40, nav.ItemHeight);
        Assert.AreEqual(12, nav.Indent);
        Assert.AreEqual(20, nav.IconSize);
        Assert.AreEqual(-1, nav.SelectedIndex);
        Assert.IsNull(nav.SelectedItem);
        nav.Dispose();
    }

    [TestMethod]
    public void NexaNavigationBar_Add_Remove_Items()
    {
        var nav = new NexaUI.Controls.NexaNavigationBar();
        var item1 = new NexaUI.Controls.NexaNavigationItem("a", "Item A");
        var item2 = new NexaUI.Controls.NexaNavigationItem("b", "Item B");
        nav.Items.Add(item1);
        nav.Items.Add(item2);
        Assert.AreEqual(2, nav.Items.Count);

        nav.RemoveItem(item1);
        Assert.AreEqual(1, nav.Items.Count);
        Assert.AreSame(item2, nav.Items[0]);

        nav.ClearItems();
        Assert.AreEqual(0, nav.Items.Count);
        nav.Dispose();
    }

    [TestMethod]
    public void NexaNavigationBar_Selection_Works()
    {
        var nav = new NexaUI.Controls.NexaNavigationBar();
        var item1 = new NexaUI.Controls.NexaNavigationItem("a", "Item A");
        var item2 = new NexaUI.Controls.NexaNavigationItem("b", "Item B");
        nav.Items.Add(item1);
        nav.Items.Add(item2);

        nav.SelectedIndex = 1;
        Assert.AreEqual(1, nav.SelectedIndex);
        Assert.AreSame(item2, nav.SelectedItem);
        Assert.IsTrue(item2.Selected);
        Assert.IsFalse(item1.Selected);
        nav.Dispose();
    }

    [TestMethod]
    public void NexaNavigationBar_Mode_Toggle()
    {
        var nav = new NexaUI.Controls.NexaNavigationBar();
        Assert.AreEqual(NexaUI.Core.NexaNavigationMode.Expanded, nav.Mode);
        nav.ToggleMode();
        Assert.AreEqual(NexaUI.Core.NexaNavigationMode.Compact, nav.Mode);
        nav.ToggleMode();
        Assert.AreEqual(NexaUI.Core.NexaNavigationMode.Expanded, nav.Mode);
        nav.Dispose();
    }

    // -------- NexaBreadcrumbItem --------

    [TestMethod]
    public void NexaBreadcrumbItem_Properties_Roundtrip()
    {
        var item = new NexaUI.Controls.NexaBreadcrumbItem("home", "Home")
        {
            Enabled = true,
            Visible = true,
            Tag = "data"
        };
        Assert.AreEqual("home", item.Key);
        Assert.AreEqual("Home", item.Text);
        Assert.IsTrue(item.Enabled);
        Assert.IsTrue(item.Visible);
        Assert.AreEqual("data", item.Tag);
    }

    // -------- NexaBreadcrumb --------

    [TestMethod]
    public void NexaBreadcrumb_Default_Properties()
    {
        var bc = new NexaUI.Controls.NexaBreadcrumb();
        Assert.AreEqual(NexaUI.Core.NexaBreadcrumbSeparator.Chevron, bc.Separator);
        Assert.AreEqual(">", bc.CustomSeparatorText);
        Assert.AreEqual(8, bc.ItemSpacing);
        Assert.AreEqual(12, bc.PaddingDips);
        Assert.AreEqual(32, bc.ItemHeight);
        bc.Dispose();
    }

    [TestMethod]
    public void NexaBreadcrumb_Add_Items()
    {
        var bc = new NexaUI.Controls.NexaBreadcrumb();
        bc.Items.Add(new NexaUI.Controls.NexaBreadcrumbItem("a", "Home"));
        bc.Items.Add(new NexaUI.Controls.NexaBreadcrumbItem("b", "Section"));
        bc.Items.Add(new NexaUI.Controls.NexaBreadcrumbItem("c", "Page"));
        Assert.AreEqual(3, bc.Items.Count);
        bc.Dispose();
    }

    [TestMethod]
    public void NexaBreadcrumb_Separator_Roundtrip()
    {
        var bc = new NexaUI.Controls.NexaBreadcrumb();
        bc.Separator = NexaUI.Core.NexaBreadcrumbSeparator.Slash;
        Assert.AreEqual(NexaUI.Core.NexaBreadcrumbSeparator.Slash, bc.Separator);
        bc.Separator = NexaUI.Core.NexaBreadcrumbSeparator.GreaterThan;
        Assert.AreEqual(NexaUI.Core.NexaBreadcrumbSeparator.GreaterThan, bc.Separator);
        bc.Separator = NexaUI.Core.NexaBreadcrumbSeparator.Custom;
        bc.CustomSeparatorText = "|";
        Assert.AreEqual("|", bc.CustomSeparatorText);
        bc.Dispose();
    }

    // -------- NexaStep --------

    [TestMethod]
    public void NexaStep_Properties_Roundtrip()
    {
        var step = new NexaUI.Controls.NexaStep("step1", "Account", "Create account")
        {
            State = NexaUI.Core.NexaStepState.Current,
            Enabled = true,
            Visible = true,
            Tag = "data"
        };
        Assert.AreEqual("step1", step.Key);
        Assert.AreEqual("Account", step.Title);
        Assert.AreEqual("Create account", step.Description);
        Assert.AreEqual(NexaUI.Core.NexaStepState.Current, step.State);
        Assert.IsTrue(step.Enabled);
        Assert.IsTrue(step.Visible);
        Assert.AreEqual("data", step.Tag);
    }

    // -------- NexaStepper --------

    [TestMethod]
    public void NexaStepper_Default_Properties()
    {
        var stepper = new NexaUI.Controls.NexaStepper();
        Assert.AreEqual(NexaUI.Core.NexaOrientation.Horizontal, stepper.Orientation);
        Assert.AreEqual(0, stepper.CurrentStepIndex);
        Assert.IsTrue(stepper.ShowDescriptions);
        Assert.IsTrue(stepper.ShowStepNumbers);
        Assert.IsTrue(stepper.AllowNavigation);
        Assert.AreEqual(36, stepper.CircleDiameter);
        Assert.AreEqual(2, stepper.ConnectorThickness);
        Assert.AreEqual(40, stepper.ItemSpacing);
        stepper.Dispose();
    }

    [TestMethod]
    public void NexaStepper_Add_Steps()
    {
        var stepper = new NexaUI.Controls.NexaStepper();
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("1", "Step 1"));
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("2", "Step 2"));
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("3", "Step 3"));
        Assert.AreEqual(3, stepper.Steps.Count);
        stepper.Dispose();
    }

    [TestMethod]
    public void NexaStepper_CurrentStep_Index()
    {
        var stepper = new NexaUI.Controls.NexaStepper();
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("1", "Step 1"));
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("2", "Step 2"));
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("3", "Step 3"));

        stepper.CurrentStepIndex = 2;
        Assert.AreEqual(2, stepper.CurrentStepIndex);
        stepper.CurrentStepIndex = 0;
        Assert.AreEqual(0, stepper.CurrentStepIndex);
        stepper.Dispose();
    }

    [TestMethod]
    public void NexaStepper_Next_Previous_Reset()
    {
        var stepper = new NexaUI.Controls.NexaStepper();
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("1", "Step 1"));
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("2", "Step 2"));
        stepper.Steps.Add(new NexaUI.Controls.NexaStep("3", "Step 3"));

        stepper.Next();
        Assert.AreEqual(1, stepper.CurrentStepIndex);
        stepper.Next();
        Assert.AreEqual(2, stepper.CurrentStepIndex);
        stepper.Next(); // Should not go past last
        Assert.AreEqual(2, stepper.CurrentStepIndex);

        stepper.Previous();
        Assert.AreEqual(1, stepper.CurrentStepIndex);
        stepper.Previous();
        Assert.AreEqual(0, stepper.CurrentStepIndex);
        stepper.Previous(); // Should not go before first
        Assert.AreEqual(0, stepper.CurrentStepIndex);

        stepper.Next();
        stepper.Next();
        stepper.Reset();
        Assert.AreEqual(0, stepper.CurrentStepIndex);
        stepper.Dispose();
    }

    [TestMethod]
    public void NexaStepper_Orientation_Roundtrip()
    {
        var stepper = new NexaUI.Controls.NexaStepper();
        stepper.Orientation = NexaUI.Core.NexaOrientation.Vertical;
        Assert.AreEqual(NexaUI.Core.NexaOrientation.Vertical, stepper.Orientation);
        stepper.Orientation = NexaUI.Core.NexaOrientation.Horizontal;
        Assert.AreEqual(NexaUI.Core.NexaOrientation.Horizontal, stepper.Orientation);
        stepper.Dispose();
    }

    [TestMethod]
    public void NexaStepper_ShowDescriptions_StepNumbers_AllowNavigation()
    {
        var stepper = new NexaUI.Controls.NexaStepper();
        stepper.ShowDescriptions = false;
        stepper.ShowStepNumbers = false;
        stepper.AllowNavigation = false;
        Assert.IsFalse(stepper.ShowDescriptions);
        Assert.IsFalse(stepper.ShowStepNumbers);
        Assert.IsFalse(stepper.AllowNavigation);
        stepper.Dispose();
    }

    // -------- Cross-control / regression --------

    [TestMethod]
    public void NavigationControls_All_Dispose_Cleanly()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        var page = new NexaUI.Controls.NexaTabPage();
        var nav = new NexaUI.Controls.NexaNavigationBar();
        var bc = new NexaUI.Controls.NexaBreadcrumb();
        var stepper = new NexaUI.Controls.NexaStepper();
        tab.Dispose();
        page.Dispose();
        nav.Dispose();
        bc.Dispose();
        stepper.Dispose();
    }

    [TestMethod]
    public void NavigationControls_ThemeSwitch_Does_Not_Throw()
    {
        var tab = new NexaUI.Controls.NexaTabControl();
        var nav = new NexaUI.Controls.NexaNavigationBar();
        var bc = new NexaUI.Controls.NexaBreadcrumb();
        var stepper = new NexaUI.Controls.NexaStepper();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            tab.Dispose();
            nav.Dispose();
            bc.Dispose();
            stepper.Dispose();
        }
    }
}
