using BinStudio.Services;
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
        public int VisibleRowsCount { get; } = 35;
        public FileBufferService BufferService { get; private set; }
        public SelectionService SelectionManager { get; private set; }
        public string FileName { get; set; }
        public ObservableCollection<HexRowViewModel> VisibleRows { get; } = new ObservableCollection<HexRowViewModel>();

        public long TotalRowsCount { get; private set; }
        public double ScrollMax { get; private set;  } // Переименовано для работы биндингов
        private double _scrollMax;
        public double ScrollMaxProperty
        {
            get => _scrollMax;
            set { _scrollMax = value; OnPropertyChanged(nameof(ScrollMaxProperty)); }
        }

        private double _scrollValue;
        public double ScrollValue
        {
            get => _scrollValue;
            set { _scrollValue = value; OnPropertyChanged(); }
        }

        public bool IsEditingMode => SelectionManager.IsEditingMode;
        public string SelectionText => SelectionManager.GetSelectionStatusText();

        private string _fileSizeText;
        public string FileSizeText
        {
            get => _fileSizeText;
            set { _fileSizeText = value; OnPropertyChanged(); }
        }

        private bool _isSearchPanelOpen = false;
        public bool IsSearchPanelOpen
        {
            get => _isSearchPanelOpen;
            set { _isSearchPanelOpen = value; OnPropertyChanged(); }
        }

        public TabContext(string filePath)
        {
            FileName = Path.GetFileName(filePath);

            // Инициализируем сервисы
            BufferService = new FileBufferService(filePath);
            SelectionManager = new SelectionService();

            double mbSize = BufferService.FileLength / (1024.0 * 1024.0);
            FileSizeText = $"{BufferService.FileLength:N0} байт ({mbSize:F2} МБ)".Replace(",", " ");

            TotalRowsCount = (long)Math.Ceiling((double)BufferService.FileLength / BytesPerRow);
            ScrollMaxProperty = Math.Max(0, TotalRowsCount - VisibleRowsCount);

            for (int i = 0; i < VisibleRowsCount; i++) VisibleRows.Add(new HexRowViewModel());
            LoadVisibleData(0);
        }

        public void LoadVisibleData(long startRowIndex)
        {
            if (BufferService == null) return;

            _scrollValue = startRowIndex;
            byte[] buffer = new byte[BytesPerRow];
            StringBuilder hexBuilder = new StringBuilder(48);
            StringBuilder asciiBuilder = new StringBuilder(16);

            for (int i = 0; i < VisibleRowsCount; i++)
            {
                long currentRowIndex = startRowIndex + i;
                long offset = currentRowIndex * BytesPerRow;
                var rowViewModel = VisibleRows[i];

                if (offset < BufferService.FileLength)
                {
                    int bytesRead = BufferService.ReadBytes(offset, buffer, BytesPerRow);
                    hexBuilder.Clear();
                    asciiBuilder.Clear();

                    rowViewModel.ParentContext = this;
                    rowViewModel.Address = offset.ToString("X8");
                    rowViewModel.RowOffset = offset;
                    rowViewModel.BytesCount = bytesRead;

                    for (int j = 0; j < BytesPerRow; j++)
                    {
                        if (j < bytesRead)
                        {
                            byte b = BufferService.GetByte(offset + j, buffer[j]);
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
                    rowViewModel.ParentContext = null;
                    rowViewModel.Address = ""; rowViewModel.HexLine = ""; rowViewModel.AsciiLine = "";
                    rowViewModel.BytesCount = 0; rowViewModel.RowOffset = 0;
                }
                rowViewModel.TriggerRefresh();
            }
        }

        public bool IsByteSelected(long offset) => SelectionManager.IsByteSelected(offset);
        public void RefreshVisibleInlines() { foreach (var row in VisibleRows) row.TriggerRefresh(); }

        public void SaveAllChanges()
        {
            BufferService.SaveAllChanges();
            LoadVisibleData((long)ScrollValue);
        }

        // ПЕРЕНРАВЛЕННЫЕ ИНТЕРФЕЙСНЫЕ МЕТОДЫ МЫШИ/КЛАВИАТУРЫ
        private bool _isMouseInitialized = false;
        public void EditorGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (_isMouseInitialized) return;
            if (sender is Grid grid && grid.FindName("HexListView") is ListView listView)
            {
                listView.AddHandler(UIElement.PointerPressedEvent, new PointerEventHandler(ListView_PointerPressed), true);
                listView.AddHandler(UIElement.PointerMovedEvent, new PointerEventHandler(ListView_PointerMoved), true);
                listView.AddHandler(UIElement.PointerReleasedEvent, new PointerEventHandler(ListView_PointerReleased), true);
                listView.AddHandler(UIElement.DoubleTappedEvent, new DoubleTappedEventHandler(ListView_DoubleTapped), true);
                _isMouseInitialized = true;
            }
        }

        public void ListView_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerPoint = e.GetCurrentPoint(sender as UIElement);
            if (!pointerPoint.Properties.IsLeftButtonPressed || !(sender is ListView listView)) return;

            double x = pointerPoint.Position.X; double y = pointerPoint.Position.Y;
            int rowIndex = (int)((y - 5.0) / 24.0);
            int byteIndex = (x >= 102.0 && x < 472.0) ? (int)((x - 102.0) / 23.1) : (x >= 520.0 ? (int)((x - 520.0) / 7.7) : -1);

            if (rowIndex >= 0 && rowIndex < VisibleRows.Count && byteIndex >= 0 && byteIndex < VisibleRows[rowIndex].BytesCount)
            {
                SelectionManager.StartSelection(VisibleRows[rowIndex].RowOffset + byteIndex);
                listView.CapturePointer(e.Pointer);
                NotifyStatusAndRefresh();
                e.Handled = true;
            }
            else
            {
                SelectionManager.ClearSelection();
                NotifyStatusAndRefresh();
            }
        }

        public void ListView_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (sender is ListView listView && SelectionManager.IsSelecting)
            {
                var point = e.GetCurrentPoint(listView);
                int rowIndex = Math.Clamp((int)((point.Position.Y - 5.0) / 24.0), 0, VisibleRows.Count - 1);
                double x = point.Position.X;
                int byteIndex = (x >= 102.0 && x < 472.0) ? (int)((x - 102.0) / 23.1) : (x >= 520.0 ? (int)((x - 520.0) / 7.7) : -1);

                if (byteIndex != -1)
                {
                    var row = VisibleRows[rowIndex];
                    byteIndex = Math.Clamp(byteIndex, 0, row.BytesCount - 1);
                    if (SelectionManager.UpdateEndOffset(row.RowOffset + byteIndex))
                    {
                        NotifyStatusAndRefresh();
                    }
                }
            }
        }

        public void ListView_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (sender is ListView listView && SelectionManager.IsSelecting)
            {
                SelectionManager.IsSelecting = false;
                listView.ReleasePointerCapture(e.Pointer);
                e.Handled = true;
            }
        }

        public void ListView_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (sender is ListView listView)
            {
                var point = e.GetPosition(listView);
                int rowIndex = (int)((point.Y - 5.0) / 24.0);
                if (rowIndex >= 0 && rowIndex < VisibleRows.Count)
                {
                    var row = VisibleRows[rowIndex];
                    double x = point.X;
                    int byteIndex = (x >= 102.0 && x < 472.0) ? (int)((x - 102.0) / 23.1) : (x >= 520.0 ? (int)((x - 520.0) / 7.7) : -1);

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        SelectionManager.IsEditingMode = true;
                        SelectionManager.IsHighNibbleEntered = false;
                        SelectionManager.StartOffset = row.RowOffset + byteIndex;
                        SelectionManager.EndOffset = SelectionManager.StartOffset;

                        NotifyStatusAndRefresh();
                        listView.Focus(FocusState.Programmatic);
                        e.Handled = true;
                    }
                }
            }
        }
        public void ListView_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!SelectionManager.IsEditingMode || SelectionManager.StartOffset == -1 || SelectionManager.StartOffset != SelectionManager.EndOffset) return;

            long targetOffset = SelectionManager.StartOffset;
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

            string keyStr = e.Key.ToString();
            char inputChar = ' ';

            if (keyStr.StartsWith("Number"))
                inputChar = keyStr[^1];
            else if (keyStr.Length == 2 && keyStr.StartsWith("D"))
                inputChar = keyStr[^1];
            else if (keyStr.Length == 1)
                inputChar = keyStr[0];

            if (!((inputChar >= '0' && inputChar <= '9') || (inputChar >= 'A' && inputChar <= 'F'))) return;
            e.Handled = true;

            if (!SelectionManager.IsHighNibbleEntered)
            {
                SelectionManager.HighNibbleChar = inputChar;
                SelectionManager.IsHighNibbleEntered = true;

                string[] parts = activeRow.HexLine.Split(' ');
                parts[targetByteIndex] = inputChar + ".";
                activeRow.HexLine = string.Join(' ', parts);
                activeRow.TriggerRefresh();
            }
            else
            {
                string fullHexByte = $"{SelectionManager.HighNibbleChar}{inputChar}";
                byte newByteValue = Convert.ToByte(fullHexByte, 16);

                activeRow.RawBytes[targetByteIndex] = newByteValue;
                BufferService.AddModification(targetOffset, newByteValue);
                activeRow.UpdateHexLineFromRaw();
                activeRow.TriggerRefresh();

                SelectionManager.IsHighNibbleEntered = false;

                if (targetOffset + 1 < BufferService.FileLength)
                {
                    SelectionManager.StartOffset++;
                    SelectionManager.EndOffset = SelectionManager.StartOffset;

                    if (targetByteIndex == 15)
                    {
                        long currentScroll = (long)ScrollValue;
                        if (currentScroll + VisibleRowsCount < TotalRowsCount)
                        {
                            ScrollValue = currentScroll + 1;
                            LoadVisibleData(currentScroll + 1);
                            SelectionManager.StartOffset = (currentScroll + 1) * 16;
                            SelectionManager.EndOffset = SelectionManager.StartOffset;
                        }
                    }
                    NotifyStatusAndRefresh();
                }
            }
        }

        public void Editor_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                var pointerPoint = e.GetCurrentPoint(grid);
                int delta = pointerPoint.Properties.MouseWheelDelta;
                long step = 3;
                long newValue = (long)ScrollValue;

                newValue = delta > 0 ? Math.Max(0, newValue - step) : Math.Min((long)ScrollMaxProperty, newValue + step);

                ScrollValue = newValue;
                LoadVisibleData(newValue);
                e.Handled = true;
            }
        }

        private void NotifyStatusAndRefresh()
        {
            OnPropertyChanged(nameof(IsEditingMode));
            OnPropertyChanged(nameof(SelectionText));
            RefreshVisibleInlines();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void Dispose() => BufferService?.Dispose();

    }
}
