using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio
{
    public class HexDocument : IDisposable
    {
        private FileStream _fileStream;
        public string ContentHeader { get; set; } // Имя файла для вкладки
        public long FileLength => _fileStream?.Length ?? 0;
        public int BytesPerRow { get; set; } = 16;

        // Общее количество строк в файле
        public long TotalRows => (long)Math.Ceiling((double)FileLength / BytesPerRow);

        public HexDocument(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            ContentHeader = Path.GetFileName(filePath);
        }

        // Чтение конкретной строки из файла
        public byte[] GetRowBytes(long rowIndex)
        {
            long offset = rowIndex * BytesPerRow;
            if (offset >= FileLength) return Array.Empty<byte>();

            int bytesToRead = (int)Math.Min(BytesPerRow, FileLength - offset);
            byte[] buffer = new byte[bytesToRead];

            _fileStream.Position = offset;
            _fileStream.Read(buffer, 0, bytesToRead);

            return buffer;
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
        }
    }
}
