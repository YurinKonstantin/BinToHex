using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinViewer
{
    public sealed partial class HexEditorView : UserControl
    {
        public HexDocumentViewModel ViewModel { get; private set; }
        public HexEditorView(HexDocumentViewModel viewModel)
        {
            this.InitializeComponent();
            this.ViewModel = viewModel;
        }
        // Обязательно добавьте это имя (x:Name="ContentScrollViewer") в тег <ScrollViewer> внутри HexEditorView.xaml
        public void ScrollToRow(int rowIndex)
        {
            // Высота одной строки в разметке Grid равна 24 (задано в DataTemplate)
            double rowHeight = 24.0;
            double targetOffset = rowIndex * rowHeight;

            // Прокручиваем ScrollViewer на вычисленную вертикальную позицию
            ContentScrollViewer.ChangeView(null, targetOffset, null, disableAnimation: false);
        }
    }
}
