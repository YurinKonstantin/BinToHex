using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace BinToHex
{
   
    public class ClassData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        Visibility isShovACSII = Visibility.Collapsed;
        public Visibility IsShowACSII
        {
            get
            {
                return isShovACSII;
            }

            set
            {
                isShovACSII = value;
                
            }
        }


        byte one1;
        public  byte One112
        {
            get
            {
                return one1;
            }
            set
            {
                one1 = value;
                OnPropertyChanged("One112");
                var mess = new MessageDialog(One112.ToString());
                mess.ShowAsync();
            }
        }
   public byte? One1
        {
            get
            {
                if (one1B == null)
                {
                    return null;
                }
                else
                {
                    return one1;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one1 = Convert.ToByte(value);
                  //  var mess = new MessageDialog(one1.ToString());
                  // mess.ShowAsync();
                    OnPropertyChanged("One1");
                }

            }
        }
        public int? one1B { get; set; }

        public int offset { get; set; }
        public int oneB { get; set; }
        byte one2;
        public byte? One2
        {
            get
            {
                if (one2B == null)
                {
                    return null;
                }
                else
                {
                    return one2;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one2 = Convert.ToByte(value);
                    OnPropertyChanged("One2");
                }

            }
        }
        public int? one2B { get; set; }

        byte one3;
        public byte? One3
        {
            get
            {
                if (one3B == null)
                {
                    return null;
                }
                else
                {
                    return one3;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one3 = Convert.ToByte(value);
                }

            }
        }
        public int? one3B { get; set; }

        byte one4;
        public byte? One4
        {
            get
            {
                if (one4B == null)
                {
                    return null;
                }
                else
                {
                    return one4;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one4 = Convert.ToByte(value);
                }

            }
        }
        public int? one4B { get; set; }

        byte one5;
        public byte? One5
        {
            get
            {
                if (one5B == null)
                {
                    return null;
                }
                else
                {
                    return one5;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one5 = Convert.ToByte(value);
                }

            }
        }
        public int? one5B { get; set; }

        byte one6;
        public byte? One6
        {
            get
            {
                if (one6B == null)
                {
                    return null;
                }
                else
                {
                    return one6;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one6 = Convert.ToByte(value);
                }

            }
        }
        public int? one6B { get; set; }

        byte one7;
        public byte? One7
        {
            get
            {
                if (one7B == null)
                {
                    return null;
                }
                else
                {
                    return one7;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one7 = Convert.ToByte(value);
                }

            }
        }
        public int? one7B { get; set; }

        byte one8;
        public byte? One8
        {
            get
            {
                if (one8B == null)
                {
                    return null;
                }
                else
                {
                    return one8;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one8 = Convert.ToByte(value);
                }

            }
        }
        public int? one8B { get; set; }

        byte one9;
        public byte? One9
        {
            get
            {
                if (one9B == null)
                {
                    return null;
                }
                else
                {
                    return one9;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one9 = Convert.ToByte(value);
                }

            }
        }
        public int? one9B { get; set; }

        byte one10;
        public byte? One10
        {
            get
            {
                if (one10B == null)
                {
                    return null;
                }
                else
                {
                    return one10;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one10 = Convert.ToByte(value);
                }

            }
        }
        public int? one10B { get; set; }

        byte one11;
        public byte? One11
        {
            get
            {
                if (one11B == null)
                {
                    return null;
                }
                else
                {
                    return one11;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one11 = Convert.ToByte(value);
                }

            }
        }
        public int? one11B { get; set; }

        byte one12;
        public byte? One12
        {
            get
            {
                if (one12B == null)
                {
                    return null;
                }
                else
                {
                    return one12;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one12 = Convert.ToByte(value);
                }

            }
        }
        public int? one12B { get; set; }

        byte one13;
        public byte? One13
        {
            get
            {
                if (one13B == null)
                {
                    return null;
                }
                else
                {
                    return one13;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one13 = Convert.ToByte(value);
                }

            }
        }
        public int? one13B { get; set; }

        byte one14;
        public byte? One14
        {
            get
            {
                if (one14B == null)
                {
                    return null;
                }
                else
                {
                    return one14;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one14 = Convert.ToByte(value);
                }

            }
        }
        public int? one14B { get; set; }

        byte one15;
        public byte? One15
        {
            get
            {
                if (one15B == null)
                {
                    return null;
                }
                else
                {
                    return one15;
                }

            }
            set
            {
                var g = value;
                if (g == null)
                {

                }
                else
                {
                    one15 = Convert.ToByte(value);
                }

            }
        }
        public int? one15B { get; set; }

        byte one16;
        public byte? One16
        {
            get
            {
                if(one16B==null)
                {
                    return null;
                }
                else
                {
                    return one16;
                }
                
            }
            set
            {
                var g = value;
                if(g==null)
                {

                }
                else
                {
                    one16 =Convert.ToByte(value);
                }
                
            }
       }
        public int? one16B { get; set; }


        public string one
        {
            get
            {
                return String.Format("{0:X2}", one1) + "\t"+ String.Format("{0:X2}", one2) + "\t" + String.Format("{0:X2}", one3)
                    + "\t" + String.Format("{0:X2}", one4) + "\t" + String.Format("{0:X2}", one5) + "\t" + String.Format("{0:X2}", one6)
                    + "\t" + String.Format("{0:X2}", one7) + "\t" + String.Format("{0:X2}", one8) + "\t" + String.Format("{0:X2}", one9)
                    + "\t" + String.Format("{0:X2}", one10) + "\t" + String.Format("{0:X2}", one11) + "\t" + String.Format("{0:X2}", one12)
                    + "\t" + String.Format("{0:X2}", one13) + "\t" + String.Format("{0:X2}", one14) + "\t" + String.Format("{0:X2}", one15) + "\t" + String.Format("{0:X2}", one16);
            }
        }
        public string oneASCII
        {
            get
            {
                string text=String.Empty;
               
                if (One1 != null)
                {
                    Char cg = (Char)One1;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One1);
                    }
                }
                if (One2 != null)
                {
                    Char cg = (Char)One2;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One2);
                    }
                }
                if (One3 != null)
                {
                    Char cg = (Char)One3;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One3);
                    }
                }
                if (One4 != null)
                {
                    Char cg = (Char)One4;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One4);
                    }
                }
                if (One5 != null)
                {
                    Char cg = (Char)One5;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One5);
                    }
                }
                if (One6 != null)
                {
                    Char cg = (Char)One6;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One6);
                    }
                }
                if (One7 != null)
                {
                    Char cg = (Char)One7;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One7);
                    }
                }
                if (One8 != null)
                {
                    Char cg = (Char)One8;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}",One8);
                    }
                }
                if (One9 != null)
                {
                    Char cg = (Char)One9;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One9);
                    }
                }
                if (One10 != null)
                {
                    Char cg = (Char)One10;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One10);
                    }
                }
                if (One11 != null)
                {
                    Char cg = (Char)One11;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One11);
                    }
                }
                if (One12 != null)
                {
                    Char cg = (Char)One12;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One12);
                    }
                }
                if (One13 != null)
                {
                    Char cg = (Char)One13;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One13);
                    }
                }
                if (One14 != null)
                {
                    Char cg = (Char)One14;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One14);
                    }
                }
                if (One15 != null)
                {
                    Char cg = (Char)One15;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One15);
                    }
                }
                if (One16 != null)
                {
                    Char cg = (Char)One16;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One16);
                    }
                }

                return text; 
            }
            set
            {
                var g = value;
            }
        }

    }
 
}
