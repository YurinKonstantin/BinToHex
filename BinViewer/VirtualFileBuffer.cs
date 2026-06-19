using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


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
            _fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
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
        public void CreateBackup(string originalFilePath)
        {
            if (string.IsNullOrEmpty(originalFilePath) || !File.Exists(originalFilePath)) return;

            // Формируем имя файла бэкапа (например, firmware.bin.bak)
            string backupPath = originalFilePath + ".bak";

            try
            {
                // Временно закрываем основной поток, чтобы гарантированно и быстро сделать бэкап на уровне ОС
                _fileStream?.Dispose();

                // Копируем исходный файл в файл бэкапа. true — разрешает перезапись старого бэкапа
                File.Copy(originalFilePath, backupPath, true);
            }
            catch (Exception ex)
            {
                // Если бэкап не удалось создать (например, нет места на диске), выбрасываем исключение наверх
                throw new IOException($"Не удалось создать резервную копию перед сохранением: {ex.Message}", ex);
            }
            finally
            {
                // В любом случае переоткрываем поток оригинального файла для последующей записи правок
                _fileStream = new FileStream(originalFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
            }
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
            if (_fileStream == null || _modifications.Count == 0) return;

            // Последовательно переносим каждую правку из ОЗУ на жесткий диск
            foreach (var change in _modifications)
            {
                _fileStream.Position = change.Key;
                _fileStream.WriteByte(change.Value);
            }

            // Принудительно сбрасываем буферы Windows на физический накопитель
            _fileStream.Flush();

            // Очищаем список правок, так как они теперь зафиксированы в самом файле
            _modifications.Clear();
        }

        public void Dispose()
        {
            _fileStream?.Dispose();
        }
        public async Task<List<long>> FindAllPatternsAsync(byte[] pattern)
        {
            var results = new List<long>();
            if (pattern == null || pattern.Length == 0 || _fileStream == null) return results;

            await Task.Run(() =>
            {
                long totalLength = _fileStream.Length;
                byte[] chunk = new byte[65536]; // Буфер 64 КБ

                long pos = 0;
                while (pos < totalLength)
                {
                    int toRead = (int)Math.Min(chunk.Length, totalLength - pos);
                    ReadRange(pos, chunk, toRead);

                    for (int i = 0; i <= toRead - pattern.Length; i++)
                    {
                        bool match = true;
                        for (int j = 0; j < pattern.Length; j++)
                        {
                            if (chunk[i + j] != pattern[j]) { match = false; break; }
                        }

                        if (match)
                        {
                            results.Add(pos + i);
                            // Ограничим список первыми 5000 результатов, чтобы не перегружать интерфейс
                            if (results.Count >= 5000) return;
                        }
                    }

                    if (toRead < chunk.Length) break;
                    // Сдвигаем окно с учетом длины паттерна, чтобы не пропустить совпадения на стыке буферов
                    pos += chunk.Length - pattern.Length;
                }
            });

            return results;
        }
    }

}
