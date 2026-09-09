using System.Windows.Forms;

namespace NexaUI.Tests;

[TestClass]
public sealed class DialogControlApiTests
{
    // -------- NexaMessageBox --------

    [TestMethod]
    public void NexaMessageBox_Static_ShowInformation_Does_Not_Throw()
    {
        var result = NexaUI.Controls.NexaMessageBox.ShowInformation(null, "Test message", "Test");
        Assert.IsTrue(true); // If we get here without exception, test passes
    }

    [TestMethod]
    public void NexaMessageBox_Static_ShowSuccess_Does_Not_Throw()
    {
        var result = NexaUI.Controls.NexaMessageBox.ShowSuccess(null, "Test message", "Test");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaMessageBox_Static_ShowWarning_Does_Not_Throw()
    {
        var result = NexaUI.Controls.NexaMessageBox.ShowWarning(null, "Test message", "Test");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaMessageBox_Static_ShowError_Does_Not_Throw()
    {
        var result = NexaUI.Controls.NexaMessageBox.ShowError(null, "Test message", "Test");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaMessageBox_Static_ShowQuestion_Does_Not_Throw()
    {
        var result = NexaUI.Controls.NexaMessageBox.ShowQuestion(null, "Test question?", "Test");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaMessageBox_Static_ShowConfirm_Does_Not_Throw()
    {
        var result = NexaUI.Controls.NexaMessageBox.ShowConfirm(null, "Confirm?", "Test");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaMessageBox_Instance_Creation()
    {
        var box = new NexaUI.Controls.NexaMessageBox
        {
            Message = "Test message",
            DialogTitle = "Test Title",
            IconKind = NexaUI.Icons.NexaIconKind.Info
        };
        box.Dispose();
    }

    // -------- NexaInputDialog --------

    [TestMethod]
    public void NexaInputDialog_Static_ShowInput_Does_Not_Throw()
    {
        var (result, text) = NexaUI.Controls.NexaInputDialog.ShowInput(null, "Enter text:", "Title", "Default");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaInputDialog_Static_ShowMultilineInput_Does_Not_Throw()
    {
        var (result, text) = NexaUI.Controls.NexaInputDialog.ShowMultilineInput(null, "Enter text:", "Title", "Default");
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void NexaInputDialog_Instance_Creation()
    {
        var dialog = new NexaUI.Controls.NexaInputDialog
        {
            Prompt = "Test prompt",
            DefaultValue = "Default",
            Placeholder = "Placeholder",
            Multiline = true
        };
        dialog.Dispose();
    }

    // -------- NexaToast --------

    [TestMethod]
    public void NexaToast_Static_Show_Does_Not_Throw()
    {
        var toast = NexaUI.Controls.NexaToast.Show(null, "Test message", "Title", NexaUI.Core.NexaToastStyle.Info);
        Assert.IsNotNull(toast);
        toast.CloseToast();
    }

    [TestMethod]
    public void NexaToast_Static_ShowAllStyles()
    {
        var styles = new[] { NexaUI.Core.NexaToastStyle.Default, NexaUI.Core.NexaToastStyle.Success, NexaUI.Core.NexaToastStyle.Warning, NexaUI.Core.NexaToastStyle.Error, NexaUI.Core.NexaToastStyle.Info };
        foreach (var style in styles)
        {
            var toast = NexaUI.Controls.NexaToast.Show(null, "Test", "Title", style);
            toast.CloseToast();
        }
    }

    [TestMethod]
    public void NexaToast_Instance_Creation()
    {
        var toast = new NexaUI.Controls.NexaToast
        {
            Message = "Test message",
            Title = "Test title",
            Style = NexaUI.Core.NexaToastStyle.Success
        };
        toast.Dispose();
    }

    // -------- NexaToastManager --------

    [TestMethod]
    public void NexaToastManager_Creation()
    {
        var manager = new NexaUI.Controls.NexaToastManager();
        manager.CloseAll();
        manager.Dispose();
    }

    [TestMethod]
    public void NexaToastManager_ShowAllStyles()
    {
        var manager = new NexaUI.Controls.NexaToastManager();
        manager.ShowInfo("Info");
        manager.ShowSuccess("Success");
        manager.ShowWarning("Warning");
        manager.ShowError("Error");
        manager.CloseAll();
        manager.Dispose();
    }

    // -------- NexaToolTip --------

    [TestMethod]
    public void NexaToolTip_Creation()
    {
        var tip = new NexaUI.Controls.NexaToolTip
        {
            InitialDelay = 100,
            AutoPopDelay = 1000,
            Position = NexaUI.Core.NexaTooltipPosition.Top
        };
        tip.Dispose();
    }

    [TestMethod]
    public void NexaToolTip_SetGetRemove()
    {
        var tip = new NexaUI.Controls.NexaToolTip();
        var btn = new Button { Text = "Test" };
        tip.SetToolTip(btn, "Test tooltip");
        Assert.AreEqual("Test tooltip", tip.GetToolTip(btn));
        tip.RemoveToolTip(btn);
        tip.Dispose();
    }

    // -------- NexaPopover --------

    [TestMethod]
    public void NexaPopover_Creation()
    {
        var popover = new NexaUI.Controls.NexaPopover
        {
            Title = "Test Popover",
            Position = NexaUI.Core.NexaPopoverPosition.Bottom,
            Width = 200,
            Height = 150,
            ShowCloseButton = true
        };
        popover.ContentPanel.Controls.Add(new Label { Text = "Test content" });
        popover.Dispose();
    }

    [TestMethod]
    public void NexaPopover_Position_Roundtrip()
    {
        var popover = new NexaUI.Controls.NexaPopover();
        popover.Position = NexaUI.Core.NexaPopoverPosition.Top;
        Assert.AreEqual(NexaUI.Core.NexaPopoverPosition.Top, popover.Position);
        popover.Position = NexaUI.Core.NexaPopoverPosition.Left;
        Assert.AreEqual(NexaUI.Core.NexaPopoverPosition.Left, popover.Position);
        popover.Dispose();
    }

    // -------- NexaLoadingOverlay --------

    [TestMethod]
    public void NexaLoadingOverlay_Creation()
    {
        var overlay = new NexaUI.Controls.NexaLoadingOverlay
        {
            Title = "Loading",
            Message = "Please wait...",
            OverlayStyle = NexaUI.Core.NexaOverlayStyle.Standard
        };
        Assert.IsFalse(overlay.Visible);
        overlay.Dispose();
    }

    [TestMethod]
    public void NexaLoadingOverlay_Show_Hide()
    {
        var overlay = new NexaUI.Controls.NexaLoadingOverlay
        {
            Title = "Loading",
            Message = "Please wait..."
        };
        overlay.Show();
        Assert.IsTrue(overlay.Visible);
        overlay.Hide();
        Assert.IsFalse(overlay.Visible);
        overlay.Dispose();
    }

    [TestMethod]
    public void NexaLoadingOverlay_Style_Roundtrip()
    {
        var overlay = new NexaUI.Controls.NexaLoadingOverlay();
        overlay.OverlayStyle = NexaUI.Core.NexaOverlayStyle.Light;
        Assert.AreEqual(NexaUI.Core.NexaOverlayStyle.Light, overlay.OverlayStyle);
        overlay.OverlayStyle = NexaUI.Core.NexaOverlayStyle.Blur;
        Assert.AreEqual(NexaUI.Core.NexaOverlayStyle.Blur, overlay.OverlayStyle);
        overlay.OverlayStyle = NexaUI.Core.NexaOverlayStyle.Minimal;
        Assert.AreEqual(NexaUI.Core.NexaOverlayStyle.Minimal, overlay.OverlayStyle);
        overlay.Dispose();
    }

    // -------- NexaModalBackground --------

    [TestMethod]
    public void NexaModalBackground_Creation()
    {
        var modal = new NexaUI.Controls.NexaModalBackground
        {
            OverlayStyle = NexaUI.Core.NexaOverlayStyle.Standard,
            ClickToClose = true
        };
        Assert.IsFalse(modal.Visible);
        modal.Dispose();
    }

    [TestMethod]
    public void NexaModalBackground_Show_Hide()
    {
        var modal = new NexaUI.Controls.NexaModalBackground();
        modal.Show();
        Assert.IsTrue(modal.Visible);
        modal.Hide();
        Assert.IsFalse(modal.Visible);
        modal.Dispose();
    }

    [TestMethod]
    public void NexaModalBackground_ShowWithContent()
    {
        var modal = new NexaUI.Controls.NexaModalBackground();
        var content = new Panel { Width = 200, Height = 100 };
        modal.ShowWithContent(content, true);
        Assert.IsTrue(modal.Visible);
        modal.CloseModal();
        Assert.IsFalse(modal.Visible);
        modal.Dispose();
    }

    [TestMethod]
    public void NexaModalBackground_ClickToClose()
    {
        var modal = new NexaUI.Controls.NexaModalBackground { ClickToClose = false };
        Assert.IsFalse(modal.ClickToClose);
        modal.ClickToClose = true;
        Assert.IsTrue(modal.ClickToClose);
        modal.Dispose();
    }

    [TestMethod]
    public void NexaModalBackground_Style_Roundtrip()
    {
        var modal = new NexaUI.Controls.NexaModalBackground();
        modal.OverlayStyle = NexaUI.Core.NexaOverlayStyle.Light;
        Assert.AreEqual(NexaUI.Core.NexaOverlayStyle.Light, modal.OverlayStyle);
        modal.OverlayStyle = NexaUI.Core.NexaOverlayStyle.Blur;
        Assert.AreEqual(NexaUI.Core.NexaOverlayStyle.Blur, modal.OverlayStyle);
        modal.OverlayStyle = NexaUI.Core.NexaOverlayStyle.Minimal;
        Assert.AreEqual(NexaUI.Core.NexaOverlayStyle.Minimal, modal.OverlayStyle);
        modal.Dispose();
    }

    // -------- NexaDialog (base) --------

    [TestMethod]
    public void NexaDialog_Creation()
    {
        var dialog = new TestNexaDialog();
        dialog.DialogTitle = "Test Dialog";
        dialog.DialogStyle = NexaUI.Core.NexaDialogStyle.Card;
        dialog.Dispose();
    }

    // -------- NexaDialog (MessageBox/InputDialog) Theme Switch --------

    [TestMethod]
    public void DialogControls_ThemeSwitch_Does_Not_Throw()
    {
        var box = new NexaUI.Controls.NexaMessageBox();
        var input = new NexaUI.Controls.NexaInputDialog();
        var toast = new NexaUI.Controls.NexaToast();
        var popover = new NexaUI.Controls.NexaPopover();
        var overlay = new NexaUI.Controls.NexaLoadingOverlay();
        var modal = new NexaUI.Controls.NexaModalBackground();
        var tip = new NexaUI.Controls.NexaToolTip();

        var previous = NexaUI.Themes.ThemeManager.Current;
        try
        {
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.DarkTheme());
            NexaUI.Themes.ThemeManager.SetTheme(new NexaUI.Themes.LightTheme());
        }
        finally
        {
            NexaUI.Themes.ThemeManager.SetTheme(previous);
            box.Dispose();
            input.Dispose();
            toast.Dispose();
            popover.Dispose();
            overlay.Dispose();
            modal.Dispose();
            tip.Dispose();
        }
    }
}

// Helper class for testing abstract NexaDialog
internal class TestNexaDialog : NexaUI.Controls.NexaDialog
{
    public TestNexaDialog() { }
}