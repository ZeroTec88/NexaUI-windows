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
        ThemeChanged?.Invoke(null, new ThemeChangedEventArgs(previous, theme));
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