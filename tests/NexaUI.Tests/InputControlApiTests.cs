using System.ComponentModel;
using System.Reflection;

namespace NexaUI.Tests;

[TestClass]
public sealed class InputControlApiTests
{
    [TestMethod]
    public void NexaTextBox_Default_Style_Is_Default()
    {
        var prop = typeof(NexaUI.Controls.NexaTextBox).GetProperty(
            nameof(NexaUI.Controls.NexaTextBox.Style),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaTextBoxStyle.Default, attr!.Value);
    }

    [TestMethod]
    public void NexaTextBox_Validation_Defaults_To_None()
    {
        var prop = typeof(NexaUI.Controls.NexaTextBox).GetProperty(
            nameof(NexaUI.Controls.NexaTextBox.ValidationState),
            BindingFlags.Public | BindingFlags.Instance);
        var attr = prop!.GetCustomAttribute<DefaultValueAttribute>();
        Assert.IsNotNull(attr);
        Assert.AreEqual(NexaUI.Core.NexaTextValidationState.None, attr!.Value);
    }

    [TestMethod]
    public void NexaTextBox_Has_Error_And_Success_Booleans()
    {
        var tb = new NexaUI.Controls.NexaTextBox();
        try
        {
            Assert.IsFalse(tb.HasError);
            Assert.IsFalse(tb.HasSuccess);
            tb.ValidationState = NexaUI.Core.NexaTextValidationState.Error;
            Assert.IsTrue(tb.HasError);
            Assert.IsFalse(tb.HasSuccess);
            tb.ValidationState = NexaUI.Core.NexaTextValidationState.Success;
            Assert.IsFalse(tb.HasError);
            Assert.IsTrue(tb.HasSuccess);
        }
        finally { tb.Dispose(); }
    }

    [TestMethod]
    public void NexaTextBox_Clears_Text_On_Clear()
    {
        var tb = new NexaUI.Controls.NexaTextBox { Text = "hello" };
        var fired = false;
        tb.Cleared += (_, _) => fired = true;
        tb.Clear();
        Assert.AreEqual(string.Empty, tb.Text);
        Assert.IsTrue(fired);
        tb.Dispose();
    }

    [TestMethod]
    public void NexaTextBox_Placeholder_Does_Not_Break_Text()
    {
        var tb = new NexaUI.Controls.NexaTextBox { PlaceholderText = "Type here" };
        tb.Text = "value";
        Assert.AreEqual("value", tb.Text);
        tb.Dispose();
    }

    [TestMethod]
    public void NexaTextBox_Forwards_Native_TextChanged()
    {
        var tb = new NexaUI.Controls.NexaTextBox();
        var fired = false;
        tb.TextChanged += (_, _) => fired = true;
        tb.Text = "abc";
        Assert.IsTrue(fired);
        tb.Dispose();
    }

    [TestMethod]
    public void NexaTextBox_ShowClearButton_Toggles_Visibility_Behavior()
    {
        var tb = new NexaUI.Controls.NexaTextBox { ShowClearButton = true, Text = "x" };
        Assert.IsTrue(tb.ShowClearButton);
        tb.ShowClearButton = false;
        Assert.IsFalse(tb.ShowClearButton);
        tb.Dispose();
    }

    [TestMethod]
    public void NexaTextBox_Style_Defaults_To_Default()
    {
        var tb = new NexaUI.Controls.NexaTextBox();
        Assert.AreEqual(NexaUI.Core.NexaTextBoxStyle.Default, tb.Style);
        tb.Dispose();
    }

    [TestMethod]
    public void NexaMaskedTextBox_Forwards_Mask_Property()
    {
        var mtb = new NexaUI.Controls.NexaMaskedTextBox();
        mtb.Mask = "(000) 000-0000";
        Assert.AreEqual("(000) 000-0000", mtb.Mask);
        mtb.Dispose();
    }

    [TestMethod]
    public void NexaMaskedTextBox_Inner_Available()
    {
        var mtb = new NexaUI.Controls.NexaMaskedTextBox();
        Assert.IsNotNull(mtb.InnerMaskedTextBox);
        mtb.Dispose();
    }

    [TestMethod]
    public void NexaMaskedTextBox_Default_Style_Is_Default()
    {
        var mtb = new NexaUI.Controls.NexaMaskedTextBox();
        Assert.AreEqual(NexaUI.Core.NexaTextBoxStyle.Default, mtb.Style);
        mtb.Dispose();
    }

    [TestMethod]
    public void NexaSearchBox_Default_Delay_Is_Positive()
    {
        var sb = new NexaUI.Controls.NexaSearchBox();
        Assert.IsGreaterThanOrEqualTo(0, sb.SearchDelayMs);
        Assert.IsTrue(sb.ShowClearButton);
        Assert.IsTrue(sb.EscapeClearsText);
        sb.Dispose();
    }

    [TestMethod]
    public void NexaSearchBox_Clear_Resets_SearchText()
    {
        var sb = new NexaUI.Controls.NexaSearchBox { SearchText = "hello" };
        var cleared = false;
        sb.Cleared += (_, _) => cleared = true;
        sb.Clear();
        Assert.AreEqual(string.Empty, sb.SearchText);
        Assert.IsTrue(cleared);
        sb.Dispose();
    }

    [TestMethod]
    public void NexaSearchBox_SearchChanged_Fires_Immediately_When_Delay_Zero()
    {
        var sb = new NexaUI.Controls.NexaSearchBox { SearchDelayMs = 0 };
        var hits = 0;
        sb.SearchChanged += (_, _) => hits++;
        // Simulate typing by writing through the inner native TextBox so SearchChanged
        // raises naturally. Programmatic SearchText setters intentionally suppress the event.
        sb.InnerTextBox.Text = "alpha";
        Assert.AreEqual(1, hits, "SearchChanged must fire once when delay is zero.");
        sb.InnerTextBox.Text = "beta";
        Assert.AreEqual(2, hits);
        sb.Dispose();
    }

    [TestMethod]
    public void NexaSearchBox_Delay_Clamped_To_Zero()
    {
        var sb = new NexaUI.Controls.NexaSearchBox { SearchDelayMs = -5 };
        Assert.AreEqual(0, sb.SearchDelayMs);
        sb.Dispose();
    }

    [TestMethod]
    public void NexaSearchBox_Has_DefaultEvent_For_SearchChanged()
    {
        var attrs = typeof(NexaUI.Controls.NexaSearchBox)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual(nameof(NexaUI.Controls.NexaSearchBox.SearchChanged), name);
    }

    [TestMethod]
    public void Input_Controls_Refresh_On_Theme_Change()
    {
        var tb = new NexaUI.Controls.NexaTextBox();
        var sb = new NexaUI.Controls.NexaSearchBox();
        var mb = new NexaUI.Controls.NexaMaskedTextBox();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
            // success = no exception
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            tb.Dispose();
            sb.Dispose();
            mb.Dispose();
        }
    }
}