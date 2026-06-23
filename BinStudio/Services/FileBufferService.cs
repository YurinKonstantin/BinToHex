using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio.Services
{
    public class FileBufferService : IDisposable
    {
        private FileStream _stream;
        public long FileLength => _stream?.Length ?? 0;
        public Dictionary<long, byte> ModifiedBytes { get; } = new Dictionary<long, byte>();

        public FileBufferService(string filePath)
        {
            _stream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read, 4096, FileOptions.SequentialScan);
        }

        public int ReadBytes(long offset, byte[] buffer, int count)
        {
            if (_stream == null) return 0;
            _stream.Position = offset;
            return _stream.Read(buffer, 0, count);
        }

        public void AddModification(long offset, byte value)
        {
            if (ModifiedBytes.ContainsKey(offset))
                ModifiedBytes[offset] = value;
            else
                ModifiedBytes.Add(offset, value);
        }

        public byte GetByte(long offset, byte fallback)
        {
            return ModifiedBytes.TryGetValue(offset, out byte modifiedValue) ? modifiedValue : fallback;
        }

        public void SaveAllChanges()
        {
            if (_stream == null || ModifiedBytes.Count == 0) return;

            foreach (var kvp in ModifiedBytes)
            {
                _stream.Position = kvp.Key;
                _stream.WriteByte(kvp.Value);
            }
            _stream.Flush();
            ModifiedBytes.Clear();
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _stream = null;
        }
    }
}
