using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Media;
using System;
using System.Globalization;
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
     
        public MainWindow()
        {
            InitializeComponent();
            //ViewModel = new ModalViewViDoc();
            this.Content.KeyDown += Global_KeyDown;

        }
        private void Global_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // 1. Проверяем, открыта ли вкладка и есть ли в ней наш редактор
            if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Content is HexEditorView editorView)
            {
                // 2. Перенаправляем нажатие клавиши напрямую в активный редактор
                editorView.HandleKeyboardInput(e);
            }
        }
        private async void OpenBuffer_Click(object sender, RoutedEventArgs e)
        {
            var picker = new FileOpenPicker();
            InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
            picker.FileTypeFilter.Add("*");

            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                // 1. Создаем ViewModel данных для файла
                var tabViewModel = new HexDocumentViewModel(file.Path);

                // 2. Создаем визуальный элемент (UI) для этой вкладки и передаем ему данные
                var editorView = new HexEditorView(tabViewModel);

                // 3. Собираем вкладку воедино
                var newTab = new TabViewItem
                {
                    Header = tabViewModel.FileName,
                    Content = editorView, // Помещаем UI внутрь вкладки
                    Tag = tabViewModel    // Сохраняем ссылку на ViewModel для поиска и сохранения
                };

                FileTabView.TabItems.Add(newTab);
                FileTabView.SelectedItem = newTab;
            }
        }
        private void SaveBuffer_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabView.SelectedItem is TabViewItem currentTab && currentTab.Tag is HexDocumentViewModel vm)
            {
                try
                {
                    vm.Buffer.Save();
                    // Здесь можно добавить всплывающее уведомление об успешном сохранении
                }
                catch (Exception ex)
                {
                    // Обработка ошибок доступа к файлу
                }
            }
        }
        private void Search_Click(object sender, RoutedEventArgs e)
        {
            if (FileTabView.SelectedItem is not TabViewItem currentTab || currentTab.Tag is not HexDocumentViewModel vm) return;

            string query = SearchBox.Text;
            if (string.IsNullOrEmpty(query)) return;

            // Пример поиска: преобразуем запрос в байты (здесь упрощенно ASCII)
            byte[] pattern = System.Text.Encoding.ASCII.GetBytes(query);
            long matchPosition = FindPatternInFile(vm.Buffer, pattern);

            if (matchPosition != -1)
            {
                // Здесь логика перехода (Scroll) к строке: matchPosition / 16
                // Для этого ScrollViewer смещается программно через ChangeView
            }
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
            if (args.Item is TabViewItem tab && tab.Tag is HexDocumentViewModel vm)
            {
                vm.Buffer.Dispose();
                FileTabView.TabItems.Remove(tab);
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


    }


}
