using System.Reflection;
using System.ComponentModel;

namespace NexaUI.Tests;

[TestClass]
public sealed class DisplayControlApiTests
{
    [TestMethod]
    public void NexaLabel_Inherits_From_Native_Label()
    {
        Assert.IsTrue(
            typeof(System.Windows.Forms.Label).IsAssignableFrom(typeof(NexaUI.Controls.NexaLabel)),
            "NexaLabel must derive from the native System.Windows.Forms.Label.");
    }

    [TestMethod]
    public void NexaLabel_Default_Style_Is_Default()
    {
        var prop = typeof(NexaUI.Controls.NexaLabel).GetProperty(
            nameof(NexaUI.Controls.NexaLabel.LabelStyle),
            BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(prop);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaLabelStyle.Default, attr!.Value);
    }

    [TestMethod]
    public void NexaLabel_Has_All_Style_Variants()
    {
        var names = new[]
        {
            "Default", "Heading", "Subheading", "Caption", "Muted",
            "Success", "Warning", "Danger", "Info"
        };
        foreach (var name in names)
        {
            Assert.IsTrue(System.Enum.IsDefined(typeof(NexaUI.Core.NexaLabelStyle), name),
                $"NexaLabelStyle.{name} must be defined.");
        }
    }

    [TestMethod]
    public void NexaLabel_Has_DefaultEvent_For_Text()
    {
        var attrs = typeof(NexaUI.Controls.NexaLabel)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
    }

    [TestMethod]
    public void NexaLabel_Disposing_Does_Not_Throw_With_No_Handle()
    {
        var label = new NexaUI.Controls.NexaLabel();
        label.Text = "Dispose me";
        label.Dispose();
        // Pass if no exception
    }

    [TestMethod]
    public void NexaLinkLabel_Inherits_From_Native_LinkLabel()
    {
        Assert.IsTrue(
            typeof(System.Windows.Forms.LinkLabel).IsAssignableFrom(typeof(NexaUI.Controls.NexaLinkLabel)),
            "NexaLinkLabel must derive from the native System.Windows.Forms.LinkLabel.");
    }

    [TestMethod]
    public void NexaLinkLabel_Default_Visited_Is_False()
    {
        var prop = typeof(NexaUI.Controls.NexaLinkLabel).GetProperty(
            nameof(NexaUI.Controls.NexaLinkLabel.Visited),
            BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(prop);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.IsFalse(attr!.Value is bool b ? b : (bool)(attr.Value ?? false));
    }

    [TestMethod]
    public void NexaLinkLabel_Dispose_Does_Not_Throw()
    {
        var link = new NexaUI.Controls.NexaLinkLabel { Text = "x" };
        link.Dispose();
    }

    [TestMethod]
    public void NexaSeparator_Has_Orientation_Default_Horizontal()
    {
        var prop = typeof(NexaUI.Controls.NexaSeparator).GetProperty(
            nameof(NexaUI.Controls.NexaSeparator.Orientation),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaSeparatorOrientation.Horizontal, attr!.Value);
    }

    [TestMethod]
    public void NexaSeparator_Default_Style_Is_Solid()
    {
        var prop = typeof(NexaUI.Controls.NexaSeparator).GetProperty(
            nameof(NexaUI.Controls.NexaSeparator.Style),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaSeparatorStyle.Solid, attr!.Value);
    }

    [TestMethod]
    public void NexaSeparator_Default_Thickness_Is_One_Dip()
    {
        var prop = typeof(NexaUI.Controls.NexaSeparator).GetProperty(
            nameof(NexaUI.Controls.NexaSeparator.ThicknessInDips),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(1, attr!.Value);
    }

    [TestMethod]
    public void NexaSeparator_Defaults_To_Not_Use_Explicit_Color()
    {
        var prop = typeof(NexaUI.Controls.NexaSeparator).GetProperty(
            nameof(NexaUI.Controls.NexaSeparator.UseExplicitColor),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.IsFalse(attr!.Value is bool b ? b : (bool)(attr.Value ?? false));
    }

    [TestMethod]
    public void NexaSeparator_Changing_Orientation_Does_Not_Throw()
    {
        var sep = new NexaUI.Controls.NexaSeparator();
        sep.Orientation = NexaUI.Core.NexaSeparatorOrientation.Vertical;
        Assert.AreEqual(NexaUI.Core.NexaSeparatorOrientation.Vertical, sep.Orientation);
        sep.Dispose();
    }

    [TestMethod]
    public void NexaSeparator_Thickness_Clamped_To_Minimum_One()
    {
        var sep = new NexaUI.Controls.NexaSeparator();
        sep.ThicknessInDips = 0;
        Assert.AreEqual(1, sep.ThicknessInDips, "Thickness must clamp to at least 1 DIP.");
        sep.ThicknessInDips = -5;
        Assert.AreEqual(1, sep.ThicknessInDips);
        sep.Dispose();
    }

    [TestMethod]
    public void NexaLabel_Refreshes_On_Theme_Change()
    {
        var label = new NexaUI.Controls.NexaLabel { Text = "Hello" };
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
            // Pass if no exception
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            label.Dispose();
        }
    }
}