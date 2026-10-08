# Window Manager

Лёгкая утилита для управления окнами в Windows.

## Возможности

- 📋 Список всех открытых окон
- 📌 Always on Top (поверх всех окон)
- 👻 Прозрачность окон (20–100%)
- 💾 Сохранение и восстановление раскладок окон
- ⌨️ Глобальные горячие клавиши
- 🔽 Работа в системном трее
- 🎨 Современный тёмный интерфейс

<img width="786" height="666" alt="Безымянный" src="https://github.com/user-attachments/assets/498c3b91-99f0-4f6a-93b8-bdb078305b9e" />

### Горячие клавиши по умолчанию

| Комбинация | Действие |
|------------|----------|
| `Ctrl + Shift + T` | Переключить Always on Top у активного окна |
| `Ctrl + Shift + ↑` | Увеличить прозрачность активного окна |
| `Ctrl + Shift + ↓` | Уменьшить прозрачность активного окна |
| `Ctrl + Shift + W` | Показать / скрыть главное окно |

## Требования

- Windows 10 / 11
- .NET 8 Desktop Runtime

## Сборка

```bash
dotnet restore
dotnet build -c Release
```

Запуск:

```bash
dotnet run -c Release
```

Или просто запустите `WindowManager.exe` из папки `bin/Release/net8.0-windows/`.

## Структура проекта

```
WindowManager/
├── Views/               # XAML окна
├── ViewModels/          # MVVM
├── Services/            # Бизнес-логика
│   ├── WindowService    # Работа с окнами через WinAPI
│   ├── LayoutService    # Сохранение/загрузка раскладок
│   ├── HotkeyService    # Глобальные хоткеи
│   └── SettingsService  # Настройки (JSON)
├── Models/              # Модели данных
├── Helpers/             # WinAPI (P/Invoke)
└── Resources/           # Иконки и ресурсы
```

## Планы на будущее

- [ ] Клики насквозь (click-through)
- [ ] Автозапуск с Windows
- [ ] Привязка окон к виртуальным рабочим столам
- [ ] Правила (автоматически делать определённые окна Always on Top)
- [ ] Поддержка нескольких мониторов (умные раскладки)
- [ ] Экспорт/импорт раскладок
