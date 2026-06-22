using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using Microsoft.Windows.ApplicationModel.Resources; // Добавьте это пространство имен
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT;
using WinRT.Interop;
using static System.Runtime.InteropServices.JavaScript.JSType;




// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinViewer
{
   

    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // Было: private readonly ResourceLoader _resourceLoader = new();
        // Стало (правильный десктопный менеджер ресурсов):
        private readonly ResourceManager _resourceManager = new();
        private readonly ResourceContext _resourceContext;
      
        public MainWindow()
        {
            InitializeComponent();
            // Создаем контекст для определения системного языка (ru-RU, en-US и т.д.)
            _resourceContext = _resourceManager.CreateResourceContext();

            //ViewModel = new ModalViewViDoc();
            this.Content.KeyDown += Global_KeyDown; 
            // Инициализация темы оформления при старте
            //InitAppTheme();
            // НОВАЯ СТРОКА: Принудительно инициализируем дефолтный локализованный статус-бар
            UpdateStatusBar();
            this.Title = GetLocalizedText("AppDisplayName");
            // Меняем иконку окна
            SetAppIcon("Assets/appicon.ico");


        }
        private void SetAppIcon(string iconPath)
        {
            // 1. Получаем хэндл (HWND) текущего окна WinUI 3
            IntPtr hWnd = WindowNative.GetWindowHandle(this);

            // 2. Получаем ID окна на основе хэндла
            WindowId windowId = Win32Interop.GetWindowIdFromWindow(hWnd);

            // 3. Получаем экземпляр AppWindow, который управляет заголовком и иконкой
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            if (appWindow != null)
            {
                // Формируем полный абсолютный путь к файлу иконки в папке сборки
                string fullPath = Path.Combine(AppContext.BaseDirectory, iconPath);

                if (File.Exists(fullPath))
                {
                    // 4. Применяем иконку к окну
                    appWindow.SetIcon(fullPath);
                }
            }
        }

        // Храним ссылку на текущую активную ViewModel, чтобы вовремя отписываться от её событий
        private HexDocumentViewModel _activeViewModel;
        private void UpdateStatusBar()
        {
            // УБЕРИТЕ ОТСЮДА любой код с += ViewModel_StatusChanged и -= ViewModel_StatusChanged!

            if (FileTabView.SelectedItem is TabViewItem currentTab &&
                currentTab.Content is HexEditorView editorView &&
                editorView.ViewModel is HexDocumentViewModel vm)
            {
                _activeViewModel = vm; // Просто запоминаем ссылку

                string sizeTemplate = GetLocalizedText("StatusSizeText");
                StatusFileSize.Text = string.Format(sizeTemplate, vm.Buffer.Length);
                UpdateOffsetStatusText(vm);
            }
            else
            {
                _activeViewModel = null;
                StatusFileSize.Text = GetLocalizedText("StatusSizeText").Replace("{0:N0}", "0");
                StatusOffset.Text = GetLocalizedText("StatusOffsetDefault");
            }
        }
        private string GetLocalizedText(string resourceKey)
        {
            try
            {
                // Ищем строку в файле Resources.resw по ключу через десктопный ResourceManager
                var resourceCandidate = _resourceManager.MainResourceMap.GetValue($"Resources/{resourceKey}", _resourceContext);
                return resourceCandidate?.ValueAsString ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
        private void Global_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
            {
                var ctrlState = Microsoft.UI.Input.InputKeyboardSource.GetKeyStateForCurrentThread(Windows.System.VirtualKey.Control);
                bool isCtrlPressed = ctrlState.HasFlag(Windows.UI.Core.CoreVirtualKeyStates.Down);

                // Перехват Ctrl+F для открытия панели поиска
                if (isCtrlPressed && e.Key == Windows.System.VirtualKey.F)
                {
                    e.Handled = true;
                    ShowSearchOverlay(showSearch: true, showGoTo: true);
                    return;
                }
                if (isCtrlPressed && e.Key == Windows.System.VirtualKey.C)
                {
                    e.Handled = true;
                    editorView.CopySelection();
                    return;
                }
                if (isCtrlPressed && e.Key == Windows.System.VirtualKey.V)
                {
                    e.Handled = true;
                    editorView.PasteSelection();
                    return;
                }

                editorView.HandleKeyboardInput(e);
            }
        }
        // Управление отображением всплывающей панели поиска
        private void ShowSearchOverlay(bool showSearch, bool showGoTo)
        {
            SearchSection.Visibility = showSearch ? Visibility.Visible : Visibility.Collapsed;
            GoToSection.Visibility = showGoTo ? Visibility.Visible : Visibility.Collapsed;
            SearchOverlayPanel.Visibility = Visibility.Visible;

            if (showSearch) SearchBox.Focus(FocusState.Programmatic);
            else if (showGoTo) GoToBox.Focus(FocusState.Programmatic);
        }
        private void CloseSearchOverlay_Click(object sender, RoutedEventArgs e)
        {
            SearchOverlayPanel.Visibility = Visibility.Collapsed;
        }
        // Обработчики кликов верхнего меню MenuBar
        private void MenuSearch_Click(object sender, RoutedEventArgs e)
        {
            ShowSearchOverlay(showSearch: true, showGoTo: false);
        }

private void MenuGoTo_Click(object sender, RoutedEventArgs e)
{
    ShowSearchOverlay(showSearch: false, showGoTo: true);
}

private void MenuGlobalCopy_Click(object sender, RoutedEventArgs e)
{
    if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
    {
        editorView.CopySelection();
    }
}

private void MenuGlobalPaste_Click(object sender, RoutedEventArgs e)
{
    if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
    {
        editorView.PasteSelection();
    }
}
        private async void OpenBuffer_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // 1. Создаем ViewModel данных для файла
                    var tabViewModel = new HexDocumentViewModel(file.Path);
                    // ПОДПИСЫВАЕМСЯ ЗДЕСЬ И ВСЕГО ОДИН РАЗ:
                    tabViewModel.PropertyChanged += ViewModel_StatusChanged;
                    // 2. Создаем визуальный элемент (UI) для этой вкладки и передаем ему данные
                    var editorView = new HexEditorView(tabViewModel);
                    

                    // 3. Собираем вкладку воедино
                    var newTab = new TabViewItem
                    {
                        Header = tabViewModel.FileName,
                        Content = editorView, // Помещаем UI внутрь вкладки
                                              // Кладем в Tag легковесную обертку вместо «тяжелой» ViewModel
                        Tag = new HexTabReference(tabViewModel)
                    };

                    FileTabView.TabItems.Add(newTab);
                    FileTabView.SelectedItem = newTab;
                    UpdateStatusBar();
                    UpdateWindowTitle(); // Добавьте вызов сюда
                }
            }
            catch (Exception ex)
            {
                // Запись в файл на рабочем столе, так как UI может не работать
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "hex_error.txt"), ex.ToString());
            }
        }
        private void SaveBuffer_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
            {
                var vm = editorView.ViewModel;
                try
                {
                    // Вызываем комплексное сохранение: Бэкап -> Запись -> Обновление UI
                    vm.SaveDocument();

                    // Сбрасываем статус клавиатуры
                    vm.IsFirstNibbleEntered = false;
                    vm.NotifySelectionChanged();

                    // Выводим успешный статус в заголовок
                    this.Title = $"BinStudio — {vm.FileName}";

                    // УМНЫЙ ТРИГГЕР ОЦЕНКИ: Проверяем, пора ли запросить отзыв
                    if (RateAppManager.ShouldRequestRating())
                    {

                        // Запускаем таймер в потоке UI, чтобы избежать сбоев cross-thread
                        var timer = new Microsoft.UI.Xaml.DispatcherTimer();
                        timer.Interval = TimeSpan.FromSeconds(1);
                        timer.Tick += async (s, args) =>
                        {
                            timer.Stop(); // Выключаем таймер, чтобы он не сработал повторно

                            // Проверяем, что окно еще открыто и контекст доступен
                            if (this.Content != null && this.Content.XamlRoot != null)
                            {
                                // ЖЕЛЕЗОБЕТОННОЕ присвоение XamlRoot прямо перед выводом на экран
                                RateDialog.XamlRoot = this.Content.XamlRoot;

                                // Настраиваем видимость текстового поля в зависимости от звезд
                                Star1.Checked += (send, a) => FeedbackTextBox.Visibility = Visibility.Visible;
                                Star2.Checked += (send, a) => FeedbackTextBox.Visibility = Visibility.Visible;
                                Star3.Checked += (send, a) => FeedbackTextBox.Visibility = Visibility.Visible;
                                Star4.Checked += (send, a) => FeedbackTextBox.Visibility = Visibility.Collapsed;
                                Star5.Checked += (send, a) => FeedbackTextBox.Visibility = Visibility.Collapsed;

                                try
                                {
                                    await RateDialog.ShowAsync();
                                }
                                catch { /* Защита от повторного открытия окна */ }
                            }
                        };
                        timer.Start();
                    }
                }
                catch (Exception ex)
                {
                    // Если произошла ошибка при создании бэкапа или сохранении, выводим её пользователю
                    this.Title = $"BinStudio — Ошибка: {ex.Message}";

                    // Дополнительно можно вывести системный диалог с ошибкой
                    ShowErrorDialog("Критическая ошибка сохранения", ex.Message);
                }
            }
        }
        // Вспомогательный метод для вывода критических ошибок
        private async void ShowErrorDialog(string title, string content)
        {
            var errorDialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "ОК",
                XamlRoot = this.Content.XamlRoot
            };
            await errorDialog.ShowAsync();
        }
        //private void SaveBuffer_Click(object sender, RoutedEventArgs e)
        //{

        //    // Проверяем, открыта ли вкладка и есть ли в ней активный редактор
        //    if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
        //    {
        //        try
        //        {
        //            // 1. Физически записываем изменения в файл на диске
        //            editorView.ViewModel.Buffer.Save();

        //            // 2. Обновляем список строк, чтобы сбросить флаги IsModified (красный цвет текста)
        //            editorView.ViewModel.RefreshAllRows();

        //            // 3. Сбрасываем статус полу-ввода клавиатуры
        //            editorView.ViewModel.IsFirstNibbleEntered = false;
        //            editorView.ViewModel.NotifySelectionChanged();

        //            this.Title = $"BinStudio — {editorView.ViewModel.FileName}";
        //            // ТРИГГЕР: Если файл успешно сохранен, проверяем, пора ли запросить оценку
        //            if (RateAppManager.ShouldRequestRating())
        //            {
        //                // Запускаем окно с микро-задержкой в 1 секунду, чтобы дать пользователю осознать успех операции
        //                Task.Delay(1000).ContinueWith(_ =>
        //                {
        //                    this.DispatcherQueue.TryEnqueue(async () =>
        //                    {
        //                        RateDialog.XamlRoot = this.Content.XamlRoot;

        //                        // Подписываемся на выбор звезд, чтобы на лету показывать/прятать текстовое поле
        //                        Star1.Checked += (s, a) => FeedbackTextBox.Visibility = Visibility.Visible;
        //                        Star2.Checked += (s, a) => FeedbackTextBox.Visibility = Visibility.Visible;
        //                        Star3.Checked += (s, a) => FeedbackTextBox.Visibility = Visibility.Visible;
        //                        Star4.Checked += (s, a) => FeedbackTextBox.Visibility = Visibility.Collapsed;
        //                        Star5.Checked += (s, a) => FeedbackTextBox.Visibility = Visibility.Collapsed;

        //                        await RateDialog.ShowAsync();
        //                    });
        //                });
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            // В случае ошибки доступа (например, файл защищен от записи) выводим информацию в заголовок окна
        //            this.Title = $"BinStudio — Ошибка сохранения: {ex.Message}";
        //        }
        //    }
        //}
        // 2. Обработка нажатия кнопки "Отправить"
        private async void RateDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (NeverShowAgainCheckBox.IsChecked == true)
            {
                RateAppManager.MarkAsRated();
            }

            // Проверяем, какую звезду выбрал пользователь
            if (Star5.IsChecked == true || Star4.IsChecked == true)
            {
                // Пользователь СЧАСТЛИВ (4 или 5 звезд) -> Маркируем как выполненное и отправляем в Store
                RateAppManager.MarkAsRated();

                // Официальный URI для открытия страницы отзывов конкретного приложения в Microsoft Store
                // Замените YOUR_STORE_PRODUCT_ID на реальный ID вашего приложения после публикации
                var storeUri = new Uri("ms-windows-store://review/?ProductId=9NNHSR22R4NF");
                await Windows.System.Launcher.LaunchUriAsync(storeUri);
            }
            else
            {
                // Пользователь НЕДОВОЛЕН (1, 2 или 3 звезды) -> Перехватываем негатив
                string userFeedback = FeedbackTextBox.Text;
                if (!string.IsNullOrWhiteSpace(userFeedback))
                {
                    // Здесь вы можете отправить текст userFeedback на ваш сервер/email по API
                    // Таким образом, негативный отзыв уйдет лично вам разработчику, а не испортит рейтинг в магазине.
                }

                // Выводим вежливую благодарность за критику
                sender.Hide();
                var thankYouDialog = new ContentDialog
                {
                    Title = GetLocalizedText("ThankYouTitle"),
                    Content = GetLocalizedText("ThankYouDesc"),
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot
                };
                await thankYouDialog.ShowAsync();
            }
        }
        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabView.SelectedItem is not TabViewItem currentTab || currentTab.Content is not HexEditorView editorView)
                return;

            string query = SearchBox.Text?.Trim();
            if (string.IsNullOrEmpty(query)) return;

            byte[] pattern = null;
            string cleanHex = query.Replace(" ", "").Replace("-", "");
            bool isHex = cleanHex.Length % 2 == 0;

            if (isHex)
            {
                try
                {
                    pattern = new byte[cleanHex.Length / 2];
                    for (int i = 0; i < pattern.Length; i++)
                        pattern[i] = Convert.ToByte(cleanHex.Substring(i * 2, 2), 16);
                }
                catch { isHex = false; }
            }

            if (!isHex)
            {
                pattern = System.Text.Encoding.ASCII.GetBytes(query);
            }

            if (sender is Button searchButton)
                searchButton.Content = GetLocalizedText("SearchProgressText"); // Динамический перевод


            // Запускаем процесс генерации списка результатов справа
            await editorView.ExecuteSearchAsync(pattern, query);

            if (sender is Button btn)
                btn.Content = GetLocalizedText("SearchOverlayBtn/Content");
        }

        private long FindPatternInFile(VirtualFileBuffer buffer, byte[] pattern)
        {
            byte[] chunk = new byte[4096];
            long totalLength = buffer.Length;

            for (long pos = 0; pos < totalLength; pos += chunk.Length - pattern.Length)
            {
                int toRead = (int)Math.Min(chunk.Length, totalLength - pos);
                buffer.ReadRange(pos, chunk, toRead);

                for (int i = 0; i <= toRead - pattern.Length; i++)
                {
                    bool match = true;
                    for (int j = 0; j < pattern.Length; j++)
                    {
                        if (chunk[i + j] != pattern[j]) { match = false; break; }
                    }
                    if (match) return pos + i;
                }
            }
            return -1; // Не найдено
        }

        private void FileTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Item is TabViewItem tab &&
        tab.Content is HexEditorView editorView &&
        editorView.ViewModel is HexDocumentViewModel vm)
            {
                // ОТПИСЫВАЕМСЯ ПЕРЕД УНИЧТОЖЕНИЕМ ВКЛАДКИ:
                vm.PropertyChanged -= ViewModel_StatusChanged;

                if (_activeViewModel == vm)
                {
                    _activeViewModel = null;
                }

                vm.Buffer.Dispose();
                FileTabView.TabItems.Remove(tab);

                UpdateStatusBar();
                UpdateWindowTitle();
            }
        }

        private void GoToOffset_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверяем, выбрана ли вкладка и есть ли в ней наш HexEditorView
            if (FileTabView.SelectedItem is not TabViewItem currentTab || currentTab.Content is not HexEditorView editorView)
                return;

            string input = GoToBox.Text?.Trim();
            if (string.IsNullOrEmpty(input))
                return;

            // 2. Парсим введенный HEX-адрес в число типа long
            if (long.TryParse(input, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long targetOffset))
            {
                var bufferLength = editorView.ViewModel.Buffer.Length;

                // Ограничиваем переход рамками размера файла
                if (targetOffset < 0) targetOffset = 0;
                if (targetOffset >= bufferLength) targetOffset = bufferLength - 1;

                // 3. Вычисляем индекс строки (в каждой строке по 16 байт)
                int targetRowIndex = (int)(targetOffset / 16);

                // 4. Даем команду на скролл
                editorView.ScrollToRow(targetRowIndex);
            }
            else
            {
                // Опционально: здесь можно вывести предупреждение, что адрес введен некорректно
                GoToBox.Text = "Ошибка ввода";
            }
        }
        // Вызывается при переключении вкладок или открытии нового файла
        private void FileTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateStatusBar();
            UpdateWindowTitle(); // Добавьте вызов сюда
        }

        //// Метод для обновления текстовых блоков строки состояния
        //private void UpdateStatusBar()
        //{
        //    if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Tag is HexDocumentViewModel vm)
        //    {
        //        // Отписываемся от старых событий (чтобы не было утечек памяти) и подписываемся на текущую вкладку
        //        vm.PropertyChanged -= ViewModel_StatusChanged;
        //        vm.PropertyChanged += ViewModel_StatusChanged;

        //        // Выводим размер файла (форматируем число с разделением тысяч)
        //        StatusFileSize.Text = $"Размер: {vm.Buffer.Length:N0} байт";
        //        StatusOffset.Text = $"Смещение: {vm.SelectionInfoText}";
        //    }
        //    else
        //    {
        //        // Если все вкладки закрыты
        //        StatusFileSize.Text = "Размер: 0 байт";
        //        StatusOffset.Text = "Смещение: -";
        //    }
        //}

        // Срабатывает каждый раз, когда пользователь кликает, выделяет или перемещается стрелочками
        private void ViewModel_StatusChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
        
            if (e.PropertyName == nameof(HexDocumentViewModel.SelectionInfoText))
            {
                if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Tag is HexTabReference tabRef)
                {
                    HexDocumentViewModel vm = tabRef.ViewModel;
                    // Обновляем только смещение (размер файла статичен)
                    //StatusOffset.Text = $"Смещение: {vm.SelectionInfoText}";
                    UpdateOffsetStatusText(vm);
                }
            }
        }
        private void UpdateOffsetStatusText(HexDocumentViewModel vm)
        {
            string offsetTemplate = GetLocalizedText("StatusOffsetText");

            if (vm.SelectionStart == -1)
            {
                StatusOffset.Text = string.Format(offsetTemplate, "-");
                return;
            }

            if (vm.SelectionStart == vm.SelectionEnd)
            {
                StatusOffset.Text = string.Format(offsetTemplate, $"{vm.SelectionStart} (0x{vm.SelectionStart:X})");
            }
            else
            {
                long min = Math.Min(vm.SelectionStart, vm.SelectionEnd);
                long max = Math.Max(vm.SelectionStart, vm.SelectionEnd);
                long count = max - min + 1;

                string bytesTemplate = GetLocalizedText("StatusBytesText");
                string bytesInfo = string.Format(bytesTemplate, min, count); // "Выделено: X байт"

                StatusOffset.Text = string.Format(offsetTemplate, $"0x{min:X} .. 0x{max:X} ({bytesInfo})");
            }
        }
        private void MenuFileInfo_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
            {
                // Передаем команду показать правую панель информации
                editorView.ToggleFileInfoPanel(true);
            }
        }
        private async void MenuAbout_Click(object sender, RoutedEventArgs e)
        {
            // ГАРАНТИРОВАННАЯ привязка к окну из кода:
            // Мы берем XamlRoot у корневого контента всего главного окна
            AboutDialog.XamlRoot = this.Content.XamlRoot;

            // Показываем диалоговое окно
            await AboutDialog.ShowAsync();
        }

        private void UpdateWindowTitle()
        {
            // Проверяем, выбрана ли вкладка и есть ли у неё ViewModel
            if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Tag is HexTabReference tabRef)
            {
                HexDocumentViewModel vm = tabRef.ViewModel;
                //    if (FileTabView.SelectedItem is TabViewItem currentTab &&
                //currentTab.Content is HexEditorView editorView &&
                //editorView.ViewModel is HexDocumentViewModel vm)
                //    {
                // Устанавливаем заголовок с именем активного файла
                this.Title = $"BinStudio — {vm.FileName}";
            }
            else
            {
                // Если открытых вкладок больше нет
                this.Title = "BinStudio";
            }
        }
        // Публичный метод для автоматического открытия файлов из Проводника или командной строки
        public void OpenFileDirectly(string filePath)
        {
            if (string.IsNullOrEmpty(filePath) || !System.IO.File.Exists(filePath)) return;

            try
            {
                // 1. Создаем ViewModel данных для переданного пути к файлу
                var tabViewModel = new HexDocumentViewModel(filePath);
                // ПОДПИСЫВАЕМСЯ ЗДЕСЬ И ВСЕГО ОДИН РАЗ:
                tabViewModel.PropertyChanged += ViewModel_StatusChanged;
                // 2. Инициализируем пользовательский интерфейс редактора
                var editorView = new HexEditorView(tabViewModel);

                // 3. Формируем вкладку
                var newTab = new TabViewItem
                {
                    Header = tabViewModel.FileName,
                    Content = editorView,
                    // Кладем в Tag легковесную обертку вместо «тяжелой» ViewModel
                    Tag = new HexTabReference(tabViewModel)
                };

                // 4. Добавляем в общую панель и делаем её активной
                FileTabView.TabItems.Add(newTab);
                FileTabView.SelectedItem = newTab;

                // 5. Синхронизируем статус-бар и динамический заголовок окна BinStudio
                UpdateStatusBar();
                UpdateWindowTitle();
            }
            catch (Exception ex)
            {
                // Если файл заблокирован другим процессом или поврежден
                this.Title = $"BinStudio — Ошибка автооткрытия: {ex.Message}";
            }
        }
        //private void InitAppTheme()
        //{
        //    // 1. Считываем сохраненную тему из LocalSettings
        //    ElementTheme currentTheme = ThemeManager.LoadTheme();

        //    // 2. Применяем её к интерфейсу
        //    if (this.Content is FrameworkElement rootElement)
        //    {
        //        rootElement.RequestedTheme = currentTheme;
        //    }

        //    // 3. Выставляем правильные галочки в пунктах меню верхнего уровня
        //    ThemeDefaultRadio.IsChecked = currentTheme == ElementTheme.Default;
        //    ThemeLightRadio.IsChecked = currentTheme == ElementTheme.Light;
        //    ThemeDarkRadio.IsChecked = currentTheme == ElementTheme.Dark;

        //    // В WinUI 3 высокая контрастность управляется через системные кисти, 
        //    // но мы можем принудительно задать темную тему в качестве базовой контрастной
        //    ThemeHighContrastRadio.IsChecked = false;
        //}

        //private void ThemeRadio_Click(object sender, RoutedEventArgs e)
        //{
        //    if (sender is RadioMenuFlyoutItem radioButton)
        //    {
        //        ElementTheme targetTheme = ElementTheme.Default;

        //        if (radioButton == ThemeLightRadio) targetTheme = ElementTheme.Light;
        //        else if (radioButton == ThemeDarkRadio) targetTheme = ElementTheme.Dark;
        //        else if (radioButton == ThemeHighContrastRadio) targetTheme = ElementTheme.Dark; // Базируется на темных ресурсах
        //        else if (radioButton == ThemeDefaultRadio) targetTheme = ElementTheme.Default;

        //        // Сохраняем выбор и мгновенно перекрашиваем все элементы BinStudio
        //        ThemeManager.SaveAndApplyTheme(this, targetTheme);
        //    }
        //}
    }


}
