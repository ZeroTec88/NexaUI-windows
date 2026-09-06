using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed tooltip that replaces the standard <see cref="ToolTip"/> with
/// custom styling, animations, and positioning options.
/// </summary>
public sealed class NexaToolTip : Component
{
    private readonly Form _tooltipForm;
    private readonly Label _textLabel;
    private readonly System.Windows.Forms.Timer _showTimer;
    private readonly System.Windows.Forms.Timer _hideTimer;
    private readonly System.Windows.Forms.Timer _fadeTimer;
    private Control? _ownerControl;
    private string _toolTipText = string.Empty;
    private int _initialDelay = 500;
    private int _autoPopDelay = 5000;
    private int _fadeDuration = 150;
    private NexaTooltipPosition _position = NexaTooltipPosition.Top;
    private float _currentOpacity = 0f;
    private bool _isShowing = false;
    private bool _isFading = false;
    private bool _disposed = false;

    /// <summary>Initializes a new instance of the <see cref="NexaToolTip"/> class.</summary>
    public NexaToolTip()
    {
        _tooltipForm = new Form
        {
            FormBorderStyle = FormBorderStyle.None,
            ShowInTaskbar = false,
            StartPosition = FormStartPosition.Manual,
            TopMost = true,
            BackColor = Color.Transparent,
            Opacity = 0,
            Size = new Size(200, 40)
        };

        _textLabel = new Label
        {
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleCenter,
            Padding = new Padding(12, 8, 12, 8),
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = false
        };
        _tooltipForm.Controls.Add(_textLabel);

        _showTimer = new System.Windows.Forms.Timer { Interval = _initialDelay };
        _showTimer.Tick += (_, _) => ShowTooltip();

        _hideTimer = new System.Windows.Forms.Timer { Interval = _autoPopDelay };
        _hideTimer.Tick += (_, _) => StartFadeOut();

        _fadeTimer = new System.Windows.Forms.Timer { Interval = 16 };
        _fadeTimer.Tick += OnFadeTick;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) =>
        {
            ThemeManager.ThemeChanged -= OnThemeChanged;
            _tooltipForm.Dispose();
        };
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => RefreshTheme(e.Current);

    private void RefreshTheme(ITheme theme)
    {
        if (_disposed) return;
        if (_tooltipForm.InvokeRequired)
        {
            _tooltipForm.BeginInvoke(() => RefreshTheme(theme));
            return;
        }

        var palette = theme.Palette;
        var bg = (Color)palette[NexaColorRole.TextPrimary].Value;
        var fg = (Color)palette[NexaColorRole.Background].Value;

        _tooltipForm.BackColor = bg;
        _tooltipForm.ForeColor = fg;
    }

    /// <summary>Text to display in the tooltip.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Text to display in the tooltip.")]
    public string ToolTipText
    {
        get => _toolTipText;
        set { _toolTipText = value ?? string.Empty; }
    }

    /// <summary>Delay before showing the tooltip (ms).</summary>
    [Category("NexaUI")]
    [DefaultValue(500)]
    [Description("Delay before showing the tooltip in milliseconds.")]
    public int InitialDelay
    {
        get => _initialDelay;
        set { _initialDelay = Math.Max(0, value); _showTimer.Interval = value; }
    }

    /// <summary>Time before auto-hiding the tooltip (ms).</summary>
    [Category("NexaUI")]
    [DefaultValue(5000)]
    [Description("Time before auto-hiding the tooltip in milliseconds.")]
    public int AutoPopDelay
    {
        get => _autoPopDelay;
        set { _autoPopDelay = Math.Max(0, value); _hideTimer.Interval = value; }
    }

    /// <summary>Fade animation duration in milliseconds.</summary>
    [Category("NexaUI")]
    [DefaultValue(150)]
    [Description("Fade animation duration in milliseconds.")]
    public int FadeDuration
    {
        get => _fadeDuration;
        set { _fadeDuration = Math.Max(10, value); }
    }

    /// <summary>Preferred position relative to the owner control.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaTooltipPosition.Top)]
    [Description("Preferred position relative to the owner control.")]
    public NexaTooltipPosition Position
    {
        get => _position;
        set { _position = value; }
    }

    /// <summary>Sets the tooltip text for a control.</summary>
    public void SetToolTip(Control control, string text)
    {
        _toolTipText = text;
        _ownerControl = control;

        control.MouseEnter += OnMouseEnter;
        control.MouseLeave += OnMouseLeave;
        control.MouseMove += OnMouseMove;
    }

