namespace NexaUI.Core;

/// <summary>
/// Result of a dialog interaction.
/// </summary>
public enum NexaDialogResult
{
    /// <summary>No result / dialog was closed without a button click.</summary>
    None,
    /// <summary>OK button was clicked.</summary>
    OK,
    /// <summary>Cancel button was clicked.</summary>
    Cancel,
    /// <summary>Yes button was clicked.</summary>
    Yes,
    /// <summary>No button was clicked.</summary>
    No,
    /// <summary>Retry button was clicked.</summary>
    Retry,
    /// <summary>Abort button was clicked.</summary>
    Abort,
    /// <summary>Ignore button was clicked.</summary>
    Ignore,
    /// <summary>Close button was clicked.</summary>
    Close
}