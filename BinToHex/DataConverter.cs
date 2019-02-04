using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace BinToHex
{
   public class DataConverter:IValueConverter
    {
        public object  Convert(object value, Type targetType, object parameter, string language)
        {
            byte? bb = (byte?)value;
            
          
            return String.Format("{0:X2}", bb);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
