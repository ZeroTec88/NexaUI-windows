namespace NexaUI.Core;

/// <summary>
/// State of a step in <see cref="NexaUI.Controls.NexaStepper"/>.
/// </summary>
public enum NexaStepState
{
    /// <summary>
    /// Step has not been started yet.
    /// </summary>
    Pending,

    /// <summary>
    /// Step is currently active.
    /// </summary>
    Current,

    /// <summary>
    /// Step has been completed successfully.
    /// </summary>
    Completed,

    /// <summary>
    /// Step has an error.
    /// </summary>
    Error,

    /// <summary>
    /// Step is disabled and cannot be interacted with.
    /// </summary>
    Disabled
}