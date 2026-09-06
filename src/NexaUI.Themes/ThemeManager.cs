using System.Threading;

namespace NexaUI.Themes;

/// <summary>
/// Centralized access to the currently active <see cref="ITheme"/>.
/// Consumers can swap themes at runtime and subscribe to <see cref="ThemeChanged"/>.
/// </summary>
public static class ThemeManager
{
    private static ITheme _current = new LightTheme();
    private static readonly object _lock = new();

    /// <summary>Raised whenever the active theme is replaced via <see cref="SetTheme"/>.</summary>
    public static event System.EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    /// <summary>The currently active theme. Never null. Defaults to <see cref="LightTheme"/>.</summary>
    public static ITheme Current
    {
        get
        {
            lock (_lock)
            {
                return _current;
            }
        }
    }

    /// <summary>Replaces the active theme and raises <see cref="ThemeChanged"/>.</summary>
    public static void SetTheme(ITheme theme)
    {
        ArgumentNullException.ThrowIfNull(theme);
        ITheme previous;
        lock (_lock)
        {
            if (ReferenceEquals(_current, theme)) return;
            previous = _current;
            _current = theme;
        }
        // Snapshot the invocation list so handlers that unsubscribe during
        // invocation do not corrupt the multicast delegate.
        var handler = ThemeChanged;
        if (handler is not null)
        {
            foreach (var d in handler.GetInvocationList())
            {
                try
                {
                    ((System.EventHandler<ThemeChangedEventArgs>)d)!.Invoke(null, new ThemeChangedEventArgs(previous, theme));
                }
                catch
                {
                    // Swallow exceptions from individual handlers so one bad subscriber
                    // does not prevent others from running.
                }
            }
        }
    }

    /// <summary>Toggles between the built-in light and dark themes.</summary>
    public static void ToggleLightDark()
    {
        SetTheme(Current.IsDark ? (ITheme)new LightTheme() : new DarkTheme());
    }
}

public sealed class ThemeChangedEventArgs : System.EventArgs
{
    public ThemeChangedEventArgs(ITheme previous, ITheme current)
    {
        Previous = previous;
        Current = current;
    }

    public ITheme Previous { get; }
    public ITheme Current { get; }
}