using System.Drawing;

namespace NexaUI.Icons;

/// <summary>
/// Resolves a <see cref="NexaIconKind"/> into an <see cref="INexaIconSource"/>.
/// Consumers can replace the default provider with their own implementation
/// without changing call sites.
/// </summary>
public static class NexaIconProvider
{
    public static INexaIconSource Resolve(NexaIconKind kind) => new NexaGlyphIcon(kind);

    public static Bitmap? ToBitmap(NexaIconKind kind, Size size, Color tint) =>
        Resolve(kind).ToBitmap(size, tint);
}