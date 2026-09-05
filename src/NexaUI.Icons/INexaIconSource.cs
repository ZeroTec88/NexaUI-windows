using System.Drawing;

namespace NexaUI.Icons;

/// <summary>
/// Abstraction over any icon source. Implementations decide how to materialize the
/// icon (glyphs, vector paths, raster images, etc.).
/// </summary>
public interface INexaIconSource
{
    NexaIconKind Kind { get; }

    /// <summary>Returns a size-aware bitmap representation of the icon, or null if not supported.</summary>
    Bitmap? ToBitmap(Size size, Color tint);
}