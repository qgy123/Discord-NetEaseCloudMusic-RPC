using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace NetEaseDcNpPlugin
{
    public class Native
    {
        public static IntPtr GetNetEaseWindowText(int processId)
        {
            IntPtr mainWindowHandle = IntPtr.Zero;

            EnumWindows((hWnd, lParam) =>
            {
                GetWindowThreadProcessId(hWnd, out var pid);

                if (pid == lParam)
                {
                    StringBuilder str = new StringBuilder(256);
                    GetClassName(hWnd, str, 256);

                    if (str.ToString() == "OrpheusBrowserHost")
                    {
                        mainWindowHandle = hWnd;
                        return false;
                    }
                }

                return true;

            }, new IntPtr(processId));

            return mainWindowHandle;
        }

        internal const uint GW_OWNER = 4;

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern int GetWindowThreadProcessId(IntPtr hWnd, out IntPtr lpdwProcessId);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("User32.Dll")]
        private static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);

        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextW(IntPtr hwnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

    }
}
