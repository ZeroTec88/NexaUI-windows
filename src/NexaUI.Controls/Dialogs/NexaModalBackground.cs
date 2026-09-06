using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A semi-transparent modal background overlay that can be used behind dialogs,
/// drawers, or other modal content to dim the background and prevent interaction.
/// </summary>
public sealed class NexaModalBackground : UserControl
{
    private NexaOverlayStyle _overlayStyle = NexaOverlayStyle.Standard;
    private bool _clickToClose = true;
    private Control? _modalContent;

    /// <summary>Initializes a new instance of the <see cref="NexaModalBackground"/> class.</summary>
    public NexaModalBackground()
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
        TabStop = false;

        Click += (_, _) =>
        {
            if (_clickToClose)
            {
                CloseModal();
            }
        };

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

        var overlayColor = style switch
        {
            NexaOverlayStyle.Light => Color.FromArgb(200, (Color)palette[NexaColorRole.Background].Value),
            NexaOverlayStyle.Blur => Color.FromArgb(180, 255, 255, 255),
            NexaOverlayStyle.Minimal => Color.Transparent,
            _ => Color.FromArgb(200, (Color)palette[NexaColorRole.Background].Value)
        };

        BackColor = overlayColor;
    }

    /// <summary>Visual style of the modal background.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaOverlayStyle.Standard)]
    [Description("Visual style of the modal background.")]
    public NexaOverlayStyle OverlayStyle
    {
        get => _overlayStyle;
        set { _overlayStyle = value; RefreshTheme(ThemeManager.Current); }
    }

    /// <summary>Whether clicking the background closes the modal.</summary>
    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("Whether clicking the background closes the modal.")]
    public bool ClickToClose
    {
        get => _clickToClose;
        set { _clickToClose = value; }
    }

    /// <summary>Shows the modal background.</summary>
    public new void Show()
    {
        Visible = true;
        BringToFront();
    }

    /// <summary>Hides the modal background.</summary>
    public new void Hide()
    {
        Visible = false;
    }

    /// <summary>Shows the modal background with a content control centered on top.</summary>
    /// <param name="content">The modal content control to display.</param>
    /// <param name="centerContent">Whether to center the content.</param>
    public void ShowWithContent(Control content, bool centerContent = true)
    {
        if (_modalContent != null)
        {
            Controls.Remove(_modalContent);
            _modalContent.Dispose();
        }

        _modalContent = content;
        Controls.Add(_modalContent);
        _modalContent.BringToFront();

        if (centerContent)
        {
            CenterContent();
        }

        Show();
    }

    private void CenterContent()
    {
        if (_modalContent == null) return;

        _modalContent.Location = new Point(
            (Width - _modalContent.Width) / 2,
            (Height - _modalContent.Height) / 2);
    }

    /// <summary>Closes the modal background and its content.</summary>
    public void CloseModal()
    {
        Hide();

        if (_modalContent != null)
        {
            Controls.Remove(_modalContent);
            _modalContent.Dispose();
            _modalContent = null;
        }

        ModalClosed?.Invoke(this, EventArgs.Empty);
    }

    /// <inheritdoc />
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        CenterContent();
    }

    /// <summary>Raised when the modal is closed.</summary>
    [Category("NexaUI")]
    [Description("Raised when the modal is closed.")]
    public event EventHandler? ModalClosed;
}