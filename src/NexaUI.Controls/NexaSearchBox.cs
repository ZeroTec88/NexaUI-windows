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
/// A Bootstrap-style themed search field. Hosts a native <see cref="TextBox"/> inside a themed
/// border, adds a search icon prefix and an optional clear button, and supports a debounced
/// <see cref="SearchChanged"/> event.
/// </summary>
[DefaultEvent(nameof(SearchChanged))]
[DefaultProperty(nameof(SearchText))]
public class NexaSearchBox : NexaInputHost
{
    private const int DefaultSearchDelayMs = 250;

    private readonly TextBox _edit;
    private readonly Panel _leftIconPanel;
    private readonly Panel _clearIconPanel;
    private readonly System.Windows.Forms.Timer _debounceTimer;

    private string _searchText = string.Empty;
    private int _searchDelayMs = DefaultSearchDelayMs;
    private bool _showClearButton = true;
    private bool _escapeClearsText = true;
    private bool _suppressSearchChanged;
    private bool _debounceActive;
    private bool _clearHovered;
    private string _placeholderText = string.Empty;

    public NexaSearchBox()
    {
        TabStop = true;

        _edit = new TextBox
        {
            BorderStyle = BorderStyle.None,
            TabStop = false,
            BackColor = Color.White
        };
        _edit.TextChanged += OnInnerTextChanged;
        _edit.GotFocus += (_, _) => BorderHost.Invalidate();
        _edit.LostFocus += (_, _) => BorderHost.Invalidate();
        _edit.KeyDown += OnInnerKeyDown;
        _edit.KeyPress += (_, e) => KeyPress?.Invoke(this, e);
        _edit.KeyUp += (_, e) => KeyUp?.Invoke(this, e);
        _edit.Enter += (_, _) => Enter?.Invoke(this, EventArgs.Empty);
        _edit.Leave += (_, _) => Leave?.Invoke(this, EventArgs.Empty);
        _edit.GotFocus += (_, _) => GotFocus?.Invoke(this, EventArgs.Empty);
        _edit.LostFocus += (_, _) => LostFocus?.Invoke(this, EventArgs.Empty);

        _leftIconPanel = new Panel
        {
            BackColor = Color.Transparent,
            Cursor = Cursors.Default
        };
        _leftIconPanel.Paint += OnLeftIconPaint;

        _clearIconPanel = new Panel
        {
            BackColor = Color.Transparent,
            Cursor = Cursors.Hand,
            Visible = false
        };
        _clearIconPanel.Paint += OnClearIconPaint;
        _clearIconPanel.MouseDown += (_, _) => Clear();
        _clearIconPanel.MouseMove += (_, _) => { _clearHovered = true; _clearIconPanel.Invalidate(); };
        _clearIconPanel.MouseLeave += (_, _) => { _clearHovered = false; _clearIconPanel.Invalidate(); };

        BorderHost.Controls.Add(_edit);
        BorderHost.Controls.Add(_leftIconPanel);
        BorderHost.Controls.Add(_clearIconPanel);

        _debounceTimer = new System.Windows.Forms.Timer { Interval = _searchDelayMs };
        _debounceTimer.Tick += OnDebounceTick;

        HandleCreated += (_, _) =>
        {
            ApplyPlaceholder();
            LayoutInner();
        };
        BorderHost.HandleCreated += (_, _) => ApplyPlaceholder();
        BorderHost.Resize += (_, _) => LayoutInner();
        Disposed += OnSelfDisposed;
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();
        _debounceTimer.Tick -= OnDebounceTick;
        _debounceTimer.Dispose();
    }

    [Browsable(false)]
    public TextBox InnerTextBox => _edit;

#pragma warning disable WFO1000
    [AllowNull]
    public string SearchText
    {
        get => _searchText;
        set
        {
            _suppressSearchChanged = true;
            try { _edit.Text = value ?? string.Empty; }
            finally { _suppressSearchChanged = false; }
            _searchText = _edit.Text;
            LayoutInner();
        }
    }

    [AllowNull]
    public string PlaceholderText
    {
        get => _placeholderText;
        set { _placeholderText = value ?? string.Empty; ApplyPlaceholder(); }
    }

    public int SearchDelayMs
    {
        get => _searchDelayMs;
        set
        {
            _searchDelayMs = Math.Max(0, value);
            _debounceTimer.Interval = Math.Max(1, _searchDelayMs);
        }
    }

    public bool ShowClearButton
    {
        get => _showClearButton;
        set { _showClearButton = value; LayoutInner(); }
    }

    public bool EscapeClearsText
    {
        get => _escapeClearsText;
        set => _escapeClearsText = value;
    }

    public bool ReadOnly
    {
        get => _edit.ReadOnly;
        set { _edit.ReadOnly = value; LayoutInner(); }
    }
#pragma warning restore WFO1000

    public new event EventHandler? Enter;
    public new event EventHandler? Leave;
    public new event EventHandler? GotFocus;
    public new event EventHandler? LostFocus;
    public new event KeyPressEventHandler? KeyPress;
    public new event KeyEventHandler? KeyUp;
    public event KeyEventHandler? SearchBoxKeyDown;
    public event EventHandler<SearchChangedEventArgs>? SearchChanged;
    public event EventHandler? Cleared;

