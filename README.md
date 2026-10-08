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

<img width="798" height="704" alt="Безымянный" src="https://github.com/user-attachments/assets/68a3d182-b9f1-48a2-89d4-1ef10007fec8" />

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
