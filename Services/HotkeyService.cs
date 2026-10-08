using System;
using System.Windows;
using System.Windows.Input;
using NHotkey;
using NHotkey.Wpf;
using WindowManager.Models;

namespace WindowManager.Services
{
    public class HotkeyService
    {
        public static HotkeyService Instance { get; } = new();

        public event Action? ToggleTopmostRequested;
        public event Action? OpacityUpRequested;
        public event Action? OpacityDownRequested;
        public event Action? ShowMainWindowRequested;

        private HotkeyService() { }

        public void RegisterHotkeys()
        {
            try
            {
                var settings = SettingsService.Instance.Settings;

                Register(settings.HotkeyToggleTopmost, "ToggleTopmost", (s, e) => ToggleTopmostRequested?.Invoke());
                Register(settings.HotkeyOpacityUp, "OpacityUp", (s, e) => OpacityUpRequested?.Invoke());
                Register(settings.HotkeyOpacityDown, "OpacityDown", (s, e) => OpacityDownRequested?.Invoke());
                Register(settings.HotkeyShowMain, "ShowMain", (s, e) => ShowMainWindowRequested?.Invoke());
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Не удалось зарегистрировать горячие клавиши:\n{ex.Message}",
                    "Window Manager", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }

        public void UnregisterHotkeys()
        {
            try
            {
                HotkeyManager.Current.Remove("ToggleTopmost");
                HotkeyManager.Current.Remove("OpacityUp");
                HotkeyManager.Current.Remove("OpacityDown");
                HotkeyManager.Current.Remove("ShowMain");
            }
            catch { }
        }

        private void Register(string gesture, string name, EventHandler<HotkeyEventArgs> handler)
        {
            if (string.IsNullOrWhiteSpace(gesture))
                return;

            try
            {
                var keyGesture = (KeyGesture)new KeyGestureConverter().ConvertFromString(gesture)!;
                HotkeyManager.Current.AddOrReplace(name, keyGesture, handler);
            }
            catch
            {
                // Некорректная комбинация — пропускаем
            }
        }
    }
}
