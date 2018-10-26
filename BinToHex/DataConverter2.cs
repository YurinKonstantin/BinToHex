using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BinToHex
{
   public class DataConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            byte[] bb = new byte[1];
           bb[0] = (byte)value;
            UnicodeEncoding unicode = new UnicodeEncoding();
            String decodedString = unicode.GetString(bb);
            string str2 = System.Convert.ToBase64String(bb);
            return decodedString ;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
