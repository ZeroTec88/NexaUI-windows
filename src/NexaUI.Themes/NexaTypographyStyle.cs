using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Describes a single typography role: font family, size in DIPs, weight, line height in DIPs,
/// and additional style flags. Sizes are DPI-scaled at consumption time.
/// </summary>
public sealed class NexaTypographyStyle
{
    public NexaTypographyStyle(
        string familyName,
        float sizeInDips,
        FontStyle style,
        float lineHeightInDips,
        float letterSpacingInDips)
    {
        FamilyName = familyName;
        SizeInDips = sizeInDips;
        Style = style;
        LineHeightInDips = lineHeightInDips;
        LetterSpacingInDips = letterSpacingInDips;
    }

    public string FamilyName { get; }
    public float SizeInDips { get; }
    public FontStyle Style { get; }
    public float LineHeightInDips { get; }
    public float LetterSpacingInDips { get; }

    public Font ToFont(float dpi)
    {
        var size = NexaDpi.ScaleF(SizeInDips, dpi);
        if (size <= 0F) size = 8F;
        return new Font(FamilyName, size, Style, GraphicsUnit.Point);
    }

    public int ToLineHeight(float dpi) => Math.Max(1, NexaDpi.Scale((int)LineHeightInDips, dpi));
}