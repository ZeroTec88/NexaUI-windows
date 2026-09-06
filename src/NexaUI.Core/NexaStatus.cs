namespace NexaUI.Core;

/// <summary>
/// Semantic status of a <see cref="NexaUI.Controls.NexaStatusIndicator"/>.
/// </summary>
public enum NexaStatus
{
    /// <summary>No status set.</summary>
    None,
    /// <summary>Online / healthy.</summary>
    Online,
    /// <summary>Offline / disconnected.</summary>
    Offline,
    /// <summary>Busy / processing.</summary>
    Busy,
    /// <summary>Warning state.</summary>
    Warning,
    /// <summary>Success state.</summary>
    Success,
    /// <summary>Error state.</summary>
    Error
}
