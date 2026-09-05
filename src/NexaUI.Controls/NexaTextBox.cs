using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A Bootstrap-style themed text input control. Hosts a native <see cref="TextBox"/> so all
/// native behavior (typing, selection, copy/paste, undo/redo, IME, accessibility, keyboard
/// navigation) is preserved. Adds themed borders, focus glow, validation icons, helper text,
/// an optional built-in label, and an optional clear button.
/// </summary>
[DefaultEvent(nameof(TextChanged))]
[DefaultProperty(nameof(Text))]
public class NexaTextBox : NexaInputHost
{
    private readonly TextBox _edit;
    private readonly Panel _iconPanel;
    private NexaIconKind _iconKind = NexaIconKind.None;
    private string _placeholderText = string.Empty;
    private bool _showClearButton;
    private bool _iconHovered;

    public NexaTextBox()
    {
        TabStop = true;

        _edit = new TextBox
        {
            BorderStyle = BorderStyle.None,
            TabStop = false,
            BackColor = Color.White
        };
        _edit.TextChanged += (_, _) =>
        {
            LayoutInner();
            TextChanged?.Invoke(this, EventArgs.Empty);
        };
        _edit.GotFocus += (_, _) => { BorderHost.Invalidate(); _iconPanel.Invalidate(); };
        _edit.LostFocus += (_, _) => { BorderHost.Invalidate(); _iconPanel.Invalidate(); };
        _edit.KeyDown += (_, e) => KeyDown?.Invoke(this, e);
        _edit.KeyPress += (_, e) => KeyPress?.Invoke(this, e);
        _edit.KeyUp += (_, e) => KeyUp?.Invoke(this, e);
        _edit.Enter += (_, _) => Enter?.Invoke(this, EventArgs.Empty);
        _edit.Leave += (_, _) => Leave?.Invoke(this, EventArgs.Empty);
        _edit.GotFocus += (_, _) => GotFocus?.Invoke(this, EventArgs.Empty);
        _edit.LostFocus += (_, _) => LostFocus?.Invoke(this, EventArgs.Empty);

        _iconPanel = new Panel
        {
            BackColor = Color.Transparent,
            Cursor = Cursors.Default
        };
        _iconPanel.Paint += OnIconPanelPaint;
        _iconPanel.MouseDown += OnIconPanelClick;
        _iconPanel.MouseMove += OnIconPanelHover;
        _iconPanel.MouseLeave += (_, _) => { _iconHovered = false; _iconPanel.Invalidate(); };

        BorderHost.Controls.Add(_edit);
        BorderHost.Controls.Add(_iconPanel);

        HandleCreated += (_, _) =>
        {
            ApplyPlaceholder();
            LayoutInner();
            ApplyTheme(ThemeManager.Current);
        };
        BorderHost.HandleCreated += (_, _) => ApplyPlaceholder();
        BorderHost.Resize += (_, _) => LayoutInner();
    }
    [Browsable(false)]
    public TextBox InnerTextBox => _edit;

    [AllowNull]
#pragma warning disable WFO1000
    public new string Text
    {
        get => _edit.Text;
        set { _edit.Text = value ?? string.Empty; LayoutInner(); }
    }
#pragma warning restore WFO1000

#pragma warning disable WFO1000
    public new int MaxLength
    {
        get => _edit.MaxLength;
        set { _edit.MaxLength = value; base.MaxLength = value; BorderHost.Invalidate(); }
    }
    public bool Multiline { get => _edit.Multiline; set => _edit.Multiline = value; LayoutInner(); }
    public bool ReadOnly { get => _edit.ReadOnly; set { _edit.ReadOnly = value; LayoutInner(); } }
    public bool UseSystemPasswordChar { get => _edit.UseSystemPasswordChar; set => _edit.UseSystemPasswordChar = value; }
    public char PasswordChar { get => _edit.PasswordChar; set => _edit.PasswordChar = value; }
    public CharacterCasing CharacterCasing { get => _edit.CharacterCasing; set => _edit.CharacterCasing = value; }
    public ScrollBars ScrollBars { get => _edit.ScrollBars; set => _edit.ScrollBars = value; }
    public bool WordWrap { get => _edit.WordWrap; set => _edit.WordWrap = value; }
    public bool AcceptsReturn { get => _edit.AcceptsReturn; set => _edit.AcceptsReturn = value; }
    public bool AcceptsTab { get => _edit.AcceptsTab; set => _edit.AcceptsTab = value; }
    public int SelectionStart { get => _edit.SelectionStart; set => _edit.SelectionStart = value; }
    public int SelectionLength { get => _edit.SelectionLength; set => _edit.SelectionLength = value; }
    public string SelectedText { get => _edit.SelectedText; set => _edit.SelectedText = value; }
    public bool HideSelection { get => _edit.HideSelection; set => _edit.HideSelection = value; }
    public string PlaceholderText
    {
        get => _placeholderText;
        set { _placeholderText = value ?? string.Empty; ApplyPlaceholder(); }
    }
    public bool ShowClearButton
    {
        get => _showClearButton;
        set { _showClearButton = value; LayoutInner(); }
    }
#pragma warning restore WFO1000

    [Category("NexaUI")]
    [DefaultValue(NexaIconKind.None)]
    [Description("Optional icon shown on the left side of the input.")]
    public NexaIconKind Icon
    {
        get => _iconKind;
        set { _iconKind = value; _iconPanel.Invalidate(); LayoutInner(); }
    }

    [Browsable(true)]
#pragma warning disable CS8765
    public override Font Font
    {
        get => base.Font;
        set { base.Font = value!; _edit.Font = value; Invalidate(); }
    }
#pragma warning restore CS8765

