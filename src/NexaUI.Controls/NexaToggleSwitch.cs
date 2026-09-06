using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A modern iOS-style toggle switch. WinForms has no native equivalent, so this control
/// uses minimal custom rendering on top of standard <see cref="ButtonBase"/> keyboard
/// behavior (Tab, Space, Enter). It supports optional subtle thumb animation and
/// fully theme-aware colors.
/// </summary>
[DefaultEvent(nameof(CheckedChanged))]
[DefaultProperty(nameof(Checked))]
public class NexaToggleSwitch : UserControl
{
    private const int DefaultAnimationDurationMs = 140;
    private const int DefaultFrameIntervalMs = 16;

    private bool _checked;
    private bool _hovered;
    private bool _focused;
    private bool _animationEnabled = true;
    private string _onText = "ON";
    private string _offText = "OFF";
    private bool _showText;
    private NexaToggleSize _size = NexaToggleSize.Medium;

    private float _thumbOffset;
    private System.Windows.Forms.Timer? _animTimer;
    private long _animStartTicks;
    private int _animDurationMs = DefaultAnimationDurationMs;
    private bool _disposed;

    public NexaToggleSwitch()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.Selectable |
            ControlStyles.StandardClick |
            ControlStyles.SupportsTransparentBackColor,
            true);

        TabStop = true;
        BackColor = Color.Transparent;
        DoubleBuffered = true;
        AutoSize = false;
        base.Size = new Size(44, 22);
        MinimumSize = new Size(36, 18);
        Cursor = Cursors.Hand;

        _thumbOffset = 0F;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += OnSelfDisposed;
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        _disposed = true;
        ThemeManager.ThemeChanged -= OnThemeChanged;
        StopAnimation();
    }

    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the switch is in the ON position.")]
    public bool Checked
    {
        get => _checked;
        set
        {
            if (_checked == value) return;
            _checked = value;
            if (_animationEnabled && IsHandleCreated && !DesignMode)
            {
                StartAnimation();
            }
            else
            {
                _thumbOffset = value ? 1F : 0F;
                Invalidate();
            }
            CheckedChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether thumb motion is animated when Checked changes.")]
    public bool AnimationEnabled
    {
        get => _animationEnabled;
        set
        {
            _animationEnabled = value;
            if (!value)
            {
                StopAnimation();
                _thumbOffset = _checked ? 1F : 0F;
                Invalidate();
            }
        }
    }

    [Category("NexaUI")]
    [DefaultValue("ON")]
    [Description("Text shown inside the track when the switch is ON and ShowText is true.")]
    public string OnText
    {
        get => _onText;
        set { _onText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue("OFF")]
    [Description("Text shown inside the track when the switch is OFF and ShowText is true.")]
    public string OffText
    {
        get => _offText;
        set { _offText = value ?? string.Empty; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Show ON / OFF text inside the track.")]
    public bool ShowText
    {
        get => _showText;
        set { _showText = value; Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(NexaToggleSize.Medium)]
    [Description("Visual size of the switch.")]
    public NexaToggleSize ToggleSize
    {
        get => _size;
        set { _size = value; ApplySize(); Invalidate(); }
    }

    [Category("NexaUI")]
    [DefaultValue(140)]
    [Description("Animation duration in milliseconds. 0 = no animation.")]
    public int AnimationDurationMs
    {
        get => _animDurationMs;
        set => _animDurationMs = Math.Max(0, value);
    }

    [Browsable(true)]
#pragma warning disable CS8765
    public override Font Font
    {
        get => base.Font;
        set { base.Font = value!; Invalidate(); }
    }
#pragma warning restore CS8765

    public event EventHandler? CheckedChanged;

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        _focused = true;
        Invalidate();
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        _focused = false;
        Invalidate();
    }

    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        _hovered = true;
        Invalidate();
    }

    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        _hovered = false;
        Invalidate();
    }

    protected override void OnClick(EventArgs e)
    {
        base.OnClick(e);
        if (!Enabled) return;
        Checked = !Checked;
        Focus();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (!Enabled) return;
        if (e.KeyCode == Keys.Space || e.KeyCode == Keys.Enter)
        {
            Checked = !Checked;
            e.Handled = true;
            e.SuppressKeyPress = true;
        }
    }

    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        Invalidate();
    }

    protected override void OnLoad(System.EventArgs e)
    {
        base.OnLoad(e);
        _thumbOffset = _checked ? 1F : 0F;
        Invalidate();
    }

    private void ApplySize()
    {
        var dpi = CurrentDpi();
        int w, h;
        switch (_size)
        {
            case NexaToggleSize.Small: w = 32; h = 16; break;
            case NexaToggleSize.Large: w = 56; h = 28; break;
            default: w = 44; h = 22; break;
        }
        Size = new Size(NexaDpi.Scale(w, dpi), NexaDpi.Scale(h, dpi));
    }

    private void StartAnimation()
    {
        if (_disposed || IsDisposed || Disposing) return;
        if (_animDurationMs <= 0)
        {
            _thumbOffset = _checked ? 1F : 0F;
            Invalidate();
            return;
        }
        StopAnimation();
        _animTimer = new System.Windows.Forms.Timer
        {
            Interval = DefaultFrameIntervalMs
        };
        _animTimer.Tick += OnAnimTick;
        _animStartTicks = Environment.TickCount64;
        _animTimer.Start();
    }

    private void StopAnimation()
    {
        if (_animTimer is not null)
        {
            _animTimer.Stop();
            _animTimer.Tick -= OnAnimTick;
            _animTimer.Dispose();
            _animTimer = null;
        }
    }

    private void OnAnimTick(object? sender, EventArgs e)
    {
        if (_disposed || IsDisposed || Disposing)
        {
            StopAnimation();
            return;
        }
        var elapsed = Environment.TickCount64 - _animStartTicks;
        float t = Math.Min(1F, (float)elapsed / _animDurationMs);
        _thumbOffset = t;
        Invalidate();
        if (t >= 1F)
        {
            StopAnimation();
            _thumbOffset = _checked ? 1F : 0F;
            Invalidate();
        }
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e)
    {
        if (_disposed || IsDisposed || Disposing) return;
        if (InvokeRequired)
        {
            BeginInvoke(() => { if (!IsDisposed && !Disposing) Invalidate(); });
            return;
        }
        Invalidate();
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi;

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        var w = Width;
        var h = Height;
        var trackRadius = h / 2F;
        var thumbInset = NexaDpi.Scale(2, dpi);
        var thumbSize = h - thumbInset * 2;

        var trackRect = new RectangleF(0, 0, w, h);

        var progress = _thumbOffset;
        Color trackColor;
        if (!Enabled)
        {
            trackColor = (Color)palette[NexaColorRole.SurfaceVariant].Value;
        }
        else if (_checked)
        {
            trackColor = (Color)palette[NexaColorRole.Primary].Value;
        }
        else
        {
            trackColor = _hovered
                ? (Color)palette[NexaColorRole.InputBorder].Value
                : (Color)palette[NexaColorRole.InputBorder].Value;
        }

        using (var trackBrush = new SolidBrush(trackColor))
        {
            g.FillRectangle(trackBrush, trackRect);
        }
        using (var trackPath = CreateRoundedPath(trackRect, trackRadius))
        {
            using var pen = new Pen(Color.FromArgb(20, Color.Black), 1F);
            g.DrawPath(pen, trackPath);
        }

        if (_showText)
        {
            var labelColor = _checked
                ? (Color)palette[NexaColorRole.TextOnAccent].Value
                : (Color)palette[NexaColorRole.TextSecondary].Value;
            var text = _checked ? _onText : _offText;
            using var font = typography.ToFont(NexaTypographyRole.Caption, dpi);
            var textSize = TextRenderer.MeasureText(g, text, font);
            var textX = _checked
                ? NexaDpi.Scale(8, dpi)
                : w - textSize.Width - NexaDpi.Scale(8, dpi);
            var textY = (h - textSize.Height) / 2F;
            TextRenderer.DrawText(g, text, font, new Point((int)textX, (int)Math.Max(0, textY)), labelColor);
        }

        var minThumbX = thumbInset;
        var maxThumbX = w - thumbInset - thumbSize;
        var thumbX = minThumbX + (maxThumbX - minThumbX) * progress;
        var thumbRect = new RectangleF(thumbX, thumbInset, thumbSize, thumbSize);

        var thumbColor = Enabled
            ? (Color)palette[NexaColorRole.Surface].Value
            : (Color)palette[NexaColorRole.InputBackground].Value;
        using (var thumbBrush = new SolidBrush(thumbColor))
        {
            g.FillEllipse(thumbBrush, thumbRect);
        }
        if (!_checked && !Enabled)
        {
            using var dimPen = new Pen(Color.FromArgb(60, (Color)palette[NexaColorRole.TextSecondary].Value), 1F);
            g.DrawEllipse(dimPen, thumbRect);
        }

        if (_focused && Enabled)
        {
            using var focusPen = new Pen((Color)palette[NexaColorRole.Focus].Value, NexaDpi.Scale(1, dpi));
            var focusRect = RectangleF.Inflate(trackRect, NexaDpi.Scale(2, dpi), NexaDpi.Scale(2, dpi));
            using var focusPath = CreateRoundedPath(focusRect, focusRect.Height / 2F + NexaDpi.Scale(2, dpi));
            g.DrawPath(focusPen, focusPath);
        }
    }

    private static GraphicsPath CreateRoundedPath(RectangleF rect, float radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0F || rect.Width <= 0F || rect.Height <= 0F)
        {
            path.AddRectangle(rect);
            return path;
        }
        var d = Math.Min(radius * 2F, Math.Min(rect.Width, rect.Height));
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}
