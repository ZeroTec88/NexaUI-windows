using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware hyperlink label. Inherits from <see cref="LinkLabel"/> so it keeps all
/// native link parsing, hit-testing, focus and accessibility behavior. Link colors are
/// derived from the active theme. Navigation is the consuming application's responsibility.
/// </summary>
[DefaultEvent(nameof(LinkClicked))]
[DefaultProperty(nameof(Text))]
public class NexaLinkLabel : LinkLabel
{
    private bool _visited;
    private bool _useStyleFont = true;

    public NexaLinkLabel()
    {
        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    [Category("NexaUI")]
    [DefaultValue(false)]
    [Description("Marks all links in the control as visited, switching them to the theme's visited color.")]
    public bool Visited
    {
        get => _visited;
        set { _visited = value; RefreshTheme(ThemeManager.Current); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("When true, the control's Font is driven by the theme typography.")]
    public bool UseStyleFont
    {
        get => _useStyleFont;
        set { _useStyleFont = value; RefreshTheme(ThemeManager.Current); }
    }

    protected override void OnEnabledChanged(System.EventArgs e)
    {
        base.OnEnabledChanged(e);
        RefreshTheme(ThemeManager.Current);
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
        var typography = theme.Typography;
        var dpi = IsHandleCreated ? NexaFormsDpi.CurrentDpi(this) : NexaDpi.BaseDpi;

        if (_useStyleFont)
        {
            Font = typography.ToFont(NexaTypographyRole.Button, dpi);
        }

        var active = (Color)palette[NexaColorRole.Primary].Value;
        var visitedColor = (Color)palette[NexaColorRole.Secondary].Value;
        var disabledColor = (Color)palette[NexaColorRole.TextDisabled].Value;

        if (!Enabled)
        {
            ActiveLinkColor = disabledColor;
            DisabledLinkColor = disabledColor;
            LinkColor = disabledColor;
            VisitedLinkColor = disabledColor;
        }
        else
        {
            LinkColor = _visited ? visitedColor : active;
            ActiveLinkColor = (Color)palette[NexaColorRole.Primary].Accent;
            VisitedLinkColor = visitedColor;
            DisabledLinkColor = disabledColor;
        }

        ForeColor = LinkColor;
        BackColor = Color.Transparent;
    }
}