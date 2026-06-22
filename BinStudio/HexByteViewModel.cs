using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinStudio
{
    public class HexByteViewModel
    {
        private string _hexValue = "  ";
        private string _charValue = " ";
        private bool _isSelected;

        private Brush _backgroundBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));
        private Brush _foregroundBrush;

        public byte? RawValue { get; set; }
        public HexByteViewModel()
        {
            // Берем дефолтный цвет текста из темы приложения (черный для светлой темы, белый для темной)
            if (Application.Current.Resources.TryGetValue("ApplicationForegroundThemeBrush", out object brush))
            {
                _foregroundBrush = brush as Brush;
            }
            else
            {
                _foregroundBrush = new SolidColorBrush(Microsoft.UI.Colors.Black); // Резервный вариант
            }
        }
        public string HexValue
        {
            get => _hexValue;
            set { _hexValue = value; OnPropertyChanged(); }
        }

        public string CharValue
        {
            get => _charValue;
            set { _charValue = value; OnPropertyChanged(); }
        }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();

                if (_isSelected)
                {
                    BackgroundBrush = new SolidColorBrush(Microsoft.UI.Colors.DodgerBlue);
                    ForegroundBrush = new SolidColorBrush(Microsoft.UI.Colors.White); // Белый текст на синем фоне
                }
                else
                {
                    BackgroundBrush = new SolidColorBrush(Windows.UI.Color.FromArgb(0, 0, 0, 0));

                    // Возвращаем стандартный цвет темы
                    if (Application.Current.Resources.TryGetValue("ApplicationForegroundThemeBrush", out object brush))
                        ForegroundBrush = brush as Brush;
                    else
                        ForegroundBrush = new SolidColorBrush(Microsoft.UI.Colors.Black);
                }
            }
        }

        public Brush BackgroundBrush
        {
            get => _backgroundBrush;
            set { _backgroundBrush = value; OnPropertyChanged(); }
        }

        public Brush ForegroundBrush
        {
            get => _foregroundBrush;
            set { _foregroundBrush = value; OnPropertyChanged(); }
        }

        public void Clear()
        {
            RawValue = null;
            HexValue = "  ";
            CharValue = " ";
            IsSelected = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
