using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// The default dark theme for NexaUI. Uses an iris blue accent on a deep slate canvas,
/// with bright text and elevated dark surfaces.
/// </summary>
public sealed class DarkTheme : NexaThemeBase
{
    public DarkTheme() : base("Nexa Dark", "Dark", true)
    {
    }

    public override NexaPalette Palette { get; } = BuildPalette();
    public override NexaTypography Typography { get; } = BuildTypography();
    public override NexaSpacing Spacing { get; } = BuildSpacing();
    public override NexaMetrics Metrics { get; } = BuildMetrics(BuildSpacing());
    public override NexaControlStatePalette ControlStates { get; } = BuildStates(BuildPalette());

    private static NexaPalette BuildPalette() => new(
        primary: new NexaSemanticColor(
            value: Color.FromArgb(132, 142, 252),
            accent: Color.FromArgb(154, 162, 254),
            contrast: Color.FromArgb(16, 18, 28),
            subtle: Color.FromArgb(40, 46, 86)),
        secondary: new NexaSemanticColor(
            value: Color.FromArgb(162, 170, 252),
            accent: Color.FromArgb(180, 188, 254),
            contrast: Color.FromArgb(16, 18, 28),
            subtle: Color.FromArgb(46, 52, 96)),
        background: new NexaSemanticColor(Color.FromArgb(18, 20, 28)),
        surface: new NexaSemanticColor(Color.FromArgb(26, 30, 40)),
        surfaceVariant: new NexaSemanticColor(Color.FromArgb(34, 38, 50)),
        border: new NexaSemanticColor(Color.FromArgb(54, 60, 76)),
        inputBorder: new NexaSemanticColor(Color.FromArgb(64, 70, 88)),
        inputBackground: new NexaSemanticColor(Color.FromArgb(30, 34, 46)),
        inputPlaceholder: new NexaSemanticColor(Color.FromArgb(120, 128, 144)),
        textPrimary: new NexaSemanticColor(Color.FromArgb(238, 240, 246)),
        textSecondary: new NexaSemanticColor(Color.FromArgb(178, 184, 198)),
        textDisabled: new NexaSemanticColor(Color.FromArgb(110, 116, 130)),
        textOnAccent: new NexaSemanticColor(Color.FromArgb(16, 18, 28)),
        success: new NexaSemanticColor(
            value: Color.FromArgb(86, 204, 132),
            accent: Color.FromArgb(110, 218, 152),
            contrast: Color.FromArgb(10, 24, 16),
            subtle: Color.FromArgb(28, 60, 42)),
        warning: new NexaSemanticColor(
            value: Color.FromArgb(248, 184, 76),
            accent: Color.FromArgb(252, 200, 110),
            contrast: Color.FromArgb(28, 18, 6),
            subtle: Color.FromArgb(80, 60, 22)),
        danger: new NexaSemanticColor(
            value: Color.FromArgb(248, 110, 116),
            accent: Color.FromArgb(252, 134, 140),
            contrast: Color.FromArgb(28, 10, 12),
            subtle: Color.FromArgb(78, 28, 32)),
        info: new NexaSemanticColor(
            value: Color.FromArgb(98, 178, 232),
            accent: Color.FromArgb(124, 196, 240),
            contrast: Color.FromArgb(8, 18, 28),
            subtle: Color.FromArgb(26, 50, 76)),
        focus: new NexaSemanticColor(
            value: Color.FromArgb(170, 178, 254),
            accent: Color.FromArgb(170, 178, 254),
            contrast: Color.FromArgb(16, 18, 28),
            subtle: Color.FromArgb(58, 66, 116)));

    private static NexaTypography BuildTypography()
    {
        var family = "Segoe UI";
        return new NexaTypography(
            body: new NexaTypographyStyle(family, 9F, FontStyle.Regular, 14F, 0F),
            bodyStrong: new NexaTypographyStyle(family, 9F, FontStyle.Bold, 14F, 0F),
            caption: new NexaTypographyStyle(family, 8F, FontStyle.Regular, 12F, 0.2F),
            label: new NexaTypographyStyle(family, 9F, FontStyle.Regular, 14F, 0.1F),
            button: new NexaTypographyStyle(family, 9F, FontStyle.Bold, 14F, 0.2F),
            heading: new NexaTypographyStyle(family, 14F, FontStyle.Bold, 20F, 0F),
            title: new NexaTypographyStyle(family, 12F, FontStyle.Bold, 18F, 0F),
            display: new NexaTypographyStyle(family, 18F, FontStyle.Bold, 24F, -0.2F));
    }

    private static NexaSpacing BuildSpacing() => new(
        none: 0,
        xxs: 2,
        xs: 4,
        sm: 6,
        md: 10,
        lg: 16,
        xl: 22,
        xxl: 32,
        huge: 48);

    private static NexaMetrics BuildMetrics(NexaSpacing spacing) => new(
        spacing: spacing,
        borderThicknessInDips: 1,
        focusRingThicknessInDips: 2,
        cornerRadiiInDips: new Dictionary<NexaCornerRadiusToken, int>
        {
            [NexaCornerRadiusToken.None] = 0,
            [NexaCornerRadiusToken.Small] = 1,
            [NexaCornerRadiusToken.Medium] = 2,
            [NexaCornerRadiusToken.Large] = 3,
            [NexaCornerRadiusToken.XLarge] = 4,
            [NexaCornerRadiusToken.Pill] = 999
        },
        elevations: new Dictionary<NexaElevationLevel, NexaElevation>
        {
            [NexaElevationLevel.Flat] = NexaElevation.Flat,
            [NexaElevationLevel.Raised] = new NexaElevation(Color.FromArgb(0, 0, 0), 14, 2, 64),
            [NexaElevationLevel.Floating] = new NexaElevation(Color.FromArgb(0, 0, 0), 22, 8, 96),
            [NexaElevationLevel.Overlay] = new NexaElevation(Color.FromArgb(0, 0, 0), 28, 12, 128),
            [NexaElevationLevel.Modal] = new NexaElevation(Color.FromArgb(0, 0, 0), 36, 18, 160)
        });

    private static NexaControlStatePalette BuildStates(NexaPalette palette)
    {
        var primary = palette[NexaColorRole.Primary];
        var surface = palette[NexaColorRole.Surface];
        var surfaceVariant = palette[NexaColorRole.SurfaceVariant];
        var border = palette[NexaColorRole.Border];
        var textPrimary = palette[NexaColorRole.TextPrimary];
        var danger = palette[NexaColorRole.Danger];
        var success = palette[NexaColorRole.Success];

        return new NexaControlStatePalette(new Dictionary<NexaControlState, NexaStateColors>
        {
            [NexaControlState.Normal] = new NexaStateColors(surface.Value, textPrimary.Value, border.Value),
            [NexaControlState.Hover] = new NexaStateColors(surfaceVariant.Value, textPrimary.Value, primary.Value),
            [NexaControlState.Pressed] = new NexaStateColors(surfaceVariant.Value, textPrimary.Value, primary.Value),
            [NexaControlState.Focused] = new NexaStateColors(surface.Value, textPrimary.Value, primary.Value),
            [NexaControlState.Selected] = new NexaStateColors(primary.Subtle.Value, primary.Value, primary.Value),
            [NexaControlState.Disabled] = new NexaStateColors(surfaceVariant.Value, palette[NexaColorRole.TextDisabled].Value, border.Value),
            [NexaControlState.Error] = new NexaStateColors(danger.Subtle.Value, danger.Value, danger.Value),
            [NexaControlState.Success] = new NexaStateColors(success.Subtle.Value, success.Value, success.Value)
        });
    }
}