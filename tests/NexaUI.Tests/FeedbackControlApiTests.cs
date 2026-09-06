using System.ComponentModel;
using System.Windows.Forms;

namespace NexaUI.Tests;

[TestClass]
public sealed class FeedbackControlApiTests
{
    // -------- NexaProgressBar --------

    [TestMethod]
    public void NexaProgressBar_Default_Properties()
    {
        var bar = new NexaUI.Controls.NexaProgressBar();
        Assert.AreEqual(0, bar.Minimum);
        Assert.AreEqual(100, bar.Maximum);
        Assert.AreEqual(0, bar.Value);
        Assert.AreEqual(NexaUI.Core.NexaProgressStyle.Default, bar.ProgressStyle);
        Assert.IsTrue(bar.ShowPercentage);
        Assert.AreEqual("{0:0}%", bar.PercentageFormat);
        Assert.IsFalse(bar.Indeterminate);
        bar.Dispose();
    }

    [TestMethod]
    public void NexaProgressBar_Value_Clamps_To_Range()
    {
        var bar = new NexaUI.Controls.NexaProgressBar { Minimum = 10, Maximum = 50 };
        bar.Value = 100;
        Assert.AreEqual(50, bar.Value);
        bar.Value = -5;
        Assert.AreEqual(10, bar.Value);
        bar.Value = 30;
        Assert.AreEqual(30, bar.Value);
        bar.Dispose();
    }

    [TestMethod]
    public void NexaProgressBar_Value_Changed_Fires_Event()
    {
        var bar = new NexaUI.Controls.NexaProgressBar();
        var fired = 0;
        bar.ValueChanged += (_, _) => fired++;
        bar.Value = 10;
        Assert.AreEqual(1, fired);
        bar.Value = 10;
        Assert.AreEqual(1, fired, "Setting Value to its current value should not fire again.");
        bar.Value = 20;
        Assert.AreEqual(2, fired);
        bar.Dispose();
    }

    [TestMethod]
    public void NexaProgressBar_Percentage_Calculates_Correctly()
    {
        var bar = new NexaUI.Controls.NexaProgressBar { Minimum = 0, Maximum = 200, Value = 50 };
        Assert.AreEqual(25.0, bar.Percentage, 0.001);
        bar.Dispose();
    }

    [TestMethod]
    public void NexaProgressBar_DefaultEvent_Is_ValueChanged()
    {
        var attrs = typeof(NexaUI.Controls.NexaProgressBar)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual(nameof(NexaUI.Controls.NexaProgressBar.ValueChanged), name);
    }

