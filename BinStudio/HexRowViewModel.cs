using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio
{
    public class HexRowViewModel : INotifyPropertyChanged
    {
        private string _address;
        private string _hexLine;
        private string _asciiLine;
        private int _selectedIndex = -1; // -1 означает, что в этой строке ничего не выделено
        // Индекс выбранного байта в строке (0-15)
        public int SelectedByteIndex { get; private set; } = -1;
        public string Address
        {
            get => _address;
            set { _address = value; OnPropertyChanged(); }
        }

        public string HexLine
        {
            get => _hexLine;
            set { _hexLine = value; OnPropertyChanged(); }
        }

        public string AsciiLine
        {
            get => _asciiLine;
            set { _asciiLine = value; OnPropertyChanged(); }
        }
        // Индекс выделенного байта (0-15)
        public int SelectedIndex
        {
            get => _selectedIndex;
            set
            {
                _selectedIndex = value;
                OnPropertyChanged();
                // Генерируем событие обновления, чтобы триггерить перерисовку Run-элементов
                OnPropertyChanged(nameof(SelectedIndex));
            }
        }
        // Чистый массив байт для внутренней работы (редактирование, сохранение)
        public byte[] RawBytes { get; set; } = new byte[16];
        public long RowOffset { get; set; } // Смещение строки в файле
        public int BytesCount { get; set; } // Сколько байт реально прочитано (актуально для конца файла)
        // Ссылка на родительскую вкладку для проверки глобального выделения
        public void TriggerRefresh()
        {
            // Стреляем фейковым свойством "RefreshInlines", чтобы Behavior перерисовал Run-кусочки
            OnPropertyChanged("RefreshInlines");
        }
        public TabContext ParentContext { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //public void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        //{
        //    var TextBlock = sender as TextBlock;
        //    if (TextBlock != null)
        //    {
        //        /// Проверить, есть ли выделенный текст
        //        if (TextBlock.SelectionStart != null && TextBlock.SelectionEnd != null)
        //        {
        //            // Получить TextPointer начала выделения
        //            TextPointer startPointer = TextBlock.SelectionStart;

        //            // Получить отступ (Offset) в символах от начала текста
        //            int charIndex = startPointer.Offset;
        //            System.Diagnostics.Debug.WriteLine($"Выделено с индекса {charIndex}, номер: {TextBlock.SelectedText}");
        //        }

        //        // Выводим информацию в отладку
        //        //System.Diagnostics.Debug.WriteLine($"Выделено с индекса {start}, длина: {length}");
        //    }
        //}
        // ОБРАБОТЧИК ВСТРОЕННОГО ВЫДЕЛЕНИЯ ТЕКСТА WINUI 3
        public void TextBlock_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                // Проверяем, что пользователь действительно выделил какой-то текст
                if (!string.IsNullOrEmpty(textBlock.SelectedText))
                {
                    string selectedText = textBlock.SelectedText.Trim();
                    int charIndex = -1;

                    // ОПРЕДЕЛЯЕМ, ГДЕ ПРОИЗОШЛО ВЫДЕЛЕНИЕ (HEX ИЛИ ASCII)
                    if (textBlock.Text == HexLine)
                    {
                        // Находим, где в строке находится выделенный байт (например, "AA")
                        charIndex = textBlock.Text.IndexOf(selectedText);
                        if (charIndex != -1)
                        {
                            // Каждая пара "FF " занимает ровно 3 символа в текстовой строке
                            SelectedByteIndex = charIndex / 3;
                        }
                    }
                    else if (textBlock.Text == AsciiLine)
                    {
                        // Находим символ в ASCII строке
                        charIndex = textBlock.Text.IndexOf(selectedText);
                        if (charIndex != -1)
                        {
                            // В ASCII один символ занимает ровно 1 позицию
                            SelectedByteIndex = charIndex;
                        }
                    }

                    // Защита и проверка: индекс должен быть в пределах прочитанных байт
                    if (SelectedByteIndex >= 0 && SelectedByteIndex < BytesCount)
                    {
                        long globalOffset = RowOffset + SelectedByteIndex;
                        byte selectedByte = RawBytes[SelectedByteIndex];

                        // Выводим результат в Debug-окно (Output в Visual Studio)
                        System.Diagnostics.Debug.WriteLine($"[BinStudio] Выделен байт: 0x{selectedByte:X2} | Индекс в строке: {SelectedByteIndex} | Смещение в файле: 0x{globalOffset:X8}");
                    }
                }
            }
        }
    }
}
