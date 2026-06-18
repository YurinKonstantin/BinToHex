using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BinViewer.DataTable
{
    public class TableColors
    {
        static public SolidColorBrush cellNull { get; set; } = new SolidColorBrush(Colors.AntiqueWhite);
        static public SolidColorBrush cell { get; set; } = new SolidColorBrush(Colors.White);
        static public SolidColorBrush cellSelect { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);


        static public SolidColorBrush column { get; set; } = new SolidColorBrush(Colors.LightGray);
        static public SolidColorBrush columnSelect { get; set; } = new SolidColorBrush(Colors.WhiteSmoke);
    }
}