    [TestMethod]
    public void NexaProgressBar_ThemeSwitch_Does_Not_Throw()
    {
        var bar = new NexaUI.Controls.NexaProgressBar();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            bar.Dispose();
        }
    }

    // -------- NexaCircularProgress --------

    [TestMethod]
    public void NexaCircularProgress_Default_Properties()
    {
        var c = new NexaUI.Controls.NexaCircularProgress();
        Assert.AreEqual(0, c.Minimum);
        Assert.AreEqual(100, c.Maximum);
        Assert.AreEqual(0, c.Value);
        Assert.AreEqual(6, c.LineThickness);
        Assert.IsTrue(c.ShowPercentage);
        Assert.AreEqual(string.Empty, c.CenterText);
        Assert.IsFalse(c.Indeterminate);
        c.Dispose();
    }

    [TestMethod]
    public void NexaCircularProgress_Value_Clamps()
    {
        var c = new NexaUI.Controls.NexaCircularProgress { Minimum = 0, Maximum = 10 };
        c.Value = 100;
        Assert.AreEqual(10, c.Value);
        c.Value = -5;
        Assert.AreEqual(0, c.Value);
        c.Value = 5;
        Assert.AreEqual(5, c.Value);
        c.Dispose();
    }

    [TestMethod]
    public void NexaCircularProgress_LineThickness_Clamps()
    {
        var c = new NexaUI.Controls.NexaCircularProgress { LineThickness = 1 };
        Assert.AreEqual(1, c.LineThickness);
        c.LineThickness = 20;
        Assert.AreEqual(20, c.LineThickness);
        c.Dispose();
    }

    [TestMethod]
    public void NexaCircularProgress_CenterText_Roundtrips()
    {
        var c = new NexaUI.Controls.NexaCircularProgress { CenterText = "Loading" };
        Assert.AreEqual("Loading", c.CenterText);
        c.Dispose();
    }

    [TestMethod]
    public void NexaCircularProgress_Percentage_Calculates()
    {
        var c = new NexaUI.Controls.NexaCircularProgress { Value = 75 };
        Assert.AreEqual(75.0, c.Percentage, 0.001);
        c.Dispose();
    }

    // -------- NexaSpinner --------

    [TestMethod]
    public void NexaSpinner_Default_Properties()
    {
        var s = new NexaUI.Controls.NexaSpinner();
        Assert.AreEqual(NexaUI.Core.NexaSpinnerStyle.Ring, s.SpinnerStyle);
        Assert.AreEqual(32, s.SpinnerSize);
        Assert.AreEqual(80, s.AnimationSpeed);
        Assert.IsTrue(s.AnimationEnabled);
        s.Dispose();
    }

    [TestMethod]
    public void NexaSpinner_AnimationEnabled_Can_Be_Toggled()
    {
        var s = new NexaUI.Controls.NexaSpinner { AnimationEnabled = false };
        Assert.IsFalse(s.AnimationEnabled);
        s.Dispose();
    }

    [TestMethod]
    public void NexaSpinner_AnimationSpeed_Clamps()
    {
        var s = new NexaUI.Controls.NexaSpinner();
        s.AnimationSpeed = 5;
        Assert.AreEqual(10, s.AnimationSpeed);
        s.AnimationSpeed = 5000;
        Assert.AreEqual(1000, s.AnimationSpeed);
        s.AnimationSpeed = 100;
        Assert.AreEqual(100, s.AnimationSpeed);
        s.Dispose();
    }

    [TestMethod]
    public void NexaSpinner_Style_Roundtrips()
    {
        var s = new NexaUI.Controls.NexaSpinner { SpinnerStyle = NexaUI.Core.NexaSpinnerStyle.Dots };
        Assert.AreEqual(NexaUI.Core.NexaSpinnerStyle.Dots, s.SpinnerStyle);
        s.Dispose();
    }

    [TestMethod]
    public void NexaSpinner_Dispose_Does_Not_Throw()
    {
        var s = new NexaUI.Controls.NexaSpinner();
        s.Dispose();
        s.Dispose(); // Double dispose safe
    }

    // -------- NexaBadge --------

    [TestMethod]
    public void NexaBadge_Default_Properties()
    {
        var b = new NexaUI.Controls.NexaBadge();
        Assert.AreEqual(string.Empty, b.Text);
        Assert.AreEqual(NexaUI.Core.NexaBadgeStyle.Default, b.BadgeStyle);
        Assert.AreEqual(NexaUI.Core.NexaBadgeSize.Medium, b.BadgeSize);
        Assert.IsTrue(b.AutoSize);
        Assert.AreEqual(0, b.MaximumCharacters);
        b.Dispose();
    }

    [TestMethod]
    public void NexaBadge_Text_Roundtrips()
    {
        var b = new NexaUI.Controls.NexaBadge { Text = "NEW" };
        Assert.AreEqual("NEW", b.Text);
        b.Dispose();
    }

    [TestMethod]
    public void NexaBadge_Style_And_Size_Roundtrip()
    {
        var b = new NexaUI.Controls.NexaBadge
        {
            BadgeStyle = NexaUI.Core.NexaBadgeStyle.Success,
            BadgeSize = NexaUI.Core.NexaBadgeSize.Large
        };
        Assert.AreEqual(NexaUI.Core.NexaBadgeStyle.Success, b.BadgeStyle);
        Assert.AreEqual(NexaUI.Core.NexaBadgeSize.Large, b.BadgeSize);
        b.Dispose();
    }

    [TestMethod]
    public void NexaBadge_MaximumCharacters_Truncates_Numeric_As_99Plus()
    {
        var b = new NexaUI.Controls.NexaBadge { Text = "1234", MaximumCharacters = 2 };
        // Display text is internal; verify via AccessibleName which the control sets to the display text.
        Assert.AreEqual("99+", b.AccessibleName);
        b.Dispose();
    }

    [TestMethod]
    public void NexaBadge_DefaultEvent_Is_TextChanged()
    {
        var attrs = typeof(NexaUI.Controls.NexaBadge)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual("TextChanged", name);
    }

    // -------- NexaAlert --------

    [TestMethod]
    public void NexaAlert_Default_Properties()
    {
        var a = new NexaUI.Controls.NexaAlert();
        Assert.AreEqual(string.Empty, a.Message);
        Assert.AreEqual(string.Empty, a.Title);
        Assert.AreEqual(NexaUI.Core.NexaAlertStyle.Information, a.AlertStyle);
        Assert.IsTrue(a.ShowIcon);
        Assert.IsTrue(a.Closable);
        Assert.IsFalse(a.AutoClose);
        Assert.AreEqual(3000, a.AutoCloseDelayMs);
        a.Dispose();
    }

    [TestMethod]
    public void NexaAlert_Message_And_Title_Roundtrip()
    {
        var a = new NexaUI.Controls.NexaAlert
        {
            Message = "Operation completed.",
            Title = "Success"
        };
        Assert.AreEqual("Operation completed.", a.Message);
        Assert.AreEqual("Success", a.Title);
        a.Dispose();
    }

    [TestMethod]
    public void NexaAlert_AlertStyle_Roundtrips()
    {
        var a = new NexaUI.Controls.NexaAlert { AlertStyle = NexaUI.Core.NexaAlertStyle.Error };
        Assert.AreEqual(NexaUI.Core.NexaAlertStyle.Error, a.AlertStyle);
        a.Dispose();
    }

    [TestMethod]
    public void NexaAlert_ShowIcon_And_Closable_Roundtrip()
    {
        var a = new NexaUI.Controls.NexaAlert
        {
            ShowIcon = false,
            Closable = false
        };
        Assert.IsFalse(a.ShowIcon);
        Assert.IsFalse(a.Closable);
        a.Dispose();
    }

    [TestMethod]
    public void NexaAlert_Close_Hides_And_Raises_Event()
    {
        var a = new NexaUI.Controls.NexaAlert { Visible = true };
        var fired = 0;
        a.Closed += (_, _) => fired++;
        a.Close();
        Assert.IsFalse(a.Visible);
        Assert.AreEqual(1, fired);
        a.Dispose();
    }

    // -------- NexaStatusIndicator --------

    [TestMethod]
    public void NexaStatusIndicator_Default_Properties()
    {
        var s = new NexaUI.Controls.NexaStatusIndicator();
        Assert.AreEqual(NexaUI.Core.NexaStatus.None, s.Status);
        Assert.AreEqual(string.Empty, s.Text);
        Assert.IsTrue(s.ShowText);
        Assert.AreEqual(10, s.IndicatorSize);
        s.Dispose();
    }

    [TestMethod]
    public void NexaStatusIndicator_Status_Roundtrips()
    {
        var s = new NexaUI.Controls.NexaStatusIndicator { Status = NexaUI.Core.NexaStatus.Online };
        Assert.AreEqual(NexaUI.Core.NexaStatus.Online, s.Status);
        s.Dispose();
    }

    [TestMethod]
    public void NexaStatusIndicator_Status_Changed_Fires_Event()
    {
        var s = new NexaUI.Controls.NexaStatusIndicator();
        var fired = 0;
        s.StatusChanged += (_, _) => fired++;
        s.Status = NexaUI.Core.NexaStatus.Offline;
        Assert.AreEqual(1, fired);
        s.Status = NexaUI.Core.NexaStatus.Offline;
        Assert.AreEqual(1, fired);
        s.Status = NexaUI.Core.NexaStatus.Busy;
        Assert.AreEqual(2, fired);
        s.Dispose();
    }

    [TestMethod]
    public void NexaStatusIndicator_Text_And_ShowText_Roundtrip()
    {
        var s = new NexaUI.Controls.NexaStatusIndicator
        {
            Text = "Online",
            ShowText = false
        };
        Assert.AreEqual("Online", s.Text);
        Assert.IsFalse(s.ShowText);
        s.Dispose();
    }

    [TestMethod]
    public void NexaStatusIndicator_AccessibleName_Reflects_Status_And_Text()
    {
        var s = new NexaUI.Controls.NexaStatusIndicator
        {
            Status = NexaUI.Core.NexaStatus.Online,
            Text = "System"
        };
        Assert.IsTrue(s.AccessibleName!.Contains("Online"));
        Assert.IsTrue(s.AccessibleName!.Contains("System"));
        s.Dispose();
    }

    // -------- Cross-control / regression --------

    [TestMethod]
    public void FeedbackControls_All_Dispose_Cleanly()
    {
        var bar = new NexaUI.Controls.NexaProgressBar();
        var circ = new NexaUI.Controls.NexaCircularProgress();
        var spin = new NexaUI.Controls.NexaSpinner();
        var badge = new NexaUI.Controls.NexaBadge();
        var alert = new NexaUI.Controls.NexaAlert();
        var status = new NexaUI.Controls.NexaStatusIndicator();
        bar.Dispose();
        circ.Dispose();
        spin.Dispose();
        badge.Dispose();
        alert.Dispose();
        status.Dispose();
    }
}
