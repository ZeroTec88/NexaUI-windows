using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// Centralized metrics for borders, corner radii, and elevation. All numeric values
/// are in DIPs and DPI-scaled at consumption time.
/// </summary>
public sealed class NexaMetrics
{
    private readonly Dictionary<NexaCornerRadiusToken, int> _radii;
    private readonly Dictionary<NexaElevationLevel, NexaElevation> _elevations;

    public NexaMetrics(
        NexaSpacing spacing,
        int borderThicknessInDips,
        int focusRingThicknessInDips,
        IReadOnlyDictionary<NexaCornerRadiusToken, int> cornerRadiiInDips,
        IReadOnlyDictionary<NexaElevationLevel, NexaElevation> elevations)
    {
        Spacing = spacing;
        BorderThicknessInDips = borderThicknessInDips;
        FocusRingThicknessInDips = focusRingThicknessInDips;
        _radii = new Dictionary<NexaCornerRadiusToken, int>(cornerRadiiInDips);
        _elevations = new Dictionary<NexaElevationLevel, NexaElevation>(elevations);
    }

    public NexaSpacing Spacing { get; }
    public int BorderThicknessInDips { get; }
    public int FocusRingThicknessInDips { get; }

    public int CornerRadiusInDips(NexaCornerRadiusToken token) => _radii.TryGetValue(token, out var value) ? value : 0;

    public int ToCornerRadius(NexaCornerRadiusToken token, float dpi) =>
        NexaDpi.Scale(CornerRadiusInDips(token), dpi);

    public NexaElevation Elevation(NexaElevationLevel level) =>
        _elevations.TryGetValue(level, out var elevation) ? elevation : NexaElevation.Flat;

    public int ToBorderThickness(float dpi) => NexaDpi.Scale(BorderThicknessInDips, dpi);
    public int ToFocusRingThickness(float dpi) => NexaDpi.Scale(FocusRingThicknessInDips, dpi);
}