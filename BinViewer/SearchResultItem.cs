using System;
using System.Collections.Generic;
using System.Text;

namespace BinViewer
{
    public class SearchResultItem
    {
        public long Address { get; set; }
        public string AddressHex => Address.ToString("X8");
        public string PreviewText { get; set; } // Текст для отображения в списке
    }
}
