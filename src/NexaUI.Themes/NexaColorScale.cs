using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Defines a tonal scale for a single hue. Used to express light/dark ramps
/// for status colors without leaking concrete Color instances.
/// </summary>
public sealed class NexaColorScale
{
    public NexaColorScale(NexaColorToken baseColor)
    {
        Base = baseColor;
        Soft = baseColor;
        Strong = baseColor;
        OnBase = NexaDpiHelper.OnColor(baseColor.Value);
    }

    public NexaColorScale(NexaColorToken baseColor, NexaColorToken soft, NexaColorToken strong, NexaColorToken onBase)
    {
        Base = baseColor;
        Soft = soft;
        Strong = strong;
        OnBase = onBase;
    }

    public NexaColorToken Base { get; }
    public NexaColorToken Soft { get; }
    public NexaColorToken Strong { get; }
    public NexaColorToken OnBase { get; }
}

internal static class NexaDpiHelper
{
    public static Color OnColor(Color color)
    {
        var luminance = (0.2126 * color.R + 0.7152 * color.G + 0.0722 * color.B) / 255.0;
        return luminance > 0.55 ? Color.FromArgb(20, 20, 24) : Color.White;
    }
}