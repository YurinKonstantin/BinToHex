using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace BinToHex
{
   public class ModalViewViDoc : INotifyPropertyChanged
    {
        private VidDoc defaulVidDoc;
        public VidDoc DefaultVidDoc
        {
            get
            {
                return this.defaulVidDoc;
            }
            set
            {
                defaulVidDoc = value;
                OnPropertyChanged();
            }
        }
     
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public void AddColTabsViDoc()
        {
            ColTabs.Add(DefaultVidDoc);
        }
        public async void poisc(int x, int poz)
        {
            int ds = 0;
            foreach (VidDoc d in ColTabs)
            {
                if (ds == x)
                {

                    d.Poz = poz;
                    //  return;
                }
                ds++;
            }
        }
        Visibility isShov = Visibility.Collapsed;
        public Visibility IsShowBar
        {
            get
            {
                return isShov;
            }

            set
            {
                isShov = value;
                OnPropertyChanged("isShov");
            }
        }
        private ObservableCollection<VidDoc> colTabs = new ObservableCollection<VidDoc>();
        public ObservableCollection<VidDoc> ColTabs
        {
            get
            {
                return this.colTabs;
            }
            set
            {
                colTabs = value;
                OnPropertyChanged("ColTabs");

            }
        }

    }
}
