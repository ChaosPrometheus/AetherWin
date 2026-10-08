using System;

namespace WindowManager.Models
{
    public class WindowInfo
    {
        public IntPtr Handle { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int ProcessId { get; set; }
        public bool IsVisible { get; set; }
        public bool IsMinimized { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsAlwaysOnTop { get; set; }
        public int Opacity { get; set; } = 100; // 0-100
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string DisplayTitle => string.IsNullOrWhiteSpace(Title) ? $"[{ProcessName}]" : Title;

        public override string ToString() => DisplayTitle;
    }
}
