using System;
using System.Runtime.InteropServices;
using Universe.Fias.Normalizer.ConsoleApp.Types;

namespace Universe.Fias.Normalizer.ConsoleApp.Senders
{
    public static class KeyboardSender
    {
        /// <summary>
        ///     Функция WinAPI для поиска хендла активного в текущий момент окна
        /// </summary>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        /// <summary>
        ///     Переключение фокуса окна
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        ///     Управление клавиатурой.
        ///     Передача нажатия клавиши.
        /// </summary>
        /// <param name="bVk"></param>
        /// <param name="bScan"></param>
        /// <param name="dwFlags"></param>
        /// <param name="dwExtraInfo"></param>
        [DllImport("user32.dll")]
        public static extern void keybd_event(Keys bVk, byte bScan, UInt32 dwFlags, IntPtr dwExtraInfo);

        public const UInt32 KEYEVENTF_EXTENDEDKEY = 1;
        public const uint KEYEVENTF_KEYUP = 2;
    }
}