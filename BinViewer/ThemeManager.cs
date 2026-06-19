using Microsoft.UI.Xaml;
using Windows.Storage;

namespace BinViewer
{
    public static class ThemeManager
    {
        private const string SelectedThemeKey = "App_SelectedTheme";

        // Загружает сохраненную тему при старте приложения
        public static ElementTheme LoadTheme()
        {
            var settings = ApplicationData.Current.LocalSettings.Values;
            if (settings.TryGetValue(SelectedThemeKey, out var themeValue) && themeValue is int themeInt)
            {
                return (ElementTheme)themeInt;
            }
            return ElementTheme.Default; // Системная тема по умолчанию
        }

        // Сохраняет выбранную тему и применяет её к окну
        public static void SaveAndApplyTheme(Window window, ElementTheme theme)
        {
            // 1. Сохраняем в реестр приложения
            ApplicationData.Current.LocalSettings.Values[SelectedThemeKey] = (int)theme;

            // 2. Принудительно меняем тему на уровне всего визуального корня окна
            if (window.Content is FrameworkElement rootElement)
            {
                rootElement.RequestedTheme = theme;

                // Заставляем WinUI 3 полностью пересчитать дерево стилей и обновить кэш кистей
                var currentVisibility = rootElement.Visibility;
                rootElement.Visibility = Visibility.Collapsed;
                rootElement.Visibility = currentVisibility;
            }
        }
    }
}
