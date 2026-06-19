using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Microsoft.Windows.AppLifecycle;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;


// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinViewer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        private Window? _window;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            _window = new MainWindow();
            // 2. Получаем расширенную информацию о запуске (был ли это клик по файлу)
            var activatedArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            string fileToOpen = string.Empty;

            // Проверяем, запущено ли приложение через ассоциацию файлов (File Type Association)
            if (activatedArgs.Kind == ExtendedActivationKind.File)
            {
                var fileArgs = activatedArgs.Data as Windows.ApplicationModel.Activation.FileActivatedEventArgs;
                if (fileArgs != null && fileArgs.Files.Count > 0)
                {
                    // Берем путь к первому кликнутому файлу
                    fileToOpen = fileArgs.Files[0].Path;
                }
            }
            _window.Activate();
            // 3. Если передан путь к файлу, даем команду главному окну открыть его
            if (!string.IsNullOrEmpty(fileToOpen))
            {
                // Вызываем публичный метод в MainWindow, который мы сейчас напишем
                ((MainWindow)_window).OpenFileDirectly(fileToOpen);
            }
        }
    }
}
