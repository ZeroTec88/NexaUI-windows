using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A popover control that displays content anchored to a control.
/// Similar to a tooltip but supports richer content and interaction.
/// </summary>
public sealed class NexaPopover : Form
{
    private readonly Panel _contentPanel;
    private readonly Label _titleLabel;
    private readonly NexaButton _closeButton;
    private readonly Panel _arrowPanel;
    private Control? _anchorControl;
    private NexaPopoverPosition _position = NexaPopoverPosition.Bottom;
    private bool _showCloseButton = true;
    private bool _isClosing = false;

    /// <summary>Initializes a new instance of the <see cref="NexaPopover"/> class.</summary>
    public NexaPopover()
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
        StartPosition = FormStartPosition.Manual;
        Size = new Size(300, 200);

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            AutoScroll = true
        };

        _titleLabel = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Top,
            Height = 32,
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point),
            Padding = new Padding(0, 0, 32, 8)
        };

        _closeButton = new NexaButton
        {
            Text = string.Empty,
            IconKind = NexaIconKind.Close,
            Style = NexaButtonStyle.Ghost,
            Width = 24,
            Height = 24,
            Dock = DockStyle.Right,
            Margin = new Padding(0, 4, 4, 0),
            TabStop = false
        };
        _closeButton.Click += (_, _) => ClosePopover();

        var titlePanel = new Panel { Height = 40, Dock = DockStyle.Top };
        titlePanel.Controls.Add(_titleLabel);
        titlePanel.Controls.Add(_closeButton);

        var mainPanel = new Panel { Dock = DockStyle.Fill };
        mainPanel.Controls.Add(_contentPanel);
        mainPanel.Controls.Add(titlePanel);

        Controls.Add(mainPanel);

        _arrowPanel = new Panel { Width = 12, Height = 6, BackColor = Color.Transparent };
        Controls.Add(_arrowPanel);

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
        BackColor = (Color)palette[NexaColorRole.Surface].Value;
        _titleLabel.ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
        _contentPanel.BackColor = (Color)palette[NexaColorRole.Background].Value;
        Invalidate();
    }

    /// <summary>Content panel for adding custom controls.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Panel ContentPanel => _contentPanel;

    /// <summary>Title text for the popover header.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Title text for the popover header.")]
    public string Title
    {
        get => _titleLabel.Text;
        set { _titleLabel.Text = value ?? string.Empty; _titleLabel.Visible = !string.IsNullOrEmpty(value); }
    }

    /// <summary>Anchor control that the popover is positioned relative to.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public Control? AnchorControl
    {
        get => _anchorControl;
        set { _anchorControl = value; UpdatePosition(); }
    }

    /// <summary>Position of the popover relative to the anchor control.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaPopoverPosition.Bottom)]
    [Description("Position of the popover relative to the anchor control.")]
    public NexaPopoverPosition Position
    {
        get => _position;
        set { _position = value; UpdatePosition(); }
    }

    /// <summary>Whether to show the close button.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether to show the close button.")]
    public bool ShowCloseButton
    {
        get => _showCloseButton;
        set { _showCloseButton = value; _closeButton.Visible = value; }
    }

    /// <summary>Shows the popover anchored to a control.</summary>
    public void Show(Control anchor, NexaPopoverPosition? position = null)
    {
        AnchorControl = anchor;
        if (position.HasValue) Position = position.Value;

        UpdatePosition();
        _isClosing = false;
        Opacity = 0;
        base.Show(anchor.FindForm());
        _fadeTimer?.Start();
    }

    private System.Windows.Forms.Timer? _fadeTimer;

    /// <summary>Closes the popover.</summary>
    public void ClosePopover()
    {
        if (_isClosing) return;
        _isClosing = true;
        // Fade out animation would go here
        Hide();
    }

    private void UpdatePosition()
    {
        if (_anchorControl == null || IsDisposed) return;

        var anchor = _anchorControl;
        var form = anchor.FindForm();
        if (form == null) return;

        var anchorScreen = anchor.PointToScreen(Point.Empty);
        var formScreen = form.PointToScreen(Point.Empty);
        var anchorRect = new Rectangle(
            anchorScreen.X - formScreen.X,
            anchorScreen.Y - formScreen.Y,
            anchor.Width,
            anchor.Height);

        int x, y;
        var offset = 8;

        switch (_position)
        {
            case NexaPopoverPosition.Top:
                x = anchorRect.Left + (anchorRect.Width - Width) / 2;
                y = anchorRect.Top - Height - offset;
                break;
            case NexaPopoverPosition.Bottom:
                x = anchorRect.Left + (anchorRect.Width - Width) / 2;
                y = anchorRect.Bottom + offset;
                break;
            case NexaPopoverPosition.Left:
                x = anchorRect.Left - Width - offset;
                y = anchorRect.Top + (anchorRect.Height - Height) / 2;
                break;
            case NexaPopoverPosition.Right:
                x = anchorRect.Right + offset;
                y = anchorRect.Top + (anchorRect.Height - Height) / 2;
                break;
            case NexaPopoverPosition.TopLeft:
                x = anchorRect.Left;
                y = anchorRect.Top - Height - offset;
                break;
            case NexaPopoverPosition.TopRight:
                x = anchorRect.Right - Width;
                y = anchorRect.Top - Height - offset;
                break;
            case NexaPopoverPosition.BottomLeft:
                x = anchorRect.Left;
                y = anchorRect.Bottom + offset;
                break;
            case NexaPopoverPosition.BottomRight:
                x = anchorRect.Right - Width;
                y = anchorRect.Bottom + offset;
                break;
            default:
                x = anchorRect.Left + (anchorRect.Width - Width) / 2;
                y = anchorRect.Bottom + offset;
                break;
        }

        // Keep within form bounds
        var formClient = form.ClientRectangle;
        x = Math.Max(formClient.Left + 8, Math.Min(x, formClient.Right - Width - 8));
        y = Math.Max(formClient.Top + 8, Math.Min(y, formClient.Bottom - Height - 8));

        Location = new Point(x, y);
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var palette = ThemeManager.Current.Palette;
        var dpi = NexaFormsDpi.CurrentDpi(this);
        var radius = NexaDpi.Scale(8, dpi);

        // Draw rounded background
        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        using var path = CreateRoundedPath(rect, radius);
        using var brush = new SolidBrush((Color)palette[NexaColorRole.Surface].Value);
        g.FillPath(brush, path);

        // Draw border
        using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
        g.DrawPath(pen, path);

        // Draw arrow
        DrawArrow(g, palette, radius);
    }

    private void DrawArrow(Graphics g, NexaPalette palette, int radius)
    {
        var arrowColor = (Color)palette[NexaColorRole.Surface].Value;
        var borderColor = (Color)palette[NexaColorRole.Border].Value;
        var arrowSize = 12;
        var arrowOffset = 8;

        Point[] arrowPoints;
        switch (_position)
        {
            case NexaPopoverPosition.Top:
            case NexaPopoverPosition.TopLeft:
            case NexaPopoverPosition.TopRight:
                // Arrow at bottom
                arrowPoints = new[]
                {
                    new Point(Width / 2 - arrowSize / 2, Height - 1),
                    new Point(Width / 2, Height - arrowSize - 1),
                    new Point(Width / 2 + arrowSize / 2, Height - 1)
                };
                break;
            case NexaPopoverPosition.Bottom:
            case NexaPopoverPosition.BottomLeft:
            case NexaPopoverPosition.BottomRight:
                // Arrow at top
                arrowPoints = new[]
                {
                    new Point(Width / 2 - arrowSize / 2, 0),
                    new Point(Width / 2, arrowSize),
                    new Point(Width / 2 + arrowSize / 2, 0)
                };
                break;
            case NexaPopoverPosition.Left:
                // Arrow at right
                arrowPoints = new[]
                {
                    new Point(Width - 1, Height / 2 - arrowSize / 2),
                    new Point(Width - arrowSize - 1, Height / 2),
                    new Point(Width - 1, Height / 2 + arrowSize / 2)
                };
                break;
            case NexaPopoverPosition.Right:
                // Arrow at left
                arrowPoints = new[]
                {
                    new Point(0, Height / 2 - arrowSize / 2),
                    new Point(arrowSize, Height / 2),
                    new Point(0, Height / 2 + arrowSize / 2)
                };
                break;
            default:
                return;
        }

        using var arrowBrush = new SolidBrush(arrowColor);
        using var arrowPen = new Pen(borderColor, 1F);
        g.FillPolygon(arrowBrush, arrowPoints);
        g.DrawPolygon(arrowPen, arrowPoints);
    }

    private static GraphicsPath CreateRoundedPath(Rectangle rect, int radius)
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
}