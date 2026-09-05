using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Shared base class for <see cref="LightTheme"/> and <see cref="DarkTheme"/>,
/// providing default control-state color resolution logic.
/// </summary>
public abstract class NexaThemeBase : ITheme
{
    protected NexaThemeBase(string name, string kind, bool isDark)
    {
        Name = name;
        Kind = kind;
        IsDark = isDark;
    }

    public string Name { get; }
    public string Kind { get; }
    public bool IsDark { get; }

    public abstract NexaPalette Palette { get; }
    public abstract NexaTypography Typography { get; }
    public abstract NexaSpacing Spacing { get; }
    public abstract NexaMetrics Metrics { get; }
    public abstract NexaControlStatePalette ControlStates { get; }

    public NexaColorToken GetColor(NexaColorRole role) => Palette[role].Value;

    public virtual NexaColorToken GetColor(NexaColorRole role, NexaControlState state)
    {
        var states = ControlStates.Get(state);
        if (states.Background != Color.Empty)
        {
            return state switch
            {
                NexaControlState.Hover => new NexaColorToken(states.Background),
                NexaControlState.Pressed => new NexaColorToken(states.Background),
                NexaControlState.Focused => new NexaColorToken(states.Background),
                NexaControlState.Selected => new NexaColorToken(states.Background),
                _ => Palette[role].Value
            };
        }
        return Palette[role].Value;
    }
}