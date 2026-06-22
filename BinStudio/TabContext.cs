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
        private const double RowHeight = 24.0;

        private double _scrollMax;
        private double _scrollValue;
        private int _visibleRowsCount = 30;

        public string FileName { get; set; }
        public ObservableCollection<HexRowViewModel> VisibleRows { get; } = new ObservableCollection<HexRowViewModel>();

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

        public TabContext(string filePath)
        {
            FileName = Path.GetFileName(filePath);
            _stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.SequentialScan);

            long totalRows = (long)Math.Ceiling((double)_stream.Length / BytesPerRow);
            ScrollMax = Math.Max(0, totalRows - _visibleRowsCount);

            LoadVisibleData(0);
        }

        public void UpdateViewportSize(double availableHeight)
        {
            if (availableHeight <= RowHeight) return;

            int neededRows = (int)Math.Ceiling(availableHeight / RowHeight);
            if (neededRows != _visibleRowsCount && neededRows > 0)
            {
                _visibleRowsCount = neededRows;
                long totalRows = (long)Math.Ceiling((double)_stream.Length / BytesPerRow);
                ScrollMax = Math.Max(0, totalRows - _visibleRowsCount);
                LoadVisibleData((long)ScrollValue);
            }
        }

        public void LoadVisibleData(long startRowIndex)
        {
            if (_stream == null) return;

            VisibleRows.Clear();
            byte[] buffer = new byte[BytesPerRow];
            StringBuilder hexBuilder = new StringBuilder(48);
            StringBuilder asciiBuilder = new StringBuilder(16);

            for (int i = 0; i < _visibleRowsCount; i++)
            {
                long currentRowIndex = startRowIndex + i;
                long offset = currentRowIndex * BytesPerRow;

                if (offset < _stream.Length)
                {
                    _stream.Position = offset;
                    int bytesRead = _stream.Read(buffer, 0, BytesPerRow);

                    hexBuilder.Clear();
                    asciiBuilder.Clear();

                    var rowViewModel = new HexRowViewModel
                    {
                        Address = offset.ToString("X8"),
                        RowOffset = offset,
                        BytesCount = bytesRead
                    };

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
                    VisibleRows.Add(rowViewModel);
                }
            }
        }

        public void SaveByteToFile(long globalOffset, byte newValue)
        {
            if (_stream == null) return;
            _stream.Position = globalOffset;
            _stream.WriteByte(newValue);
            _stream.Flush();
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
