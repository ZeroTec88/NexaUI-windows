using System.ComponentModel;
using System.Windows.Forms;
using NexaUI.Core;

namespace NexaUI.Tests;

[TestClass]
public sealed class SelectionControlApiTests
{
    [TestMethod]
    public void NexaCheckBox_Default_Is_Unchecked()
    {
        var cb = new NexaUI.Controls.NexaCheckBox();
        Assert.IsFalse(cb.Checked);
        Assert.IsFalse(cb.ThreeState);
        Assert.IsFalse(cb.IsIndeterminate);
        cb.Dispose();
    }

    [TestMethod]
    public void NexaCheckBox_Checked_Toggles()
    {
        var cb = new NexaUI.Controls.NexaCheckBox();
        var fired = 0;
        cb.CheckedChanged += (_, _) => fired++;
        cb.Checked = true;
        Assert.IsTrue(cb.Checked);
        Assert.AreEqual(1, fired);
        cb.Checked = false;
        Assert.IsFalse(cb.Checked);
        Assert.AreEqual(2, fired);
        cb.Dispose();
    }

    [TestMethod]
    public void NexaCheckBox_Indeterminate_State_Works()
    {
        var cb = new NexaUI.Controls.NexaCheckBox { ThreeState = true };
        cb.CheckState = CheckState.Indeterminate;
        Assert.IsTrue(cb.IsIndeterminate);
        Assert.AreEqual(CheckState.Indeterminate, cb.CheckState);
        cb.CheckState = CheckState.Checked;
        Assert.IsFalse(cb.IsIndeterminate);
        cb.Dispose();
    }

    [TestMethod]
    public void NexaCheckBox_DefaultEvent_Is_CheckedChanged()
    {
        var attrs = typeof(NexaUI.Controls.NexaCheckBox)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual(nameof(NexaUI.Controls.NexaCheckBox.CheckedChanged), name);
    }

