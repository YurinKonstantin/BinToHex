using System;
using System.Collections.Generic;
using System.IO;


namespace BinViewer
{
    public class VirtualFileBuffer : IDisposable
    {
        private FileStream _fileStream;
        // Хранит изменения: Ключ = смещение в файле, Значение = новый байт
        private readonly Dictionary<long, byte> _modifications = new();

        public long Length => _fileStream?.Length ?? 0;

        public void Open(string filePath)
        {
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
        }
        public bool HasModification(long position)
        {
            return _modifications.ContainsKey(position);
        }
        public void SetByte(long position, byte value)
        {
            // Записываем измененный байт во внутренний словарь изменений
            _modifications[position] = value;
        }
        public byte ReadByte(long position)
        {
            // Было: if (_modifications.TryGetValue(position, ref var modifiedByte))
            // Стало (исправлено на out):
            if (_modifications.TryGetValue(position, out var modifiedByte))
                return modifiedByte;

            _fileStream.Position = position;
            return (byte)_fileStream.ReadByte();
        }

        public void ReadRange(long startPosition, byte[] buffer, int count)
        {
            _fileStream.Position = startPosition;
            _fileStream.Read(buffer, 0, count);

            // Накладываем измененные пользователем байты поверх считанных
            for (int i = 0; i < count; i++)
            {
                long currentPos = startPosition + i;
                if (_modifications.TryGetValue(currentPos, out byte modifiedByte))
                {
                    buffer[i] = modifiedByte;
                }
            }
        }

        public void WriteByte(long position, byte value)
        {
            _modifications[position] = value;
        }

        public void Save()
        {
            foreach (var change in _modifications)
            {
                _fileStream.Position = change.Key;
                _fileStream.WriteByte(change.Value);
            }
            _modifications.Clear();
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
        }
    }

}
