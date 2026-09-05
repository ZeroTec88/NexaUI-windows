namespace NexaUI.Core;

/// <summary>
/// Utility helpers used across NexaUI to keep APIs simple and friendly from C# and VB.NET.
/// </summary>
public static class NexaGuard
{
    public static T NotNull<T>(T value, string paramName) where T : class
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
        return value;
    }
}