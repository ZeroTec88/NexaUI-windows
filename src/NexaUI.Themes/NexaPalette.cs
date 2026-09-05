using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Aggregates every semantic color exposed by a theme. Provides a strongly typed
/// lookup via <see cref="Get"/> and indexed access via <see cref="this[ NexaColorRole]"/>.
/// </summary>
public sealed class NexaPalette
{
    private readonly Dictionary<NexaColorRole, NexaSemanticColor> _colors;

    public NexaPalette(
        NexaSemanticColor primary,
        NexaSemanticColor secondary,
        NexaSemanticColor background,
        NexaSemanticColor surface,
        NexaSemanticColor surfaceVariant,
        NexaSemanticColor border,
        NexaSemanticColor inputBorder,
        NexaSemanticColor inputBackground,
        NexaSemanticColor inputPlaceholder,
        NexaSemanticColor textPrimary,
        NexaSemanticColor textSecondary,
        NexaSemanticColor textDisabled,
        NexaSemanticColor textOnAccent,
        NexaSemanticColor success,
        NexaSemanticColor warning,
        NexaSemanticColor danger,
        NexaSemanticColor info,
        NexaSemanticColor focus)
    {
        _colors = new Dictionary<NexaColorRole, NexaSemanticColor>
        {
            [NexaColorRole.Primary] = primary,
            [NexaColorRole.Secondary] = secondary,
            [NexaColorRole.Background] = background,
            [NexaColorRole.Surface] = surface,
            [NexaColorRole.SurfaceVariant] = surfaceVariant,
            [NexaColorRole.Border] = border,
            [NexaColorRole.InputBorder] = inputBorder,
            [NexaColorRole.InputBackground] = inputBackground,
            [NexaColorRole.InputPlaceholder] = inputPlaceholder,
            [NexaColorRole.TextPrimary] = textPrimary,
            [NexaColorRole.TextSecondary] = textSecondary,
            [NexaColorRole.TextDisabled] = textDisabled,
            [NexaColorRole.TextOnAccent] = textOnAccent,
            [NexaColorRole.Success] = success,
            [NexaColorRole.Warning] = warning,
            [NexaColorRole.Danger] = danger,
            [NexaColorRole.Info] = info,
            [NexaColorRole.Focus] = focus
        };
    }

    public NexaSemanticColor this[NexaColorRole role] => _colors[role];

    public NexaSemanticColor Get(NexaColorRole role) => _colors[role];

    public bool TryGet(NexaColorRole role, out NexaSemanticColor color) => _colors.TryGetValue(role, out color!);
}
