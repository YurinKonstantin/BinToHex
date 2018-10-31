using System;


namespace BinToHex
{
   
    public class ClassData
    {
       public byte?  one1  { get; set; }
        public int? one1B { get; set; }

        public int offset { get; set; }
        public int oneB { get; set; }
        public byte? one2 { get; set; }
        public int? one2B { get; set; }

        public byte? one3 { get; set; }
        public int? one3B { get; set; }

        public byte? one4 { get; set; }
        public int? one4B { get; set; }

        public byte? one5 { get; set; }
        public int? one5B { get; set; }

        public byte? one6 { get; set; }
        public int? one6B { get; set; }

        public byte? one7 { get; set; }
        public int? one7B { get; set; }

        public byte? one8 { get; set; }
        public int? one8B { get; set; }

        public byte? one9 { get; set; }
        public int? one9B { get; set; }

        public byte?  one10 { get; set; }
        public int? one10B { get; set; }

        public byte? one11 { get; set; }
        public int? one11B { get; set; }

        public byte? one12 { get; set; }
        public int? one12B { get; set; }

        public byte? one13 { get; set; }
        public int? one13B { get; set; }

        public byte? one14 { get; set; }
        public int? one14B { get; set; }

        public byte? one15 { get; set; }
        public int? one15B { get; set; }

        public byte? one16 { get; set; }
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
               
                if (one1 != null)
                {
                    Char cg = (Char)one1;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one1);
                    }
                }
                if (one2 != null)
                {
                    Char cg = (Char)one2;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one2);
                    }
                }
                if (one3 != null)
                {
                    Char cg = (Char)one3;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one3);
                    }
                }
                if (one4 != null)
                {
                    Char cg = (Char)one4;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one4);
                    }
                }
                if (one5 != null)
                {
                    Char cg = (Char)one5;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one5);
                    }
                }
                if (one6 != null)
                {
                    Char cg = (Char)one6;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one6);
                    }
                }
                if (one7 != null)
                {
                    Char cg = (Char)one7;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one7);
                    }
                }
                if (one8 != null)
                {
                    Char cg = (Char)one8;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one8);
                    }
                }
                if (one9 != null)
                {
                    Char cg = (Char)one9;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one9);
                    }
                }
                if (one10 != null)
                {
                    Char cg = (Char)one10;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one10);
                    }
                }
                if (one11 != null)
                {
                    Char cg = (Char)one11;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one11);
                    }
                }
                if (one12 != null)
                {
                    Char cg = (Char)one12;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one12);
                    }
                }
                if (one13 != null)
                {
                    Char cg = (Char)one13;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one13);
                    }
                }
                if (one14 != null)
                {
                    Char cg = (Char)one14;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one14);
                    }
                }
                if (one15 != null)
                {
                    Char cg = (Char)one15;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one15);
                    }
                }
                if (one16 != null)
                {
                    Char cg = (Char)one16;
                    if (char.IsLetterOrDigit(cg) || char.IsPunctuation(cg))
                    {
                        text += cg;
                    }
                    else
                    {
                        text += String.Format("{0:X2}", one16);
                    }
                }

                return text; 
            }
        }

    }
 
}
