using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace BinViewer
{
    public class HexDocumentViewModel
    {
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public VirtualFileBuffer Buffer { get; set; }
        public VirtualHexRowsList Rows { get; set; }

        public HexDocumentViewModel(string filePath)
        {
            FullPath = filePath;
            FileName = Path.GetFileName(filePath);
            Buffer = new VirtualFileBuffer();
            Buffer.Open(filePath);
            Rows = new VirtualHexRowsList(Buffer);
        }
    }
    // Виртуализированный список строк для UI
    public class VirtualHexRowsList : IReadOnlyList<HexRow>
    {
        private readonly VirtualFileBuffer _buffer;
        private const int BytesPerRow = 16;

        public VirtualHexRowsList(VirtualFileBuffer buffer) => _buffer = buffer;

        public int Count => (int)Math.Ceiling((double)_buffer.Length / BytesPerRow);

        public HexRow this[int index]
        {
            get
            {
                long position = (long)index * BytesPerRow;

                // Вычисляем, сколько реальных байт осталось до конца файла
                int realBytesCount = (int)Math.Min(BytesPerRow, _buffer.Length - position);

                // Буфер ВСЕГДА создаем фиксированного размера — 16 байт
                byte[] bytes = new byte[BytesPerRow];

                // Считываем доступные байты из файла
                _buffer.ReadRange(position, bytes, realBytesCount);

                return new HexRow
                {
                    Address = position,
                    Bytes = bytes,
                    ValidBytesCount = realBytesCount // Передаем информацию, сколько байт реальные
                };
            }
        }

        public IEnumerator<HexRow> GetEnumerator() { throw new NotImplementedException("Для виртуализации UI итератор не нужен"); }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
