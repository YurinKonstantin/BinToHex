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
        public ObservableCollection<TabContext> Tabs { get; } = new();

        public MainWindow()
        {
            this.InitializeComponent();
            DocTabView.TabItemsSource = Tabs;
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

        private void EditorGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is TabContext context)
            {
                context.UpdateViewportSize(grid.ActualHeight);
            }
        }

        private void EditorGrid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is TabContext context)
            {
                context.UpdateViewportSize(e.NewSize.Height);
            }
        }

        private void ScrollBar_Scroll(object sender, ScrollEventArgs e)
        {
            if (sender is ScrollBar scrollBar && scrollBar.DataContext is TabContext context)
            {
                context.LoadVisibleData((long)e.NewValue);
            }
        }

        private void EditorGrid_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid grid && grid.DataContext is TabContext context)
            {
                var pointerPoint = e.GetCurrentPoint(grid);
                int delta = pointerPoint.Properties.MouseWheelDelta;

                long step = 3;
                long newValue = (long)context.ScrollValue;

                if (delta > 0)
                    newValue = Math.Max(0, newValue - step);
                else
                    newValue = (long)Math.Min(context.ScrollMax, newValue + step);

                context.ScrollValue = newValue;
                context.LoadVisibleData(newValue);
                e.Handled = true;
            }
        }

        // КЛИК ПО БАЙТУ: Ловим нажатие мыши на конкретном Border байта
        //private void ByteBorder_PointerPressed(object sender, PointerRoutedEventArgs e)
        //{
        //    if (sender is Border border && border.DataContext is HexByteViewModel clickedByte)
        //    {
        //        // Нам нужно найти, к какой строке принадлежит этот байт, чтобы сбросить остальные выделения
        //        // Для этого мы временно используем поиск по визуальному дереву WinUI или логику контекста.
        //        // Но проще всего передать управление через XAML. Сейчас мы настроим это.

        //        // Снимаем выделение со всех байт во всех вкладках и выделяем текущий
        //        if (DocTabView.SelectedItem is TabContext activeTab)
        //        {
        //            foreach (var row in activeTab.VisibleRows)
        //            {
        //                foreach (var b in row.Bytes)
        //                {
        //                    b.IsSelected = (b == clickedByte);
        //                }
        //            }

        //            if (clickedByte.RawValue.HasValue)
        //            {
        //                this.Title = $"BinStudio - Выбран байт: 0x{clickedByte.HexValue} ('{clickedByte.CharValue}')";
        //            }
        //        }
        //    }
        //}

        private void DocTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            if (args.Item is TabContext context)
            {
                context.Dispose();
                Tabs.Remove(context);
            }
        }
        private void Row_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (sender is Grid rowGrid && rowGrid.DataContext is HexRowViewModel row)
            {
                var point = e.GetCurrentPoint(rowGrid);
                double x = point.Position.X;

                // В XAML у нас: Колонка Смещения = 90 пикселей.
                // Колонка HEX начинается с 90px и имеет ширину 410px.
                if (x >= 90 && x < 500)
                {
                    double clickInsideHex = x - 90;

                    // Магическое число: При размере шрифта 14 в Consolas 
                    // один байт вместе с пробелом ("FF ") занимает ровно 24.3 пикселя.
                    int byteIndex = (int)(clickInsideHex / 24.3);

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        long globalOffset = row.RowOffset + byteIndex;
                        byte selectedByte = row.RawBytes[byteIndex];

                        // Выводим информацию о выбранном байте в заголовок приложения!
                        this.Title = $"BinStudio - Смещение: 0x{globalOffset:X8} | Байт: 0x{selectedByte:X2} | Индекс в строке: {byteIndex}";

                        // Передаем фокус и готовим к изменению (это база для будущего ввода)
                        e.Handled = true;
                    }
                }
                // Колонка ASCII начинается с 500px
                else if (x >= 500)
                {
                    double clickInsideAscii = x - 500;
                    // Один символ ASCII ("A") занимает ровно 8.1 пикселя
                    int byteIndex = (int)(clickInsideAscii / 8.1);

                    if (byteIndex >= 0 && byteIndex < row.BytesCount)
                    {
                        long globalOffset = row.RowOffset + byteIndex;
                        this.Title = $"BinStudio - Клик в ASCII! Смещение: 0x{globalOffset:X8}";
                        e.Handled = true;
                    }
                }
            }
        }
    }


}
