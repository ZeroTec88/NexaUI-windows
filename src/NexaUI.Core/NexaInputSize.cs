namespace NexaUI.Core;

/// <summary>
/// Visual size of input controls. Drives the editor height, vertical padding, and corner radius.
/// </summary>
public enum NexaInputSize
{
    /// <summary>Compact input — ~32px tall. Useful for dense forms and toolbars.</summary>
    Small,
    /// <summary>Default input — ~40px tall. Matches Bootstrap form-control-sm/lg defaults to Medium.</summary>
    Medium,
    /// <summary>Large input — ~48px tall. Useful for hero forms and emphasis.</summary>
    Large
}
