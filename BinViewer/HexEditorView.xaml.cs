using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinViewer
{
    public sealed partial class HexEditorView : UserControl
    {
        public HexDocumentViewModel ViewModel { get; private set; }
        private bool _isSelecting = false;

        // Список всех активных (видимых на экране) ячеек для быстрой перекраски
        private readonly List<Border> _activeBorders = new();
        private long _editingAddress = -1; // -1 означает, что режим редактирования выключен


        public HexEditorView(HexDocumentViewModel viewModel)
        {
            this.InitializeComponent();
            this.ViewModel = viewModel;

            // Подписываемся на обновление выделения
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            // ГАРАНТИРОВАННЫЙ перехват клавиш до того, как их заберут внутренние списки
          
        }
        // Коллекция для идеального рендеринга ячеек заголовка
        public string[] HeaderPositions { get; } =
            { "00", "01", "02", "03", "04", "05", "06", "07", "08", "09", "0A", "0B", "0C", "0D", "0E", "0F" };
        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HexDocumentViewModel.SelectionStart) || e.PropertyName == "SelectionStart")
            {
                UpdateBordersVisuals();
            }
        }


        private void UpdateBordersVisuals()
        {
            var accentBrush = (Brush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
            var modifiedBrush = new SolidColorBrush(Colors.Red);
            var transparentBrush = new SolidColorBrush(Colors.Transparent);

            var textSelectedBrush = new SolidColorBrush(Colors.White);
            var textModifiedBrush = new SolidColorBrush(Colors.Red);

            // ИСПРАВЛЕНО: берем актуальный цвет текста для текущей темы (светлой или темной)
            var textNormalBrush = (Brush)Application.Current.Resources["TextControlForeground"];

            foreach (var border in _activeBorders)
            {
                if (border.DataContext is HexByteItem byteItem && byteItem.IsValid)
                {
                    bool isSelected = ViewModel.IsByteSelected(byteItem.AbsoluteAddress);
                    bool isEditingThis = byteItem.AbsoluteAddress == _editingAddress;

                    if (isSelected || isEditingThis)
                    {
                        border.Background = accentBrush;
                        if (border.Child is TextBlock tb) tb.Foreground = textSelectedBrush;
                    }
                    else if (byteItem.IsModified)
                    {
                        border.Background = transparentBrush;
                        if (border.Child is TextBlock tb) tb.Foreground = textModifiedBrush;
                    }
                    else
                    {
                        border.Background = transparentBrush;
                        if (border.Child is TextBlock tb) tb.Foreground = textNormalBrush; // Динамический цвет
                    }
                }
            }

            UpdateDataInspector();
        }

        // Обязательно добавьте это имя (x:Name="ContentScrollViewer") в тег <ScrollViewer> внутри HexEditorView.xaml
        public void ScrollToRow(int rowIndex)
        {
            // Высота одной строки в разметке Grid равна 24 (задано в DataTemplate)
            double rowHeight = 24.0;
            double targetOffset = rowIndex * rowHeight;

            // Прокручиваем ScrollViewer на вычисленную вертикальную позицию
            ContentScrollViewer.ChangeView(null, targetOffset, null, disableAnimation: false);
        }
        // Синхронизируем цвета видимых ячеек в зависимости от глобального выделения
       

        // Трекаем появление ячеек на экране
        private void ByteBorder_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (sender is Border border)
            {
                if (!_activeBorders.Contains(border)) _activeBorders.Add(border);

                // Сразу красим при появлении строки
                if (border.DataContext is HexByteItem byteItem && byteItem.IsValid)
                {
                    var accentBrush = (Brush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
                    bool isSelected = ViewModel.IsByteSelected(byteItem.AbsoluteAddress);
                    border.Background = isSelected ? accentBrush : new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        // Одинарный клик — ТОЛЬКО выделение
        private void Byte_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerProperties = e.GetCurrentPoint(this).Properties;
            if (!pointerProperties.IsLeftButtonPressed) return;

            if (sender is Border border && border.DataContext is HexByteItem byteItem && byteItem.IsValid)
            {
                _isSelecting = true;
                MainGrid.CapturePointer(e.Pointer);

                // Сбрасываем старое редактирование при клике в другое место
                _editingAddress = -1;
                ViewModel.IsFirstNibbleEntered = false;

                ViewModel.SelectionStart = byteItem.AbsoluteAddress;
                ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                ViewModel.NotifySelectionChanged();
            }
        }

        // Двойной клик — вход в режим редактирования
        private void Byte_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is HexByteItem byteItem && byteItem.IsValid)
            {
                // Фиксируем адрес для редактирования
                _editingAddress = byteItem.AbsoluteAddress;
                ViewModel.SelectionStart = byteItem.AbsoluteAddress;
                ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                ViewModel.IsFirstNibbleEntered = false;

                // Забираем фокус ввода клавиатуры на UserControl
                this.Focus(FocusState.Programmatic);
                UpdateBordersVisuals();
            }
        }


        // Движение мыши с зажатой кнопкой
        private void Byte_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (_isSelecting && sender is Border border && border.DataContext is HexByteItem byteItem && byteItem.IsValid)
            {
                ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                ViewModel.NotifySelectionChanged(); // Обновить цвета динамически
            }
        }

        // Отпускание мыши — конец выделения
        private void Byte_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                if (sender is Border border)
                {
                    border.ReleasePointerCapture(e.Pointer);
                }
            }
        }


     

 

        // Отпускание мыши в любом месте сетки завершает операцию
        private void MainGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                MainGrid.ReleasePointerCaptures();
            }
        }






 

        // Движение мыши отслеживается глобально над таблицей
        private void MainGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isSelecting) return;

            // Дополнительная страховка: проверяем, что левая кнопка все еще зажата при движении
            var pointerProperties = e.GetCurrentPoint(MainGrid).Properties;
            if (!pointerProperties.IsLeftButtonPressed)
            {
                _isSelecting = false;
                MainGrid.ReleasePointerCaptures();
                return;
            }

            // Получаем текущую координату курсора относительно главной сетки
            Point currentPoint = e.GetCurrentPoint(MainGrid).Position;

            // Ищем, над каким из видимых элементов Border сейчас находится курсор
            foreach (var border in _activeBorders)
            {
                if (border.DataContext is HexByteItem byteItem && byteItem.IsValid)
                {
                    // Вычисляем границы Border на экране
                    var transform = border.TransformToVisual(MainGrid);
                    Point borderPos = transform.TransformPoint(new Point(0, 0));
                    Rect borderRect = new Rect(borderPos.X, borderPos.Y, border.ActualWidth, border.ActualHeight);

                    // Если курсор попал в эту ячейку, обновляем конечную точку выделения
                    if (borderRect.Contains(currentPoint))
                    {
                        if (ViewModel.SelectionEnd != byteItem.AbsoluteAddress)
                        {
                            ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                            ViewModel.NotifySelectionChanged();
                        }
                        break;
                    }
                }
            }
        }





        // Обработчик нажатия клавиш

        // Обработка клавиатуры
        public void HandleKeyboardInput(Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (_editingAddress == -1) return;

            int keyValue = -1;

            // 1. Проверяем цифры из ВЕРХНЕГО ряда клавиатуры (VirtualKey.Number0 = 48, Number9 = 57)
            if (e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9)
            {
                keyValue = e.Key - Windows.System.VirtualKey.Number0;
            }
            // 2. Проверяем буквы A-F для ввода шестнадцатеричных значений
            else if (e.Key >= Windows.System.VirtualKey.A && e.Key <= Windows.System.VirtualKey.F)
            {
                keyValue = 10 + (e.Key - Windows.System.VirtualKey.A);
            }
            // 3. Проверяем цифры с боковой клавиатуры (Numpad0 = 96, Numpad9 = 105)
            else if (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9)
            {
                keyValue = e.Key - Windows.System.VirtualKey.NumberPad0;
            }

            // Если нажата валидная HEX клавиша (0-9, A-F)
            if (keyValue != -1)
            {
                e.Handled = true;
                byte currentByte = ViewModel.Buffer.ReadByte(_editingAddress);

                if (!ViewModel.IsFirstNibbleEntered)
                {
                    // Ввод первого символа (старший полубайт)
                    byte newValue = (byte)((keyValue << 4) | (currentByte & 0x0F));
                    ViewModel.ChangeByteAt(_editingAddress, newValue);
                    ViewModel.IsFirstNibbleEntered = true;

                    UpdateActiveBorderText(_editingAddress, keyValue.ToString("X") + "?");
                }
                else
                {
                    // Ввод второго символа (младший полубайт)
                    byte newValue = (byte)((currentByte & 0xF0) | keyValue);
                    ViewModel.ChangeByteAt(_editingAddress, newValue);
                    ViewModel.IsFirstNibbleEntered = false;

                    // Автопереход на СЛЕДУЮЩИЙ байт
                    if (_editingAddress + 1 < ViewModel.Buffer.Length)
                    {
                        _editingAddress++;
                        ViewModel.SelectionStart = _editingAddress;
                        ViewModel.SelectionEnd = _editingAddress;
                    }
                    else
                    {
                        _editingAddress = -1;
                    }
                    ViewModel.NotifySelectionChanged();
                }
            }
            // Навигация стрелочками
            else if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Right ||
                     e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.Down)
            {
                long newAddress = _editingAddress;
                if (e.Key == Windows.System.VirtualKey.Left && _editingAddress > 0) newAddress--;
                if (e.Key == Windows.System.VirtualKey.Right && _editingAddress + 1 < ViewModel.Buffer.Length) newAddress++;
                if (e.Key == Windows.System.VirtualKey.Up && _editingAddress >= 16) newAddress -= 16;
                if (e.Key == Windows.System.VirtualKey.Down && _editingAddress + 16 < ViewModel.Buffer.Length) newAddress += 16;

                if (newAddress != _editingAddress)
                {
                    e.Handled = true;
                    _editingAddress = newAddress;
                    ViewModel.SelectionStart = newAddress;
                    ViewModel.SelectionEnd = newAddress;
                    ViewModel.IsFirstNibbleEntered = false;
                    ViewModel.NotifySelectionChanged();

                    // Автоматическая докрутка ScrollViewer при переходе стрелочками
                    int targetRowIndex = (int)(_editingAddress / 16);
                    ScrollToRow(targetRowIndex);
                }
            }
            // Выход по Escape
            else if (e.Key == Windows.System.VirtualKey.Escape)
            {
                _editingAddress = -1;
                ViewModel.IsFirstNibbleEntered = false;
                ViewModel.NotifySelectionChanged();
            }
        }

        // Вспомогательный метод для красивого отображения полуввода (например, "A?")
        private void UpdateActiveBorderText(long address, string text)
        {
            foreach (var border in _activeBorders)
            {
                if (border.DataContext is HexByteItem byteItem && byteItem.AbsoluteAddress == address)
                {
                    if (border.Child is TextBlock tb)
                    {
                        tb.Text = text;
                    }
                    break;
                }
            }
        }
        // Метод для копирования выделенных байт
        public void CopySelection()
        {
            if (ViewModel.SelectionStart == -1 || ViewModel.SelectionEnd == -1) return;

            long start = Math.Min(ViewModel.SelectionStart, ViewModel.SelectionEnd);
            long end = Math.Max(ViewModel.SelectionStart, ViewModel.SelectionEnd);
            int count = (int)(end - start + 1);

            byte[] buffer = new byte[count];
            ViewModel.Buffer.ReadRange(start, buffer, count);

            // Преобразуем байты в HEX-строку через пробел (например, "41 42 43")
            string hexString = BitConverter.ToString(buffer).Replace("-", " ");

            var dataPackage = new DataPackage();
            dataPackage.SetText(hexString);
            Clipboard.SetContent(dataPackage);
        }

        // Метод для вставки байт из буфера обмена
        public async void PasteSelection()
        {
            // Вставляем с позиции SelectionStart (или начала файла, если ничего не выбрано)
            long targetAddress = ViewModel.SelectionStart != -1 ? ViewModel.SelectionStart : 0;

            var dataPackageView = Clipboard.GetContent();
            if (dataPackageView.Contains(StandardDataFormats.Text))
            {
                try
                {
                    string text = await dataPackageView.GetTextAsync();
                    if (string.IsNullOrWhiteSpace(text)) return;

                    // Очищаем строку от лишних символов и разбиваем по пробелам/запятым/новым строкам
                    string[] hexParts = text.Split(new[] { ' ', '-', ',', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

                    for (int i = 0; i < hexParts.Length; i++)
                    {
                        long currentAddress = targetAddress + i;
                        if (currentAddress >= ViewModel.Buffer.Length) break; // Защита от выхода за конец файла

                        if (byte.TryParse(hexParts[i], System.Globalization.NumberStyles.HexNumber, null, out byte b))
                        {
                            ViewModel.ChangeByteAt(currentAddress, b);
                        }
                    }

                    // Выделяем вставленный фрагмент для наглядности
                    ViewModel.SelectionStart = targetAddress;
                    ViewModel.SelectionEnd = Math.Min(ViewModel.Buffer.Length - 1, targetAddress + hexParts.Length - 1);
                    ViewModel.NotifySelectionChanged();
                }
                catch { /* Игнорируем ошибки невалидного формата в буфере */ }
            }
        }
        private void MenuCopy_Click(object sender, RoutedEventArgs e)
        {
            // Если контекстное меню вызвано, а выделения нет — копировать нечего
            if (ViewModel.SelectionStart == -1) return;
            CopySelection();
        }

        private void MenuPaste_Click(object sender, RoutedEventArgs e)
        {
            // Если меню вызвали по кнопке, но перед этим кликнули правой кнопкой на байт —
            // контекст элемента (DataContext) поможет определить, куда кликнули.
            if (sender is MenuFlyoutItem menuItem && menuItem.DataContext is HexByteItem byteItem)
            {
                // Если этот байт не входит в текущее выделение, принудительно перенесем курсор на него
                if (!ViewModel.IsByteSelected(byteItem.AbsoluteAddress))
                {
                    ViewModel.SelectionStart = byteItem.AbsoluteAddress;
                    ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                    ViewModel.NotifySelectionChanged();
                }
            }

            PasteSelection();
        }

        // Метод запускает массовый поиск и выводит боковую панель
        public async Task ExecuteSearchAsync(byte[] pattern, string originalQuery)
        {
            // 1. Ищем все совпадения в фоновом потоке
            var addresses = await ViewModel.Buffer.FindAllPatternsAsync(pattern);

            if (addresses.Count > 0)
            {
                var displayItems = new List<SearchResultItem>();

                foreach (long addr in addresses)
                {
                    // Генерируем небольшое текстовое превью для списка (например, 8 байт после находки)
                    int previewSize = (int)Math.Min(8, ViewModel.Buffer.Length - addr);
                    byte[] previewBytes = new byte[previewSize];
                    ViewModel.Buffer.ReadRange(addr, previewBytes, previewSize);
                    string hexPreview = BitConverter.ToString(previewBytes).Replace("-", " ");

                    displayItems.Add(new SearchResultItem
                    {
                        Address = addr,
                        PreviewText = $"[{hexPreview}]"
                    });
                }

                // 2. Выводим результаты в UI
                SearchResultsList.ItemsSource = displayItems;
                SearchResultsPanel.Visibility = Visibility.Visible; // Показываем панель

                // Сразу переходим к первому найденному элементу
                SearchResultsList.SelectedIndex = 0;
            }
            else
            {
                SearchResultsPanel.Visibility = Visibility.Collapsed; // Прячем, если ничего не нашли
            }
        }

        // Клик по найденному элементу в списке справо
        private void SearchResultsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SearchResultsList.SelectedItem is SearchResultItem selectedItem)
            {
                long targetAddress = selectedItem.Address;

                // Выделяем найденный блок байт (длина берется условная или равная запросу)
                ViewModel.SelectionStart = targetAddress;
                // Для простоты подсветим первый байт совпадения
                ViewModel.SelectionEnd = targetAddress;
                ViewModel.NotifySelectionChanged();

                // Рассчитываем строку и прокручиваем таблицу
                int targetRowIndex = (int)(targetAddress / 16);
                ScrollToRow(targetRowIndex);
            }
        }
        private void CloseSearchPanel_Click(object sender, RoutedEventArgs e)
        {
            // Просто скрываем боковую панель поиска
            SearchResultsPanel.Visibility = Visibility.Collapsed;
        }

        // Публичный метод для открытия панели метаданных
        public void ToggleFileInfoPanel(bool show)
        {
            if (show)
            {
                // Наполняем метаданные файла
                InfoFileName.Text = ViewModel.FileName;
                InfoFilePath.Text = ViewModel.FullPath;

                FileInfoPanel.Visibility = Visibility.Visible;
                UpdateDataInspector(); // Обновляем инспектор
            }
            else
            {
                FileInfoPanel.Visibility = Visibility.Collapsed;
            }
        }

        private void CloseFileInfoPanel_Click(object sender, RoutedEventArgs e)
        {
            ToggleFileInfoPanel(false);
        }

        // Вызов обновления инспектора при изменении выделения (добавьте вызов этого метода в самый конец метода UpdateBordersVisuals)
        private void UpdateDataInspector()
        {
            if (FileInfoPanel.Visibility == Visibility.Collapsed) return;

            // Считываем позицию курсора (SelectionStart)
            long addr = ViewModel.SelectionStart;
            if (addr == -1 || addr >= ViewModel.Buffer.Length)
            {
                ResetInspectorText();
                return;
            }

            // Буфер для чтения структуры данных (максимум 8 байт для Int64)
            byte[] inspectBuffer = new byte[8];
            int toRead = (int)Math.Min(8, ViewModel.Buffer.Length - addr);
            ViewModel.Buffer.ReadRange(addr, inspectBuffer, toRead);

            // Расшифровываем байты в различные типы данных (с проверкой на доступный размер буфера)
            InspectByte.Text = inspectBuffer[0].ToString();
            InspectInt16.Text = toRead >= 2 ? BitConverter.ToInt16(inspectBuffer, 0).ToString() : "-";
            InspectInt32.Text = toRead >= 4 ? BitConverter.ToInt32(inspectBuffer, 0).ToString() : "-";
            InspectInt64.Text = toRead >= 8 ? BitConverter.ToInt64(inspectBuffer, 0).ToString() : "-";
            InspectFloat.Text = toRead >= 4 ? BitConverter.ToSingle(inspectBuffer, 0).ToString("F4") : "-";
        }

        private void ResetInspectorText()
        {
            InspectByte.Text = "-";
            InspectInt16.Text = "-";
            InspectInt32.Text = "-";
            InspectInt64.Text = "-";
            InspectFloat.Text = "-";
        }
    }
}
