using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Defines the surface of a single semantic color, including its normal value and
/// optional accent/contrast companion values used for hover/pressed/selected states.
/// </summary>
public sealed class NexaSemanticColor
{
    public NexaSemanticColor(NexaColorToken value)
    {
        Value = value;
        Accent = value;
        Contrast = ToAutoContrast(value.Value);
        Subtle = value;
    }

    public NexaSemanticColor(NexaColorToken value, NexaColorToken accent, NexaColorToken contrast, NexaColorToken subtle)
    {
        Value = value;
        Accent = accent;
        Contrast = contrast;
        Subtle = subtle;
    }

    public NexaColorToken Value { get; }
    public NexaColorToken Accent { get; }
    public NexaColorToken Contrast { get; }
    public NexaColorToken Subtle { get; }

    private static NexaColorToken ToAutoContrast(Color value)
    {
        var luminance = (0.2126 * value.R + 0.7152 * value.G + 0.0722 * value.B) / 255.0;
        return luminance > 0.55 ? Color.FromArgb(20, 20, 24) : Color.White;
    }
}