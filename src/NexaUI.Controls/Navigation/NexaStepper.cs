using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A stepper control for multi-step workflows and wizards.
/// Supports horizontal and vertical orientations, clickable steps, and various step states.
/// </summary>
[DefaultEvent(nameof(CurrentStepChanged))]
[DefaultProperty(nameof(Steps))]
[Designer("System.Windows.Forms.Design.ControlDesigner, System.Design")]
public class NexaStepper : Control
{
    private readonly List<NexaStep> _steps = new();
    private int _currentStepIndex = 0;
    private NexaOrientation _orientation = NexaOrientation.Horizontal;
    private bool _showDescriptions = true;
    private bool _showStepNumbers = true;
    private bool _allowNavigation = true;
    private int _circleDiameterDips = 36;
    private int _connectorThicknessDips = 2;
    private int _itemSpacingDips = 40;

    /// <summary>Initializes a new instance of the <see cref="NexaStepper"/> class.</summary>
    public NexaStepper()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);

        DoubleBuffered = true;
        TabStop = true;
        BackColor = Color.Transparent;

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Collection of steps.</summary>
    [Category("NexaUI")]
    [Description("Collection of steps.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Collection<NexaStep> Steps => new(_steps);

    /// <summary>Orientation of the stepper.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaOrientation.Horizontal)]
    [Description("Orientation of the stepper.")]
    public NexaOrientation Orientation
    {
        get => _orientation;
        set { _orientation = value; Invalidate(); }
    }

    /// <summary>Index of the currently active step.</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Index of the currently active step.")]
    public int CurrentStepIndex
    {
        get => _currentStepIndex;
        set
        {
            if (value >= 0 && value < _steps.Count)
            {
                _currentStepIndex = value;
                Invalidate();
                CurrentStepChanged?.Invoke(this, new NexaStepChangedEventArgs(_steps[value]));
            }
        }
    }

    /// <summary>Whether to show step descriptions.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to show step descriptions.")]
    public bool ShowDescriptions
    {
        get => _showDescriptions;
        set { _showDescriptions = value; Invalidate(); }
    }

    /// <summary>Whether to show step numbers in the circles.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to show step numbers in the circles.")]
    public bool ShowStepNumbers
    {
        get => _showStepNumbers;
        set { _showStepNumbers = value; Invalidate(); }
    }

    /// <summary>Whether steps can be clicked to navigate.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether steps can be clicked to navigate.")]
    public bool AllowNavigation
    {
        get => _allowNavigation;
        set { _allowNavigation = value; Invalidate(); }
    }

    /// <summary>Diameter of the step circles in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(36)]
    [Description("Diameter of the step circles in device-independent pixels.")]
    public int CircleDiameter
    {
        get => _circleDiameterDips;
        set { _circleDiameterDips = Math.Max(20, value); Invalidate(); }
    }

    /// <summary>Thickness of connector lines in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(2)]
    [Description("Thickness of connector lines in device-independent pixels.")]
    public int ConnectorThickness
    {
        get => _connectorThicknessDips;
        set { _connectorThicknessDips = Math.Max(1, value); Invalidate(); }
    }

    /// <summary>Spacing between step items in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(40)]
    [Description("Spacing between step items in device-independent pixels.")]
    public int ItemSpacing
    {
        get => _itemSpacingDips;
        set { _itemSpacingDips = Math.Max(10, value); Invalidate(); }
    }

    /// <summary>Moves to the next step if possible.</summary>
    public void Next()
    {
        if (_currentStepIndex < _steps.Count - 1)
        {
            CurrentStepIndex++;
        }
    }

    /// <summary>Moves to the previous step if possible.</summary>
    public void Previous()
    {
        if (_currentStepIndex > 0)
        {
            CurrentStepIndex--;
        }
    }

    /// <summary>Resets the stepper to the first step.</summary>
    public void Reset()
    {
        CurrentStepIndex = 0;
    }

    /// <summary>Updates the theme colors for the control.</summary>
    public void RefreshColors() => RefreshTheme(ThemeManager.Current);

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
        BackColor = Color.Transparent;
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

        var theme = ThemeManager.Current;
        var palette = theme.Palette;
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        var circleDiameter = NexaDpi.Scale(_circleDiameterDips, dpi);
        var connectorThickness = NexaDpi.Scale(_connectorThicknessDips, dpi);
        var itemSpacing = NexaDpi.Scale(_itemSpacingDips, dpi);
        var radius = circleDiameter / 2;

        var visibleSteps = _steps.Where(s => s.Visible).ToList();
        if (visibleSteps.Count == 0) return;

        if (_orientation == NexaOrientation.Horizontal)
        {
            PaintHorizontal(g, palette, typography, dpi, visibleSteps, circleDiameter, connectorThickness, itemSpacing, radius);
        }
        else
        {
            PaintVertical(g, palette, typography, dpi, visibleSteps, circleDiameter, connectorThickness, itemSpacing, radius);
        }
    }

    private void PaintHorizontal(Graphics g, NexaPalette palette, NexaTypography typography, float dpi,
        List<NexaStep> steps, int circleDiameter, int connectorThickness, int itemSpacing, int radius)
    {
        var startY = Height / 2;
        var x = 0;
        var labelWidth = 120; // Fixed width for labels

        for (int i = 0; i < steps.Count; i++)
        {
            var step = steps[i];
            var isLast = i == steps.Count - 1;
            var stepIndex = _steps.IndexOf(step);
            var isCurrent = stepIndex == _currentStepIndex;
            var isCompleted = stepIndex < _currentStepIndex;
            var isError = step.State == NexaStepState.Error;
            var isDisabled = !step.Enabled;

            // Determine colors based on state
            var circleColor = isDisabled
                ? (Color)palette[NexaColorRole.TextDisabled].Value
                : isError
                    ? (Color)palette[NexaColorRole.Danger].Value
                    : isCompleted || isCurrent
                        ? (Color)palette[NexaColorRole.Primary].Value
                        : (Color)palette[NexaColorRole.Border].Value;

            var connectorColor = isCompleted
                ? (Color)palette[NexaColorRole.Primary].Value
                : (Color)palette[NexaColorRole.Border].Value;

            var textColor = isDisabled
                ? (Color)palette[NexaColorRole.TextDisabled].Value
                : isCurrent
                    ? (Color)palette[NexaColorRole.TextPrimary].Value
                    : (Color)palette[NexaColorRole.TextSecondary].Value;

            var circleX = x + radius;
            var circleY = startY;

            // Draw connector line (before the circle, except for first item)
            if (i > 0)
            {
                var connectorY = startY;
                var connectorX = x - radius - itemSpacing / 2;
                var connectorWidth = itemSpacing;

                using var pen = new Pen(connectorColor, 2);
                g.DrawLine(pen,
                    x - radius - itemSpacing,
                    connectorY,
                    x - radius,
                    connectorY);
            }

            // Draw circle
            var circleRect = new Rectangle(x - radius, startY - radius, circleDiameter, circleDiameter);
            using (var brush = new SolidBrush(circleColor))
            {
                g.FillEllipse(brush, circleRect);
            }

            // Draw circle border for pending steps
            if (!isCompleted && !isCurrent && !isError)
            {
                using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 2);
                g.DrawEllipse(pen, circleRect);
            }

            // Draw step number or checkmark
            using var font = typography.ToFont(NexaTypographyRole.Label, dpi);
            var circleTextColor = isCompleted || isCurrent
                ? Color.White
                : (Color)palette[NexaColorRole.TextSecondary].Value;

            string circleText;
            if (isCompleted && !isError)
            {
                circleText = "✓";
            }
            else if (_showStepNumbers)
            {
                circleText = (i + 1).ToString();
            }
            else
            {
                circleText = "";
            }

            if (!string.IsNullOrEmpty(circleText))
            {
                var textRect = new Rectangle(x - radius, startY - radius, circleDiameter, circleDiameter);
                TextRenderer.DrawText(g, circleText, font, textRect, Color.White,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter |
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }

            // Draw title and description
            var labelX = x + radius + 8;
            var labelY = startY - radius;

            if (!string.IsNullOrEmpty(step.Title))
            {
                using var titleFont = typography.ToFont(NexaTypographyRole.BodyStrong, dpi);
                var titleRect = new Rectangle(
                    (int)labelX,
                    labelY,
                    labelWidth,
                    NexaDpi.Scale(20, dpi));

                var titleColor = isDisabled
                    ? (Color)palette[NexaColorRole.TextDisabled].Value
                    : isCurrent
                        ? (Color)palette[NexaColorRole.TextPrimary].Value
                        : (Color)palette[NexaColorRole.TextPrimary].Value;

                TextRenderer.DrawText(g, step.Title, titleFont, titleRect, textColor,
                    TextFormatFlags.SingleLine | TextFormatFlags.NoPadding);
            }

            if (_showDescriptions && !string.IsNullOrEmpty(step.Description))
            {
                using var descFont = typography.ToFont(NexaTypographyRole.Caption, dpi);
                var descRect = new Rectangle(
                    (int)labelX,
                    labelY + NexaDpi.Scale(20, dpi),
                    labelWidth,
                    NexaDpi.Scale(20, dpi));

                TextRenderer.DrawText(g, step.Description, descFont, descRect,
                    (Color)palette[NexaColorRole.TextSecondary].Value,
                    TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding);
            }

            x += radius * 2 + itemSpacing + labelWidth;
        }
    }

    private void PaintVertical(Graphics g, NexaPalette palette, NexaTypography typography, float dpi,
        List<NexaStep> steps, int circleDiameter, int connectorThickness, int itemSpacing, int radius)
    {
        // Similar to horizontal but vertical - simplified for Phase 11
        // For now, just delegate to a basic implementation
        PaintHorizontal(g, palette, typography, dpi, steps, circleDiameter, connectorThickness, itemSpacing, radius);
    }

    /// <inheritdoc />
    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (!_allowNavigation) return;

        var dpi = CurrentDpi();
        var circleDiameter = NexaDpi.Scale(_circleDiameterDips, dpi);
        var itemSpacing = NexaDpi.Scale(_itemSpacingDips, dpi);
        var radius = circleDiameter / 2;
        var labelWidth = 120;

        var visibleSteps = _steps.Where(s => s.Visible && s.Enabled).ToList();
        var x = 0;

        for (int i = 0; i < visibleSteps.Count; i++)
        {
            var step = visibleSteps[i];
            var stepIndex = _steps.IndexOf(step);
            var circleRect = new Rectangle(x - radius, Height / 2 - radius, circleDiameter, circleDiameter);

            if (circleRect.Contains(e.Location))
            {
                CurrentStepIndex = stepIndex;
                StepClick?.Invoke(this, new NexaStepClickEventArgs(step));
                break;
            }

            x += radius * 2 + itemSpacing + 120;
        }
    }

    /// <summary>Raised when the current step changes.</summary>
    [Category("NexaUI")]
    [Description("Raised when the current step changes.")]
    public event EventHandler<NexaStepChangedEventArgs>? CurrentStepChanged;

    /// <summary>Raised when a step is clicked.</summary>
    [Category("NexaUI")]
    [Description("Raised when a step is clicked.")]
    public event EventHandler<NexaStepClickEventArgs>? StepClick;
}

/// <summary>
/// Event arguments for stepper step changed events.
/// </summary>
public class NexaStepChangedEventArgs : EventArgs
{
    public NexaStep Step { get; }

    public NexaStepChangedEventArgs(NexaStep step)
    {
        Step = step;
    }
}

/// <summary>
/// Event arguments for stepper step click events.
/// </summary>
public class NexaStepClickEventArgs : EventArgs
{
    public NexaStep Step { get; }

    public NexaStepClickEventArgs(NexaStep step)
    {
        Step = step;
    }
}