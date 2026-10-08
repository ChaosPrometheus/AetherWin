using System;
using System.Windows;
using WindowManager.Services;
using WindowManager.ViewModels;

namespace WindowManager.Views
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            // Восстанавливаем позицию окна
            var settings = SettingsService.Instance.Settings;
            Left = settings.WindowLeft;
            Top = settings.WindowTop;
            Width = settings.WindowWidth;
            Height = settings.WindowHeight;

            // Обработка горячей клавиши показа окна
            HotkeyService.Instance.ShowMainWindowRequested += ShowFromTray;

            // Сворачиваем в трей при закрытии
            Closing += MainWindow_Closing;
            StateChanged += MainWindow_StateChanged;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            // Сохраняем позицию
            var settings = SettingsService.Instance.Settings;
            settings.WindowLeft = Left;
            settings.WindowTop = Top;
            settings.WindowWidth = Width;
            settings.WindowHeight = Height;
            SettingsService.Instance.Save();

            // Вместо закрытия — прячем в трей
            e.Cancel = true;
            Hide();
        }

        private void MainWindow_StateChanged(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }
        }

        private void ShowFromTray()
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        }

        private void ShowWindow_Click(object sender, RoutedEventArgs e)
        {
            ShowFromTray();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            // Реально выходим
            Closing -= MainWindow_Closing;
            System.Windows.Application.Current.Shutdown();
        }
    }
}
