using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed inline notification panel provided by NexaUI. Composes a themed
/// background, an optional icon, an optional title, a message, and an optional
/// close button. Raises <see cref="Closed"/> when dismissed.
/// </summary>
[DefaultEvent(nameof(Closed))]
[DefaultProperty(nameof(Message))]
public class NexaAlert : UserControl
{
    private readonly Label _titleLabel;
    private readonly Label _messageLabel;
    private readonly Label _iconLabel;
    private readonly Button _closeButton;

    private string _message = string.Empty;
    private string _title = string.Empty;
    private NexaAlertStyle _style = NexaAlertStyle.Information;
    private bool _showIcon = true;
    private bool _closable = true;
    private bool _autoClose;
    private int _autoCloseDelayMs = 3000;
    private System.Windows.Forms.Timer? _autoCloseTimer;

    /// <summary>Initializes a new instance of the <see cref="NexaAlert"/> class.</summary>
    public NexaAlert()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        AccessibleRole = AccessibleRole.Alert;
        Padding = new Padding(12, 10, 12, 10);
        MinimumSize = new Size(200, 56);

        SuspendLayout();
        _iconLabel = new Label
        {
            AutoSize = false,
            BackColor = Color.Transparent,
            TextAlign = ContentAlignment.MiddleLeft,
            Dock = DockStyle.Left,
            Width = 28
        };
        _titleLabel = new Label
        {
            AutoSize = true,
            BackColor = Color.Transparent,
            Dock = DockStyle.Top,
            Font = new Font(SystemFonts.MessageBoxFont ?? SystemFonts.DefaultFont, FontStyle.Bold)
        };
        _messageLabel = new Label
        {
            AutoSize = true,
            BackColor = Color.Transparent,
            Dock = DockStyle.Top
        };
        _closeButton = new Button
        {
            Text = "×",
            FlatStyle = FlatStyle.Flat,
            Width = 24,
            Height = 24,
            Cursor = Cursors.Hand,
            TabStop = false
        };
        _closeButton.FlatAppearance.BorderSize = 0;
        _closeButton.Click += (_, _) => Close();
        _closeButton.AccessibleName = "Close alert";

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        content.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        content.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var textPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink
        };
        textPanel.Controls.Add(_titleLabel);
        textPanel.Controls.Add(_messageLabel);

        content.Controls.Add(_iconLabel, 0, 0);
        content.Controls.Add(textPanel, 1, 0);
        content.Controls.Add(_closeButton, 2, 0);

        Controls.Add(content);
        ResumeLayout(performLayout: false);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += OnSelfDisposed;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        StopAutoClose();
        ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => ApplyTheme(e.Current);

    private void ApplyTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => ApplyTheme(theme));
            return;
        }
        var palette = theme.Palette;
        var (bg, accent) = ResolveStyleColors(palette);
        BackColor = bg;
        _titleLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        _messageLabel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        _iconLabel.ForeColor = accent;
        _closeButton.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        _closeButton.BackColor = Color.Transparent;
        Invalidate();
    }

    private (Color bg, Color accent) ResolveStyleColors(NexaPalette palette) => _style switch
    {
        NexaAlertStyle.Success => (
            Color.FromArgb(20, (Color)palette[NexaColorRole.Success].Value),
            (Color)palette[NexaColorRole.Success].Value),
        NexaAlertStyle.Warning => (
            Color.FromArgb(20, (Color)palette[NexaColorRole.Warning].Value),
            (Color)palette[NexaColorRole.Warning].Value),
        NexaAlertStyle.Error => (
            Color.FromArgb(20, (Color)palette[NexaColorRole.Danger].Value),
            (Color)palette[NexaColorRole.Danger].Value),
        _ => (
            Color.FromArgb(20, (Color)palette[NexaColorRole.Info].Value),
            (Color)palette[NexaColorRole.Info].Value)
    };

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>The alert message (primary text).</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("The alert message (primary text).")]
    public string Message
    {
        get => _message;
        set { _message = value ?? string.Empty; _messageLabel.Text = _message; UpdateLayout(); }
    }

    /// <summary>Optional alert title (shown above the message when set).</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional alert title (shown above the message when set).")]
    public string Title
    {
        get => _title;
        set { _title = value ?? string.Empty; _titleLabel.Text = _title; _titleLabel.Visible = !string.IsNullOrEmpty(_title); UpdateLayout(); }
    }

    /// <summary>Visual style of the alert.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaAlertStyle.Information)]
    [Description("Visual style of the alert.")]
    public NexaAlertStyle AlertStyle
    {
        get => _style;
        set { _style = value; UpdateIcon(); ApplyTheme(ThemeManager.Current); }
    }

    /// <summary>Whether to show a leading icon for the alert style.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to show a leading icon for the alert style.")]
    public bool ShowIcon
    {
        get => _showIcon;
        set { _showIcon = value; _iconLabel.Visible = value && _showIcon; }
    }

    /// <summary>Whether the alert can be closed by the user.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the alert can be closed by the user.")]
    public bool Closable
    {
        get => _closable;
        set { _closable = value; _closeButton.Visible = value; }
    }

    /// <summary>Whether the alert automatically closes after <see cref="AutoCloseDelayMs"/>.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the alert automatically closes after AutoCloseDelayMs.")]
    public bool AutoClose
    {
        get => _autoClose;
        set
        {
            _autoClose = value;
            if (value) StartAutoClose();
            else StopAutoClose();
        }
    }

    /// <summary>Delay in milliseconds before auto-close.</summary>
    [Category("NexaUI")]
    [DefaultValue(3000)]
    [Description("Delay in milliseconds before auto-close.")]
    public int AutoCloseDelayMs
    {
        get => _autoCloseDelayMs;
        set { _autoCloseDelayMs = Math.Max(100, value); if (_autoClose) StartAutoClose(); }
    }

    private void StartAutoClose()
    {
        StopAutoClose();
        if (!IsHandleCreated) return;
        _autoCloseTimer = new System.Windows.Forms.Timer { Interval = _autoCloseDelayMs };
        _autoCloseTimer.Tick += (_, _) => { StopAutoClose(); Close(); };
        _autoCloseTimer.Start();
    }

    private void StopAutoClose()
    {
        if (_autoCloseTimer is null) return;
        _autoCloseTimer.Stop();
        _autoCloseTimer.Dispose();
        _autoCloseTimer = null;
    }

    private void UpdateIcon()
    {
        if (!Visible) return;
        _iconLabel.Text = _showIcon ? IconGlyph() : string.Empty;
        _iconLabel.Visible = _showIcon;
    }

    private string IconGlyph() => _style switch
    {
        NexaAlertStyle.Success => "\uE73E", // check
        NexaAlertStyle.Warning => "!",
        NexaAlertStyle.Error => "\uE783",   // x
        _ => "i"
    };

    private void UpdateLayout()
    {
        if (IsHandleCreated) PerformLayout();
    }

    /// <summary>Closes the alert (hides it and raises <see cref="Closed"/>).</summary>
    public void Close()
    {
        if (IsDisposed || Disposing) return;
        Visible = false;
        Closed?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>Raised when the alert is closed by the user or auto-closed.</summary>
    [Category("NexaUI")]
    [Description("Raised when the alert is closed by the user or auto-closed.")]
    public event EventHandler? Closed;

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var dpi = CurrentDpi();
        var palette = ThemeManager.Current.Palette;
        var w = Width;
        var h = Height;
        if (w <= 0 || h <= 0) return;

        var radius = NexaDpi.Scale(4, dpi);
        var rect = new Rectangle(0, 0, w, h);

        using (var path = NexaPanel.CreateRoundedPathPublic(rect, radius))
        {
            using var brush = new SolidBrush(BackColor);
            g.FillPath(brush, path);
        }

        var (bg, accent) = ResolveStyleColors(palette);
        using (var pen = new Pen(Color.FromArgb(120, accent), NexaDpi.Scale(1, dpi)))
        {
            using var path = NexaPanel.CreateRoundedPathPublic(rect, radius);
            g.DrawPath(pen, path);
        }

        if (_showIcon) _iconLabel.Text = IconGlyph();
    }

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);
}
