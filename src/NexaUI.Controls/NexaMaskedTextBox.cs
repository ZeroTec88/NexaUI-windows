using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A Bootstrap-style themed masked text input control. Hosts a native <see cref="MaskedTextBox"/>
/// so all native masking behavior, validation, and accessibility remain intact. Adds themed
/// borders, focus glow, validation icons, helper text, and an optional built-in label.
/// </summary>
[DefaultEvent(nameof(TextChanged))]
[DefaultProperty(nameof(Mask))]
public class NexaMaskedTextBox : NexaInputHost
{
    private readonly MaskedTextBox _edit;

    public NexaMaskedTextBox()
    {
        TabStop = true;
        _edit = new MaskedTextBox
        {
            Dock = DockStyle.None,
            BorderStyle = BorderStyle.None,
            TabStop = false,
            BackColor = Color.White
        };
        _edit.TextChanged += (_, _) => TextChanged?.Invoke(this, EventArgs.Empty);
        _edit.MaskInputRejected += (_, e) => MaskInputRejected?.Invoke(this, e);
        _edit.GotFocus += (_, _) => BorderHost.Invalidate();
        _edit.LostFocus += (_, _) => BorderHost.Invalidate();
        _edit.KeyDown += (_, e) => KeyDown?.Invoke(this, e);
        _edit.KeyPress += (_, e) => KeyPress?.Invoke(this, e);
        _edit.KeyUp += (_, e) => KeyUp?.Invoke(this, e);
        _edit.Enter += (_, _) => Enter?.Invoke(this, EventArgs.Empty);
        _edit.Leave += (_, _) => Leave?.Invoke(this, EventArgs.Empty);
        _edit.GotFocus += (_, _) => GotFocus?.Invoke(this, EventArgs.Empty);
        _edit.LostFocus += (_, _) => LostFocus?.Invoke(this, EventArgs.Empty);

        BorderHost.Controls.Add(_edit);

        HandleCreated += (_, _) => LayoutInner();
        BorderHost.Resize += (_, _) => LayoutInner();
    }

    [Browsable(false)]
    public MaskedTextBox InnerMaskedTextBox => _edit;

#pragma warning disable WFO1000
    [AllowNull]
    public new string Text
    {
        get => _edit.Text;
        set { _edit.Text = value ?? string.Empty; }
    }

    public string Mask
    {
        get => _edit.Mask;
        set => _edit.Mask = value ?? string.Empty;
    }

    public char PromptChar
    {
        get => _edit.PromptChar;
        set => _edit.PromptChar = value;
    }

    public bool AsciiOnly { get => _edit.AsciiOnly; set => _edit.AsciiOnly = value; }
    public bool BeepOnError { get => _edit.BeepOnError; set => _edit.BeepOnError = value; }
    public MaskFormat CutCopyMaskFormat { get => _edit.CutCopyMaskFormat; set => _edit.CutCopyMaskFormat = value; }
    public bool IncludeLiterals
    {
        get => (_edit.TextMaskFormat & MaskFormat.IncludeLiterals) != 0;
        set => _edit.TextMaskFormat = value
            ? _edit.TextMaskFormat | MaskFormat.IncludeLiterals
            : _edit.TextMaskFormat & ~MaskFormat.IncludeLiterals;
    }
    public bool IncludePrompt
    {
        get => (_edit.TextMaskFormat & MaskFormat.IncludePrompt) != 0;
        set => _edit.TextMaskFormat = value
            ? _edit.TextMaskFormat | MaskFormat.IncludePrompt
            : _edit.TextMaskFormat & ~MaskFormat.IncludePrompt;
    }
    public bool HidePromptOnLeave { get => _edit.HidePromptOnLeave; set => _edit.HidePromptOnLeave = value; }
    public Type? ValidatingType { get => _edit.ValidatingType; set => _edit.ValidatingType = value; }
    public int SelectionStart { get => _edit.SelectionStart; set => _edit.SelectionStart = value; }
    public int SelectionLength { get => _edit.SelectionLength; set => _edit.SelectionLength = value; }
    public string SelectedText { get => _edit.SelectedText; set => _edit.SelectedText = value; }
    public bool HideSelection { get => _edit.HideSelection; set => _edit.HideSelection = value; }
    public bool ReadOnly { get => _edit.ReadOnly; set => _edit.ReadOnly = value; }
    public bool Multiline { get => _edit.Multiline; set => _edit.Multiline = value; }
    public new int MaxLength
    {
        get => _edit.MaxLength;
        set { _edit.MaxLength = value; base.MaxLength = value; BorderHost.Invalidate(); }
    }

    public bool MaskCompleted => _edit.MaskCompleted;
    public bool MaskFull => _edit.MaskFull;
#pragma warning restore WFO1000

    [Browsable(true)]
#pragma warning disable CS8765
    public override Font Font
    {
        get => base.Font;
        set { base.Font = value!; _edit.Font = value; Invalidate(); }
    }
#pragma warning restore CS8765

    public new event EventHandler? TextChanged;
    public event MaskInputRejectedEventHandler? MaskInputRejected;
    public new event KeyEventHandler? KeyDown;
    public new event KeyPressEventHandler? KeyPress;
    public new event KeyEventHandler? KeyUp;
    public new event EventHandler? Enter;
    public new event EventHandler? Leave;
    public new event EventHandler? GotFocus;
    public new event EventHandler? LostFocus;

    public void Clear() => _edit.Clear();
    public void SelectAll() => _edit.SelectAll();
    public void Copy() => _edit.Copy();
    public void Cut() => _edit.Cut();
    public void Paste() => _edit.Paste();
    public void Undo() => _edit.Undo();

    protected override bool InnerHasFocus() => _edit.Focused;
    protected override int InnerTextLength() => _edit.TextLength;

    protected override void OnGotFocus(System.EventArgs e)
    {
        base.OnGotFocus(e);
        if (!_edit.Focused) _edit.Focus();
    }

    protected override void OnEnabledChanged(System.EventArgs e)
    {
        base.OnEnabledChanged(e);
        _edit.Enabled = Enabled;
        BorderHost.Invalidate();
    }

    private void LayoutInner()
    {
        if (BorderHost is null || BorderHost.Width == 0) return;
        float dpi = CurrentDpi();
        var inner = new Rectangle(0, 0, BorderHost.Width, BorderHost.Height);
        var topPad = VerticalPaddingFor(dpi);
        var sidePad = NexaDpi.Scale(12, dpi);
        var rightReserved = HasValidationIcon() ? ValidationIconReservedWidth(dpi) : (ShowCounter ? CounterReservedWidth(dpi) : sidePad);

        _edit.Bounds = new Rectangle(
            inner.Left + sidePad,
            inner.Top + topPad,
            Math.Max(0, inner.Width - sidePad - rightReserved),
            Math.Max(0, inner.Height - 2 * topPad));
    }

    protected override void OnThemeApplied(ITheme theme)
    {
        base.OnThemeApplied(theme);
        ApplyTheme(theme);
        LayoutInner();
    }

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        var palette = theme.Palette;
        var typography = theme.Typography;
        float dpi = CurrentDpi();

        var textColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        var disabled = (Color)palette[NexaColorRole.TextDisabled].Value;

        _edit.ForeColor = Enabled ? textColor : disabled;
        _edit.BackColor = Style == NexaInputStyle.Filled
            ? (Color)palette[NexaColorRole.SurfaceVariant].Value
            : (Color)palette[NexaColorRole.InputBackground].Value;
        _edit.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
    }
}
