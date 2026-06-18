using Microsoft.UI;
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
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace BinViewer
{
    public sealed partial class HexEditorView : UserControl
    {
        public HexDocumentViewModel ViewModel { get; private set; }
        private bool _isSelecting = false;

        // Список всех активных (видимых на экране) ячеек для быстрой перекраски
        private readonly List<Border> _activeBorders = new();
        private long _editingAddress = -1; // -1 означает, что режим редактирования выключен


        public HexEditorView(HexDocumentViewModel viewModel)
        {
            this.InitializeComponent();
            this.ViewModel = viewModel;

            // Подписываемся на обновление выделения
            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            // ГАРАНТИРОВАННЫЙ перехват клавиш до того, как их заберут внутренние списки
          
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(HexDocumentViewModel.SelectionStart) || e.PropertyName == "SelectionStart")
            {
                UpdateBordersVisuals();
            }
        }


        private void UpdateBordersVisuals()
        {
            var accentBrush = (Brush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
            var modifiedBrush = new SolidColorBrush(Colors.Red); // Измененные красятся красным
            var transparentBrush = new SolidColorBrush(Colors.Transparent);

            var textSelectedBrush = new SolidColorBrush(Colors.White);
            var textModifiedBrush = new SolidColorBrush(Colors.Red);
            var textNormalBrush = (Brush)Application.Current.Resources["ApplicationForegroundThemeBrush"];

            foreach (var border in _activeBorders)
            {
                if (border.DataContext is HexByteItem byteItem && byteItem.IsValid)
                {
                    bool isSelected = ViewModel.IsByteSelected(byteItem.AbsoluteAddress);
                    bool isEditingThis = byteItem.AbsoluteAddress == _editingAddress;

                    // Приоритет цветов: Редактирование/Выделение -> Изменено -> Обычный
                    if (isSelected || isEditingThis)
                    {
                        border.Background = accentBrush;
                        if (border.Child is TextBlock tb) tb.Foreground = textSelectedBrush;
                    }
                    else if (byteItem.IsModified)
                    {
                        border.Background = transparentBrush;
                        if (border.Child is TextBlock tb) tb.Foreground = textModifiedBrush;
                    }
                    else
                    {
                        border.Background = transparentBrush;
                        if (border.Child is TextBlock tb) tb.Foreground = textNormalBrush;
                    }
                }
            }
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
        // Синхронизируем цвета видимых ячеек в зависимости от глобального выделения
       

        // Трекаем появление ячеек на экране
        private void ByteBorder_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (sender is Border border)
            {
                if (!_activeBorders.Contains(border)) _activeBorders.Add(border);

                // Сразу красим при появлении строки
                if (border.DataContext is HexByteItem byteItem && byteItem.IsValid)
                {
                    var accentBrush = (Brush)Application.Current.Resources["SystemControlHighlightAccentBrush"];
                    bool isSelected = ViewModel.IsByteSelected(byteItem.AbsoluteAddress);
                    border.Background = isSelected ? accentBrush : new SolidColorBrush(Colors.Transparent);
                }
            }
        }

        // Одинарный клик — ТОЛЬКО выделение
        private void Byte_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var pointerProperties = e.GetCurrentPoint(this).Properties;
            if (!pointerProperties.IsLeftButtonPressed) return;

            if (sender is Border border && border.DataContext is HexByteItem byteItem && byteItem.IsValid)
            {
                _isSelecting = true;
                MainGrid.CapturePointer(e.Pointer);

                // Сбрасываем старое редактирование при клике в другое место
                _editingAddress = -1;
                ViewModel.IsFirstNibbleEntered = false;

                ViewModel.SelectionStart = byteItem.AbsoluteAddress;
                ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                ViewModel.NotifySelectionChanged();
            }
        }

        // Двойной клик — вход в режим редактирования
        private void Byte_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            if (sender is Border border && border.DataContext is HexByteItem byteItem && byteItem.IsValid)
            {
                // Фиксируем адрес для редактирования
                _editingAddress = byteItem.AbsoluteAddress;
                ViewModel.SelectionStart = byteItem.AbsoluteAddress;
                ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                ViewModel.IsFirstNibbleEntered = false;

                // Забираем фокус ввода клавиатуры на UserControl
                this.Focus(FocusState.Programmatic);
                UpdateBordersVisuals();
            }
        }


        // Движение мыши с зажатой кнопкой
        private void Byte_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            if (_isSelecting && sender is Border border && border.DataContext is HexByteItem byteItem && byteItem.IsValid)
            {
                ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                ViewModel.NotifySelectionChanged(); // Обновить цвета динамически
            }
        }

        // Отпускание мыши — конец выделения
        private void Byte_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                if (sender is Border border)
                {
                    border.ReleasePointerCapture(e.Pointer);
                }
            }
        }


     

 

        // Отпускание мыши в любом месте сетки завершает операцию
        private void MainGrid_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (_isSelecting)
            {
                _isSelecting = false;
                MainGrid.ReleasePointerCaptures();
            }
        }






 

        // Движение мыши отслеживается глобально над таблицей
        private void MainGrid_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!_isSelecting) return;

            // Дополнительная страховка: проверяем, что левая кнопка все еще зажата при движении
            var pointerProperties = e.GetCurrentPoint(MainGrid).Properties;
            if (!pointerProperties.IsLeftButtonPressed)
            {
                _isSelecting = false;
                MainGrid.ReleasePointerCaptures();
                return;
            }

            // Получаем текущую координату курсора относительно главной сетки
            Point currentPoint = e.GetCurrentPoint(MainGrid).Position;

            // Ищем, над каким из видимых элементов Border сейчас находится курсор
            foreach (var border in _activeBorders)
            {
                if (border.DataContext is HexByteItem byteItem && byteItem.IsValid)
                {
                    // Вычисляем границы Border на экране
                    var transform = border.TransformToVisual(MainGrid);
                    Point borderPos = transform.TransformPoint(new Point(0, 0));
                    Rect borderRect = new Rect(borderPos.X, borderPos.Y, border.ActualWidth, border.ActualHeight);

                    // Если курсор попал в эту ячейку, обновляем конечную точку выделения
                    if (borderRect.Contains(currentPoint))
                    {
                        if (ViewModel.SelectionEnd != byteItem.AbsoluteAddress)
                        {
                            ViewModel.SelectionEnd = byteItem.AbsoluteAddress;
                            ViewModel.NotifySelectionChanged();
                        }
                        break;
                    }
                }
            }
        }





        // Обработчик нажатия клавиш

        // Обработка клавиатуры
        public void HandleKeyboardInput(Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (_editingAddress == -1) return;

            int keyValue = -1;

            // 1. Проверяем цифры из ВЕРХНЕГО ряда клавиатуры (VirtualKey.Number0 = 48, Number9 = 57)
            if (e.Key >= Windows.System.VirtualKey.Number0 && e.Key <= Windows.System.VirtualKey.Number9)
            {
                keyValue = e.Key - Windows.System.VirtualKey.Number0;
            }
            // 2. Проверяем буквы A-F для ввода шестнадцатеричных значений
            else if (e.Key >= Windows.System.VirtualKey.A && e.Key <= Windows.System.VirtualKey.F)
            {
                keyValue = 10 + (e.Key - Windows.System.VirtualKey.A);
            }
            // 3. Проверяем цифры с боковой клавиатуры (Numpad0 = 96, Numpad9 = 105)
            else if (e.Key >= Windows.System.VirtualKey.NumberPad0 && e.Key <= Windows.System.VirtualKey.NumberPad9)
            {
                keyValue = e.Key - Windows.System.VirtualKey.NumberPad0;
            }

            // Если нажата валидная HEX клавиша (0-9, A-F)
            if (keyValue != -1)
            {
                e.Handled = true;
                byte currentByte = ViewModel.Buffer.ReadByte(_editingAddress);

                if (!ViewModel.IsFirstNibbleEntered)
                {
                    // Ввод первого символа (старший полубайт)
                    byte newValue = (byte)((keyValue << 4) | (currentByte & 0x0F));
                    ViewModel.ChangeByteAt(_editingAddress, newValue);
                    ViewModel.IsFirstNibbleEntered = true;

                    UpdateActiveBorderText(_editingAddress, keyValue.ToString("X") + "?");
                }
                else
                {
                    // Ввод второго символа (младший полубайт)
                    byte newValue = (byte)((currentByte & 0xF0) | keyValue);
                    ViewModel.ChangeByteAt(_editingAddress, newValue);
                    ViewModel.IsFirstNibbleEntered = false;

                    // Автопереход на СЛЕДУЮЩИЙ байт
                    if (_editingAddress + 1 < ViewModel.Buffer.Length)
                    {
                        _editingAddress++;
                        ViewModel.SelectionStart = _editingAddress;
                        ViewModel.SelectionEnd = _editingAddress;
                    }
                    else
                    {
                        _editingAddress = -1;
                    }
                    ViewModel.NotifySelectionChanged();
                }
            }
            // Навигация стрелочками
            else if (e.Key == Windows.System.VirtualKey.Left || e.Key == Windows.System.VirtualKey.Right ||
                     e.Key == Windows.System.VirtualKey.Up || e.Key == Windows.System.VirtualKey.Down)
            {
                long newAddress = _editingAddress;
                if (e.Key == Windows.System.VirtualKey.Left && _editingAddress > 0) newAddress--;
                if (e.Key == Windows.System.VirtualKey.Right && _editingAddress + 1 < ViewModel.Buffer.Length) newAddress++;
                if (e.Key == Windows.System.VirtualKey.Up && _editingAddress >= 16) newAddress -= 16;
                if (e.Key == Windows.System.VirtualKey.Down && _editingAddress + 16 < ViewModel.Buffer.Length) newAddress += 16;

                if (newAddress != _editingAddress)
                {
                    e.Handled = true;
                    _editingAddress = newAddress;
                    ViewModel.SelectionStart = newAddress;
                    ViewModel.SelectionEnd = newAddress;
                    ViewModel.IsFirstNibbleEntered = false;
                    ViewModel.NotifySelectionChanged();

                    // Автоматическая докрутка ScrollViewer при переходе стрелочками
                    int targetRowIndex = (int)(_editingAddress / 16);
                    ScrollToRow(targetRowIndex);
                }
            }
            // Выход по Escape
            else if (e.Key == Windows.System.VirtualKey.Escape)
            {
                _editingAddress = -1;
                ViewModel.IsFirstNibbleEntered = false;
                ViewModel.NotifySelectionChanged();
            }
        }

        // Вспомогательный метод для красивого отображения полуввода (например, "A?")
        private void UpdateActiveBorderText(long address, string text)
        {
            foreach (var border in _activeBorders)
            {
                if (border.DataContext is HexByteItem byteItem && byteItem.AbsoluteAddress == address)
                {
                    if (border.Child is TextBlock tb)
                    {
                        tb.Text = text;
                    }
                    break;
                }
            }
        }
    }
}
