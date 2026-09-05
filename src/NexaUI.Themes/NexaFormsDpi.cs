using System.Drawing;
using NexaUI.Core;

namespace NexaUI.Themes;

/// <summary>
/// WinForms-aware DPI helpers used by themed controls and demo screens.
/// </summary>
public static class NexaFormsDpi
{
    public static float CurrentScaleFactor(IntPtr handle)
    {
        if (handle == IntPtr.Zero) return 1F;
        using var graphics = Graphics.FromHwnd(handle);
        return NexaDpi.ScaleFactor(graphics.DpiY);
    }

    public static float CurrentScaleFactor(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);
        if (!control.IsHandleCreated) return 1F;
        return CurrentScaleFactor(control.Handle);
    }

    public static float CurrentDpi(Control control)
    {
        ArgumentNullException.ThrowIfNull(control);
        if (!control.IsHandleCreated) return NexaDpi.BaseDpi;
        using var graphics = Graphics.FromHwnd(control.Handle);
        return graphics.DpiY;
    }

    public static Size Scale(Size size, Control control) =>
        new(NexaDpi.Scale(size.Width, CurrentDpi(control)), NexaDpi.Scale(size.Height, CurrentDpi(control)));

    public static int Scale(int dips, Control control) => NexaDpi.Scale(dips, CurrentDpi(control));
}