    public void Clear()
    {
        _suppressSearchChanged = true;
        try { _edit.Clear(); }
        finally { _suppressSearchChanged = false; }
        _searchText = string.Empty;
        _debounceTimer.Stop();
        _debounceActive = false;
        LayoutInner();
        RaiseSearchChangedNow();
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    public void SelectAll() => _edit.SelectAll();
    public void FocusEdit() => _edit.Focus();

    public bool IsDebounceActive => _debounceActive;

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
        var sidePad = NexaDpi.Scale(10, dpi);
        var leftReserved = NexaDpi.Scale(34, dpi);
        var rightReserved = (_showClearButton && !_edit.ReadOnly && _edit.TextLength > 0) ? NexaDpi.Scale(30, dpi) : sidePad;

        _edit.Bounds = new Rectangle(
            inner.Left + sidePad + leftReserved,
            inner.Top + topPad,
            Math.Max(0, inner.Width - sidePad - leftReserved - rightReserved),
            Math.Max(0, inner.Height - 2 * topPad));

        _leftIconPanel.Bounds = new Rectangle(inner.Left, inner.Top, leftReserved, inner.Height);

        if (_showClearButton && !_edit.ReadOnly && _edit.TextLength > 0)
        {
            _clearIconPanel.Visible = true;
            _clearIconPanel.Bounds = new Rectangle(inner.Right - rightReserved, inner.Top, rightReserved, inner.Height);
            _clearIconPanel.Invalidate();
        }
        else
        {
            _clearIconPanel.Visible = false;
        }
    }

    private void OnInnerTextChanged(object? sender, EventArgs e)
    {
        _searchText = _edit.Text;
        LayoutInner();

        if (_suppressSearchChanged) return;

        if (_searchDelayMs <= 0)
        {
            RaiseSearchChangedNow();
        }
        else
        {
            _debounceTimer.Stop();
            _debounceTimer.Interval = _searchDelayMs;
            _debounceTimer.Start();
            _debounceActive = true;
        }
    }

    private void OnDebounceTick(object? sender, EventArgs e)
    {
        _debounceTimer.Stop();
        _debounceActive = false;
        RaiseSearchChangedNow();
    }

    private void RaiseSearchChangedNow()
    {
        SearchChanged?.Invoke(this, new SearchChangedEventArgs(_searchText));
    }

    private void OnInnerKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && _escapeClearsText && _edit.TextLength > 0)
        {
            Clear();
            e.SuppressKeyPress = true;
            e.Handled = true;
            return;
        }
        SearchBoxKeyDown?.Invoke(this, e);
    }

    private void OnLeftIconPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var tint = Enabled
            ? InnerHasFocus()
                ? (Color)palette[NexaColorRole.Primary].Value
                : (Color)palette[NexaColorRole.TextSecondary].Value
            : (Color)palette[NexaColorRole.TextDisabled].Value;
        float dpi = CurrentDpi();
        var size = NexaDpi.Scale(15, dpi);
        var bmp = NexaIconProvider.ToBitmap(NexaIconKind.Search, new Size(size, size), tint);
        if (bmp is null) return;
        var x = (_leftIconPanel.Width - bmp.Width) / 2;
        var y = (_leftIconPanel.Height - bmp.Height) / 2;
        g.DrawImage(bmp, x, y);
    }

    private void OnClearIconPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var dpi = CurrentDpi();
        var panel = _clearIconPanel.ClientRectangle;

        if (_clearHovered)
        {
            var bg = Color.FromArgb(40, (Color)palette[NexaColorRole.TextPrimary].Value);
            using var bgBrush = new SolidBrush(bg);
            g.FillEllipse(bgBrush, panel);
        }

        var tint = _clearHovered
            ? (Color)palette[NexaColorRole.TextPrimary].Value
            : (Color)palette[NexaColorRole.TextSecondary].Value;
        var size = NexaDpi.Scale(13, dpi);
        var bmp = NexaIconProvider.ToBitmap(NexaIconKind.Cross, new Size(size, size), tint);
        if (bmp is null) return;
        g.DrawImage(bmp, (panel.Width - bmp.Width) / 2, (panel.Height - bmp.Height) / 2);
    }

    private void ApplyPlaceholder()
    {
        if (IsDisposed || Disposing) return;
        if (!_edit.IsHandleCreated) return;
        NativeMethods.SetCueBanner(_edit.Handle, _placeholderText ?? string.Empty);
    }

    protected override void OnThemeApplied(ITheme theme)
    {
        base.OnThemeApplied(theme);
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

        _leftIconPanel.BackColor = _edit.BackColor;
        _clearIconPanel.BackColor = _edit.BackColor;
        _leftIconPanel.Invalidate();
        _clearIconPanel.Invalidate();
        LayoutInner();
    }
}

public sealed class SearchChangedEventArgs : System.EventArgs
{
    public SearchChangedEventArgs(string searchText)
    {
        SearchText = searchText;
    }

    public string SearchText { get; }
}
