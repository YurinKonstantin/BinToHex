using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Popups;
using Windows.UI.Xaml;

namespace BinToHex
{

    public class ClassData : INotifyPropertyChanged
    {
        byte?[] masByte = new byte?[16];
        public byte?[] MasByte
        {
            get
            {
                return masByte;
            }
            set
            {
                masByte = value;
            }
        }
      public void addByte(int b, byte bb)
        {
            masByte[b] = bb;
        }
     
        public byte? One1mas
        {
            get
            {
                if (masByte[0] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[0];
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
                    masByte[0] = Convert.ToByte(value);
                }

            }
        }
        public int One1Tag
        {
            get
            {
                return 1+Convert.ToInt32(offset);
            }
        
        }
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


     

        public int offset { get; set; }
        public int oneB { get; set; }
        
        public byte? One2mas
        {
            get
            {
                if (masByte[1] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[1];
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
                    masByte[1] = Convert.ToByte(value);
                }

            }
        }
        public int One2Tag
        {
            get
            {
                return 2 + Convert.ToInt32(offset);
            }

        }




        public byte? One3mas
        {
            get
            {
                if (masByte[2] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[2];
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
                    masByte[2] = Convert.ToByte(value);
                }

            }
        }
        public int One3Tag
        {
            get
            {
                return 3 + Convert.ToInt32(offset);
            }

        }

        public byte? One4mas
        {
            get
            {
                if (masByte[3] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[3];
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
                    masByte[3] = Convert.ToByte(value);
                }

            }
        }
        public int One4Tag
        {
            get
            {
                return 4 + Convert.ToInt32(offset);
            }

        }


        public byte? One5mas
        {
            get
            {
                if (masByte[4] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[4];
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
                    masByte[4] = Convert.ToByte(value);
                }

            }
        }
        public int One5Tag
        {
            get
            {
                return 5 + Convert.ToInt32(offset);
            }

        }




        public byte? One6mas
        {
            get
            {
                if (masByte[5] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[5];
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
                    masByte[5] = Convert.ToByte(value);
                }

            }
        }
        public int One6Tag
        {
            get
            {
                return 6 + Convert.ToInt32(offset);
            }

        }

        public byte? One7mas
        {
            get
            {
                if (masByte[6] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[6];
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
                    masByte[6] = Convert.ToByte(value);
                }

            }
        }
        public int One7Tag
        {
            get
            {
                return 7 + Convert.ToInt32(offset);
            }

        }

        public byte? One8mas
        {
            get
            {
                if (masByte[7] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[7];
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
                    masByte[7] = Convert.ToByte(value);
                }

            }
        }
        public int One8Tag
        {
            get
            {
                return 8 + Convert.ToInt32(offset);
            }

        }
        public byte? One9mas
        {
            get
            {
                if (masByte[8] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[8];
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
                    masByte[8] = Convert.ToByte(value);
                }

            }
        }
        public int One9Tag
        {
            get
            {
                return 9 + Convert.ToInt32(offset);
            }

        }

        public byte? One10mas
        {
            get
            {
                if (masByte[9] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[9];
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
                    masByte[9] = Convert.ToByte(value);
                }

            }
        }
        public int One10Tag
        {
            get
            {
                return 10 + Convert.ToInt32(offset);
            }

        }

        public byte? One11mas
        {
            get
            {
                if (masByte[10] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[10];
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
                    masByte[10] = Convert.ToByte(value);
                }

            }
        }
        public int One11Tag
        {
            get
            {
                return 11 + Convert.ToInt32(offset);
            }

        }

        public byte? One12mas
        {
            get
            {
                if (masByte[11] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[11];
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
                    masByte[11] = Convert.ToByte(value);
                }

            }
        }
        public int One12Tag
        {
            get
            {
                return 12 + Convert.ToInt32(offset);
            }

        }
        public byte? One13mas
        {
            get
            {
                if (masByte[12] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[12];
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
                    masByte[12] = Convert.ToByte(value);
                }

            }
        }
        public int One13Tag
        {
            get
            {
                return 13 + Convert.ToInt32(offset);
            }

        }

