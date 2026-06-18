using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BinViewer.ModalView
{
    public class ClassData : INotifyPropertyChanged
    {
        //byte[] masByte = new byte[16];

        public Stream? stream { get; set; } = null;
        public int? SizeMas { get; set; } = null;
        byte[] masByte = null;
        public byte[] MasByte
        {
            get
            {
                return masByte;
            }

            set
            {
                masByte = value;
                OnPropertyChanged("ASCII"); OnPropertyChanged();


            }
        }
        // {
        //  get
        // {
        //     return masByte;
        // }
        //set
        // {
        //    masByte = value;
        //}
        // }
   
       

        public byte? One1mas
        {
            get
            {



                //if (MasByte == null || MasByte.Length == 0)
                //{
                //    long count = stream.Length;
                //    // Перемещаем курсор (Seek) на нужную позицию
                //    stream.Seek(offset, SeekOrigin.Begin);
                //    MasByte = new byte[(int)SizeMas];
                //    // Читаем заданный участок в буфер
                //    stream.Read(MasByte, 0, (int)SizeMas);
                //}
                if (MasByte==null || MasByte.Length < 1)
                {
                      return null;
                }
                else
                {
                     return MasByte[0];
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
                    MasByte[0] = Convert.ToByte(value);
                    OnPropertyChanged();
                }

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



        long offset;
        public long Offset 
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value; 
                OnPropertyChanged();
            }
        }

        public byte? One2mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 2)
                {
                    return null;
                }
                else
                {
                    return MasByte[1];
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
                    MasByte[1] = Convert.ToByte(value);
                    OnPropertyChanged();
                }

            }
        }




        public byte? One3mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 3)
                {
                    return null;
                }
                else
                {
                    return MasByte[2];
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
                    MasByte[2] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One4mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 4)
                {
                    return null;
                }
                else
                {
                    return MasByte[3];
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
                    MasByte[3] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }



        public byte? One5mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 5)
                {
                    return null;
                }
                else
                {
                    return MasByte[4];
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
                    MasByte[4] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }





        public byte? One6mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 6)
                {
                    return null;
                }
                else
                {
                    return MasByte[5];
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
                    MasByte[5] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One7mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 7)
                {
                    return null;
                }
                else
                {
                    return MasByte[6];
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
                    MasByte[6] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One8mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 8)
                {
                    return null;
                }
                else
                {
                    return MasByte[7];
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
                    MasByte[7] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }

        public byte? One9mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 9)
                {
                    return null;
                }
                else
                {
                    return MasByte[8];
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
                    MasByte[8] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One10mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 10)
                {
                    return null;
                }
                else
                {
                    return MasByte[9];
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
                    MasByte[9] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One11mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 11)
                {
                    return null;
                }
                else
                {
                    return MasByte[10];
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
                    MasByte[10] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One12mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 12)
                {
                    return null;
                }
                else
                {
                    return MasByte[11];
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
                    MasByte[11] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }

        public byte? One13mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 13)
                {
                    return null;
                }
                else
                {
                    return MasByte[12];
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
                    MasByte[12] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }

        public byte? One14mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 14)
                {
                    return null;
                }
                else
                {
                    return MasByte[13];
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
                    MasByte[13] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }


        public byte? One15mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 15)
                {
                    return null;
                }
                else
                {
                    return MasByte[14];
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
                    MasByte[14] = Convert.ToByte(value);
                }
                OnPropertyChanged();
            }
        }



        public byte? One16mas
        {
            get
            {
                if (MasByte == null || MasByte.Length < 16)
                {
                    return null;
                }
                else
                {
                    return MasByte[15];
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
                    MasByte[15] = Convert.ToByte(value);
                }
                OnPropertyChanged();
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
        public string ASCII
        {
            get
            {
               

                if(MasByte == null)
                {
                    return String.Empty;
                }
                return Encoding.ASCII.GetString(MasByte);
            }
            set
            {
                var g = value;
                OnPropertyChanged();
            }
        }
    }


}
