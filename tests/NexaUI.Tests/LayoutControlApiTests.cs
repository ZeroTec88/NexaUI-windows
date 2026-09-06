using System.ComponentModel;
using System.Windows.Forms;

namespace NexaUI.Tests;

[TestClass]
public sealed class LayoutControlApiTests
{
    // -------- NexaPanel --------

    [TestMethod]
    public void NexaPanel_Default_Properties()
    {
        var panel = new NexaUI.Controls.NexaPanel();
        Assert.AreEqual(NexaUI.Core.NexaPanelSurfaceStyle.Default, panel.SurfaceStyle);
        Assert.AreEqual(NexaUI.Core.NexaBorderStyleEx.None, panel.BorderStyleEx);
        Assert.AreEqual(2, panel.CornerRadius);
        Assert.AreEqual(1, panel.BorderThickness);
        Assert.IsFalse(panel.ShadowEnabled);
        Assert.AreEqual(0, panel.ShadowDepth);
        panel.Dispose();
    }

    [TestMethod]
    public void NexaPanel_Inherits_From_Native_Panel()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.Panel).IsAssignableFrom(typeof(NexaUI.Controls.NexaPanel)));
    }

    [TestMethod]
    public void NexaPanel_BorderProperties_Roundtrip()
    {
        var panel = new NexaUI.Controls.NexaPanel
        {
            BorderStyleEx = NexaUI.Core.NexaBorderStyleEx.Solid,
            BorderThickness = 3,
            CornerRadius = 8,
            ShadowEnabled = true,
            ShadowDepth = 4,
            SurfaceStyle = NexaUI.Core.NexaPanelSurfaceStyle.Elevated
        };
        Assert.AreEqual(NexaUI.Core.NexaBorderStyleEx.Solid, panel.BorderStyleEx);
        Assert.AreEqual(3, panel.BorderThickness);
        Assert.AreEqual(8, panel.CornerRadius);
        Assert.IsTrue(panel.ShadowEnabled);
        Assert.AreEqual(4, panel.ShadowDepth);
        Assert.AreEqual(NexaUI.Core.NexaPanelSurfaceStyle.Elevated, panel.SurfaceStyle);
        panel.Dispose();
    }

    [TestMethod]
    public void NexaPanel_ThemeSwitch_Does_Not_Throw()
    {
        var panel = new NexaUI.Controls.NexaPanel();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            panel.Dispose();
        }
    }

    // -------- NexaCard --------

    [TestMethod]
    public void NexaCard_Default_Properties()
    {
        var card = new NexaUI.Controls.NexaCard();
        Assert.AreEqual(string.Empty, card.Title);
        Assert.AreEqual(string.Empty, card.Subtitle);
        Assert.IsTrue(card.HeaderVisible);
        Assert.IsFalse(card.FooterVisible);
        Assert.AreEqual(4, card.CornerRadius);
        Assert.AreEqual(1, card.BorderThickness);
        Assert.IsNotNull(card.ContentPanel);
        Assert.IsNotNull(card.HeaderPanel);
        Assert.IsNotNull(card.FooterPanel);
        card.Dispose();
    }

    [TestMethod]
    public void NexaCard_Title_And_Subtitle_Roundtrip()
    {
        var card = new NexaUI.Controls.NexaCard
        {
            Title = "System Information",
            Subtitle = "Current status"
        };
        Assert.AreEqual("System Information", card.Title);
        Assert.AreEqual("Current status", card.Subtitle);
        card.Dispose();
    }

    [TestMethod]
    public void NexaCard_HeaderAndFooter_Visibility_Roundtrip()
    {
        var card = new NexaUI.Controls.NexaCard
        {
            HeaderVisible = false,
            FooterVisible = true
        };
        Assert.IsFalse(card.HeaderVisible);
        Assert.IsTrue(card.FooterVisible);
        Assert.IsFalse(card.HeaderPanel.Visible);
        Assert.IsTrue(card.FooterPanel.Visible);
        card.Dispose();
    }

    [TestMethod]
    public void NexaCard_ContentPanel_Accepts_Child_Controls()
    {
        var card = new NexaUI.Controls.NexaCard();
        var button = new NexaUI.Controls.NexaButton { Text = "Click me" };
        card.ContentPanel.Controls.Add(button);
        Assert.AreEqual(1, card.ContentPanel.Controls.Count);
        Assert.AreSame(button, card.ContentPanel.Controls[0]);
        card.Dispose();
    }

    [TestMethod]
    public void NexaCard_Shadow_Properties_Roundtrip()
    {
        var card = new NexaUI.Controls.NexaCard
        {
            ShadowEnabled = true,
            ShadowDepth = 6,
            CornerRadius = 10,
            BorderThickness = 2
        };
        Assert.IsTrue(card.ShadowEnabled);
        Assert.AreEqual(6, card.ShadowDepth);
        Assert.AreEqual(10, card.CornerRadius);
        Assert.AreEqual(2, card.BorderThickness);
        card.Dispose();
    }

    [TestMethod]
    public void NexaCard_ThemeSwitch_Does_Not_Throw()
    {
        var card = new NexaUI.Controls.NexaCard();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
            Assert.IsTrue(true);
        }
        catch (System.Exception ex)
        {
            Assert.Fail($"Theme switch threw: {ex.Message}");
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            card.Dispose();
        }
    }

    // -------- NexaGroupBox --------

    [TestMethod]
    public void NexaGroupBox_Inherits_From_Native_GroupBox()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.GroupBox).IsAssignableFrom(typeof(NexaUI.Controls.NexaGroupBox)));
    }

    [TestMethod]
    public void NexaGroupBox_Default_Properties()
    {
        var gb = new NexaUI.Controls.NexaGroupBox();
        Assert.AreEqual(3, gb.CornerRadius);
        Assert.AreEqual(1, gb.BorderThickness);
        Assert.AreEqual(string.Empty, gb.Text);
        gb.Dispose();
    }

    [TestMethod]
    public void NexaGroupBox_Text_And_Children_Roundtrip()
    {
        var gb = new NexaUI.Controls.NexaGroupBox { Text = "Account Settings" };
        var tb = new NexaUI.Controls.NexaTextBox { Text = "value" };
        gb.Controls.Add(tb);
        Assert.AreEqual("Account Settings", gb.Text);
        Assert.AreEqual(1, gb.Controls.Count);
        Assert.AreSame(tb, gb.Controls[0]);
        gb.Dispose();
    }

    [TestMethod]
    public void NexaGroupBox_ThemeSwitch_Does_Not_Throw()
    {
        var gb = new NexaUI.Controls.NexaGroupBox { Text = "Theme test" };
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            gb.Dispose();
        }
    }

    // -------- NexaFlowPanel --------

    [TestMethod]
    public void NexaFlowPanel_Inherits_From_Native_FlowLayoutPanel()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.FlowLayoutPanel).IsAssignableFrom(typeof(NexaUI.Controls.NexaFlowPanel)));
    }

    [TestMethod]
    public void NexaFlowPanel_Default_Properties()
    {
        var flow = new NexaUI.Controls.NexaFlowPanel();
        Assert.AreEqual(FlowDirection.LeftToRight, flow.FlowDirection);
        Assert.IsTrue(flow.WrapContents);
        Assert.IsFalse(flow.BorderEnabled);
        Assert.AreEqual(2, flow.CornerRadius);
        Assert.AreEqual(1, flow.BorderThickness);
        flow.Dispose();
    }

    [TestMethod]
    public void NexaFlowPanel_FlowDirection_And_Children()
    {
        var flow = new NexaUI.Controls.NexaFlowPanel
        {
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        var btn1 = new NexaUI.Controls.NexaButton { Text = "A" };
        var btn2 = new NexaUI.Controls.NexaButton { Text = "B" };
        flow.Controls.Add(btn1);
        flow.Controls.Add(btn2);
        Assert.AreEqual(FlowDirection.TopDown, flow.FlowDirection);
        Assert.IsFalse(flow.WrapContents);
        Assert.AreEqual(2, flow.Controls.Count);
        flow.Dispose();
    }

    // -------- NexaTablePanel --------

    [TestMethod]
    public void NexaTablePanel_Inherits_From_Native_TableLayoutPanel()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.TableLayoutPanel).IsAssignableFrom(typeof(NexaUI.Controls.NexaTablePanel)));
    }

    [TestMethod]
    public void NexaTablePanel_Default_Properties()
    {
        var table = new NexaUI.Controls.NexaTablePanel();
        Assert.IsFalse(table.BorderEnabled);
        Assert.AreEqual(2, table.CornerRadius);
        Assert.AreEqual(1, table.BorderThickness);
        table.Dispose();
    }

    [TestMethod]
    public void NexaTablePanel_RowColumn_And_Children()
    {
        var table = new NexaUI.Controls.NexaTablePanel
        {
            ColumnCount = 2,
            RowCount = 2
        };
        var label1 = new NexaUI.Controls.NexaLabel { Text = "Name" };
        var textBox = new NexaUI.Controls.NexaTextBox { Text = "" };
        table.Controls.Add(label1, 0, 0);
        table.Controls.Add(textBox, 1, 0);
        Assert.AreEqual(2, table.ColumnCount);
        Assert.AreEqual(2, table.RowCount);
        Assert.AreEqual(2, table.Controls.Count);
        Assert.AreSame(label1, table.GetControlFromPosition(0, 0));
        Assert.AreSame(textBox, table.GetControlFromPosition(1, 0));
        table.Dispose();
    }

    // -------- Cross-control / regression --------

    [TestMethod]
    public void LayoutControls_All_Dispose_Cleanly()
    {
        var panel = new NexaUI.Controls.NexaPanel();
        var card = new NexaUI.Controls.NexaCard();
        var gb = new NexaUI.Controls.NexaGroupBox();
        var flow = new NexaUI.Controls.NexaFlowPanel();
        var table = new NexaUI.Controls.NexaTablePanel();
        panel.Dispose();
        card.Dispose();
        gb.Dispose();
        flow.Dispose();
        table.Dispose();
        // No exceptions means clean disposal.
    }

    [TestMethod]
    public void NexaCard_DefaultEvent_Is_Load()
    {
        var attrs = typeof(NexaUI.Controls.NexaCard)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual(nameof(NexaUI.Controls.NexaCard.Load), name);
    }
}
