using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed animated loading spinner provided by NexaUI. Draws a rotating arc (Ring)
/// or a sequence of pulsing dots (Dots) using a single control-owned timer that is
/// stopped on dispose and when <see cref="AnimationEnabled"/> is false.
/// </summary>
[DefaultEvent(nameof(AnimationEnabledChanged))]
[DefaultProperty(nameof(SpinnerStyle))]
public class NexaSpinner : Control
{
    private NexaSpinnerStyle _style = NexaSpinnerStyle.Ring;
    private int _sizeDips = 32;
    private int _speed = 80; // ms per frame
    private bool _animationEnabled = true;
    private float _phase;
    private System.Windows.Forms.Timer? _timer;
    private Color _spinnerColor = Color.Empty;

    /// <summary>Initializes a new instance of the <see cref="NexaSpinner"/> class.</summary>
    public NexaSpinner()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        AccessibleRole = AccessibleRole.Animation;
        AccessibleName = "Loading";
        AccessibleDescription = "Indicates that an operation is in progress";
        Size = new Size(40, 40);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += OnSelfDisposed;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        StopTimer();
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
        BackColor = (Color)theme.Palette[NexaColorRole.Background].Value;
        Invalidate();
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Spinner animation style.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaSpinnerStyle.Ring)]
    [Description("Spinner animation style.")]
    public NexaSpinnerStyle SpinnerStyle
    {
        get => _style;
        set { _style = value; Invalidate(); }
    }

    /// <summary>Spinner diameter in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(32)]
    [Description("Spinner diameter in device-independent pixels.")]
    public int SpinnerSize
    {
        get => _sizeDips;
        set
        {
            _sizeDips = Math.Max(8, value);
            var s = NexaDpi.Scale(_sizeDips, CurrentDpi());
            Size = new Size(s, s);
            Invalidate();
        }
    }

    /// <summary>Animation interval in milliseconds. Lower = faster.</summary>
    [Category("NexaUI")]
    [DefaultValue(80)]
    [Description("Animation interval in milliseconds. Lower values are faster.")]
    public int AnimationSpeed
    {
        get => _speed;
        set
        {
            _speed = Math.Clamp(value, 10, 1000);
            if (_timer is not null) _timer.Interval = _speed;
        }
    }

    /// <summary>Whether the spinner is currently animating.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the spinner is currently animating.")]
    public bool AnimationEnabled
    {
        get => _animationEnabled;
        set
        {
            if (_animationEnabled == value) return;
            _animationEnabled = value;
            if (value) StartTimer();
            else StopTimer();
            AnimationEnabledChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    /// <summary>Optional spinner color. Defaults to the theme Primary role.</summary>
    [Category("NexaUI")]
    [Description("Spinner color. Defaults to the theme Primary role when left empty.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WinForms", "WFO1000", Justification = "Themed property; theme drives the value by default.")]
    public Color SpinnerColor
    {
        get => _spinnerColor;
        set { _spinnerColor = value; Invalidate(); }
    }

    /// <summary>Raised when <see cref="AnimationEnabled"/> changes.</summary>
    [Category("NexaUI")]
    [Description("Raised when AnimationEnabled changes.")]
    public event EventHandler? AnimationEnabledChanged;

    private void StartTimer()
    {
        if (_timer is null)
        {
            _timer = new System.Windows.Forms.Timer { Interval = _speed };
            _timer.Tick += OnTimerTick;
        }
        if (!IsHandleCreated) return;
        _timer.Start();
    }

    private void StopTimer()
    {
        if (_timer is null) return;
        _timer.Stop();
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (IsDisposed || Disposing) { StopTimer(); return; }
        _phase = (_phase + 0.1f) % 1f;
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        if (Visible && _animationEnabled && IsHandleCreated) StartTimer();
        else if (!Visible) StopTimer();
    }

    /// <inheritdoc />
    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        if (Enabled && _animationEnabled && Visible && IsHandleCreated) StartTimer();
        else if (!Enabled) StopTimer();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        if (_animationEnabled && Visible && Enabled) StartTimer();
    }

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

        var color = _spinnerColor.IsEmpty
            ? (Color)palette[NexaColorRole.Primary].Value
            : _spinnerColor;
        if (!Enabled) color = (Color)palette[NexaColorRole.TextDisabled].Value;

        if (_style == NexaSpinnerStyle.Ring)
        {
            DrawRing(g, w, h, color);
        }
        else
        {
            DrawDots(g, w, h, color, dpi);
        }
    }

    private void DrawRing(Graphics g, int w, int h, Color color)
    {
        var thickness = Math.Max(2f, w / 10f);
        var pad = thickness;
        var size = Math.Min(w, h) - (int)(thickness * 2);
        if (size <= 0) return;
        var rect = new Rectangle((w - size) / 2, (h - size) / 2, size, size);
        var startAngle = _phase * 360f - 90f;
        using var pen = new Pen(color, thickness);
        pen.StartCap = LineCap.Round;
        pen.EndCap = LineCap.Round;
        g.DrawArc(pen, rect, startAngle, 270f);
    }

    private void DrawDots(Graphics g, int w, int h, Color color, float dpi)
    {
        var count = 8;
        var radius = (Math.Min(w, h) - NexaDpi.Scale(8, dpi)) / 2f;
        if (radius <= 0) return;
        var dotSize = Math.Max(2f, w / 12f);
        var centerX = w / 2f;
        var centerY = h / 2f;
        var palette = ThemeManager.Current.Palette;
        var dimmed = Color.FromArgb(80, color);
        for (var i = 0; i < count; i++)
        {
            var angle = (i / (float)count) * 2f * (float)Math.PI - (float)Math.PI / 2f;
            var x = centerX + radius * (float)Math.Cos(angle) - dotSize / 2f;
            var y = centerY + radius * (float)Math.Sin(angle) - dotSize / 2f;
            var dist = (count - i + (_phase * count)) % count / count;
            var c = LerpColor(dimmed, color, dist);
            using var brush = new SolidBrush(c);
            g.FillEllipse(brush, x, y, dotSize, dotSize);
        }
    }

    private static Color LerpColor(Color a, Color b, float t)
    {
        t = Math.Clamp(t, 0f, 1f);
        return Color.FromArgb(
            (int)(a.A + (b.A - a.A) * t),
            (int)(a.R + (b.R - a.R) * t),
            (int)(a.G + (b.G - a.G) * t),
            (int)(a.B + (b.B - a.B) * t));
    }

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);
}
