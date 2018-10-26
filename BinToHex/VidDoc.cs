﻿using System;
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
    public class VidDoc : INotifyPropertyChanged
    {
        /// <summary>
        /// Открытый файл в байтовом виде
        /// </summary>
        // public byte[] BiteFile { get; set; }
        /// <summary>
        /// Имя файла
        /// </summary>
        public string Name { get; set; }
        public int ccc { get; set; }
        public string Path { get; set; }
        string text;
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                OnPropertyChanged();
            }
        }
        public VidDoc(byte[] b)
        {
            bb1 = b;
            OpenF();
        }
        public async void OpenF()
        {

            iniz();
            int i = 0;
            int ofs = 0;
            int ss = 0;
            int s = 0;
            var fd = new ClassData() { };
            string text;
            // text = String.Format("{0:X2}", ofs) + "\t";
            // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            // {
            fd.offset = ofs;
            fd.oneB = ss;
            //  text = String.Format("{0:X2}", ofs)+"\t";

            //   });
            while (i < bb1.Length)
            {
                if (s == 15)
                {
                    s = 0;

                    fd.oneB = ss;
                    ofs += 16;
                    //  text += "\n";

                    ClassDatas1.Add(fd);

                    ss += 15;
                    fd = new ClassData() { };

                    // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    //  {
                    //   text += String.Format("{0:X2}", ofs) + "\t";
                    fd.offset =ofs;
                    // });

                }
                switch (s)
                {
                    case 0:
                        fd.one1 = bb1[i];
                        fd.one1B = i+1;
                        break;
                    case 1:
                        fd.one2 = bb1[i];
                        fd.one2B = i + 1;
                        break;
                    case 2:
                        fd.one3 = bb1[i];
                        fd.one3B = i + 1;
                        break;
                    case 3:                  
                        fd.one4 = bb1[i];
                        fd.one4B = i + 1;
                        break;
                    case 4:
                        fd.one5 = bb1[i];
                        fd.one5B = i + 1;
                        break;
                    case 5:
                        fd.one6 = bb1[i];
                        fd.one6B = i + 1;
                        break;
                    case 6:
                        fd.one7 = bb1[i];
                        fd.one7B = i + 1;
                        break;
                    case 7:
                        fd.one8 = bb1[i];
                        fd.one8B = i + 1;
                        break;
                    case 8:
                        fd.one9 = bb1[i];
                        fd.one9B = i + 1;
                        break;
                    case 9:
                        fd.one10 = bb1[i];
                        fd.one10B = i + 1;
                        break;
                    case 10:
                        fd.one11 = bb1[i];
                        fd.one11B = i + 1;
                        break;
                    case 11:
                        fd.one12 = bb1[i];
                        fd.one12B = i + 1;
                        break;
                    case 12:
                        fd.one13 = bb1[i];
                        fd.one13B = i + 1;
                        break;
                    case 13:
                        fd.one14 = bb1[i];
                        fd.one14B = i + 1;
                        break;
                    case 14:
                        fd.one15 = bb1[i];
                        fd.one15B = i + 1;
                        break;
                }
                s++;
                i++;
            }

           
        }
        public byte[] bb1 { get; set; }
        public string Size { get; set; }
        public Visibility IsShow { get; set; }
     
        bool sppan;
        public bool Sppan
        {
            get
            {
                return sppan;
            }

            set
            {
                sppan = (value);
                OnPropertyChanged();
            }
        }
        int poz;
        public int Poz
        {
            get
            {
                return poz;
            }

            set
            {
                poz = (value);
                OnPropertyChanged();
            }
        }
        ObservableCollection<ClassData> classDatas1 = new ObservableCollection<ClassData>();
        public ObservableCollection<ClassData> ClassDatas1 { get { return classDatas1; } set { classDatas1 = value; } }


        public void iniz()
        {

            ClassDatas1 = new ObservableCollection<ClassData>();


        }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}