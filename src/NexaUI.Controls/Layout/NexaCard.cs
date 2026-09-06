using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A modern card container provided by NexaUI. Composes a themed background, an
/// optional header (title + subtitle), a content area, and an optional footer. Add
/// child controls to <see cref="ContentPanel"/> (or to <see cref="Control.ControlCollection"/>
/// via the standard <c>card.Controls.Add(...)</c> which routes into the content area).
/// Inherits from <see cref="UserControl"/> so docking, anchoring, tab order, and
/// Visual Studio designer behavior are all preserved.
/// </summary>
[DefaultEvent(nameof(Load))]
[DefaultProperty(nameof(Title))]
public class NexaCard : UserControl
{
    private readonly Panel _headerPanel;
    private readonly Panel _contentPanel;
    private readonly Panel _footerPanel;
    private readonly Label _titleLabel;
    private readonly Label _subtitleLabel;

    private string _title = string.Empty;
    private string _subtitle = string.Empty;
    private bool _headerVisible = true;
    private bool _footerVisible;
    private int _cornerRadiusDips = 4;
    private int _borderThicknessDips = 1;
    private int _shadowDepthDips = 0;
    private bool _shadowEnabled;
    private int _headerHeightDips = 56;
    private int _footerHeightDips = 40;
    private int _cardPaddingDips = 12;

    /// <summary>Initializes a new instance of the <see cref="NexaCard"/> class.</summary>
    public NexaCard()
    {
        SetStyle(
            ControlStyles.UserPaint |
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.OptimizedDoubleBuffer |
            ControlStyles.ResizeRedraw |
            ControlStyles.SupportsTransparentBackColor,
            true);
        DoubleBuffered = true;

        SuspendLayout();

        _headerPanel = new Panel { Dock = DockStyle.Top, Name = "Header" };
        _headerPanel.Resize += (_, _) => LayoutHeader();
        _titleLabel = new Label { AutoSize = true, BackColor = Color.Transparent };
        _subtitleLabel = new Label { AutoSize = true, BackColor = Color.Transparent };
        _headerPanel.Controls.Add(_titleLabel);
        _headerPanel.Controls.Add(_subtitleLabel);

        _contentPanel = new Panel { Dock = DockStyle.Fill, Name = "Content" };

        _footerPanel = new Panel { Dock = DockStyle.Bottom, Name = "Footer" };

        Controls.Add(_contentPanel);
        Controls.Add(_headerPanel);
        Controls.Add(_footerPanel);

        ResumeLayout(performLayout: false);

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => ApplyTheme(ThemeManager.Current);

        ApplySizing();
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>The content panel — add child controls here (or to <c>card.Controls</c>).</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Panel ContentPanel => _contentPanel;

    /// <summary>The header panel — title and subtitle labels are drawn on it.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Panel HeaderPanel => _headerPanel;

    /// <summary>The footer panel — add footer controls here.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Panel FooterPanel => _footerPanel;

    /// <summary>Card title shown in the header.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Card title shown in the header.")]
    public string Title
    {
        get => _title;
        set { _title = value ?? string.Empty; _titleLabel.Text = _title; LayoutHeader(); Invalidate(); }
    }

    /// <summary>Optional card subtitle shown below the title.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional card subtitle shown below the title.")]
    public string Subtitle
    {
        get => _subtitle;
        set { _subtitle = value ?? string.Empty; _subtitleLabel.Text = _subtitle; _subtitleLabel.Visible = !string.IsNullOrEmpty(_subtitle); LayoutHeader(); Invalidate(); }
    }

    /// <summary>Whether the header area is visible.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether the header area is visible.")]
    public bool HeaderVisible
    {
        get => _headerVisible;
        set { _headerVisible = value; _headerPanel.Visible = value; Invalidate(); }
    }

    /// <summary>Whether the footer area is visible.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the footer area is visible.")]
    public bool FooterVisible
    {
        get => _footerVisible;
        set { _footerVisible = value; _footerPanel.Visible = value; Invalidate(); }
    }

    /// <summary>Border color. Defaults to the theme Border role.</summary>
    [Category("NexaUI")]
    [Description("Border color. Defaults to the theme Border role when left empty.")]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("WinForms", "WFO1000", Justification = "Themed property; theme drives the value by default.")]
    public Color BorderColor { get; set; } = Color.Empty;

