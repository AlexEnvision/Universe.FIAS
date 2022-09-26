using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Universe.Fias.Normalizer.ConsoleApp.Senders
{
    public static class WindowSender
    {
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, uint pvParam, uint fWinIni);

        [DllImport("user32.dll", EntryPoint = "SetForegroundWindow")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "ShowWindowAsync")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);

        private const int WS_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;
        private const int SW_SHOWMAXIMIZED = 3;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowPlacement(IntPtr hWnd, ref WINDOWPLACEMENT lpwndpl);

        public static void SetForegroundWindowByName(string processName)
        {
            Process[] processes = Process.GetProcesses();
            foreach (Process p in processes)
            {
                if (p.MainWindowTitle.Contains(processName))
                {
                    WINDOWPLACEMENT wp = new WINDOWPLACEMENT();

                    wp.length = Marshal.SizeOf(wp);
                    GetWindowPlacement(p.MainWindowHandle, ref wp);

                    int proposedPlacement = wp.showCmd;
                    //if (wp.showCmd == SW_SHOWMINIMIZED)
                    //    proposedPlacement = SW_SHOWMAXIMIZED;

                    SystemParametersInfo((uint)0x2001, 0, 0, 0x0002 | 0x0001);
                    ShowWindowAsync(p.MainWindowHandle, proposedPlacement);
                    SetForegroundWindow(p.MainWindowHandle);
                    SystemParametersInfo((uint)0x2001, 200000, 200000, 0x0002 | 0x0001);
                }
            }
        }
    }
}