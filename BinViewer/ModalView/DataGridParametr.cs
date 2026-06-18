using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinViewer.ModalView
{
    public class DataGridParametr : INotifyPropertyChanged
    {
        /// <summary>
        /// Высота строки
        /// </summary>
        public double RowHeight { get; set; } = 32;
        /// <summary>
        /// Высота заголовка
        /// </summary>
        public double ColumnHeaderHeight { get; set; } = 40;
        double dataGridExtentHeight;
        /// <summary>
        /// Высота всей области
        /// </summary>
        public double DataGridExtentHeight 
        { 
            get
            {
                return this.dataGridExtentHeight;
            }
            set
            {
                dataGridExtentHeight = value;
                OnPropertyChanged("DataGridExtentHeight");
            }
        }
        /// <summary>
        /// Высота видимой части
        /// </summary>
        public double DataGridViewportHeight { get; set; }
        public long CountRowFull {  get; set; }
        public int CountRowViewport { get; set; }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        double verticalOffset;
        public double VerticalOffset
        {
            get { return this.verticalOffset; }
            set { verticalOffset = value; OnPropertyChanged(); }
        }
        public int zoom { get; set; } = 1;

    }
}
