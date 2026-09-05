using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// The default light theme for NexaUI. Uses an iris blue accent on a near-white canvas,
/// with neutral text and elevated surfaces.
/// </summary>
public sealed class LightTheme : NexaThemeBase
{
    public LightTheme() : base("Nexa Light", "Light", false)
    {
    }

    public override NexaPalette Palette { get; } = BuildPalette();
    public override NexaTypography Typography { get; } = BuildTypography();
    public override NexaSpacing Spacing { get; } = BuildSpacing();
    public override NexaMetrics Metrics { get; } = BuildMetrics(BuildSpacing());
    public override NexaControlStatePalette ControlStates { get; } = BuildStates(BuildPalette());

    private static NexaPalette BuildPalette() => new(
        primary: new NexaSemanticColor(
            value: Color.FromArgb(85, 92, 240),
            accent: Color.FromArgb(70, 78, 230),
            contrast: Color.White,
            subtle: Color.FromArgb(232, 233, 252)),
        secondary: new NexaSemanticColor(
            value: Color.FromArgb(120, 124, 232),
            accent: Color.FromArgb(96, 100, 210),
            contrast: Color.White,
            subtle: Color.FromArgb(232, 233, 252)),
        background: new NexaSemanticColor(Color.FromArgb(248, 249, 252)),
        surface: new NexaSemanticColor(Color.White),
        surfaceVariant: new NexaSemanticColor(Color.FromArgb(243, 244, 248)),
        border: new NexaSemanticColor(Color.FromArgb(220, 223, 230)),
        textPrimary: new NexaSemanticColor(Color.FromArgb(24, 28, 36)),
        textSecondary: new NexaSemanticColor(Color.FromArgb(86, 92, 108)),
        textDisabled: new NexaSemanticColor(Color.FromArgb(170, 176, 188)),
        textOnAccent: new NexaSemanticColor(Color.White),
        success: new NexaSemanticColor(
            value: Color.FromArgb(34, 158, 86),
            accent: Color.FromArgb(24, 132, 72),
            contrast: Color.White,
            subtle: Color.FromArgb(220, 244, 230)),
        warning: new NexaSemanticColor(
            value: Color.FromArgb(232, 158, 36),
            accent: Color.FromArgb(212, 138, 18),
            contrast: Color.FromArgb(28, 24, 12),
            subtle: Color.FromArgb(252, 240, 212)),
        danger: new NexaSemanticColor(
            value: Color.FromArgb(220, 64, 72),
            accent: Color.FromArgb(196, 48, 56),
            contrast: Color.White,
            subtle: Color.FromArgb(252, 226, 228)),
        info: new NexaSemanticColor(
            value: Color.FromArgb(40, 144, 220),
            accent: Color.FromArgb(28, 124, 200),
            contrast: Color.White,
            subtle: Color.FromArgb(220, 238, 250)),
        focus: new NexaSemanticColor(
            value: Color.FromArgb(120, 140, 250),
            accent: Color.FromArgb(120, 140, 250),
            contrast: Color.White,
            subtle: Color.FromArgb(220, 226, 252)));

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
            [NexaCornerRadiusToken.Small] = 4,
            [NexaCornerRadiusToken.Medium] = 8,
            [NexaCornerRadiusToken.Large] = 12,
            [NexaCornerRadiusToken.XLarge] = 18,
            [NexaCornerRadiusToken.Pill] = 999
        },
        elevations: new Dictionary<NexaElevationLevel, NexaElevation>
        {
            [NexaElevationLevel.Flat] = NexaElevation.Flat,
            [NexaElevationLevel.Raised] = new NexaElevation(Color.FromArgb(20, 24, 36), 12, 2, 24),
            [NexaElevationLevel.Floating] = new NexaElevation(Color.FromArgb(28, 32, 48), 18, 6, 32),
            [NexaElevationLevel.Overlay] = new NexaElevation(Color.FromArgb(28, 32, 48), 24, 10, 40),
            [NexaElevationLevel.Modal] = new NexaElevation(Color.FromArgb(36, 40, 56), 32, 14, 56)
        });

    private static NexaControlStatePalette BuildStates(NexaPalette palette)
    {
        var primary = palette[NexaColorRole.Primary];
        var surface = palette[NexaColorRole.Surface];
        var surfaceVariant = palette[NexaColorRole.SurfaceVariant];
        var border = palette[NexaColorRole.Border];
        var textPrimary = palette[NexaColorRole.TextPrimary];
        var textOnAccent = palette[NexaColorRole.TextOnAccent];
        var danger = palette[NexaColorRole.Danger];
        var success = palette[NexaColorRole.Success];

        return new NexaControlStatePalette(new Dictionary<NexaControlState, NexaStateColors>
        {
            [NexaControlState.Normal] = new NexaStateColors(surface.Value, textPrimary.Value, border.Value),
            [NexaControlState.Hover] = new NexaStateColors(surfaceVariant.Value, textPrimary.Value, primary.Value),
            [NexaControlState.Pressed] = new NexaStateColors(surfaceVariant.Value, textPrimary.Value, primary.Value),
            [NexaControlState.Focused] = new NexaStateColors(surface.Value, textPrimary.Value, primary.Value),
            [NexaControlState.Selected] = new NexaStateColors(primary.Subtle.Value, primary.Accent.Value, primary.Value),
            [NexaControlState.Disabled] = new NexaStateColors(surfaceVariant.Value, palette[NexaColorRole.TextDisabled].Value, border.Value),
            [NexaControlState.Error] = new NexaStateColors(danger.Subtle.Value, danger.Value, danger.Value),
            [NexaControlState.Success] = new NexaStateColors(success.Subtle.Value, success.Value, success.Value)
        });
    }
}