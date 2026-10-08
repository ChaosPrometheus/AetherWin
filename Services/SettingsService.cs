using System;
using System.IO;
using Newtonsoft.Json;
using WindowManager.Models;

namespace WindowManager.Services
{
    public class SettingsService
    {
        public static SettingsService Instance { get; } = new();

        private readonly string _settingsPath;
        public AppSettings Settings { get; private set; } = new();

        private SettingsService()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string folder = Path.Combine(appData, "WindowManager");
            Directory.CreateDirectory(folder);
            _settingsPath = Path.Combine(folder, "settings.json");
        }

        public void Load()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    string json = File.ReadAllText(_settingsPath);
                    Settings = JsonConvert.DeserializeObject<AppSettings>(json) ?? new AppSettings();
                }
            }
            catch
            {
                Settings = new AppSettings();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonConvert.SerializeObject(Settings, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch
            {
                // Игнорируем ошибки сохранения
            }
        }

        public void AddLayout(WindowLayout layout)
        {
            Settings.SavedLayouts.RemoveAll(l => l.Name.Equals(layout.Name, StringComparison.OrdinalIgnoreCase));
            Settings.SavedLayouts.Add(layout);
            Save();
        }

        public void RemoveLayout(string name)
        {
            Settings.SavedLayouts.RemoveAll(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            Save();
        }
    }
}
