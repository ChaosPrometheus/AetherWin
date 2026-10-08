using System.Collections.Generic;
using System.Linq;
using WindowManager.Models;

namespace WindowManager.Services
{
    public class LayoutService
    {
        public static LayoutService Instance { get; } = new();

        private LayoutService() { }

        public WindowLayout CaptureCurrentLayout(string name)
        {
            var windows = WindowService.Instance.GetAllWindows();

            var layout = new WindowLayout
            {
                Name = name,
                Windows = windows.Select(w => new WindowPosition
                {
                    Title = w.Title,
                    ProcessName = w.ProcessName,
                    X = w.X,
                    Y = w.Y,
                    Width = w.Width,
                    Height = w.Height,
                    IsMaximized = w.IsMaximized,
                    IsAlwaysOnTop = w.IsAlwaysOnTop,
                    Opacity = w.Opacity
                }).ToList()
            };

            return layout;
        }

        public void ApplyLayout(WindowLayout layout)
        {
            var currentWindows = WindowService.Instance.GetAllWindows();

            foreach (var saved in layout.Windows)
            {
                // Ищем окно по названию процесса + заголовку (сначала точное совпадение)
                var match = currentWindows.FirstOrDefault(w =>
                    w.ProcessName.Equals(saved.ProcessName, System.StringComparison.OrdinalIgnoreCase) &&
                    w.Title.Equals(saved.Title, System.StringComparison.OrdinalIgnoreCase));

                // Если не нашли — пробуем только по процессу
                match ??= currentWindows.FirstOrDefault(w =>
                    w.ProcessName.Equals(saved.ProcessName, System.StringComparison.OrdinalIgnoreCase));

                if (match == null)
                    continue;

                if (saved.IsMaximized)
                {
                    WindowService.Instance.MaximizeWindow(match.Handle);
                }
                else
                {
                    WindowService.Instance.RestoreWindow(match.Handle);
                    WindowService.Instance.MoveWindow(match.Handle, saved.X, saved.Y, saved.Width, saved.Height);
                }

                WindowService.Instance.SetAlwaysOnTop(match.Handle, saved.IsAlwaysOnTop);

                if (saved.Opacity < 100)
                    WindowService.Instance.SetOpacity(match.Handle, saved.Opacity);
            }
        }
    }
}
