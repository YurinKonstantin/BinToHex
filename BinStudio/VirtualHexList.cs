using BinStudio.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections;
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
    // Умная коллекция: симулирует огромный список, читая файл «на лету»
    public class VirtualHexList : IList, IReadOnlyList<HexRowViewModel>, IDisposable
    {
        private readonly FileStream _stream;
        private const int BytesPerRow = 16;
        private readonly int _count;
        // Буфер для уменьшения аллокаций памяти при чтении
        private readonly byte[] _rowBuffer = new byte[BytesPerRow];
        private readonly StringBuilder _hexBuilder = new StringBuilder(48);
        private readonly StringBuilder _asciiBuilder = new StringBuilder(16);

        public VirtualHexList(string path)
        {
            // Открываем файл в режиме последовательного доступа и отключаем лишние буферы ОС
            _stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.SequentialScan);
            _count = (int)Math.Ceiling((double)_stream.Length / BytesPerRow);
        }

        // ItemsRepeater запрашивает конкретную строку по индексу, когда она появляется на экране
        public HexRowViewModel this[int index]
        {
            get
            {
                long offset = (long)index * BytesPerRow;
                if (offset >= _stream.Length) return null;

                _stream.Position = offset;
                int bytesRead = _stream.Read(_rowBuffer, 0, BytesPerRow);

                _hexBuilder.Clear();
                _asciiBuilder.Clear();

                for (int i = 0; i < BytesPerRow; i++)
                {
                    if (i < bytesRead)
                    {
                        byte b = _rowBuffer[i];
                        _hexBuilder.Append(b.ToString("X2")).Append(' ');
                        _asciiBuilder.Append(b >= 32 && b <= 126 ? (char)b : '.');
                    }
                    else
                    {
                        _hexBuilder.Append("   ");
                        _asciiBuilder.Append(' ');
                    }
                }

                // Возвращаем легковесную структуру данных для одной строки
                return new HexRowViewModel
                {
                    Address = offset.ToString("X8"),
                    //HexLine = _hexBuilder.ToString().TrimEnd(),
                    //AsciiLine = _asciiBuilder.ToString()
                };
            }
            set => throw new NotImplementedException();
        }
        // Реализация интерфейса IList (необходима для работы ItemsRepeater)
        public int Count => _count;
        public bool IsReadOnly => true;
        public bool IsFixedSize => true;
        public bool IsSynchronized => false;
        public object SyncRoot => null;

        public int Add(object value) => throw new NotImplementedException();
        public void Clear() => throw new NotImplementedException();
        public bool Contains(object value) => false;
        public int IndexOf(object value) => -1;
        public void Insert(int index, object value) => throw new NotImplementedException();
        public void Remove(object value) => throw new NotImplementedException();
        public void RemoveAt(int index) => throw new NotImplementedException();
        public void CopyTo(Array array, int index) => throw new NotImplementedException();
        public IEnumerator GetEnumerator() => throw new NotImplementedException();
        object IList.this[int index] { get => this[index]; set => throw new NotImplementedException(); }
        IEnumerator<HexRowViewModel> IEnumerable<HexRowViewModel>.GetEnumerator() => throw new NotImplementedException();

        public void Dispose()
        {
            _stream?.Dispose();
        }
    }

  
    
}



 




