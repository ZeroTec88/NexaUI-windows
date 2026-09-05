using System.Drawing;

namespace NexaUI.Core;

/// <summary>
/// Provides a strongly typed color token for the NexaUI design system.
/// </summary>
public readonly record struct NexaColorToken(Color Value)
{
    public static implicit operator Color(NexaColorToken token) => token.Value;
    public static implicit operator NexaColorToken(Color color) => new(color);

    public override string ToString() => Value.IsNamedColor
        ? Value.Name
        : $"#{Value.R:X2}{Value.G:X2}{Value.B:X2}";
}