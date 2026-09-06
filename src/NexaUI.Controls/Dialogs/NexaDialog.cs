using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A flexible themed dialog that can host arbitrary content.
/// Use this for custom dialogs with complex layouts.
/// </summary>
public class NexaDialog : Form
{
    private Panel _titleBar;
    private Label _titleLabel;
    private NexaButton _closeButton;
    private Panel _contentPanel;
    private Panel _buttonPanel;
    private FlowLayoutPanel _buttonLayout;
    private bool _isDragging;
    private Point _dragStart;
    private readonly List<NexaButton> _buttons = new();

    private NexaDialogStyle _dialogStyle = NexaDialogStyle.Standard;
    private string _dialogTitle = string.Empty;
    private NexaDialogResult _dialogResult = NexaDialogResult.None;

    /// <summary>Initializes a new instance of the <see cref="NexaDialog"/> class.</summary>
    protected NexaDialog()
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
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(320, 180);
        Padding = new Padding(1);

        InitializeComponents();

        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    private void InitializeComponents()
    {
        _titleBar = new Panel
        {
            Height = 40,
            Dock = DockStyle.Top,
            BackColor = Color.Transparent
        };
        _titleBar.MouseDown += OnTitleBarMouseDown;
        _titleBar.MouseMove += OnTitleBarMouseMove;
        _titleBar.MouseUp += OnTitleBarMouseUp;

        _titleLabel = new Label
        {
            Text = string.Empty,
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(16, 0, 0, 0),
            Font = new Font(new FontFamily("Segoe UI"), 10F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.White
        };

        _closeButton = new NexaButton
        {
            Text = string.Empty,
            IconKind = NexaIconKind.Close,
            Style = NexaButtonStyle.Ghost,
            Width = 40,
            Height = 40,
            Dock = DockStyle.Right,
            Margin = new Padding(0, 0, 8, 0),
            TabStop = false
        };
        _closeButton.Click += (_, _) => CloseDialog(NexaDialogResult.Cancel);

        _titleBar.Controls.Add(_titleLabel);
        _titleBar.Controls.Add(_closeButton);

        _contentPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(20),
            AutoScroll = true
        };

        _buttonPanel = new Panel
        {
            Height = 60,
            Dock = DockStyle.Bottom,
            Padding = new Padding(16, 8, 16, 16)
        };

        _buttonLayout = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            Margin = Padding.Empty
        };

        _buttonPanel.Controls.Add(_buttonLayout);

        Controls.Add(_contentPanel);
        Controls.Add(_buttonPanel);
        Controls.Add(_titleBar);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Dialog visual style.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaDialogStyle.Standard)]
    [Description("Visual style of the dialog.")]
    public NexaDialogStyle DialogStyle
    {
        get => _dialogStyle;
        set { _dialogStyle = value; UpdateStyle(); }
    }

    /// <summary>Dialog title text.</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Dialog title text.")]
    public string DialogTitle
    {
        get => _dialogTitle;
        set { _dialogTitle = value ?? string.Empty; _titleLabel.Text = _dialogTitle; }
    }

    /// <summary>Content panel for derived classes to add controls.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected Panel ContentPanel => _contentPanel;

    /// <summary>Button panel for derived classes to add buttons.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    protected FlowLayoutPanel ButtonLayout => _buttonLayout;

    /// <summary>Result of the dialog interaction.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new NexaDialogResult DialogResult
    {
        get => _dialogResult;
        protected set { _dialogResult = value; }
    }

    /// <summary>Adds a button to the dialog's button area.</summary>
    /// <param name="text">Button text.</param>
    /// <param name="result">Dialog result to return when clicked.</param>
    /// <param name="style">Button style.</param>
    /// <param name="iconKind">Optional icon.</param>
    /// <param name="isDefault">Whether this is the default button (Enter key).</param>
    /// <returns>The created button.</returns>
    protected NexaButton AddButton(string text, NexaDialogResult result, NexaButtonStyle style = NexaButtonStyle.Primary, NexaIconKind iconKind = NexaIconKind.None, bool isDefault = false)
    {
        var btn = new NexaButton
        {
            Text = text,
            Style = style,
            IconKind = iconKind,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Margin = new Padding(8, 0, 0, 0)
        };
        btn.Click += (_, _) => CloseDialog(result);
        _buttonLayout.Controls.Add(btn);
        _buttons.Add(btn);

        if (isDefault)
        {
            AcceptButton = btn;
        }

        return btn;
    }

    /// <summary>Closes the dialog with the specified result.</summary>
    protected void CloseDialog(NexaDialogResult result)
    {
        DialogResult = result;
        base.DialogResult = (System.Windows.Forms.DialogResult)result;
        Close();
    }

    /// <summary>Updates the visual style based on DialogStyle property.</summary>
    protected virtual void UpdateStyle()
    {
        Invalidate();
    }

    private void OnThemeChanged(object? sender, ThemeChangedEventArgs e) => RefreshTheme(e.Current);

    protected void RefreshTheme(ITheme theme)
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
        _buttonPanel.BackColor = (Color)palette[NexaColorRole.Surface].Value;
        Invalidate();
    }

    private void OnTitleBarMouseDown(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            _isDragging = true;
            _dragStart = e.Location;
        }
    }

    private void OnTitleBarMouseMove(object? sender, MouseEventArgs e)
    {
        if (_isDragging && e.Button == MouseButtons.Left)
        {
            Location = new Point(
                Location.X + e.X - _dragStart.X,
                Location.Y + e.Y - _dragStart.Y);
        }
    }

    private void OnTitleBarMouseUp(object? sender, MouseEventArgs e)
    {
        _isDragging = false;
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;

        var palette = ThemeManager.Current.Palette;
        var dpi = CurrentDpi();

        // Draw border
        var borderRect = new Rectangle(0, 0, Width - 1, Height - 1);
        var radius = _dialogStyle == NexaDialogStyle.Card
            ? NexaDpi.Scale(8, dpi)
            : 0;

        if (radius > 0)
        {
            using var path = CreateRoundedPath(borderRect, radius);
            using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            g.DrawPath(pen, path);
        }
        else
        {
            using var pen = new Pen((Color)palette[NexaColorRole.Border].Value, 1F);
            g.DrawRectangle(pen, borderRect);
        }

        // Draw title bar background
        var titleRect = new Rectangle(0, 0, Width, _titleBar.Height);
        if (radius > 0)
        {
            var clipPath = CreateRoundedPath(titleRect, radius);
            g.SetClip(clipPath);
        }
        using (var brush = new SolidBrush((Color)palette[NexaColorRole.Surface].Value))
        {
            g.FillRectangle(brush, titleRect);
        }
        g.ResetClip();
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