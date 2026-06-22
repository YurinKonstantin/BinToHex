using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio
{
    public class HexRowViewModel : INotifyPropertyChanged
    {
        private string _address;
        private string _hexLine;
        private string _asciiLine;

        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public string HexLine
        {
            get => _hexLine;
            set { _hexLine = value; OnPropertyChanged(); }
        }

        public string AsciiLine
        {
            get => _asciiLine;
            set { _asciiLine = value; OnPropertyChanged(); }
        }

        // Чистый массив байт для внутренней работы (редактирование, сохранение)
        public byte[] RawBytes { get; set; } = new byte[16];
        public long RowOffset { get; set; } // Смещение строки в файле
        public int BytesCount { get; set; } // Сколько байт реально прочитано (актуально для конца файла)

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
