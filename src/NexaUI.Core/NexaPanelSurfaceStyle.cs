namespace NexaUI.Core;

/// <summary>
/// Visual style of a <see cref="NexaUI.Controls.NexaPanel"/> surface.
/// </summary>
public enum NexaPanelSurfaceStyle
{
    /// <summary>Uses the default theme background color.</summary>
    Default,
    /// <summary>Uses the theme surface color (subtle elevation).</summary>
    Surface,
    /// <summary>Uses the theme surface variant color (clearer elevation).</summary>
    Elevated,
    /// <summary>Transparent surface; only border/shape is drawn.</summary>
    Transparent
}

/// <summary>
/// Border style for a <see cref="NexaUI.Controls.NexaPanel"/>.
/// </summary>
public enum NexaBorderStyleEx
{
    /// <summary>No border is drawn.</summary>
    None,
    /// <summary>A solid border is drawn.</summary>
    Solid
}
