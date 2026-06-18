using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized; // Добавьте это пространство имен

namespace BinViewer
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    public class HexDocumentViewModel : INotifyPropertyChanged
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public VirtualFileBuffer Buffer { get; set; }
        public VirtualHexRowsList Rows { get; set; }


        private bool _isFirstNibbleEntered = false;
        public bool IsFirstNibbleEntered
        {
            get => _isFirstNibbleEntered;
            set { _isFirstNibbleEntered = value; OnPropertyChanged(); }
        }

        // Метод для записи измененного байта через буфер
        public void ChangeByteAt(long address, byte newValue)
        {
            // 1. Записываем изменение в буфер
            Buffer.SetByte(address, newValue);

            // 2. Вычисляем, в какой именно строке таблицы находится этот байт
            int rowIndex = (int)(address / 16);

            // 3. Даем команду списку обновить эту строку на экране
            Rows.RefreshRow(rowIndex);
        }


        private long _selectionStart = -1;
        private long _selectionEnd = -1;

        public long SelectionStart
        {
            get => _selectionStart;
            set { _selectionStart = value; OnPropertyChanged(); }
        }

        public long SelectionEnd
        {
            get => _selectionEnd;
            set { _selectionEnd = value; OnPropertyChanged(); }
        }

        // Проверяет, выделен ли конкретный байт по его абсолютному адресу
        public bool IsByteSelected(long address)
        {
            if (SelectionStart == -1 || SelectionEnd == -1) return false;
            long min = Math.Min(SelectionStart, SelectionEnd);
            long max = Math.Max(SelectionStart, SelectionEnd);
            return address >= min && address <= max;
        }
  
        // Метод для принудительного обновления UI при выделении
        public void NotifySelectionChanged()
        {
            OnPropertyChanged(nameof(SelectionStart));
        }

        public HexDocumentViewModel(string filePath)
        {
            FullPath = filePath;
            FileName = Path.GetFileName(filePath);
            Buffer = new VirtualFileBuffer();
            Buffer.Open(filePath);
            Rows = new VirtualHexRowsList(Buffer, this); // Передаем ссылку на себя
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }

    // Виртуализированный список строк для UI
    public class VirtualHexRowsList : IReadOnlyList<HexRow>, INotifyCollectionChanged
    {
        private readonly VirtualFileBuffer _buffer;
        private const int BytesPerRow = 16;
 
        private readonly HexDocumentViewModel _viewModel;
        // Событие, которое сообщает WinUI 3 о необходимости обновить строку на экране
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public VirtualHexRowsList(VirtualFileBuffer buffer, HexDocumentViewModel viewModel)
        {
            _buffer = buffer;
            _viewModel = viewModel;
        }

        public int Count => (int)Math.Ceiling((double)_buffer.Length / BytesPerRow);

        public HexRow this[int index]
        {
            get
            {
                long position = (long)index * BytesPerRow;
                int realBytesCount = (int)Math.Min(BytesPerRow, _buffer.Length - position);
                byte[] bytes = new byte[BytesPerRow];
                _buffer.ReadRange(position, bytes, realBytesCount);

                var row = new HexRow { Address = position };
                char[] asciiChars = new char[BytesPerRow];

                for (int i = 0; i < BytesPerRow; i++)
                {
                    long currentAddress = position + i;
                    bool isValid = i < realBytesCount;
                    // Проверяем, есть ли этот байт в словаре изменений нашего буфера
                    // Для этого в класс VirtualFileBuffer нужно добавить метод: public bool HasModification(long pos) => _modifications.ContainsKey(pos);
                    bool isModified = _buffer.HasModification(currentAddress);
                    row.ByteItems.Add(new HexByteItem
                    {
                        AbsoluteAddress = currentAddress,
                        Value = bytes[i],
                        IsValid = isValid,
                        IsModified = isModified // Записываем состояние
                    });

                    if (isValid)
                        asciiChars[i] = (bytes[i] >= 32 && bytes[i] <= 126) ? (char)bytes[i] : '.';
                    else
                        asciiChars[i] = ' ';
                }

                row.AsciiString = new string(asciiChars);
                return row;
            }
        }
        public void RefreshRow(int rowIndex)
        {
            if (rowIndex >= 0 && rowIndex < Count)
            {
                var updatedRow = this[rowIndex];
                // Посылаем WinUI сигнал: "Элемент под этим индексом заменился на новый"
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(
                    NotifyCollectionChangedAction.Replace, updatedRow, updatedRow, rowIndex));
            }
        }
        public IEnumerator<HexRow> GetEnumerator() => throw new NotImplementedException();
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();



    }
}
