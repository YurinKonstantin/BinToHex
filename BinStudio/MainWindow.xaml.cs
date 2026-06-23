using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Pickers;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinStudio
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<TabContext> Tabs { get; } = new ObservableCollection<TabContext>();

        public MainWindow()
        {
            this.InitializeComponent();
            
            DocTabView.TabItemsSource = Tabs;
            this.SystemBackdrop = new Microsoft.UI.Xaml.Media.MicaBackdrop();
        }
        // Вызывается, когда сетка внутри вкладки полностью загружена в UI
     
        private async void OpenFile_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            IntPtr hWnd = WindowNative.GetWindowHandle(this);
            InitializeWithWindow.Initialize(openPicker, hWnd);
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.FileTypeFilter.Add("*");

            var file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                var tabContext = new TabContext(file.Path);
                Tabs.Add(tabContext);
                DocTabView.SelectedIndex = Tabs.Count - 1;
            }
        }
        private bool _isMouseInitialized = false;
        private void SaveFile_Click(object sender, RoutedEventArgs e)
        {
            if (DocTabView.SelectedItem is TabContext activeTab)
            {
                activeTab.SaveAllChanges();
            }
        }

        // 2. Скроллинг перетаскиванием ползунка (Критически важно для больших файлов)
        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (sender is ScrollBar scrollBar && scrollBar.DataContext is TabContext context)
            {
                

                context.LoadVisibleData((long)e.NewValue);
            }
        }

        private void Row_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid rowGrid && rowGrid.DataContext is HexRowViewModel row)
            {
                var point = e.GetCurrentPoint(rowGrid);
                double x = point.Position.X;

                // Извлекаем контекст вкладки, к которой принадлежит текущая строка
                if (DocTabView.SelectedItem is TabContext context)
                {
                    int rowIndex = context.VisibleRows.IndexOf(row);
                    if (rowIndex == -1) return;

                    // Зона HEX (90px - 500px)
                    if (x >= 90 && x < 500)
                    {
                        double clickInsideHex = x - 90;
                        int byteIndex = (int)(clickInsideHex / 24.3); // Ширина "FF " в Consolas 14

                        if(byteIndex >= 0 && byteIndex < row.BytesCount)
{
                            long globalOffset = row.RowOffset + byteIndex;
                            byte selectedByte = row.RawBytes[byteIndex];
                            this.Title = $"BinStudio - Смещение: 0x{globalOffset:X8} | Байт: 0x{selectedByte:X2}";

                            // ИСПРАВЛЕННАЯ МАТЕМАТИКА КУРСОРA
                            // Вычисляем точную позицию X без накопления погрешности
                            double cursorX = 91.0 + (byteIndex * 24.22);
                            // Центрируем рамку по вертикали внутри строки высотой 24px (высота рамки 18px, значит отступ 3px)
                            double cursorY = (rowIndex * 24.0) + 3.0;

                            // Записываем идеальные координаты в свойства контекста текущего документа
                            context.CursorX = cursorX;
                            context.CursorY = cursorY;
                            context.CursorVisibility = Visibility.Visible;

                            e.Handled = true;
                        }
                    }
                    // Кликнули мимо байт — скрываем рамку этого документа
                    else
                    {
                        context.CursorVisibility = Visibility.Collapsed;
                    }
                }
            }
        }
       


       
       
        private void DocTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Item is TabContext context)
            {
                context.Dispose();
                Tabs.Remove(context);
            }
        }

        
    }
   
}
