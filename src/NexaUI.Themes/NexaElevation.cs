using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Describes a single elevation level, expressed as a shadow color, blur radius,
/// and vertical offset. All values are in DIPs and DPI-scaled at use time.
/// </summary>
public sealed class NexaElevation
{
    public static NexaElevation Flat { get; } = new(Color.Transparent, 0, 0, 0);

    public NexaElevation(Color shadowColor, int blurRadiusInDips, int yOffsetInDips, int opacity)
    {
        ShadowColor = shadowColor;
        BlurRadiusInDips = blurRadiusInDips;
        YOffsetInDips = yOffsetInDips;
        Opacity = opacity;
    }

    public Color ShadowColor { get; }
    public int BlurRadiusInDips { get; }
    public int YOffsetInDips { get; }
    public int Opacity { get; }
}