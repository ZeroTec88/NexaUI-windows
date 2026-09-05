using System.Reflection;

namespace NexaUI.Tests;

[TestClass]
public sealed class AssemblyLoadingTests
{
    [TestMethod]
    public void Core_Assembly_Exists()
    {
        var assembly = typeof(NexaUI.Core.NexaUICoreMarker).Assembly;
        Assert.AreEqual("NexaUI.Core", assembly.GetName().Name);
    }

    [TestMethod]
    public void Themes_Assembly_Exists()
    {
        var assembly = typeof(NexaUI.Themes.NexaUIThemesMarker).Assembly;
        Assert.AreEqual("NexaUI.Themes", assembly.GetName().Name);
    }

    [TestMethod]
    public void Icons_Assembly_Exists()
    {
        var assembly = typeof(NexaUI.Icons.NexaUIIconsMarker).Assembly;
        Assert.AreEqual("NexaUI.Icons", assembly.GetName().Name);
    }

    [TestMethod]
    public void Controls_Assembly_Exists()
    {
        var assembly = typeof(NexaUI.Controls.NexaUIControlsMarker).Assembly;
        Assert.AreEqual("NexaUI.Controls", assembly.GetName().Name);
    }

    [TestMethod]
    public void Core_Assembly_Has_Net10_Windows_Target()
    {
        var assembly = typeof(NexaUI.Core.NexaUICoreMarker).Assembly;
        Assert.IsNotNull(assembly.Location,
            "NexaUI.Core assembly must be loaded from disk for verification.");
        StringAssert.Contains(assembly.Location, "net10.0-windows",
            "NexaUI.Core must target net10.0-windows.");
    }
}

[TestClass]
public sealed class NexaButtonApiTests
{
    [TestMethod]
    public void NexaButton_Inherits_From_Native_Button()
    {
        Assert.IsTrue(
            typeof(System.Windows.Forms.Button).IsAssignableFrom(typeof(NexaUI.Controls.NexaButton)),
            "NexaButton must derive from the native System.Windows.Forms.Button.");
    }

    [TestMethod]
    public void NexaButton_Default_Style_Is_Primary()
    {
        var prop = typeof(NexaUI.Controls.NexaButton).GetProperty(
            nameof(NexaUI.Controls.NexaButton.Style),
            BindingFlags.Public | BindingFlags.Instance);
        Assert.IsNotNull(prop);
        var attr = prop!.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaButtonStyle.Primary, attr!.Value);
    }

    [TestMethod]
    public void NexaButton_Default_Size_Is_Medium()
    {
        var prop = typeof(NexaUI.Controls.NexaButton).GetProperty(
            nameof(NexaUI.Controls.NexaButton.SizeMode),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaButtonSize.Medium, attr!.Value);
    }

    [TestMethod]
    public void NexaButton_Has_Designer_Click_Attribute()
    {
        var attrs = typeof(NexaUI.Controls.NexaButton)
            .GetCustomAttributes(typeof(System.ComponentModel.DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs, "NexaButton must declare a DefaultEvent so the Designer wires Click.");
        Assert.AreEqual(nameof(System.Windows.Forms.Button.Click), ((System.ComponentModel.DefaultEventAttribute)attrs[0]!).Name);
    }

    [TestMethod]
    public void NexaButton_Has_Designer_Text_Attribute()
    {
        var attrs = typeof(NexaUI.Controls.NexaButton)
            .GetCustomAttributes(typeof(System.ComponentModel.DefaultPropertyAttribute), inherit: true);
        Assert.IsNotEmpty(attrs, "NexaButton must declare a DefaultProperty for the Designer.");
    }

    [TestMethod]
    public void NexaButton_BorderRadius_Has_Default_Value()
    {
        var prop = typeof(NexaUI.Controls.NexaButton).GetProperty(
            nameof(NexaUI.Controls.NexaButton.BorderRadius),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(6, attr!.Value);
    }

    [TestMethod]
    public void NexaButton_LoadingText_Has_Default_Value()
    {
        var prop = typeof(NexaUI.Controls.NexaButton).GetProperty(
            nameof(NexaUI.Controls.NexaButton.LoadingText),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<System.ComponentModel.DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual("Loading...", attr!.Value);
    }
}