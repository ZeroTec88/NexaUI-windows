using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A single toast notification that can be displayed by <see cref="NexaToastManager"/>.
/// </summary>
public sealed class NexaToast : Form
{
    private readonly Label _messageLabel;
    private readonly Label _iconLabel;
    private readonly Label _titleLabel;
    private readonly NexaButton _closeButton;
    private readonly TableLayoutPanel _mainLayout;
    private readonly System.Windows.Forms.Timer _autoCloseTimer;
    private readonly System.Windows.Forms.Timer _fadeTimer;
    private NexaToastStyle _style = NexaToastStyle.Default;
    private string _title = string.Empty;
    private bool _isClosing = false;
    private float _opacity = 0f;

    /// <summary>Initializes a new instance of the <see cref="NexaToast"/> class.</summary>
    public NexaToast()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        DoubleBuffered = true;
        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        Size = new Size(360, 80);
        Opacity = 0;

        _mainLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 1,
            Padding = new Padding(12, 8, 12, 8)
        };
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 40F));
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32F));

        _iconLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 18F, FontStyle.Regular, GraphicsUnit.Point)
        };

        _titleLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 20,
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point)
        };

        _messageLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.TopLeft,
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point)
        };

        var textPanel = new Panel { Dock = DockStyle.Fill };
        textPanel.Controls.Add(_messageLabel);
        textPanel.Controls.Add(_titleLabel);

        _closeButton = new NexaButton
        {
            Text = string.Empty,
            IconKind = NexaIconKind.Close,
            Style = NexaButtonStyle.Ghost,
            Width = 24,
            Height = 24,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 0, 0, 4),
            TabStop = false
        };
        _closeButton.Click += (_, _) => CloseToast();

        _mainLayout.Controls.Add(_iconLabel, 0, 0);
        _mainLayout.Controls.Add(textPanel, 1, 0);
        _mainLayout.Controls.Add(_closeButton, 2, 0);

        Controls.Add(_mainLayout);

        _autoCloseTimer = new System.Windows.Forms.Timer { Interval = 5000 };
        _autoCloseTimer.Tick += (_, _) => CloseToast();

        _fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
        _fadeTimer.Tick += OnFadeTick;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => RefreshTheme(e.Current);

    private void RefreshTheme(ITheme theme)
    {
        if (IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => RefreshTheme(theme));
            return;
        }

        var palette = theme.Palette;
        var (bg, fg) = ResolveColors(palette, _style);
        BackColor = bg;
        _titleLabel.ForeColor = fg;
        _messageLabel.ForeColor = fg;
        UpdateIcon();
        Invalidate();
    }

    private (Color bg, Color fg) ResolveColors(NexaPalette palette, NexaToastStyle style)
    {
        return style switch
        {
            NexaToastStyle.Success => (
                Color.FromArgb(20, (Color)palette[NexaColorRole.Success].Value),
                (Color)palette[NexaColorRole.Success].Value),
            NexaToastStyle.Warning => (
                Color.FromArgb(20, (Color)palette[NexaColorRole.Warning].Value),
                (Color)palette[NexaColorRole.Warning].Value),
            NexaToastStyle.Error => (
                Color.FromArgb(20, (Color)palette[NexaColorRole.Danger].Value),
                (Color)palette[NexaColorRole.Danger].Value),
            NexaToastStyle.Info => (
                Color.FromArgb(20, (Color)palette[NexaColorRole.Info].Value),
                (Color)palette[NexaColorRole.Info].Value),
            _ => (
                Color.FromArgb(20, (Color)palette[NexaColorRole.Primary].Value),
                (Color)palette[NexaColorRole.Primary].Value)
        };
    }

    private void UpdateIcon()
    {
        var (_, fg) = ResolveColors(ThemeManager.Current.Palette, _style);
        var iconBmp = NexaIconProvider.ToBitmap(_style switch
        {
            NexaToastStyle.Success => NexaIconKind.Check,
            NexaToastStyle.Warning => NexaIconKind.Warning,
            NexaToastStyle.Error => NexaIconKind.Error,
            NexaToastStyle.Info => NexaIconKind.Info,
            _ => NexaIconKind.Info
        }, new Size(24, 24), fg);

        if (iconBmp != null)
        {
            _iconLabel.Image = iconBmp;
        }
    }

    /// <summary>Title of the toast.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Title of the toast.")]
    public string Title
    {
        get => _title;
        set { _title = value ?? string.Empty; _titleLabel.Text = _title; _titleLabel.Visible = !string.IsNullOrEmpty(_title); }
    }

    /// <summary>Message text of the toast.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Message text of the toast.")]
    public string Message
    {
        get => _messageLabel.Text;
        set { _messageLabel.Text = value ?? string.Empty; }
    }

    /// <summary>Visual style of the toast.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaToastStyle.Default)]
    [Description("Visual style of the toast.")]
    public NexaToastStyle Style
    {
        get => _style;
        set { _style = value; RefreshTheme(ThemeManager.Current); }
    }

    /// <summary>Shows the toast at the specified position.</summary>
    /// <param name="owner">Owner window for positioning.</param>
    /// <param name="position">Screen position.</param>
    /// <param name="autoCloseDelayMs">Auto-close delay in milliseconds. 0 = no auto-close.</param>
    public void Show(IWin32Window? owner, NexaToastPosition position = NexaToastPosition.TopRight, int autoCloseDelayMs = 5000)
    {
        PositionToast(owner, position);
        _opacity = 0;
        Opacity = 0;
        base.Show(owner);
        _fadeTimer.Start();

        if (autoCloseDelayMs > 0)
        {
            _autoCloseTimer.Interval = autoCloseDelayMs;
            _autoCloseTimer.Start();
        }
    }

    private void PositionToast(IWin32Window? owner, NexaToastPosition position)
    {
        var screen = owner != null
            ? Screen.FromControl((Control)owner)
            : Screen.PrimaryScreen;

        var wa = screen.WorkingArea;
        var x = position switch
        {
            NexaToastPosition.TopLeft => wa.Left + 16,
            NexaToastPosition.TopCenter => wa.Left + (wa.Width - Width) / 2,
            NexaToastPosition.TopRight => wa.Right - Width - 16,
            NexaToastPosition.BottomLeft => wa.Left + 16,
            NexaToastPosition.BottomCenter => wa.Left + (wa.Width - Width) / 2,
            NexaToastPosition.BottomRight => wa.Right - Width - 16,
            _ => wa.Right - Width - 16
        };

        var y = position switch
        {
            NexaToastPosition.TopLeft or NexaToastPosition.TopCenter or NexaToastPosition.TopRight => wa.Top + 16,
            NexaToastPosition.BottomLeft or NexaToastPosition.BottomCenter or NexaToastPosition.BottomRight => wa.Bottom - Height - 16,
            _ => wa.Top + 16
        };

        Location = new Point(x, y);
    }

    private void OnFadeTick(object? sender, EventArgs e)
    {
        if (_isClosing)
        {
            _opacity -= 0.08f;
            if (_opacity <= 0)
            {
                _fadeTimer.Stop();
                base.Close();
            }
        }
        else
        {
            _opacity += 0.08f;
            if (_opacity >= 1)
            {
                _opacity = 1;
                _fadeTimer.Stop();
            }
        }
        Opacity = _opacity;
    }

    /// <summary>Closes the toast with fade-out animation.</summary>
    public void CloseToast()
    {
        if (_isClosing) return;
        _isClosing = true;
        _autoCloseTimer.Stop();
        _fadeTimer.Start();
    }

    /// <summary>Shows a toast notification.</summary>
    public static NexaToast Show(
        IWin32Window? owner,
        string message,
        string title = "",
        NexaToastStyle style = NexaToastStyle.Default,
        NexaToastPosition position = NexaToastPosition.TopRight,
        int autoCloseDelayMs = 5000)
    {
        var toast = new NexaToast
        {
            Message = message,
            Title = title,
            Style = style
        };
        toast.Show(owner, position, autoCloseDelayMs);
        return toast;
    }
}