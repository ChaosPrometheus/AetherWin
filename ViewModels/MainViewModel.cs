using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using WindowManager.Models;
using WindowManager.Services;

namespace WindowManager.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DispatcherTimer _refreshTimer;

        [ObservableProperty]
        private ObservableCollection<WindowInfo> _windows = new();

        [ObservableProperty]
        private WindowInfo? _selectedWindow;

        [ObservableProperty]
        private ObservableCollection<WindowLayout> _layouts = new();

        [ObservableProperty]
        private WindowLayout? _selectedLayout;

        [ObservableProperty]
        private string _newLayoutName = string.Empty;

        [ObservableProperty]
        private int _opacity = 100;

        [ObservableProperty]
        private string _statusText = "Готово";

        public MainViewModel()
        {
            RefreshWindows();
            LoadLayouts();

            _refreshTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(2)
            };
            _refreshTimer.Tick += (s, e) => RefreshWindows();
            _refreshTimer.Start();

            // Подписка на горячие клавиши
            HotkeyService.Instance.ToggleTopmostRequested += OnToggleTopmostHotkey;
            HotkeyService.Instance.OpacityUpRequested += OnOpacityUpHotkey;
            HotkeyService.Instance.OpacityDownRequested += OnOpacityDownHotkey;
        }

        [RelayCommand]
        private void RefreshWindows()
        {
            var current = WindowService.Instance.GetAllWindows();
            var selectedHandle = SelectedWindow?.Handle;

            Windows.Clear();
            foreach (var w in current)
                Windows.Add(w);

            if (selectedHandle != null)
                SelectedWindow = Windows.FirstOrDefault(w => w.Handle == selectedHandle);

            StatusText = $"Окон: {Windows.Count}";
        }

        [RelayCommand]
        private void ToggleAlwaysOnTop()
        {
            if (SelectedWindow == null) return;

            bool newState = !SelectedWindow.IsAlwaysOnTop;
            WindowService.Instance.SetAlwaysOnTop(SelectedWindow.Handle, newState);
            SelectedWindow.IsAlwaysOnTop = newState;

            StatusText = newState
                ? $"«{SelectedWindow.DisplayTitle}» — поверх всех окон"
                : $"«{SelectedWindow.DisplayTitle}» — обычный режим";
        }

        [RelayCommand]
        private void ApplyOpacity()
        {
            if (SelectedWindow == null) return;

            WindowService.Instance.SetOpacity(SelectedWindow.Handle, Opacity);
            SelectedWindow.Opacity = Opacity;
            StatusText = $"Прозрачность «{SelectedWindow.DisplayTitle}»: {Opacity}%";
        }

        [RelayCommand]
        private void ActivateWindow()
        {
            if (SelectedWindow == null) return;
            WindowService.Instance.ActivateWindow(SelectedWindow.Handle);
        }

        [RelayCommand]
        private void MinimizeWindow()
        {
            if (SelectedWindow == null) return;
            WindowService.Instance.MinimizeWindow(SelectedWindow.Handle);
            RefreshWindows();
        }

        [RelayCommand]
        private void MaximizeWindow()
        {
            if (SelectedWindow == null) return;
            WindowService.Instance.MaximizeWindow(SelectedWindow.Handle);
            RefreshWindows();
        }

        [RelayCommand]
        private void RestoreWindow()
        {
            if (SelectedWindow == null) return;
            WindowService.Instance.RestoreWindow(SelectedWindow.Handle);
            RefreshWindows();
        }

        [RelayCommand]
        private void SaveLayout()
        {
            if (string.IsNullOrWhiteSpace(NewLayoutName))
            {
                StatusText = "Введите название раскладки";
                return;
            }

            var layout = LayoutService.Instance.CaptureCurrentLayout(NewLayoutName.Trim());
            SettingsService.Instance.AddLayout(layout);
            LoadLayouts();
            NewLayoutName = string.Empty;
            StatusText = $"Раскладка «{layout.Name}» сохранена ({layout.Windows.Count} окон)";
        }

        [RelayCommand]
        private void ApplyLayout()
        {
            if (SelectedLayout == null) return;

            LayoutService.Instance.ApplyLayout(SelectedLayout);
            RefreshWindows();
            StatusText = $"Раскладка «{SelectedLayout.Name}» применена";
        }

        [RelayCommand]
        private void DeleteLayout()
        {
            if (SelectedLayout == null) return;

            var name = SelectedLayout.Name;
            SettingsService.Instance.RemoveLayout(name);
            LoadLayouts();
            StatusText = $"Раскладка «{name}» удалена";
        }

        private void LoadLayouts()
        {
            Layouts.Clear();
            foreach (var layout in SettingsService.Instance.Settings.SavedLayouts)
                Layouts.Add(layout);
        }

        private void OnToggleTopmostHotkey()
        {
            var hwnd = WindowService.Instance.GetForegroundWindow();
            var info = WindowService.Instance.GetWindowInfo(hwnd);
            if (info == null) return;

            bool newState = !info.IsAlwaysOnTop;
            WindowService.Instance.SetAlwaysOnTop(hwnd, newState);
            StatusText = newState ? "Always on Top включён" : "Always on Top выключен";
        }

        private void OnOpacityUpHotkey()
        {
            ChangeForegroundOpacity(10);
        }

        private void OnOpacityDownHotkey()
        {
            ChangeForegroundOpacity(-10);
        }

        private void ChangeForegroundOpacity(int delta)
        {
            var hwnd = WindowService.Instance.GetForegroundWindow();
            var info = WindowService.Instance.GetWindowInfo(hwnd);
            if (info == null) return;

            int newOpacity = Math.Clamp(info.Opacity + delta, 20, 100);
            WindowService.Instance.SetOpacity(hwnd, newOpacity);
            info.Opacity = newOpacity;
            StatusText = $"Прозрачность: {newOpacity}%";
        }
    }
}
