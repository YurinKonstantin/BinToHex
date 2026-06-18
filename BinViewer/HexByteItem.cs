using System;
using System.Collections.Generic;
using System.Text;

namespace BinViewer
{
    public class HexByteItem
    {

        public long AbsoluteAddress { get; set; }
        public byte Value { get; set; }
        public string HexText => Value.ToString("X2");
        public bool IsValid { get; set; }
        public bool IsModified { get; set; } // Добавили флаг изменений
        public string DisplayText => IsValid ? HexText : "  ";

  






  
    }
}
