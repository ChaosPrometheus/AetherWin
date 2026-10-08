using System.Windows;
using WindowManager.Services;

namespace WindowManager
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Инициализация сервисов
            SettingsService.Instance.Load();
            HotkeyService.Instance.RegisterHotkeys();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            HotkeyService.Instance.UnregisterHotkeys();
            SettingsService.Instance.Save();
            base.OnExit(e);
        }
    }
}
