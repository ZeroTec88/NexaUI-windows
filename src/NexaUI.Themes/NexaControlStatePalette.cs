using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Maps control states to background, foreground, and border colors for a given theme.
/// </summary>
public sealed class NexaControlStatePalette
{
    private readonly Dictionary<NexaControlState, NexaStateColors> _states;

    public NexaControlStatePalette(IReadOnlyDictionary<NexaControlState, NexaStateColors> states)
    {
        _states = new Dictionary<NexaControlState, NexaStateColors>(states);
    }

    public NexaStateColors this[NexaControlState state] => _states.TryGetValue(state, out var value)
        ? value
        : (_states.TryGetValue(NexaControlState.Normal, out var normal) ? normal : NexaStateColors.Empty);

    public NexaStateColors Get(NexaControlState state) => this[state];
}

public readonly struct NexaStateColors
{
    public static NexaStateColors Empty { get; } = new(Color.Empty, Color.Empty, Color.Empty);

    public NexaStateColors(Color background, Color foreground, Color border)
    {
        Background = background;
        Foreground = foreground;
        Border = border;
    }

    public Color Background { get; }
    public Color Foreground { get; }
    public Color Border { get; }
}