    /// <summary>Gets the tooltip text for a control.</summary>
    public string GetToolTip(Control control)
    {
        return _toolTipText;
    }

    /// <summary>Removes the tooltip from a control.</summary>
    public void RemoveToolTip(Control control)
    {
        control.MouseEnter -= OnMouseEnter;
        control.MouseLeave -= OnMouseLeave;
        control.MouseMove -= OnMouseMove;

        if (_ownerControl == control)
        {
            HideTooltip();
        }
    }

    private void OnMouseEnter(object? sender, EventArgs e)
    {
        if (sender is Control c)
        {
            _ownerControl = c;
            _showTimer.Stop();
            _showTimer.Start();
        }
    }

    private void OnMouseLeave(object? sender, EventArgs e)
    {
        _showTimer.Stop();
        HideTooltip();
    }

    private void OnMouseMove(object? sender, MouseEventArgs e)
    {
        if (sender is Control c && _isShowing)
        {
            PositionTooltip(c);
        }
    }

    private void ShowTooltip()
    {
        if (string.IsNullOrEmpty(_toolTipText) || _ownerControl == null) return;

        _textLabel.Text = _toolTipText;
        _textLabel.ForeColor = ThemeManager.Current.Palette[NexaColorRole.Background].Value;
        _tooltipForm.BackColor = ThemeManager.Current.Palette[NexaColorRole.TextPrimary].Value;

        // Measure text to size the tooltip
        using var g = _tooltipForm.CreateGraphics();
        var size = TextRenderer.MeasureText(_toolTipText, _textLabel.Font);
        var padding = 24;
        _tooltipForm.Size = new Size(
            Math.Min(size.Width + padding, 400),
            Math.Max(size.Height + 16, 36));

        PositionTooltip(_ownerControl!);
        _tooltipForm.TopMost = true;
        _tooltipForm.Opacity = 0;
        _tooltipForm.Show();
        _isShowing = true;

        // Start fade-in
        _currentOpacity = 0;
        _fadeTimer.Start();
    }

    private void PositionTooltip(Control control)
    {
        if (_tooltipForm.IsDisposed || _disposed) return;

        var screen = control.PointToScreen(Point.Empty);
        var dpi = 1f; // Simplified for now

        int x, y;
        var offset = 8;
        var tooltipWidth = _tooltipForm.Width;
        var tooltipHeight = _tooltipForm.Height;

        switch (_position)
        {
            case NexaTooltipPosition.Top:
                x = control.Left + (control.Width - _tooltipForm.Width) / 2;
                y = control.Top - _tooltipForm.Height - 8;
                break;
            case NexaTooltipPosition.Bottom:
                x = control.Left + (control.Width - _tooltipForm.Width) / 2;
                y = control.Bottom + 8;
                break;
            case NexaTooltipPosition.Left:
                x = control.Left - _tooltipForm.Width - 8;
                y = control.Top + (control.Height - _tooltipForm.Height) / 2;
                break;
            case NexaTooltipPosition.Right:
                x = control.Right + 8;
                y = control.Top + (control.Height - _tooltipForm.Height) / 2;
                break;
            default:
                x = control.Left + (control.Width - _tooltipForm.Width) / 2;
                y = control.Top - _tooltipForm.Height - 8;
                break;
        }

        // Convert to screen coordinates
        var screenPos = control.Parent?.PointToScreen(new Point(x, y)) ?? control.PointToScreen(new Point(x, y));
        _tooltipForm.Location = screenPos;
    }

    private void HideTooltip()
    {
        if (!_isShowing) return;
        _hideTimer.Stop();
        StartFadeOut();
    }

    private void StartFadeOut()
    {
        _hideTimer.Stop();
        _fadeTimer.Start();
    }

    private void OnFadeTick(object? sender, EventArgs e)
    {
        if (_tooltipForm.Opacity <= 0)
        {
            _tooltipForm.Hide();
            _isShowing = false;
            return;
        }

        if (_tooltipForm.Opacity > 0)
        {
            _tooltipForm.Opacity = Math.Max(0, _tooltipForm.Opacity - 0.1);
        }
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        _disposed = true;
        if (disposing)
        {
            _tooltipForm?.Dispose();
        }
        base.Dispose(disposing);
    }
}