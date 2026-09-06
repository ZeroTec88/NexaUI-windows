using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Icons;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A themed tab page inheriting from the native <see cref="TabPage"/>.
/// Preserves all native functionality while adding optional icon and badge support.
/// </summary>
[DefaultEvent(nameof(Click))]
[DefaultProperty(nameof(Text))]
public class NexaTabPage : TabPage
{
    private NexaIconKind _iconKind = NexaIconKind.None;
    private string _badgeText = string.Empty;
    private bool _badgeVisible = false;

    /// <summary>Initializes a new instance of the <see cref="NexaTabPage"/> class.</summary>
    public NexaTabPage()
    {
        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;
        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    private float CurrentDpi() => IsHandleCreated && !DesignMode
        ? NexaFormsDpi.CurrentDpi(this)
        : NexaDpi.BaseDpi;

    /// <summary>Icon to display alongside the tab text.</summary>
    [Category("NexaUI")]
    [DefaultValue(NexaIconKind.None)]
    [Description("Icon to display alongside the tab text.")]
    public NexaIconKind IconKind
    {
        get => _iconKind;
        set { _iconKind = value; Invalidate(); }
    }

    /// <summary>Optional badge text (e.g., count or status).</summary>
    [Category("NexaUI")]
    [DefaultValue("")]
    [Description("Optional badge text (e.g., count or status).")]
    public string BadgeText
    {
        get => _badgeText;
        set { _badgeText = value ?? string.Empty; Invalidate(); }
    }

    /// <summary>Whether the badge is visible.</summary>
    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Whether the badge is visible.")]
    public bool BadgeVisible
    {
        get => _badgeVisible;
        set { _badgeVisible = value; Invalidate(); }
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
        BackColor = (Color)palette[NexaColorRole.Background].Value;
        ForeColor = (Color)palette[NexaColorRole.TextPrimary].Value;
    }
}