using System.Collections.Generic;

namespace WindowManager.Models
{
    public class WindowLayout
    {
        public string Name { get; set; } = string.Empty;
        public List<WindowPosition> Windows { get; set; } = new();
    }

    public class WindowPosition
    {
        public string Title { get; set; } = string.Empty;
        public string ProcessName { get; set; } = string.Empty;
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsMaximized { get; set; }
        public bool IsAlwaysOnTop { get; set; }
        public int Opacity { get; set; } = 100;
    }
}