    public new event EventHandler? TextChanged;
    public new event KeyEventHandler? KeyDown;
    public new event KeyPressEventHandler? KeyPress;
    public new event KeyEventHandler? KeyUp;
    public new event EventHandler? Enter;
    public new event EventHandler? Leave;
    public new event EventHandler? GotFocus;
    public new event EventHandler? LostFocus;
    public event EventHandler? Cleared;

    public void Clear()
    {
        _edit.Clear();
        LayoutInner();
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    public void SelectAll() => _edit.SelectAll();
    public void Copy() => _edit.Copy();
    public void Cut() => _edit.Cut();
    public void Paste() => _edit.Paste();
    public void Undo() => _edit.Undo();

    protected override bool InnerHasFocus() => _edit.Focused;
    protected override int InnerTextLength() => _edit.TextLength;

    protected override void OnEnabledChanged(System.EventArgs e)
    {
        base.OnEnabledChanged(e);
        _edit.Enabled = Enabled;
        BorderHost.Invalidate();
        _iconPanel.Invalidate();
    }

    protected override void OnGotFocus(System.EventArgs e)
    {
        base.OnGotFocus(e);
        if (!_edit.Focused) _edit.Focus();
    }

    private void LayoutInner()
    {
        if (BorderHost is null || BorderHost.Width == 0) return;
        var dpi = CurrentDpi();
        var inner = new Rectangle(0, 0, BorderHost.Width, BorderHost.Height);
        var topPad = VerticalPaddingFor(dpi);
        var sidePad = NexaDpi.Scale(12, dpi);

        var leftReserved = 0;
        if (_iconKind != NexaIconKind.None) leftReserved = NexaDpi.Scale(36, dpi);
        var rightReserved = 0;
        if (_showClearButtonActive) rightReserved = NexaDpi.Scale(32, dpi);

        var editRect = new Rectangle(
            inner.Left + sidePad + leftReserved,
            inner.Top + topPad,
            Math.Max(0, inner.Width - sidePad - leftReserved - sidePad - rightReserved),
            Math.Max(0, inner.Height - 2 * topPad));
        _edit.Bounds = editRect;

        if (_iconKind != NexaIconKind.None || _showClearButtonActive)
        {
            _iconPanel.Visible = true;
            if (_iconKind != NexaIconKind.None)
            {
                _iconPanel.Bounds = new Rectangle(inner.Left, inner.Top, NexaDpi.Scale(36, dpi), inner.Height);
                _iconPanel.Cursor = Cursors.Default;
            }
            else
            {
                _iconPanel.Bounds = new Rectangle(inner.Right - NexaDpi.Scale(32, dpi), inner.Top, NexaDpi.Scale(32, dpi), inner.Height);
                _iconPanel.Cursor = Cursors.Hand;
            }
        }
        else
        {
            _iconPanel.Visible = false;
        }
    }

    private bool _showClearButtonActive => _showClearButton && !_edit.ReadOnly && _edit.TextLength > 0;

    private void OnIconPanelPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = CurrentDpi();

        if (_showClearButtonActive)
        {
            var tint = _iconHovered
                ? (Color)palette[NexaColorRole.TextPrimary].Value
                : (Color)palette[NexaColorRole.TextSecondary].Value;
            var size = NexaDpi.Scale(14, dpi);
            var bmp = NexaIconProvider.ToBitmap(NexaIconKind.Cross, new Size(size, size), tint);
            if (bmp is not null)
            {
                g.DrawImage(bmp, (_iconPanel.Width - bmp.Width) / 2, (_iconPanel.Height - bmp.Height) / 2);
            }
        }
        else if (_iconKind != NexaIconKind.None)
        {
            var tint = Enabled
                ? InnerHasFocus()
                    ? (Color)palette[NexaColorRole.Primary].Value
                    : (Color)palette[NexaColorRole.TextSecondary].Value
                : (Color)palette[NexaColorRole.TextDisabled].Value;
            var size = NexaDpi.Scale(16, dpi);
            var bmp = NexaIconProvider.ToBitmap(_iconKind, new Size(size, size), tint);
            if (bmp is not null)
            {
                g.DrawImage(bmp, NexaDpi.Scale(10, dpi), (_iconPanel.Height - bmp.Height) / 2);
            }
        }
    }

    private void OnIconPanelHover(object? sender, MouseEventArgs e)
    {
        var hovered = _iconPanel.ClientRectangle.Contains(e.Location);
        if (hovered != _iconHovered)
        {
            _iconHovered = hovered;
            _iconPanel.Invalidate();
        }
    }

    private void OnIconPanelClick(object? sender, MouseEventArgs e)
    {
        if (_showClearButtonActive)
        {
            Clear();
        }
    }

    private void ApplyPlaceholder()
    {
        if (IsDisposed || Disposing) return;
        if (!_edit.IsHandleCreated) return;
        NativeMethods.SetCueBanner(_edit.Handle, _placeholderText ?? string.Empty);
    }

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        var textColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        var disabled = (Color)palette[NexaColorRole.TextDisabled].Value;

        _edit.ForeColor = Enabled ? textColor : disabled;
        _edit.BackColor = Style == NexaInputStyle.Filled
            ? (Color)palette[NexaColorRole.SurfaceVariant].Value
            : (Color)palette[NexaColorRole.InputBackground].Value;
        _edit.Font = typography.ToFont(NexaTypographyRole.Body, dpi);
        _iconPanel.BackColor = _edit.BackColor;
        _iconPanel.Invalidate();
    }

    private void OnThemeChangedHandler(object? sender, ThemeChangedEventArgs e) => ApplyTheme(e.Current);

    protected override void OnThemeApplied(ITheme theme)
    {
        base.OnThemeApplied(theme);
        ApplyTheme(theme);
        LayoutInner();
    }
}
