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

                    // Заполняем внутренние свойства и метаданные существующего объекта
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
