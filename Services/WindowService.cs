using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WindowManager.Helpers;
using WindowManager.Models;

namespace WindowManager.Services
{
    public class WindowService
    {
        public static WindowService Instance { get; } = new();

        private WindowService() { }

        public List<WindowInfo> GetAllWindows()
        {
            var windows = new List<WindowInfo>();

            NativeMethods.EnumWindows((hWnd, lParam) =>
            {
                if (!NativeMethods.IsWindowVisible(hWnd))
                    return true;

                int length = NativeMethods.GetWindowTextLength(hWnd);
                if (length == 0)
                    return true;

                var sb = new StringBuilder(length + 1);
                NativeMethods.GetWindowText(hWnd, sb, sb.Capacity);
                string title = sb.ToString();

                if (string.IsNullOrWhiteSpace(title))
                    return true;

                NativeMethods.GetWindowThreadProcessId(hWnd, out uint pid);
                string processName = "Unknown";

                try
                {
                    var process = Process.GetProcessById((int)pid);
                    processName = process.ProcessName;
                }
                catch { }

                if (processName.Equals("WindowManager", StringComparison.OrdinalIgnoreCase) ||
                    processName.Equals("AetherWin", StringComparison.OrdinalIgnoreCase) ||
                    processName.Equals("ApplicationFrameHost", StringComparison.OrdinalIgnoreCase))
                    return true;

                NativeMethods.GetWindowRect(hWnd, out var rect);

                int exStyle = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE);
                bool isTopMost = (exStyle & NativeMethods.WS_EX_TOPMOST) != 0;

                windows.Add(new WindowInfo
                {
                    Handle = hWnd,
                    Title = title,
                    ProcessName = processName,
                    ProcessId = (int)pid,
                    IsVisible = true,
                    IsMinimized = NativeMethods.IsIconic(hWnd),
                    IsMaximized = NativeMethods.IsZoomed(hWnd),
                    IsAlwaysOnTop = isTopMost,
                    X = rect.Left,
                    Y = rect.Top,
                    Width = rect.Right - rect.Left,
                    Height = rect.Bottom - rect.Top,
                    Opacity = 100
                });

                return true;
            }, IntPtr.Zero);

            return windows.OrderBy(w => w.Title).ToList();
        }

        public void SetAlwaysOnTop(IntPtr hWnd, bool enable)
        {
            IntPtr insertAfter = enable
                ? new IntPtr(NativeMethods.HWND_TOPMOST)
                : new IntPtr(NativeMethods.HWND_NOTOPMOST);

            NativeMethods.SetWindowPos(
                hWnd,
                insertAfter,
                0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOACTIVATE);
        }

        public void SetOpacity(IntPtr hWnd, int opacityPercent)
        {
            opacityPercent = Math.Clamp(opacityPercent, 10, 100);
            byte alpha = (byte)(opacityPercent * 255 / 100);

            int exStyle = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE);
            NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE, exStyle | NativeMethods.WS_EX_LAYERED);

            NativeMethods.SetLayeredWindowAttributes(hWnd, 0, alpha, NativeMethods.LWA_ALPHA);
        }

        public void SetClickThrough(IntPtr hWnd, bool enable)
        {
            int exStyle = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE);

            if (enable)
                exStyle |= NativeMethods.WS_EX_TRANSPARENT | NativeMethods.WS_EX_LAYERED;
            else
                exStyle &= ~NativeMethods.WS_EX_TRANSPARENT;

            NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_EXSTYLE, exStyle);
        }

        public void MoveWindow(IntPtr hWnd, int x, int y, int width, int height)
        {
            NativeMethods.SetWindowPos(
                hWnd,
                IntPtr.Zero,
                x, y, width, height,
                NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_SHOWWINDOW);
        }

        /// <summary>
        /// Блокирует или разблокирует изменение размера окна
        /// </summary>
        public void LockWindowSize(IntPtr hWnd, bool lockSize)
        {
            int style = NativeMethods.GetWindowLong(hWnd, NativeMethods.GWL_STYLE);

            if (lockSize)
            {
                // Убираем возможность менять размер и разворачивать
                style &= ~NativeMethods.WS_THICKFRAME;
                style &= ~NativeMethods.WS_MAXIMIZEBOX;
            }
            else
            {
                // Возвращаем возможность менять размер
                style |= NativeMethods.WS_THICKFRAME;
                style |= NativeMethods.WS_MAXIMIZEBOX;
            }

            NativeMethods.SetWindowLong(hWnd, NativeMethods.GWL_STYLE, style);

            // Применяем изменения рамки
            NativeMethods.SetWindowPos(
                hWnd,
                IntPtr.Zero,
                0, 0, 0, 0,
                NativeMethods.SWP_NOMOVE |
                NativeMethods.SWP_NOSIZE |
                NativeMethods.SWP_NOZORDER |
                NativeMethods.SWP_FRAMECHANGED);
        }

        public void RestoreWindow(IntPtr hWnd)
        {
            NativeMethods.ShowWindow(hWnd, NativeMethods.SW_RESTORE);
        }

        public void MaximizeWindow(IntPtr hWnd)
        {
            NativeMethods.ShowWindow(hWnd, NativeMethods.SW_MAXIMIZE);
        }

        public void MinimizeWindow(IntPtr hWnd)
        {
            NativeMethods.ShowWindow(hWnd, NativeMethods.SW_MINIMIZE);
        }

        public void ActivateWindow(IntPtr hWnd)
        {
            if (NativeMethods.IsIconic(hWnd))
                NativeMethods.ShowWindow(hWnd, NativeMethods.SW_RESTORE);

            NativeMethods.SetForegroundWindow(hWnd);
        }

        public IntPtr GetForegroundWindow()
        {
            return NativeMethods.GetForegroundWindow();
        }

        public WindowInfo? GetWindowInfo(IntPtr hWnd)
        {
            return GetAllWindows().FirstOrDefault(w => w.Handle == hWnd);
        }
    }
}