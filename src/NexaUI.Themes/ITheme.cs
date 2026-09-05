using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Defines a complete NexaUI visual theme. Concrete implementations are
/// <see cref="LightTheme"/> and <see cref="DarkTheme"/>.
/// </summary>
public interface ITheme
{
    string Name { get; }
    string Kind { get; }
    bool IsDark { get; }

    NexaPalette Palette { get; }
    NexaTypography Typography { get; }
    NexaSpacing Spacing { get; }
    NexaMetrics Metrics { get; }
    NexaControlStatePalette ControlStates { get; }

    NexaColorToken GetColor(NexaColorRole role);
    NexaColorToken GetColor(NexaColorRole role, NexaControlState state);
}