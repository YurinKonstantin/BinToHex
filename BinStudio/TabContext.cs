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
        public bool IsSelecting { get; set; } = false;
        // Метод для проверки, попадает ли конкретный байт в выделенный диапазон
        private bool _isMouseInitialized = false; // Флаг защиты от повторной подписки

        // МЕТОД ИНИЦИАЛИЗАЦИИ ИЗ СИСТЕМЫ С КЛИКАМИ
        public void EditorGrid_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            // Если мы уже настроили левую кнопку мыши для этой вкладки — выходим
            if (_isMouseInitialized) return;

            if (sender is Grid grid)
            {
                var listView = grid.FindName("HexListView") as ListView;
                if (listView != null)
                {
                    // Регистрируем кастомные обработчики с флагом handledEventsToo = true
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.PointerPressedEvent, new PointerEventHandler(ListView_PointerPressed), true);
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.PointerMovedEvent, new PointerEventHandler(ListView_PointerMoved), true);
                    listView.AddHandler(Microsoft.UI.Xaml.UIElement.PointerReleasedEvent, new PointerEventHandler(ListView_PointerReleased), true);

                    _isMouseInitialized = true; // Фиксируем, что подписка выполнена строго один раз!
                    System.Diagnostics.Debug.WriteLine($"[BinStudio] Левая кнопка мыши успешно привязана один раз для вкладки: {FileName}");
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
        public void ListView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(sender as UIElement);

            // Проверяем, что нажата именно ЛЕВАЯ кнопка мыши
            if (!pointerPoint.Properties.IsLeftButtonPressed) return;

            if (sender is ListView listView)
            {
                double x = pointerPoint.Position.X;
                double y = pointerPoint.Position.Y;

                int rowIndex = (int)((y - 5.0) / 24.0);
                int byteIndex = (int)((x - 102.0) / 24.3);

                if (rowIndex >= 0 && rowIndex < VisibleRows.Count && byteIndex >= 0 && byteIndex < 16)
                {
                    var row = VisibleRows[rowIndex];
                    if (byteIndex < row.BytesCount)
                    {
                        IsSelecting = true;
                        SelectionStartOffset = row.RowOffset + byteIndex;
                        SelectionEndOffset = SelectionStartOffset;

                        listView.CapturePointer(e.Pointer);
                        RefreshVisibleInlines();
                        e.Handled = true;
                    }
                }
                else
                {
                    SelectionStartOffset = -1;
                    SelectionEndOffset = -1;
                    RefreshVisibleInlines();
                }
            }
        }
        public void ListView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is ListView listView && IsSelecting)
            {
                var point = e.GetCurrentPoint(listView);
                double x = point.Position.X;
                double y = point.Position.Y;

                int rowIndex = (int)((y - 5.0) / 24.0);
                int byteIndex = (int)((x - 102.0) / 24.3); // Учитываем 102px вместо 90px

                rowIndex = Math.Clamp(rowIndex, 0, VisibleRows.Count - 1);
                byteIndex = Math.Clamp(byteIndex, 0, 15);

                var row = VisibleRows[rowIndex];
                long currentOffset = row.RowOffset + Math.Min(byteIndex, row.BytesCount - 1);

                if (SelectionEndOffset != currentOffset)
                {
                    SelectionEndOffset = currentOffset;
                    RefreshVisibleInlines(); // Динамически перерисовываем синий шлейф протягивания
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
    }
}
