using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A full-screen or container-scoped loading overlay with spinner and optional message.
/// </summary>
public sealed class NexaLoadingOverlay : UserControl
{
    private readonly Panel _overlayPanel;
    private readonly NexaSpinner _spinner;
    private readonly Label _messageLabel;
    private readonly Label _titleLabel;
    private readonly TableLayoutPanel _layout;
    private NexaOverlayStyle _overlayStyle = NexaOverlayStyle.Standard;
    private string _message = string.Empty;
    private string _title = string.Empty;
    private bool _isVisible = false;

    /// <summary>Initializes a new instance of the <see cref="NexaLoadingOverlay"/> class.</summary>
    public NexaLoadingOverlay()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        Dock = DockStyle.Fill;
        Visible = false;

        _layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        _layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        _layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        _titleLabel = new Label
        {
            Text = string.Empty,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point),
            Margin = new Padding(0, 0, 0, 16)
        };

        _spinner = new NexaSpinner
        {
            SpinnerSize = 48,
            AnimationSpeed = 80,
            Dock = DockStyle.None,
            Anchor = AnchorStyles.None
        };

        _messageLabel = new Label
        {
            Text = string.Empty,
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            Margin = new Padding(0, 16, 0, 0)
        };

        var centerPanel = new Panel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Anchor = AnchorStyles.None
        };
        centerPanel.Controls.Add(_messageLabel);
        centerPanel.Controls.Add(_titleLabel);

        var spinnerPanel = new Panel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Anchor = AnchorStyles.None
        };
        spinnerPanel.Controls.Add(_spinner);

        _layout.Controls.Add(new Panel { Dock = DockStyle.Fill }, 0, 0);
        _layout.Controls.Add(spinnerPanel, 0, 1);
        _layout.Controls.Add(centerPanel, 0, 2);

        _overlayPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Transparent
        };
        _overlayPanel.Controls.Add(_layout);

        Controls.Add(_overlayPanel);

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
        var style = _overlayStyle;

        var (overlayBg, spinnerColor) = style switch
        {
            NexaOverlayStyle.Light => (
                Color.FromArgb(200, (Color)palette[NexaColorRole.Background].Value),
                (Color)palette[NexaColorRole.Primary].Value),
            NexaOverlayStyle.Blur => (
                Color.FromArgb(180, 255, 255, 255),
                (Color)palette[NexaColorRole.Primary].Value),
            NexaOverlayStyle.Minimal => (
                Color.Transparent,
                (Color)palette[NexaColorRole.Primary].Value),
            _ => (
                Color.FromArgb(200, (Color)palette[NexaColorRole.Background].Value),
                (Color)palette[NexaColorRole.Primary].Value)
        };

        _overlayPanel.BackColor = overlayBg;
        _titleLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        _messageLabel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;

        _spinner.SpinnerColor = (Color)palette[NexaColorRole.Primary].Value;
    }

    /// <summary>Message to display below the spinner.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Message to display below the spinner.")]
    public string Message
    {
        get => _message;
        set { _message = value ?? string.Empty; _messageLabel.Text = _message; }
    }

    /// <summary>Optional title above the message.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional title above the message.")]
    public string Title
    {
        get => _title;
        set { _title = value ?? string.Empty; _titleLabel.Text = _title; _titleLabel.Visible = !string.IsNullOrEmpty(_title); }
    }

    /// <summary>Visual style of the overlay.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaOverlayStyle.Standard)]
    [Description("Visual style of the overlay.")]
    public NexaOverlayStyle OverlayStyle
    {
        get => _overlayStyle;
        set { _overlayStyle = value; RefreshTheme(ThemeManager.Current); }
    }

    /// <summary>Shows the loading overlay.</summary>
    public new void Show()
    {
        Visible = true;
        BringToFront();
        _spinner.AnimationEnabled = true;
    }

    /// <summary>Hides the loading overlay.</summary>
    public new void Hide()
    {
        Visible = false;
        _spinner.AnimationEnabled = false;
    }
}