    [TestMethod]
    public void NexaCheckBox_Inherits_From_Native_CheckBox()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.CheckBox).IsAssignableFrom(typeof(NexaUI.Controls.NexaCheckBox)));
    }

    [TestMethod]
    public void NexaRadioButton_Default_Is_Unchecked()
    {
        var rb = new NexaUI.Controls.NexaRadioButton();
        Assert.IsFalse(rb.Checked);
        rb.Dispose();
    }

    [TestMethod]
    public void NexaRadioButton_Checked_Fires_Event()
    {
        var rb = new NexaUI.Controls.NexaRadioButton();
        var fired = 0;
        rb.CheckedChanged += (_, _) => fired++;
        rb.Checked = true;
        Assert.IsTrue(rb.Checked);
        Assert.AreEqual(1, fired);
        rb.Checked = true;
        Assert.AreEqual(1, fired, "Setting Checked to its current value should not fire again.");
        rb.Dispose();
    }

    [TestMethod]
    public void NexaRadioButton_Group_In_Same_Parent_Is_Mutually_Exclusive()
    {
        var host = new System.Windows.Forms.Panel { Size = new System.Drawing.Size(200, 100) };
        var a = new NexaUI.Controls.NexaRadioButton { Parent = host, Text = "A" };
        var b = new NexaUI.Controls.NexaRadioButton { Parent = host, Text = "B" };
        var c = new NexaUI.Controls.NexaRadioButton { Parent = host, Text = "C" };

        a.Checked = true;
        Assert.IsTrue(a.Checked);
        Assert.IsFalse(b.Checked);
        Assert.IsFalse(c.Checked);

        b.Checked = true;
        Assert.IsFalse(a.Checked, "Native radio grouping must uncheck a when b is selected.");
        Assert.IsTrue(b.Checked);
        Assert.IsFalse(c.Checked);

        host.Dispose();
    }

    [TestMethod]
    public void NexaRadioButton_DefaultEvent_Is_CheckedChanged()
    {
        var attrs = typeof(NexaUI.Controls.NexaRadioButton)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual(nameof(NexaUI.Controls.NexaRadioButton.CheckedChanged), name);
    }

    [TestMethod]
    public void NexaRadioButton_Inherits_From_Native_RadioButton()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.RadioButton).IsAssignableFrom(typeof(NexaUI.Controls.NexaRadioButton)));
    }

    [TestMethod]
    public void Selection_Controls_Refresh_On_Theme_Change()
    {
        var cb = new NexaUI.Controls.NexaCheckBox();
        var rb = new NexaUI.Controls.NexaRadioButton();
        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            cb.Dispose();
            rb.Dispose();
        }
    }

    [TestMethod]
    public void NexaComboBox_Inherits_From_Native_ComboBox()
    {
        Assert.IsTrue(typeof(System.Windows.Forms.ComboBox).IsAssignableFrom(typeof(NexaUI.Controls.NexaComboBox)));
    }

    [TestMethod]
    public void NexaComboBox_Default_Has_No_Items_And_No_Selection()
    {
        var combo = new NexaUI.Controls.NexaComboBox();
        Assert.AreEqual(-1, combo.SelectedIndex);
        Assert.IsNull(combo.SelectedItem);
        Assert.IsNull(combo.SelectedValue);
        Assert.AreEqual(0, combo.Items.Count);
        Assert.AreEqual(NexaComboBoxStyle.Outlined, combo.Style);
        Assert.AreEqual(string.Empty, combo.Text);
        Assert.AreEqual(string.Empty, combo.PlaceholderText);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_Add_Items_And_Select()
    {
        var combo = new NexaUI.Controls.NexaComboBox();
        combo.Items.Add("Alpha");
        combo.Items.Add("Beta");
        combo.Items.Add("Gamma");

        Assert.AreEqual(3, combo.Items.Count);
        combo.SelectedIndex = 1;
        Assert.AreEqual(1, combo.SelectedIndex);
        Assert.AreEqual("Beta", combo.SelectedItem);
        Assert.AreEqual("Beta", combo.Text);

        combo.SelectedItem = "Gamma";
        Assert.AreEqual(2, combo.SelectedIndex);
        Assert.AreEqual("Gamma", combo.Text);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_SelectedIndex_Works_Before_Handle_Is_Created()
    {
        var combo = new NexaUI.Controls.NexaComboBox();
        combo.Items.AddRange(new object[] { "ICT", "Software Engineering", "Graphic Design" });
        combo.SelectedIndex = 0;
        Assert.AreEqual(0, combo.SelectedIndex);
        Assert.AreEqual("ICT", combo.Text);
        combo.Dispose();
    }

    [TestMethod]
    public void Native_ComboBox_DataSource_SelectedIndex_Throws_Before_Handle_Created()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "Mathematics");

        var combo = new System.Windows.Forms.ComboBox
        {
            DataSource = table,
            DisplayMember = "Name",
            ValueMember = "Id"
        };
        Assert.IsFalse(combo.IsHandleCreated);
        try
        {
            combo.SelectedIndex = 0;
        }
        catch (System.ArgumentOutOfRangeException)
        {
            return;
        }
        Assert.Fail("Native ComboBox should have thrown before handle creation with DataSource.");
    }

    [TestMethod]
    public void NexaComboBox_Mirrors_Demo_DataBound_Flow_Without_SelectedIndex()
    {
        var combo = new NexaUI.Controls.NexaComboBox { Width = 240 };
        var table = new System.Data.DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "Mathematics");
        table.Rows.Add(2, "Physics");
        table.Rows.Add(3, "Chemistry");
        table.Rows.Add(4, "Biology");
        combo.DataSource = table;
        combo.DisplayMember = "Name";
        combo.ValueMember = "Id";
        Assert.AreEqual(-1, combo.SelectedIndex);
        Assert.IsNull(combo.SelectedValue);
        Assert.AreEqual(string.Empty, combo.Text);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_Mirrors_Demo_Success_Combo_Flow()
    {
        var combo = new NexaUI.Controls.NexaComboBox
        {
            Width = 240,
            Text = "Mathematics",
            ValidationState = NexaTextValidationState.Success,
            HelperText = "Subject confirmed."
        };
        combo.Items.AddRange(new object[] { "Mathematics", "Physics", "Chemistry" });
        combo.SelectedIndex = 0;
        Assert.AreEqual(0, combo.SelectedIndex);
        Assert.AreEqual("Mathematics", combo.Text);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_Mirrors_Demo_Error_Combo_Flow()
    {
        var combo = new NexaUI.Controls.NexaComboBox
        {
            Width = 240,
            Text = "Not a valid option",
            ValidationState = NexaTextValidationState.Error,
            ErrorText = "Please pick a value from the list"
        };
        combo.Items.AddRange(new object[] { "Apples", "Oranges", "Pears" });
        combo.SelectedIndex = -1;
        Assert.AreEqual(-1, combo.SelectedIndex);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_Mirrors_Demo_Registration_Course_Flow()
    {
        var combo = new NexaUI.Controls.NexaComboBox { Width = 220, Style = NexaComboBoxStyle.Outlined, Dock = DockStyle.Fill };
        combo.Items.AddRange(new object[] { "ICT", "Software Engineering", "Graphic Design", "Networking", "Business Management" });
        combo.SelectedIndex = 0;
        Assert.AreEqual(0, combo.SelectedIndex);
        Assert.AreEqual("ICT", combo.Text);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_DataSource_SelectedValue_Does_Not_Throw_When_No_Selection()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "Mathematics");
        table.Rows.Add(2, "Physics");
        var combo = new NexaUI.Controls.NexaComboBox
        {
            DataSource = table,
            DisplayMember = "Name",
            ValueMember = "Id"
        };
        var sv = combo.SelectedValue;
        Assert.IsNull(sv);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_DataSource_DisplayMember_ValueMember_Are_Applied()
    {
        var table = new System.Data.DataTable();
        table.Columns.Add("Id", typeof(int));
        table.Columns.Add("Name", typeof(string));
        table.Rows.Add(1, "One");
        table.Rows.Add(2, "Two");
        table.Rows.Add(3, "Three");

        var combo = new NexaUI.Controls.NexaComboBox
        {
            DataSource = table,
            DisplayMember = "Name",
            ValueMember = "Id"
        };
        Assert.AreEqual("Name", combo.DisplayMember);
        Assert.AreEqual("Id", combo.ValueMember);
        Assert.IsNotNull(combo.DataSource);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_PlaceholderText_Roundtrips()
    {
        var combo = new NexaUI.Controls.NexaComboBox { PlaceholderText = "Choose..." };
        Assert.AreEqual("Choose...", combo.PlaceholderText);
        Assert.AreEqual(string.Empty, combo.Text);
        combo.Dispose();
    }

    [TestMethod]
    public void NexaComboBox_DefaultEvent_Is_SelectedIndexChanged()
    {
        var attrs = typeof(NexaUI.Controls.NexaComboBox)
            .GetCustomAttributes(typeof(DefaultEventAttribute), inherit: true);
        Assert.IsNotEmpty(attrs);
        var name = ((DefaultEventAttribute)attrs[0]!).Name;
        Assert.AreEqual(nameof(NexaUI.Controls.NexaComboBox.SelectedIndexChanged), name);
    }

    [TestMethod]
    public void NexaToggleSwitch_Default_Is_Unchecked()
    {
        var ts = new NexaUI.Controls.NexaToggleSwitch();
        Assert.IsFalse(ts.Checked);
        Assert.IsTrue(ts.AnimationEnabled);
        Assert.AreEqual("ON", ts.OnText);
        Assert.AreEqual("OFF", ts.OffText);
        Assert.IsFalse(ts.ShowText);
        Assert.AreEqual(NexaToggleSize.Medium, ts.ToggleSize);
        Assert.AreEqual(140, ts.AnimationDurationMs);
        ts.Dispose();
    }

    [TestMethod]
    public void NexaToggleSwitch_Checked_Toggles_And_Fires_Event()
    {
        var ts = new NexaUI.Controls.NexaToggleSwitch();
        var fired = 0;
        ts.CheckedChanged += (_, _) => fired++;
        ts.Checked = true;
        Assert.IsTrue(ts.Checked);
        Assert.AreEqual(1, fired);
        ts.Checked = false;
        Assert.IsFalse(ts.Checked);
        Assert.AreEqual(2, fired);
        ts.Checked = false;
        Assert.AreEqual(2, fired, "Setting Checked to its current value should not fire again.");
        ts.Dispose();
    }

    [TestMethod]
    public void NexaToggleSwitch_OnClick_Toggles_State()
    {
        var ts = new NexaUI.Controls.NexaToggleSwitch();
        Assert.IsFalse(ts.Checked);
        typeof(NexaUI.Controls.NexaToggleSwitch)
            .GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(ts, new object[] { EventArgs.Empty });
        Assert.IsTrue(ts.Checked);
        typeof(NexaUI.Controls.NexaToggleSwitch)
            .GetMethod("OnClick", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!
            .Invoke(ts, new object[] { EventArgs.Empty });
        Assert.IsFalse(ts.Checked);
        ts.Dispose();
    }

    [TestMethod]
    public void NexaToggleSwitch_AnimationEnabled_Can_Be_Disabled()
    {
        var ts = new NexaUI.Controls.NexaToggleSwitch { AnimationEnabled = false };
        Assert.IsFalse(ts.AnimationEnabled);
        ts.Checked = true;
        Assert.IsTrue(ts.Checked);
        ts.Dispose();
    }

    [TestMethod]
    public void NexaToggleSwitch_Property_Hides_Control_Size()
    {
        var prop = typeof(NexaUI.Controls.NexaToggleSwitch).GetProperty(
            nameof(NexaUI.Controls.NexaToggleSwitch.ToggleSize));
        Assert.IsNotNull(prop);
        Assert.AreEqual(typeof(NexaToggleSize), prop!.PropertyType);
    }
}
