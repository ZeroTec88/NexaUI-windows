using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed message box dialog with customizable buttons, icons, and styling.
/// Replaces the standard <see cref="MessageBox"/> with a fully themeable alternative.
/// </summary>
public sealed class NexaMessageBox : NexaDialog
{
    private readonly Label _messageLabel;
    private readonly Label _iconLabel;
    private readonly TableLayoutPanel _mainLayout;
    private NexaIconKind _iconKind = NexaIconKind.Info;

    private NexaMessageBox()
    {
        _mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Padding = new Padding(8)
        };
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 48F));
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        _iconLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 24F, FontStyle.Regular, GraphicsUnit.Point)
        };

        _messageLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point)
        };

        _mainLayout.Controls.Add(_iconLabel, 0, 0);
        _mainLayout.Controls.Add(_messageLabel, 1, 0);

        ContentPanel.Controls.Add(_mainLayout);

        Size = new Size(420, 180);
        DialogTitle = "Message";
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Icon to display in the message box.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaIconKind.Info)]
    [Description("Icon to display in the message box.")]
    public NexaIconKind IconKind
    {
        get => _iconKind;
        set
        {
            _iconKind = value;
            UpdateIcon();
        }
    }

    /// <summary>Message text to display.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Message text to display.")]
    public string Message
    {
        get => _messageLabel.Text;
        set { _messageLabel.Text = value ?? string.Empty; }
    }

    /// <summary>Shows a message box with the specified text, caption, buttons, and icon.</summary>
    public static NexaDialogResult Show(
        IWin32Window? owner,
        string text,
        string caption = "Message",
        NexaDialogStyle style = NexaDialogStyle.Standard,
        params (string Text, NexaDialogResult Result, NexaButtonStyle Style, bool IsDefault)[] buttons)
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            DialogStyle = style
        };

        // Default buttons if none provided
        if (buttons.Length == 0)
        {
            buttons = new[]
            {
                ("OK", NexaDialogResult.OK, NexaButtonStyle.Primary, true)
            };
        }

        foreach (var (btnText, result, btnStyle, isDefault) in buttons)
        {
            box.AddButton(btnText, result, btnStyle, isDefault: isDefault);
        }

        return owner != null
            ? (NexaDialogResult)box.ShowDialog(owner)
            : (NexaDialogResult)box.ShowDialog();
    }

    /// <summary>Shows an information message box.</summary>
    public static NexaDialogResult ShowInformation(IWin32Window? owner, string text, string caption = "Information")
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            IconKind = NexaIconKind.Info
        };
        box.AddButton("OK", NexaDialogResult.OK, NexaButtonStyle.Primary, isDefault: true);
        return (NexaDialogResult)box.ShowDialog(owner);
    }

    /// <summary>Shows a success message box.</summary>
    public static NexaDialogResult ShowSuccess(IWin32Window? owner, string text, string caption = "Success")
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            IconKind = NexaIconKind.Check
        };
        box.AddButton("OK", NexaDialogResult.OK, NexaButtonStyle.Success, isDefault: true);
        return (NexaDialogResult)box.ShowDialog(owner);
    }

    /// <summary>Shows a warning message box.</summary>
    public static NexaDialogResult ShowWarning(IWin32Window? owner, string text, string caption = "Warning")
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            IconKind = NexaIconKind.Warning
        };
        box.AddButton("OK", NexaDialogResult.OK, NexaButtonStyle.Warning, isDefault: true);
        return (NexaDialogResult)box.ShowDialog(owner);
    }

    /// <summary>Shows an error message box.</summary>
    public static NexaDialogResult ShowError(IWin32Window? owner, string text, string caption = "Error")
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            IconKind = NexaIconKind.Error
        };
        box.AddButton("OK", NexaDialogResult.OK, NexaButtonStyle.Danger, isDefault: true);
        return (NexaDialogResult)box.ShowDialog(owner);
    }

    /// <summary>Shows a question message box with Yes/No buttons.</summary>
    public static NexaDialogResult ShowQuestion(IWin32Window? owner, string text, string caption = "Question")
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            IconKind = NexaIconKind.Question
        };
        box.AddButton("Yes", NexaDialogResult.Yes, NexaButtonStyle.Primary, isDefault: true);
        box.AddButton("No", NexaDialogResult.No, NexaButtonStyle.Secondary);
        return (NexaDialogResult)box.ShowDialog(owner);
    }

    /// <summary>Shows a confirmation message box with Yes/No/Cancel buttons.</summary>
    public static NexaDialogResult ShowConfirm(IWin32Window? owner, string text, string caption = "Confirm")
    {
        var box = new NexaMessageBox
        {
            DialogTitle = caption,
            Message = text,
            IconKind = NexaIconKind.Question
        };
        box.AddButton("Yes", NexaDialogResult.Yes, NexaButtonStyle.Primary, isDefault: true);
        box.AddButton("No", NexaDialogResult.No, NexaButtonStyle.Secondary);
        box.AddButton("Cancel", NexaDialogResult.Cancel, NexaButtonStyle.Ghost);
        return (NexaDialogResult)box.ShowDialog(owner);
    }

    private void UpdateIcon()
    {
        var iconColor = _iconKind switch
        {
            NexaIconKind.Check => ThemeManager.Current.Palette[NexaColorRole.Success].Value,
            NexaIconKind.Warning => ThemeManager.Current.Palette[NexaColorRole.Warning].Value,
            NexaIconKind.Error => ThemeManager.Current.Palette[NexaColorRole.Danger].Value,
            NexaIconKind.Question => ThemeManager.Current.Palette[NexaColorRole.Info].Value,
            _ => ThemeManager.Current.Palette[NexaColorRole.Info].Value
        };

        var iconBmp = NexaIconProvider.ToBitmap(_iconKind, new Size(32, 32), (Color)iconColor);
        if (iconBmp != null)
        {
            _iconLabel.Image = iconBmp;
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => OnThemeChanged(sender, e));
            return;
        }
        UpdateIcon();
        RefreshTheme(e.Current);
    }
}