        public byte? One14mas
        {
            get
            {
                if (masByte[13] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[13];
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
                    masByte[13] = Convert.ToByte(value);
                }

            }
        }
        public int One14Tag
        {
            get
            {
                return 14 + Convert.ToInt32(offset);
            }

        }

        public byte? One15mas
        {
            get
            {
                if (masByte[14] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[14];
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
                    masByte[14] = Convert.ToByte(value);
                }

            }
        }
        public int One15Tag
        {
            get
            {
                return 15 + Convert.ToInt32(offset);
            }

        }

      
        public byte? One16mas
        {
            get
            {
                if (masByte[15] == null)
                {
                    return null;
                }
                else
                {
                    return masByte[15];
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
                    masByte[15] = Convert.ToByte(value);
                }

            }
        }
        public int One16Tag
        {
            get
            {
                return 16 + Convert.ToInt32(offset);
            }

        }


        public string one
        {
            get
            {
               
                return String.Format("{0:X2}", One1mas) + "\t" + String.Format("{0:X2}", One2mas) + "\t" + String.Format("{0:X2}", One3mas)
                    + "\t" + String.Format("{0:X2}", One4mas) + "\t" + String.Format("{0:X2}", One5mas) + "\t" + String.Format("{0:X2}", One6mas)
                    + "\t" + String.Format("{0:X2}", One7mas) + "\t" + String.Format("{0:X2}", One8mas) + "\t" + String.Format("{0:X2}", One9mas)
                    + "\t" + String.Format("{0:X2}", One10mas) + "\t" + String.Format("{0:X2}", One11mas) + "\t" + String.Format("{0:X2}", One12mas)
                    + "\t" + String.Format("{0:X2}", One13mas) + "\t" + String.Format("{0:X2}", One14mas) + "\t" + String.Format("{0:X2}", One15mas) + "\t" + String.Format("{0:X2}", One16mas);
            }
        }
        public string oneASCII
        {
            get
            {
                string text = String.Empty;

                if (One1mas != null)
                {
                    Char cg = (Char)One1mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One1mas);
                    }
                }
                if (One2mas != null)
                {
                    Char cg = (Char)One2mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One2mas);
                    }
                }
                if (One3mas != null)
                {
                    Char cg = (Char)One3mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One3mas);
                    }
                }
                if (One4mas != null)
                {
                    Char cg = (Char)One4mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One4mas);
                    }
                }
                if (One5mas != null)
                {
                    Char cg = (Char)One5mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One5mas);
                    }
                }
                if (One6mas != null)
                {
                    Char cg = (Char)One6mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One6mas);
                    }
                }
                if (One7mas != null)
                {
                    Char cg = (Char)One7mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One7mas);
                    }
                }
                if (One8mas != null)
                {
                    Char cg = (Char)One8mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One8mas);
                    }
                }
                if (One9mas != null)
                {
                    Char cg = (Char)One9mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One9mas);
                    }
                }
                if (One10mas != null)
                {
                    Char cg = (Char)One10mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One10mas);
                    }
                }
                if (One11mas != null)
                {
                    Char cg = (Char)One11mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One11mas);
                    }
                }
                if (One12mas != null)
                {
                    Char cg = (Char)One12mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One12mas);
                    }
                }
                if (One13mas != null)
                {
                    Char cg = (Char)One13mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One13mas);
                    }
                }
                if (One14mas != null)
                {
                    Char cg = (Char)One14mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One14mas);
                    }
                }
                if (One15mas != null)
                {
                    Char cg = (Char)One15mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One15mas);
                    }
                }
                if (One16mas != null)
                {
                    Char cg = (Char)One16mas;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", One16mas);
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
