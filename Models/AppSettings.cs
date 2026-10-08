using System.Collections.Generic;

namespace WindowManager.Models
{
    public class AppSettings
    {
        public bool StartWithWindows { get; set; } = false;
        public bool StartMinimized { get; set; } = true;
        public bool ShowInTaskbar { get; set; } = false;
        public string HotkeyToggleTopmost { get; set; } = "Ctrl+Shift+T";
        public string HotkeyOpacityDown { get; set; } = "Ctrl+Shift+Down";
        public string HotkeyOpacityUp { get; set; } = "Ctrl+Shift+Up";
        public string HotkeyShowMain { get; set; } = "Ctrl+Shift+W";
        public List<WindowLayout> SavedLayouts { get; set; } = new();
        public double WindowLeft { get; set; } = 100;
        public double WindowTop { get; set; } = 100;
        public double WindowWidth { get; set; } = 900;
        public double WindowHeight { get; set; } = 600;
    }
}
