using System;
using System.Runtime.InteropServices;

// ── P/Invoke ───────────────────────────────────────────────────────────────────

static class NativeMethods
{
    [DllImport("kernel32.dll")]
    public static extern bool FreeConsole();

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int value, int size);

    public static void SetDarkTitleBar(IntPtr hwnd)
    {
        int v = 1;
        DwmSetWindowAttribute(hwnd, 20, ref v, 4);
    }
}
