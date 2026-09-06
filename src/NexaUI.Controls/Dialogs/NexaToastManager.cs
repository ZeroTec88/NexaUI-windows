using System.Collections.Generic;
using System.Windows.Forms;
using NexaUI.Core;

namespace NexaUI.Controls;

/// <summary>
/// Manages multiple toast notifications, stacking them appropriately.
/// </summary>
public sealed class NexaToastManager
{
    private readonly List<NexaToast> _activeToasts = new();
    private readonly IWin32Window? _owner;
    private readonly NexaToastPosition _defaultPosition;
    private readonly int _defaultAutoCloseDelayMs;

    /// <summary>Initializes a new instance of the <see cref="NexaToastManager"/> class.</summary>
    /// <param name="owner">Owner window for positioning toasts.</param>
    /// <param name="defaultPosition">Default toast position.</param>
    /// <param name="defaultAutoCloseDelayMs">Default auto-close delay in milliseconds.</param>
    public NexaToastManager(IWin32Window? owner = null, NexaToastPosition defaultPosition = NexaToastPosition.TopRight, int defaultAutoCloseDelayMs = 5000)
    {
        _owner = owner;
        _defaultPosition = defaultPosition;
        _defaultAutoCloseDelayMs = defaultAutoCloseDelayMs;
    }

    /// <summary>Shows a toast notification.</summary>
    public NexaToast Show(string message, string title = "", NexaToastStyle style = NexaToastStyle.Default, NexaToastPosition? position = null, int? autoCloseDelayMs = null)
    {
        var toast = new NexaToast
        {
            Message = message,
            Title = title,
            Style = style
        };

        var pos = position ?? _defaultPosition;
        var delay = autoCloseDelayMs ?? _defaultAutoCloseDelayMs;
        toast.Show(_owner, pos, delay);

        _activeToasts.Add(toast);
        toast.FormClosed += (_, _) => _activeToasts.Remove(toast);

        RepositionToasts();
        return toast;
    }

    /// <summary>Shows a success toast.</summary>
    public NexaToast ShowSuccess(string message, string title = "", NexaToastPosition? position = null, int? autoCloseDelayMs = null)
    {
        return Show(message, title, NexaToastStyle.Success, position, autoCloseDelayMs);
    }

    /// <summary>Shows a warning toast.</summary>
    public NexaToast ShowWarning(string message, string title = "", NexaToastPosition? position = null, int? autoCloseDelayMs = null)
    {
        return Show(message, title, NexaToastStyle.Warning, position, autoCloseDelayMs);
    }

    /// <summary>Shows an error toast.</summary>
    public NexaToast ShowError(string message, string title = "", NexaToastPosition? position = null, int? autoCloseDelayMs = null)
    {
        return Show(message, title, NexaToastStyle.Error, position, autoCloseDelayMs);
    }

    /// <summary>Shows an info toast.</summary>
    public NexaToast ShowInfo(string message, string title = "", NexaToastPosition? position = null, int? autoCloseDelayMs = null)
    {
        return Show(message, title, NexaToastStyle.Info, position, autoCloseDelayMs);
    }

    /// <summary>Closes all active toasts.</summary>
    public void CloseAll()
    {
        foreach (var toast in _activeToasts.ToArray())
        {
            toast.CloseToast();
        }
    }

    private void RepositionToasts()
    {
        if (_owner == null) return;

        var screen = Screen.FromControl((Control)_owner);
        var wa = screen.WorkingArea;
        var gap = 8;

        // Stack from top-right by default
        var x = wa.Right;
        var y = wa.Top + 16;

        foreach (var toast in _activeToasts)
        {
            toast.Location = new Point(wa.Right - toast.Width - 16, y);
            y += toast.Height + gap;
        }
    }
}