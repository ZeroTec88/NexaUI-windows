using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed search field. Hosts a native <see cref="TextBox"/> inside a themed border,
/// adds a search icon prefix and an optional clear button. Supports an optional debounce
/// so <see cref="SearchChanged"/> fires only after the user stops typing.
/// </summary>
[DefaultEvent(nameof(SearchChanged))]
[DefaultProperty(nameof(SearchText))]
public class NexaSearchBox : NexaInputHost
{
    private const int DefaultSearchDelayMs = 250;

    private readonly TextBox _edit;
    private readonly Button _clearButton;
    private readonly Panel _iconPanel;
    private readonly TableLayoutPanel _layout;
    private readonly System.Windows.Forms.Timer _debounceTimer;

    private string _searchText = string.Empty;
    private int _searchDelayMs = DefaultSearchDelayMs;
    private bool _showClearButton = true;
    private bool _escapeClearsText = true;
    private bool _suppressSearchChanged;
    private bool _debounceActive;

    public NexaSearchBox()
    {
        TabStop = true;

        _layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(0),
            BackColor = Color.Transparent
        };
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 22F));

        _iconPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };
        _iconPanel.Paint += OnIconPaint;

        _edit = new TextBox
        {
            Dock = DockStyle.Fill,
            BorderStyle = BorderStyle.None,
            TabStop = false
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

        _clearButton = new Button
        {
            Text = "✕",
            Dock = DockStyle.Fill,
            Visible = false,
            FlatStyle = FlatStyle.Flat,
            TabStop = false,
            Cursor = Cursors.Hand,
            Margin = new Padding(0),
            Font = new Font("Segoe UI Symbol", 9F, FontStyle.Regular, GraphicsUnit.Point)
        };
        _clearButton.FlatAppearance.BorderSize = 0;
        _clearButton.FlatAppearance.MouseDownBackColor = Color.Transparent;
        _clearButton.FlatAppearance.MouseOverBackColor = Color.Transparent;
        _clearButton.FlatAppearance.CheckedBackColor = Color.Transparent;
        _clearButton.Click += (_, _) => Clear();

        _layout.Controls.Add(_iconPanel, 0, 0);
        _layout.Controls.Add(_edit, 1, 0);
        _layout.Controls.Add(_clearButton, 2, 0);
        BorderHost.Controls.Add(_layout);

        _debounceTimer = new System.Windows.Forms.Timer { Interval = _searchDelayMs };
        _debounceTimer.Tick += OnDebounceTick;

        ThemeManager.ThemeChanged += (_, e) => ApplyTheme(e.Current);
        Disposed += OnSelfDisposed;
        HandleCreated += (_, _) =>
        {
            ApplyTheme(ThemeManager.Current);
            ApplyPlaceholder();
        };
        BorderHost.HandleCreated += (_, _) => ApplyPlaceholder();
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        ThemeManager.ThemeChanged -= OnSelfDisposed;
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
            UpdateClearButtonVisibility();
        }
    }

    [AllowNull]
    public string PlaceholderText
    {
        get => _placeholderText;
        set
        {
            _placeholderText = value ?? string.Empty;
            ApplyPlaceholder();
        }
    }
    private string _placeholderText = string.Empty;

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
        set { _showClearButton = value; UpdateClearButtonVisibility(); }
    }

    public bool EscapeClearsText
    {
        get => _escapeClearsText;
        set => _escapeClearsText = value;
    }

    public bool ReadOnly
    {
        get => _edit.ReadOnly;
        set { _edit.ReadOnly = value; UpdateClearButtonVisibility(); }
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
        _searchText = string.Empty;
        _edit.Clear();
        UpdateClearButtonVisibility();
        RaiseSearchChangedNow();
        Cleared?.Invoke(this, EventArgs.Empty);
    }

    public void SelectAll() => _edit.SelectAll();
    public void FocusEdit() => _edit.Focus();

    protected override bool InnerHasFocus() => _edit.Focused;

    protected override void OnGotFocus(System.EventArgs e)
    {
        base.OnGotFocus(e);
        if (!_edit.Focused) _edit.Focus();
    }

    protected override void OnEnabledChanged(System.EventArgs e)
    {
        base.OnEnabledChanged(e);
        _edit.Enabled = Enabled;
        UpdateClearButtonVisibility();
    }

    private void OnInnerTextChanged(object? sender, EventArgs e)
    {
        _searchText = _edit.Text;
        UpdateClearButtonVisibility();

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
        var args = new SearchChangedEventArgs(_searchText);
        SearchChanged?.Invoke(this, args);
    }

    private void OnInnerKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Escape && _escapeClearsText && _edit.TextLength > 0)
        {
            _suppressSearchChanged = true;
            try { _edit.Text = string.Empty; }
            finally { _suppressSearchChanged = false; }
            _searchText = string.Empty;
            _debounceTimer.Stop();
            _debounceActive = false;
            UpdateClearButtonVisibility();
            RaiseSearchChangedNow();
            Cleared?.Invoke(this, EventArgs.Empty);
            e.SuppressKeyPress = true;
            e.Handled = true;
            return;
        }
        SearchBoxKeyDown?.Invoke(this, e);
    }

    private void UpdateClearButtonVisibility()
    {
        _clearButton.Visible = _showClearButton && !_edit.ReadOnly && _edit.TextLength > 0;
    }

    private void OnIconPaint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        var theme = ThemeManager.Current;
        var tint = Enabled
            ? (Color)theme.Palette[NexaColorRole.TextSecondary].Value
            : (Color)theme.Palette[NexaColorRole.TextDisabled].Value;
        var size = NexaDpi.Scale(14, IsHandleCreated && !DesignMode ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi);
        var bmp = NexaIconProvider.ToBitmap(NexaIconKind.Search, new Size(size, size), tint);
        if (bmp is null) return;
        var x = (_iconPanel.Width - bmp.Width) / 2;
        var y = (_iconPanel.Height - bmp.Height) / 2;
        g.DrawImage(bmp, x, y);
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
        var dpi = IsHandleCreated && !DesignMode ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi;

        _edit.ForeColor = Enabled
            ? (Color)palette[NexaColorRole.TextPrimary].Value
            : (Color)palette[NexaColorRole.TextDisabled].Value;
        _edit.BackColor = Style == NexaTextBoxStyle.Filled
            ? (Color)palette[NexaColorRole.SurfaceVariant].Value
            : (Color)palette[NexaColorRole.Surface].Value;
        _edit.Font = typography.ToFont(NexaTypographyRole.Body, dpi);

        _clearButton.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        _clearButton.BackColor = _edit.BackColor;

        _iconPanel.Invalidate();
        BorderHost.Invalidate();
    }

    public bool IsDebounceActive => _debounceActive;
}

public sealed class SearchChangedEventArgs : System.EventArgs
{
    public SearchChangedEventArgs(string searchText)
    {
        SearchText = searchText;
    }

    public string SearchText { get; }
}