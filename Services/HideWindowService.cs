using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;

namespace PocketLLM.Services
{
    public class HideWindowService
    {
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, uint dwNewLong);

        [DllImport("user32.dll")]
        private static extern uint GetWindowLong(IntPtr hWnd, int nIndex);

        private const int GWL_EXSTYLE = -20;
        private const uint WS_EX_TOOLWINDOW = 0x00000080;
        private const uint WS_EX_APPWINDOW = 0x00040000;

        public static void HideFromAltTab(Window window)
        {
            var hwnd = new WindowInteropHelper(window).Handle;
            var exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            // Убираем WS_EX_APPWINDOW, добавляем WS_EX_TOOLWINDOW
            SetWindowLong(hwnd, GWL_EXSTYLE, (exStyle & ~WS_EX_APPWINDOW) | WS_EX_TOOLWINDOW);
        }
    }
}
