namespace NexaUI.Core;

/// <summary>
/// Visual variant for input controls. Inspired by common form-control designs:
/// <list type="bullet">
///   <item><c>Outline</c> — bordered with rounded corners and a white/transparent background. Default.</item>
///   <item><c>Filled</c> — solid background fill, subtle border, no glow on focus.</item>
///   <item><c>Underline</c> — bottom-border only, like Material Design text fields.</item>
/// </list>
/// </summary>
public enum NexaInputStyle
{
    Outline,
    Filled,
    Underline
}
