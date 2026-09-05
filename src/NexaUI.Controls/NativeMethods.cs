using System.Runtime.InteropServices;

namespace NexaUI.Controls;

/// <summary>
/// Internal Win32 helpers used by NexaUI input controls.
/// </summary>
internal static class NativeMethods
{
    public const int EM_SETCUEBANNER = 0x1501;

    [DllImport("user32.dll", CharSet = CharSet.Unicode, EntryPoint = "SendMessageW")]
    private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, string lParam);

    public static void SetCueBanner(IntPtr handle, string text)
    {
        if (handle == IntPtr.Zero) return;
        SendMessage(handle, EM_SETCUEBANNER, IntPtr.Zero, text ?? string.Empty);
    }
}