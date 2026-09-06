using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed horizontal progress bar provided by NexaUI. Modern visual style with
/// rounded corners, theme-aware track and fill colors, optional centered percentage
/// label, and full theme integration.
/// </summary>
[DefaultEvent(nameof(ValueChanged))]
[DefaultProperty(nameof(Value))]
public class NexaProgressBar : Control
{
    private int _minimum;
    private int _maximum = 100;
    private int _value;
    private NexaProgressStyle _style = NexaProgressStyle.Default;
    private bool _showPercentage = true;
    private string _percentageFormat = "{0:0}%";
    private int _cornerRadiusDips = 3;
    private int _heightDips = 10;
    private bool _indeterminate;
    private float _indeterminatePhase;
    private System.Windows.Forms.Timer? _indeterminateTimer;

    /// <summary>Initializes a new instance of the <see cref="NexaProgressBar"/> class.</summary>
    public NexaProgressBar()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;
        AccessibleRole = AccessibleRole.ProgressBar;
        Size = new Size(280, 20);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += OnSelfDisposed;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);
    }

    private void OnSelfDisposed(object? sender, EventArgs e)
    {
        StopIndeterminate();
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
        if (!IsHandleCreated) return;
        try
        {
            BackColor = (Color)theme.Palette[NexaColorRole.Background].Value;
            Invalidate();
        }
        catch
        {
            // Ignore painting errors during theme transitions in headless test environments.
        }
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Lower bound of the progress range.</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Lower bound of the progress range.")]
    public int Minimum
    {
        get => _minimum;
        set
        {
            if (value > _maximum) _maximum = value + 1;
            _minimum = value;
            if (_value < _minimum) _value = _minimum;
            Invalidate();
        }
    }

    /// <summary>Upper bound of the progress range.</summary>
    [Category("NexaUI")]
    [DefaultValue(100)]
    [Description("Upper bound of the progress range.")]
    public int Maximum
    {
        get => _maximum;
        set
        {
            if (value < _minimum) _minimum = value - 1;
            _maximum = value;
            if (_value > _maximum) _value = _maximum;
            Invalidate();
        }
    }

    /// <summary>Current progress value.</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Current progress value.")]
    public int Value
    {
        get => _value;
        set
        {
            var clamped = Math.Clamp(value, _minimum, _maximum);
            if (clamped == _value) return;
            _value = clamped;
            ValueChanged?.Invoke(this, EventArgs.Empty);
            Invalidate();
        }
    }

    /// <summary>Visual style of the progress fill.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaProgressStyle.Default)]
    [Description("Visual style of the progress fill.")]
    public NexaProgressStyle ProgressStyle
    {
        get => _style;
        set { _style = value; Invalidate(); }
    }

    /// <summary>Whether to display the percentage label centered on the bar.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to display the percentage label centered on the bar.")]
    public bool ShowPercentage
    {
        get => _showPercentage;
        set { _showPercentage = value; Invalidate(); }
    }

    /// <summary>Format string for the percentage label. Receives a double (0–100).</summary>
    [Category("NexaUI")]
    [DefaultValue("{0:0}%")]
    [Description("Format string for the percentage label. Receives a double (0-100).")]
    public string PercentageFormat
    {
        get => _percentageFormat;
        set { _percentageFormat = value ?? "{0:0}%"; Invalidate(); }
    }

    /// <summary>Corner radius in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(3)]
    [Description("Corner radius in device-independent pixels.")]
    public int CornerRadius
    {
        get => _cornerRadiusDips;
        set { _cornerRadiusDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Track height in DIPs (the bar centers vertically in the control).</summary>
    [Category("NexaUI")]
    [DefaultValue(10)]
    [Description("Track height in device-independent pixels.")]
    public int BarHeight
    {
        get => _heightDips;
        set
        {
            _heightDips = Math.Max(1, value);
            Size = new Size(Width, Math.Max(20, NexaDpi.Scale(_heightDips, CurrentDpi()) + NexaDpi.Scale(10, CurrentDpi())));
            Invalidate();
        }
    }

    /// <summary>Whether the progress bar is in indeterminate (animated) mode.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the progress bar is in indeterminate (animated) mode.")]
    public bool Indeterminate
    {
        get => _indeterminate;
        set
        {
            if (_indeterminate == value) return;
            _indeterminate = value;
            if (value) StartIndeterminate();
            else StopIndeterminate();
            Invalidate();
        }
    }

    private void StartIndeterminate()
    {
        if (_indeterminateTimer is null)
        {
            _indeterminateTimer = new System.Windows.Forms.Timer { Interval = 30 };
            _indeterminateTimer.Tick += (_, _) =>
            {
                if (IsDisposed || Disposing) { StopIndeterminate(); return; }
                _indeterminatePhase = (_indeterminatePhase + 0.04f) % 1f;
                Invalidate();
            };
        }
        if (!IsHandleCreated) return;
        _indeterminateTimer.Start();
    }

    private void StopIndeterminate()
    {
        if (_indeterminateTimer is null) return;
        _indeterminateTimer.Stop();
    }

    /// <summary>Raised when <see cref="Value"/> changes.</summary>
    [Category("NexaUI")]
    [Description("Raised when the progress Value changes.")]
    public event EventHandler? ValueChanged;

    /// <summary>Returns the current progress as a percentage (0–100).</summary>
    [Browsable(false)]
    public double Percentage
    {
        get
        {
            if (_maximum == _minimum) return 0;
            return (_value - _minimum) * 100.0 / (_maximum - _minimum);
        }
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var dpi = CurrentDpi();
        var palette = ThemeManager.Current.Palette;
        var typography = ThemeManager.Current.Typography;
        var w = Width;
        var h = Height;
        if (w <= 0 || h <= 0) return;

        var barHeight = NexaDpi.Scale(_heightDips, dpi);
        var barRect = new Rectangle(0, (h - barHeight) / 2, w, barHeight);
        if (barRect.Width <= 0 || barRect.Height <= 0) return;

        var radius = Math.Min(NexaDpi.Scale(_cornerRadiusDips, dpi), barHeight / 2);

        // Track
        using (var trackPath = CreateRoundedPath(barRect, radius))
        {
            using var trackBrush = new SolidBrush((Color)palette[NexaColorRole.SurfaceVariant].Value);
            g.FillPath(trackBrush, trackPath);
        }

        // Fill
        var fillColor = ResolveProgressColor(palette);
        if (_indeterminate)
        {
            // Animated sliding block.
            var blockWidth = Math.Max(40, barRect.Width / 4);
            var travel = barRect.Width + blockWidth;
            var x = (int)((travel * _indeterminatePhase) - blockWidth);
            var fillRect = new Rectangle(x, barRect.Y, blockWidth, barRect.Height);
            using (var fillPath = CreateRoundedPath(fillRect, radius))
            {
                using var fillBrush = new SolidBrush(fillColor);
                g.FillPath(fillBrush, fillPath);
            }
        }
        else
        {
            var percent = Percentage / 100.0;
            var fillW = (int)Math.Round(barRect.Width * percent);
            if (fillW > 0)
            {
                var fillRect = new Rectangle(barRect.X, barRect.Y, fillW, barRect.Height);
                using var fillPath = CreateRoundedPath(fillRect, radius);
                using var fillBrush = new SolidBrush(fillColor);
                g.FillPath(fillBrush, fillPath);
            }
        }

        // Optional percentage label
        if (_showPercentage && !_indeterminate)
        {
            var text = string.Format(CultureInfo.CurrentCulture, _percentageFormat, Percentage);
            using var font = typography.ToFont(NexaTypographyRole.Caption, dpi);
            var textColor = Enabled
                ? (Color)palette[NexaColorRole.TextPrimary].Value
                : (Color)palette[NexaColorRole.TextDisabled].Value;
            var textRect = new Rectangle(barRect.X, barRect.Y, barRect.Width, barRect.Height);
            TextRenderer.DrawText(g, text, font, textRect, textColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
        }
    }

    private Color ResolveProgressColor(NexaPalette palette) => _style switch
    {
        NexaProgressStyle.Success => (Color)palette[NexaColorRole.Success].Value,
        NexaProgressStyle.Warning => (Color)palette[NexaColorRole.Warning].Value,
        NexaProgressStyle.Danger => (Color)palette[NexaColorRole.Danger].Value,
        NexaProgressStyle.Info => (Color)palette[NexaColorRole.Info].Value,
        _ => (Color)palette[NexaColorRole.Primary].Value
    };

    internal static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
    {
        var path = new GraphicsPath();
        if (radius <= 0 || rect.Width <= 0 || rect.Height <= 0)
        {
            path.AddRectangle(rect);
            return path;
        }
        var d = Math.Min(radius * 2, Math.Min(rect.Width, rect.Height));
        path.AddArc(rect.X, rect.Y, d, d, 180, 90);
        path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
        path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
        path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);
}
