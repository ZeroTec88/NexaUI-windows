using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed input dialog for getting text input from the user.
/// </summary>
public sealed class NexaInputDialog : NexaDialog
{
    private readonly NexaTextBox _inputBox;
    private readonly Label _promptLabel;

    private string _prompt = string.Empty;
    private string _defaultValue = string.Empty;
    private string _placeholder = string.Empty;
    private bool _multiline = false;

    public NexaInputDialog()
    {
        _promptLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 40,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(0, 0, 0, 8),
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point)
        };

        _inputBox = new NexaTextBox
        {
            Dock = DockStyle.Top,
            Height = 40,
            Margin = new Padding(0, 0, 0, 16)
        };

        ContentPanel.Controls.Add(_inputBox);
        ContentPanel.Controls.Add(_promptLabel);

        AddButton("Cancel", NexaDialogResult.Cancel, NexaButtonStyle.Ghost);
        AddButton("OK", NexaDialogResult.OK, NexaButtonStyle.Primary, isDefault: true);

        Size = new Size(400, 220);
        DialogTitle = "Input";
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Prompt text to display above the input box.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Prompt text to display above the input box.")]
    public string Prompt
    {
        get => _prompt;
        set { _prompt = value ?? string.Empty; _promptLabel.Text = _prompt; }
    }

    /// <summary>Default value for the input box.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Default value for the input box.")]
    public string DefaultValue
    {
        get => _defaultValue;
        set { _defaultValue = value ?? string.Empty; _inputBox.Text = _defaultValue; }
    }

    /// <summary>Placeholder text when the input is empty.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Placeholder text when the input is empty.")]
    public string Placeholder
    {
        get => _placeholder;
        set { _placeholder = value ?? string.Empty; _inputBox.PlaceholderText = _placeholder; }
    }

    /// <summary>Whether the input box allows multiple lines.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the input box allows multiple lines.")]
    public bool Multiline
    {
        get => _multiline;
        set
        {
            _multiline = value;
            _inputBox.Multiline = value;
            if (value)
            {
                _inputBox.Height = 100;
                _inputBox.ScrollBars = ScrollBars.Vertical;
            }
        }
    }

    /// <summary>Gets the entered text after the dialog closes.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string EnteredText => _inputBox.Text;

    /// <summary>Shows an input dialog and returns the entered text if OK was clicked.</summary>
    public static (NexaDialogResult Result, string Text) Show(
        IWin32Window? owner,
        string prompt,
        string caption = "Input",
        string defaultValue = "",
        string placeholder = "",
        bool multiline = false)
    {
        var dialog = new NexaInputDialog
        {
            DialogTitle = caption,
            Prompt = prompt,
            DefaultValue = defaultValue,
            Placeholder = placeholder,
            Multiline = multiline
        };

        var result = owner != null
            ? (NexaDialogResult)dialog.ShowDialog(owner)
            : (NexaDialogResult)dialog.ShowDialog();

        return (result, dialog.EnteredText);
    }

    /// <summary>Shows a simple single-line input dialog.</summary>
    public static (NexaDialogResult Result, string Text) ShowInput(
        IWin32Window? owner,
        string prompt,
        string caption = "Input",
        string defaultValue = "")
    {
        return Show(owner, prompt, caption, defaultValue);
    }

    /// <summary>Shows a multi-line input dialog.</summary>
    public static (NexaDialogResult Result, string Text) ShowMultilineInput(
        IWin32Window? owner,
        string prompt,
        string caption = "Input",
        string defaultValue = "")
    {
        return Show(owner, prompt, caption, defaultValue, multiline: true);
    }
}