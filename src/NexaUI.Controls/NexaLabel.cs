using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using NexaUI.Core;
using NexaUI.Themes;

namespace NexaUI.Controls;

/// <summary>
/// A theme-aware text label. Inherits from the native <see cref="Label"/> so all standard
/// behavior — font measurement, AutoSize, accessibility, text alignment, padding — is preserved.
/// The label's font and foreground color are derived from <see cref="NexaLabelStyle"/> and the
/// currently active <see cref="NexaTheme"/>.
/// </summary>
[DefaultEvent(nameof(TextChanged))]
[DefaultProperty(nameof(Text))]
public class NexaLabel : Label
{
    private NexaLabelStyle _style = NexaLabelStyle.Default;
    private bool _useStyleColor = true;
    private bool _useStyleFont = true;

    public NexaLabel()
    {
        ThemeManager.ThemeChanged += OnThemeChanged;
        Disposed += (_, _) => ThemeManager.ThemeChanged -= OnThemeChanged;

        HandleCreated += (_, _) => RefreshTheme(ThemeManager.Current);
    }

    [Category("NexaUI")]
    [DefaultValue(NexaLabelStyle.Default)]
    [Description("Visual style variant that maps the label to a typography role and semantic color.")]
    public NexaLabelStyle LabelStyle
    {
        get => _style;
        set { _style = value; RefreshTheme(ThemeManager.Current); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("When true, the label's ForeColor is driven by the LabelStyle.")]
    public bool UseStyleColor
    {
        get => _useStyleColor;
        set { _useStyleColor = value; RefreshTheme(ThemeManager.Current); }
    }

    [Category("NexaUI")]
    [DefaultValue(true)]
    [Description("When true, the label's Font is driven by the LabelStyle and theme typography.")]
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

    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set { base.Text = value ?? string.Empty; }
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

        if (_useStyleFont && IsHandleCreated)
        {
            try
            {
                Font = typography.ToFont(ResolveTypographyRole(_style), dpi);
            }
            catch
            {
                // Ignore font assignment errors in headless test environments.
            }
        }

        var role = ResolveColorRole(_style);
        var textColor = (Color)palette[role].Value;
        if (!Enabled) textColor = (Color)palette[NexaColorRole.TextDisabled].Value;
        if (_useStyleColor && IsHandleCreated)
        {
            try
            {
                ForeColor = textColor;
            }
            catch
            {
                // Ignore color assignment errors in headless test environments.
            }
        }
    }

    private static NexaTypographyRole ResolveTypographyRole(NexaLabelStyle style) => style switch
    {
        NexaLabelStyle.Heading => NexaTypographyRole.Heading,
        NexaLabelStyle.Subheading => NexaTypographyRole.Title,
        NexaLabelStyle.Caption => NexaTypographyRole.Caption,
        NexaLabelStyle.Muted => NexaTypographyRole.Body,
        NexaLabelStyle.Default => NexaTypographyRole.Body,
        NexaLabelStyle.Success => NexaTypographyRole.BodyStrong,
        NexaLabelStyle.Warning => NexaTypographyRole.BodyStrong,
        NexaLabelStyle.Danger => NexaTypographyRole.BodyStrong,
        NexaLabelStyle.Info => NexaTypographyRole.BodyStrong,
        _ => NexaTypographyRole.Body
    };

    private static NexaColorRole ResolveColorRole(NexaLabelStyle style) => style switch
    {
        NexaLabelStyle.Heading => NexaColorRole.TextPrimary,
        NexaLabelStyle.Subheading => NexaColorRole.TextPrimary,
        NexaLabelStyle.Caption => NexaColorRole.TextSecondary,
        NexaLabelStyle.Muted => NexaColorRole.TextSecondary,
        NexaLabelStyle.Default => NexaColorRole.TextPrimary,
        NexaLabelStyle.Success => NexaColorRole.Success,
        NexaLabelStyle.Warning => NexaColorRole.Warning,
        NexaLabelStyle.Danger => NexaColorRole.Danger,
        NexaLabelStyle.Info => NexaColorRole.Info,
        _ => NexaColorRole.TextPrimary
    };
}