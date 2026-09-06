using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed circular progress indicator provided by NexaUI. Draws a ring with a
/// theme-colored arc representing progress, an optional centered label, and
/// supports an animated indeterminate mode.
/// </summary>
[DefaultEvent(nameof(ValueChanged))]
[DefaultProperty(nameof(Value))]
public class NexaCircularProgress : Control
{
    private int _minimum;
    private int _maximum = 100;
    private int _value;
    private int _lineThicknessDips = 6;
    private NexaProgressStyle _style = NexaProgressStyle.Default;
    private bool _showPercentage = true;
    private string _percentageFormat = "{0:0}%";
    private string _centerText = string.Empty;
    private bool _indeterminate;
    private float _indeterminatePhase;
    private System.Windows.Forms.Timer? _indeterminateTimer;

    /// <summary>Initializes a new instance of the <see cref="NexaCircularProgress"/> class.</summary>
    public NexaCircularProgress()
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
        Size = new Size(96, 96);

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
        BackColor = (Color)theme.Palette[NexaColorRole.Background].Value;
        Invalidate();
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

    /// <summary>Line thickness in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(6)]
    [Description("Line thickness in device-independent pixels.")]
    public int LineThickness
    {
        get => _lineThicknessDips;
        set { _lineThicknessDips = Math.Max(1, value); Invalidate(); }
    }

    /// <summary>Visual style of the progress arc.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaProgressStyle.Default)]
    [Description("Visual style of the progress arc.")]
    public NexaProgressStyle ProgressStyle
    {
        get => _style;
        set { _style = value; Invalidate(); }
    }

    /// <summary>Whether to display the percentage label centered in the ring.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to display the percentage label centered in the ring.")]
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

    /// <summary>Optional center text. When set, it replaces the percentage label.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional center text. When set, it replaces the percentage label.")]
    public string CenterText
    {
        get => _centerText;
        set { _centerText = value ?? string.Empty; Invalidate(); }
    }

    /// <summary>Whether the indicator is in indeterminate (spinning) mode.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the indicator is in indeterminate (spinning) mode.")]
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

    /// <summary>Current progress as a percentage (0–100).</summary>
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

        var lineThickness = NexaDpi.Scale(_lineThicknessDips, dpi);
        var pad = lineThickness;
        var size = Math.Min(w, h) - pad * 2;
        if (size <= 0) return;
        var rect = new Rectangle((w - size) / 2, (h - size) / 2, size, size);
        var trackColor = (Color)palette[NexaColorRole.SurfaceVariant].Value;
        var progressColor = ResolveProgressColor(palette);

        // Track ring
        using (var trackPen = new Pen(trackColor, lineThickness))
        {
            trackPen.StartCap = LineCap.Round;
            trackPen.EndCap = LineCap.Round;
            g.DrawEllipse(trackPen, rect);
        }

        // Progress arc
        if (_indeterminate)
        {
            var sweep = 90f;
            var start = _indeterminatePhase * 360f;
            using var pen = new Pen(progressColor, lineThickness);
            pen.StartCap = LineCap.Round;
            pen.EndCap = LineCap.Round;
            g.DrawArc(pen, rect, start, sweep);
        }
        else
        {
            var sweep = (float)(Percentage * 3.6);
            if (sweep > 0)
            {
                using var pen = new Pen(progressColor, lineThickness);
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;
                g.DrawArc(pen, rect, -90f, sweep);
            }
        }

        // Center text
        var display = !string.IsNullOrEmpty(_centerText)
            ? _centerText
            : (_showPercentage && !_indeterminate
                ? string.Format(CultureInfo.CurrentCulture, _percentageFormat, Percentage)
                : string.Empty);
        if (!string.IsNullOrEmpty(display))
        {
            using var font = typography.ToFont(NexaTypographyRole.Title, dpi);
            var textColor = Enabled
                ? (Color)palette[NexaColorRole.TextPrimary].Value
                : (Color)palette[NexaColorRole.TextDisabled].Value;
            var textRect = new Rectangle(0, 0, w, h);
            TextRenderer.DrawText(g, display, font, textRect, textColor,
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

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);
}
