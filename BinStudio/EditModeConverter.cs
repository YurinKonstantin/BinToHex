using Microsoft.UI.Xaml.Data;
using System;

namespace BinStudio
{
    public class EditModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isEditing && isEditing)
                return "ПРАВКА";
            return "НАВИГАЦИЯ";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new NotImplementedException();
    }
}
