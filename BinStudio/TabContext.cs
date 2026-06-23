using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio
{
    // Контекст одной вкладки документа
    public class TabContext : INotifyPropertyChanged, IDisposable
    {
        private FileStream _stream;
        private const int BytesPerRow = 16;

        private double _scrollMax;
        private double _scrollValue;
        public int VisibleRowsCount { get; } = 35; // Фиксированный размер пула строк

        public string FileName { get; set; }
        public ObservableCollection<HexRowViewModel> VisibleRows { get; } = new ObservableCollection<HexRowViewModel>();

        // НОВЫЕ СВОЙСТВА ДЛЯ КУРСОРA ВНУТРИ ВКЛАДКИ
        private double _cursorX;
        private double _cursorY;
        private Visibility _cursorVisibility = Visibility.Collapsed;
        // Глобальные маркеры выделения диапазона байт в файле
        public long SelectionStartOffset { get; set; } = -1;
        public long SelectionEndOffset { get; set; } = -1;
        // БУФЕР ИЗМЕНЕНИЙ: Хранит [Глобальное смещение -> Новый байт]
        public Dictionary<long, byte> ModifiedBytes { get; } = new Dictionary<long, byte>();

        // Состояние режима редактирования
        public bool IsEditingMode { get; set; } = false;
        public bool IsSelecting { get; set; } = false;
        // Метод для проверки, попадает ли конкретный байт в выделенный диапазон
        private bool _isMouseInitialized = false; // Флаг защиты от повторной подписки

        // МЕТОД ИНИЦИАЛИЗАЦИИ ИЗ СИСТЕМЫ С КЛИКАМИ
        // МЕТОД ИНИЦИАЛИЗАЦИИ КЛИКОВ МЫШИ ДЛЯ ТЕКУЩЕГО ДОКУМЕНТА
        public void EditorGrid_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Если для этой вкладки мышь уже была настроена — ничего не делаем
            if (_isMouseInitialized) return;

            if (sender is Microsoft.UI.Xaml.Controls.Grid grid)
            {
                // Находим ListView по имени внутри этой конкретной вкладки
                var listView = grid.FindName("HexListView") as Microsoft.UI.Xaml.Controls.ListView;
                if (listView != null)
                {
                    // Принудительно регистрируем кастомные обработчики кликов левой кнопкой мыши (handledEventsToo: true)
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.PointerPressedEvent, new Microsoft.UI.Xaml.Input.PointerEventHandler(ListView_PointerPressed), true);
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.PointerMovedEvent, new Microsoft.UI.Xaml.Input.PointerEventHandler(ListView_PointerMoved), true);
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.PointerReleasedEvent, new Microsoft.UI.Xaml.Input.PointerEventHandler(ListView_PointerReleased), true);

                    // Также жестко привязываем обработчик двойного клика для входа в редактирование
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.DoubleTappedEvent, new Microsoft.UI.Xaml.Input.DoubleTappedEventHandler(ListView_DoubleTapped), true);

                    _isMouseInitialized = true; // Фиксируем успешную подписку
                    System.Diagnostics.Debug.WriteLine($"[BinStudio] Левая кнопка мыши и DoubleTapped успешно привязаны один раз для вкладки: {FileName}");
                }
            }
        }
        // МЕТОД ПРОКРУТКИ КОЛЕСИКОМ МЫШИ ВНУТРИ TABCONTEXT
        public void Editor_PointerWheelChanged(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.Controls.Grid grid)
            {
                var pointerPoint = e.GetCurrentPoint(grid);
                int delta = pointerPoint.Properties.MouseWheelDelta;

                long step = 3; // Шаг прокрутки (по 3 строки)
                long newValue = (long)ScrollValue;

                if (delta > 0) // Скролл вверх
                {
                    newValue = Math.Max(0, newValue - step);
                }
                else // Скролл вниз
                {
                    newValue = (long)Math.Min(ScrollMax, newValue + step);
                }

                // Прямо внутри контекста обновляем значения и подгружаем байты
                ScrollValue = newValue;
                LoadVisibleData(newValue);

                e.Handled = true; // Перехватываем событие, чтобы оно не уходило дальше
            }
        }
        public double ScrollMax
        {
            get => _scrollMax;
            set { _scrollMax = value; OnPropertyChanged(); }
        }

        public double ScrollValue
        {
            get => _scrollValue;
            set { _scrollValue = value; OnPropertyChanged(); }
        }
        // Координата X рамки выделения
        public double CursorX
        {
            get => _cursorX;
            set { _cursorX = value; OnPropertyChanged(); }
        }

        // Координата Y рамки выделения
        public double CursorY
        {
            get => _cursorY;
            set { _cursorY = value; OnPropertyChanged(); }
        }

        // Видимость рамки выделения
        public Visibility CursorVisibility
        {
            get => _cursorVisibility;
            set { _cursorVisibility = value; OnPropertyChanged(); }
        }

        public TabContext(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            _stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.SequentialScan);

            // Форматируем размер файла с разделением тысяч для читаемости
            long fileLength = _stream.Length;
            double mbSize = fileLength / (1024.0 * 1024.0);
            FileSizeText = $"{fileLength:N0} байт ({mbSize:F2} МБ)".Replace(",", " ");


            long totalRows = (long)Math.Ceiling((double)_stream.Length / BytesPerRow);
            ScrollMax = Math.Max(0, totalRows - VisibleRowsCount);

            // ИНИЦИАЛИЗАЦИЯ ПУЛА: Создаем 35 объектов строк один раз на всю жизнь вкладки
            for (int i = 0; i < VisibleRowsCount; i++)
            {
                VisibleRows.Add(new HexRowViewModel());
            }

            // Загружаем начальные данные
            LoadVisibleData(0);
        }
        // Вспомогательный метод для обновления текста выделенного диапазона
        public void UpdateSelectionStatusText()
        {
            // Если ничего не выделено
            if (SelectionStartOffset == -1 || SelectionEndOffset == -1)
            {
                SelectionText = "нет выделения";
                return;
            }

            long min = Math.Min(SelectionStartOffset, SelectionEndOffset);
            long max = Math.Max(SelectionStartOffset, SelectionEndOffset);
            long count = max - min + 1;

            if (count == 1)
            {
                // Одиночный байт
                SelectionText = $"0x{min:X8}";
            }
            else
            {
                // Диапазон байт
                SelectionText = $"0x{min:X8} - 0x{max:X8} ({count:N0} байт)";
            }
        }
        // БЕЗМИГАТЕЛЬНАЯ ЛЕНИВАЯ ЗАГРУЗКА: Перезаписываем текст внутри существующих объектов
        public void LoadVisibleData(long startRowIndex)
        {
            if (_stream == null) return;

            // При любой прокрутке скрываем курсор вкладки, так как байт уходит с экрана
            CursorVisibility = Visibility.Collapsed;

            // КРИТИЧЕСКИ ВАЖНО: Больше НЕ вызываем VisibleRows.Clear()!
            byte[] buffer = new byte[BytesPerRow];
            StringBuilder hexBuilder = new StringBuilder(48);
            StringBuilder asciiBuilder = new StringBuilder(16);

            for (int i = 0; i < VisibleRowsCount; i++)
            {
                long currentRowIndex = startRowIndex + i;
                long offset = currentRowIndex * BytesPerRow;

                // Достаем уже существующий объект из пула строк вместо создания нового через 'new'
                var rowViewModel = VisibleRows[i];

                if (offset < _stream.Length)
                {
                    _stream.Position = offset;
                    int bytesRead = _stream.Read(buffer, 0, BytesPerRow);

                    hexBuilder.Clear();
                    asciiBuilder.Clear();

                    // СНАЧАЛА привязываем родительский контекст, а затем пишем строки
                    rowViewModel.ParentContext = this;
                    rowViewModel.Address = offset.ToString("X8");
                    rowViewModel.RowOffset = offset;
                    rowViewModel.BytesCount = bytesRead;

                    for (int j = 0; j < BytesPerRow; j++)
                    {
                        if (j < bytesRead)
                        {
                            byte b = buffer[j];
                            rowViewModel.RawBytes[j] = b;
                            hexBuilder.Append(b.ToString("X2")).Append(' ');
                            asciiBuilder.Append(b >= 32 && b <= 126 ? (char)b : '.');
                        }
                        else
                        {
                            hexBuilder.Append("   ");
                            asciiBuilder.Append(' ');
                        }
                    }

                    rowViewModel.HexLine = hexBuilder.ToString().TrimEnd();
                    rowViewModel.AsciiLine = asciiBuilder.ToString();
                }
                else
                {
                    // Если файл закончился, а пул строк еще не заполнен (хвост документа)
                    rowViewModel.Address = "";
                    rowViewModel.HexLine = "";
                    rowViewModel.AsciiLine = "";
                    rowViewModel.BytesCount = 0;
                    rowViewModel.RowOffset = 0;
                }
            }
        }

        // МЕТОДЫ ЖЕСТОВ МЫШИ, ВЫНЕСЕННЫЕ В TABCONTEXT
        // Логика управления выделением мыши (Добавим сброс режима редактирования при клике)
        // 1. ОДИНОЧНЫЙ КЛИК МЫШИ (Выделение диапазона)
        public void ListView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(sender as UIElement);
            if (!pointerPoint.Properties.IsLeftButtonPressed) return;

            if (sender is ListView listView)
            {
                IsEditingMode = false; // Сбрасываем редактирование при обычном клике
                OnPropertyChanged(nameof(IsEditingMode)); // Сообщаем StatusBar, что режим сменился на Навигацию
                _isHighNibbleEntered = false;

                double x = pointerPoint.Position.X;
                double y = pointerPoint.Position.Y;

                // 5.0 — это Top Padding у ListView. Высота строки строго 24px.
                int rowIndex = (int)((y - 5.0) / 24.0);

                if (rowIndex >= 0 && rowIndex < VisibleRows.Count)
                {
                    var row = VisibleRows[rowIndex];
                    int byteIndex = -1;

                    // ЗОНА HEX: начинается со 102.0px (90px смещение + 12px Padding списка)
                    // Длина всей зоны HEX: 16 байт * 3 символа ("FF ") * 7.7px = ~370px
                    if (x >= 102.0 && x < 472.0)
                    {
                        double localX = x - 102.0;
                        byteIndex = (int)(localX / 23.1); // 3 символа ("FF ") = 3 * 7.7 = 23.1px
                    }
                    // ЗОНА ASCII: начинается примерно с 520px
                    else if (x >= 520.0)
                    {
                        double localX = x - 520.0;
                        byteIndex = (int)(localX / 7.7); // 1 символ ASCII = 7.7px
                    }

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        IsSelecting = true;
                        SelectionStartOffset = row.RowOffset + byteIndex;
                        SelectionEndOffset = SelectionStartOffset;

                        listView.CapturePointer(e.Pointer);
                        RefreshVisibleInlines();
                        e.Handled = true;
                    }
                }
                UpdateSelectionStatusText(); // Обновляем адрес
            }
        }
        // 2. ДВИЖЕНИЕ МЫШИ (Шлейф выделения) — также через GetCharacterIndexAtPoint
        // 2. ДВИЖЕНИЕ МЫШИ (Отрисовка шлейфа выделения)
        public void ListView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is ListView listView && IsSelecting)
            {
                var point = e.GetCurrentPoint(listView);
                double x = point.Position.X;
                double y = point.Position.Y;

                int rowIndex = (int)((y - 5.0) / 24.0);
                rowIndex = Math.Clamp(rowIndex, 0, VisibleRows.Count - 1);
                var row = VisibleRows[rowIndex];

                int byteIndex = -1;
                if (x >= 102.0 && x < 472.0)
                {
                    byteIndex = (int)((x - 102.0) / 23.1);
                }
                else if (x >= 520.0)
                {
                    byteIndex = (int)((x - 520.0) / 7.7);
                }

                if (byteIndex != -1)
                {
                    byteIndex = Math.Clamp(byteIndex, 0, row.BytesCount - 1);
                    long currentOffset = row.RowOffset + byteIndex;

                    if (SelectionEndOffset != currentOffset)
                    {
                        SelectionEndOffset = currentOffset;
                        UpdateSelectionStatusText(); // Обновляем диапазон при протягивании!
                        RefreshVisibleInlines();
                    }
                }
            }
        }
        public void ListView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is ListView listView && IsSelecting)
            {
                IsSelecting = false;
                listView.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }

        // Проверка вхождения байта в диапазон выделения
        public bool IsByteSelected(long globalOffset)
        {
            if (SelectionStartOffset == -1 || SelectionEndOffset == -1) return false;

            long min = Math.Min(SelectionStartOffset, SelectionEndOffset);
            long max = Math.Max(SelectionStartOffset, SelectionEndOffset);

            return globalOffset >= min && globalOffset <= max;
        }
        public void SaveByteToFile(long globalOffset, byte newValue)
        {
            if (_stream == null) return;
            _stream.Position = globalOffset;
            _stream.WriteByte(newValue);
            _stream.Flush();
        }

        // Заставляет все строки на экране принудительно перерисовать свои элементы Run
        public void RefreshVisibleInlines()
        {
            foreach (var row in VisibleRows)
            {
                row.TriggerRefresh();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _stream = null;
        }
        private bool _isHighNibbleEntered = false; // Флаг: введен ли первый полубайт
        private char _highNibbleChar = '0';         // Хранилище для первого полубайта

        // ОБРАБОТЧИК КЛАВИАТУРЫ ДЛЯ РЕДАКТИРОВАНИЯ БАЙТ
        public void ListView_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            // Если ничего не выделено глобально (или выделен диапазон, а не один байт) — игнорируем
            if (!IsEditingMode || SelectionStartOffset == -1 || SelectionStartOffset != SelectionEndOffset) return;

            long targetOffset = SelectionStartOffset;

            // Ищем, в какой из видимых 35 строк находится этот offset
            HexRowViewModel activeRow = null;
            int targetByteIndex = -1;

            foreach (var row in VisibleRows)
            {
                if (targetOffset >= row.RowOffset && targetOffset < row.RowOffset + row.BytesCount)
                {
                    activeRow = row;
                    targetByteIndex = (int)(targetOffset - row.RowOffset);
                    break;
                }
            }

            if (activeRow == null || targetByteIndex == -1) return;

            // Переводим нажатую клавишу в символ верхнего регистра
            string keyStr = e.Key.ToString();
            char inputChar = ' ';

            // Обработка цифрового блока (0-9)
            if (keyStr.StartsWith("Number"))
                inputChar = keyStr[^1];
            else if (keyStr.Length == 2 && keyStr.StartsWith("D")) // Клавиши D0-D9
                inputChar = keyStr[1];
            else if (keyStr.Length == 1) // Клавиши A-F
                inputChar = keyStr[0];

            // Проверяем, является ли символ валидным Hex (0-9, A-F)
            if (!((inputChar >= '0' && inputChar <= '9') || (inputChar >= 'A' && inputChar <= 'F'))) return;

            e.Handled = true; // Перехватываем символ, чтобы фокус не ушел

            if (!_isHighNibbleEntered)
            {
                // ШАГ 1: Введен первый полубайт
                _highNibbleChar = inputChar;
                _isHighNibbleEntered = true;

                // Временно отображаем в строке первый символ и точку (например, "A.") для обратной связи в UI
                string[] parts = activeRow.HexLine.Split(' ');
                parts[targetByteIndex] = inputChar + ".";
                activeRow.HexLine = string.Join(' ', parts);
                activeRow.TriggerRefresh();
            }
            else
            {
                // ШАГ 2: Введен второй полубайт. Собираем полную строку байта (например, "A5")
                // Собираем полную строку байта (например, "A5")
                string fullHexByte = $"{_highNibbleChar}{inputChar}";
                byte newByteValue = Convert.ToByte(fullHexByte, 16);

                activeRow.RawBytes[targetByteIndex] = newByteValue;

                // Добавляем в буфер изменений памяти
                if (ModifiedBytes.ContainsKey(targetOffset))
                    ModifiedBytes[targetOffset] = newByteValue;
                else
                    ModifiedBytes.Add(targetOffset, newByteValue);

                // Пересобираем текстовую строку HexLine и AsciiLine
                activeRow.UpdateHexLineFromRaw();

                // КРИТИЧЕСКИЙ ВАЖНЫЙ ВЫЗОВ: Принудительно пинаем HexTextBehavior,
                // чтобы он заново перерисовал Run-ы и окрасил этот байт в КРАСНЫЙ цвет!
                activeRow.TriggerRefresh();

                _isHighNibbleEntered = false; // Сбрасываем флаг полубайта

                // Автоматический сдвиг курсора на следующий байт
                if (targetOffset + 1 < _stream.Length)
                {
                    SelectionStartOffset++;
                    SelectionEndOffset = SelectionStartOffset;
                    UpdateSelectionStatusText(); // Сдвигаем адрес в StatusBar вместе с шагом клавиатуры!

                    if (targetByteIndex == 15)
                    {
                        long currentScroll = (long)ScrollValue;
                        if (currentScroll + VisibleRowsCount < ScrollMax + VisibleRowsCount)
                        {
                            ScrollValue = currentScroll + 1;
                            LoadVisibleData(currentScroll + 1);
                            SelectionStartOffset = (currentScroll + 1) * 16;
                            SelectionEndOffset = SelectionStartOffset;
                        }
                    }
                    RefreshVisibleInlines();
                }
            }
        }
        // МЕТОД КНОПКИ «СОХРАНИТЬ» (Вызывается из верхнего меню)
        public void SaveAllChanges()
        {
            if (_stream == null || ModifiedBytes.Count == 0) return;

            // Записываем все изменения на диск одной транзакцией
            foreach (var kvp in ModifiedBytes)
            {
                _stream.Position = kvp.Key;
                _stream.WriteByte(kvp.Value);
            }
            _stream.Flush();

            ModifiedBytes.Clear(); // Очищаем буфер памяти
            LoadVisibleData((long)ScrollValue); // Перечитываем экран (красный цвет пропадет, станет обычным)
        }
        // 3. ДВОЙНОЙ КЛИК (Вход в режим редактирования)
        public void ListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var point = e.GetPosition(listView);
                double x = point.X;
                double y = point.Y;

                int rowIndex = (int)((y - 5.0) / 24.0);

                if (rowIndex >= 0 && rowIndex < VisibleRows.Count)
                {
                    var row = VisibleRows[rowIndex];
                    int byteIndex = -1;

                    if (x >= 102.0 && x < 472.0)
                    {
                        byteIndex = (int)((x - 102.0) / 23.1);
                    }
                    else if (x >= 520.0)
                    {
                        byteIndex = (int)((x - 520.0) / 7.7);
                    }

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        IsEditingMode = true;
                        _isHighNibbleEntered = false; // Сбрасываем полушаги ввода

                        SelectionStartOffset = row.RowOffset + byteIndex;
                        SelectionEndOffset = SelectionStartOffset;

                        RefreshVisibleInlines();

                        // Принудительно ставим фокус на ListView, чтобы клавиатура заработала мгновенно
                        listView.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);

                        System.Diagnostics.Debug.WriteLine($"[BinStudio] Редактирование ВКЛЮЧЕНО для байта №{byteIndex}, смещение: 0x{SelectionStartOffset:X8}");
                        e.Handled = true;
                    }
                }
            }
        }
        private string _fileSizeText = "0 байт";
        private string _selectionText = "нет выделения";
        // Свойство текстового представления размера файла (например: "1 048 576 байт (1.00 МБ)")
        public string FileSizeText
        {
            get => _fileSizeText;
            set { _fileSizeText = value; OnPropertyChanged(); }
        }

        // Свойство динамического текста выделения (например: "0x00000010 - 0x0000001F (16 байт)")
        public string SelectionText
        {
            get => _selectionText;
            set { _selectionText = value; OnPropertyChanged(); }
        }
        // ДВОЙНОЙ КЛИК: Точный вход в режим редактирования по ширине символа Consolas (7.7px)
        public void TextBlock_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            if (sender is Microsoft.UI.Xaml.Controls.TextBlock textBlock)
            {
                var point = e.GetPosition(textBlock);
                double x = point.X;

                if (textBlock.DataContext is HexRowViewModel row)
                {
                    int byteIndex = -1;

                    // Определяем, в какой колонке кликнули дважды
                    if (textBlock.Width == 410) // Колонна HEX
                    {
                        byteIndex = (int)(x / 23.1); // 3 символа "FF " = 3 * 7.7 = 23.1px
                    }
                    else // Колонна ASCII
                    {
                        byteIndex = (int)(x / 7.7); // 1 символ ASCII = 7.7px
                    }

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        IsEditingMode = true;
                        _isHighNibbleEntered = false; // Сбрасываем шаги ввода полубайтов
                        OnPropertyChanged(nameof(IsEditingMode)); // Обновляем режим в StatusBar

                        SelectionStartOffset = row.RowOffset + byteIndex;
                        SelectionEndOffset = SelectionStartOffset;

                        UpdateSelectionStatusText(); // Обновляем адрес в StatusBar
                        RefreshVisibleInlines();     // Перерисовываем маркеры на экране

                        // Находим родительский ListView и принудительно даем ему фокус, 
                        // чтобы клавиатура (KeyDown) сразу перехватывала нажатия
                        DependencyObject parent = textBlock;
                        while (parent != null && !(parent is Microsoft.UI.Xaml.Controls.ListView))
                        {
                            parent = Microsoft.UI.Xaml.Media.VisualTreeHelper.GetParent(parent);
                        }

                        if (parent is Microsoft.UI.Xaml.Controls.ListView listView)
                        {
                            listView.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
                        }

                        System.Diagnostics.Debug.WriteLine($"[BinStudio] Редактирование ВКЛЮЧЕНО для байта №{byteIndex}, смещение: 0x{SelectionStartOffset:X8}");
                        e.Handled = true;
                    }
                }
            }
        }

    }
}
