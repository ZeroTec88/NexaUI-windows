namespace NexaUI.Core;

/// <summary>
/// Visual style variants for overlays (loading, modal background, etc.).
/// </summary>
public enum NexaOverlayStyle
{
    /// <summary>Semi-transparent dark overlay with centered spinner.</summary>
    Standard,
    /// <summary>Light overlay with subtle spinner.</summary>
    Light,
    /// <summary>Blur effect overlay (when supported).</summary>
    Blur,
    /// <summary>Minimal overlay - just a spinner with no backdrop.</summary>
    Minimal
}