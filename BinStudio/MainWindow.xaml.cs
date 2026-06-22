using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
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

        // 1. Скроллинг колесиком мыши (Лениво обновляет ScrollValue и подгружает данные)
        private void Editor_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is TabContext context)
            {
                var pointerPoint = e.GetCurrentPoint(grid);
                int delta = pointerPoint.Properties.MouseWheelDelta;

                long step = 3; // Шаг прокрутки
                long newValue = (long)context.ScrollValue;

                if (delta > 0)
                    newValue = Math.Max(0, newValue - step);
                else
                    newValue = (long)Math.Min(context.ScrollMax, newValue + step);

                context.ScrollValue = newValue; // Обновит ползунок на экране
                context.LoadVisibleData(newValue); // Обновит байты на экране
                e.Handled = true;
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

                if (x >= 90 && x < 500)
                {
                    double clickInsideHex = x - 90;
                    int byteIndex = (int)(clickInsideHex / 24.3);

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        long globalOffset = row.RowOffset + byteIndex;
                        byte selectedByte = row.RawBytes[byteIndex];
                        this.Title = $"BinStudio - Смещение: 0x{globalOffset:X8} | Байт: 0x{selectedByte:X2}";
                    }
                }
                else if (x >= 500)
                {
                    double clickInsideAscii = x - 500;
                    int byteIndex = (int)(clickInsideAscii / 8.1);

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        long globalOffset = row.RowOffset + byteIndex;
                        this.Title = $"BinStudio - ASCII Смещение: 0x{globalOffset:X8}";
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
