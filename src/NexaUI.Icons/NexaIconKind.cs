using System.Drawing;

namespace NexaUI.Icons;

/// <summary>
/// The minimal set of icon identifiers exposed by NexaUI's built-in icon system.
/// Consumers may extend this set with their own implementations of <see cref="INexaIconSource"/>.
/// </summary>
public enum NexaIconKind
{
    None,
    Check,
    Cross,
    ChevronDown,
    ChevronRight,
    Search,
    Settings,
    User,
    Info,
    Warning,
    Error,
    Sun,
    Moon
}