    /// <summary>Border thickness in DIPs.</summary>
    [Category("NexaUI")]
    [DefaultValue(1)]
    [Description("Border thickness in device-independent pixels.")]
    public int BorderThickness
    {
        get => _borderThicknessDips;
        set { _borderThicknessDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Corner radius in DIPs (visual only).</summary>
    [Category("NexaUI")]
    [DefaultValue(4)]
    [Description("Corner radius in device-independent pixels.")]
    public int CornerRadius
    {
        get => _cornerRadiusDips;
        set { _cornerRadiusDips = Math.Max(0, value); Invalidate(); }
    }

    /// <summary>Whether a subtle drop shadow is drawn behind the card.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether a subtle drop shadow is drawn behind the card.")]
    public bool ShadowEnabled
    {
        get => _shadowEnabled;
        set { _shadowEnabled = value; Invalidate(); }
    }

    /// <summary>Shadow depth in DIPs (0–8).</summary>
    [Category("NexaUI")]
    [DefaultValue(0)]
    [Description("Shadow depth in device-independent pixels (0–8).")]
    public int ShadowDepth
    {
        get => _shadowDepthDips;
        set { _shadowDepthDips = Math.Clamp(value, 0, 8); Invalidate(); }
    }

    /// <summary>Re-applies theme colors to the control.</summary>
    public void RefreshColors() => ApplyTheme(ThemeManager.Current);

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
        var typography = theme.Typography;
        var dpi = CurrentDpi();

        BackColor = (Color)palette[NexaColorRole.Surface].Value;
        _contentPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        _headerPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        _footerPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;

        _titleLabel.Font = typography.ToFont(NexaTypographyRole.Title, dpi);
        _subtitleLabel.Font = typography.ToFont(NexaTypographyRole.Caption, dpi);
        _titleLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        _subtitleLabel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;
        _footerPanel.ForeColor = (Color)palette[NexaColorRole.TextSecondary].Value;

        Invalidate();
    }

    private void ApplySizing()
    {
        var dpi = CurrentDpi();
        _headerPanel.MinimumSize = new Size(0, NexaDpi.Scale(_headerHeightDips, dpi));
        _footerPanel.MinimumSize = new Size(0, NexaDpi.Scale(_footerHeightDips, dpi));
        _contentPanel.Padding = new Padding(NexaDpi.Scale(_cardPaddingDips, dpi));
        LayoutHeader();
    }

    private void LayoutHeader()
    {
        if (IsDisposed || Disposing) return;
        var dpi = CurrentDpi();
        var padX = NexaDpi.Scale(12, dpi);
        var padY = NexaDpi.Scale(8, dpi);
        var lineGap = NexaDpi.Scale(2, dpi);
        _titleLabel.Location = new Point(padX, padY);
        if (!string.IsNullOrEmpty(_subtitle))
        {
            _subtitleLabel.Location = new Point(padX, _titleLabel.Bottom + lineGap);
        }
    }

    /// <inheritdoc />
    protected override void OnResize(System.EventArgs e)
    {
        base.OnResize(e);
        ApplySizing();
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

        var radius = NexaDpi.Scale(_cornerRadiusDips, dpi);
        var borderThickness = NexaDpi.Scale(_borderThicknessDips, dpi);
        var shadowDepth = _shadowEnabled ? NexaDpi.Scale(_shadowDepthDips, dpi) : 0;

        var rect = new Rectangle(shadowDepth, shadowDepth, w - shadowDepth, h - shadowDepth);
        if (rect.Width <= 0 || rect.Height <= 0) return;

        if (shadowDepth > 0)
        {
            var shadowColor = (Color)palette[NexaColorRole.Border].Value;
            for (var i = shadowDepth; i >= 1; i--)
            {
                var bounds = new Rectangle(rect.X + i, rect.Y + i, rect.Width, rect.Height);
                var alpha = (int)(shadowColor.A * 0.2f * (1f - (i - 1) / (float)shadowDepth));
                if (alpha <= 0) continue;
                using var sp = NexaPanel.CreateRoundedPathPublic(bounds, radius);
                using var pen = new Pen(Color.FromArgb(alpha, shadowColor), 1f);
                g.DrawPath(pen, sp);
            }
        }

        using (var path = NexaPanel.CreateRoundedPathPublic(rect, radius))
        {
            using var brush = new SolidBrush(BackColor);
            g.FillPath(brush, path);
        }

        if (borderThickness > 0)
        {
            var borderColor = BorderColor.IsEmpty
                ? (Color)palette[NexaColorRole.Border].Value
                : BorderColor;
            using var path = NexaPanel.CreateRoundedPathPublic(rect, radius);
            using var pen = new Pen(borderColor, borderThickness);
            g.DrawPath(pen, path);
        }
    }
}
