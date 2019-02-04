using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Data;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls.Primitives;

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
        public string NameType { get; set; }
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
  
        public DataGrid dataGrid = new DataGrid();
        public DataTable booksTable;
        public VidDoc()
            {
            classDatas1 = new ObservableCollection<DataRow>();
            booksTable = new DataTable("name");
            DataColumn idColumn = new DataColumn("offset", Type.GetType("System.Int32"));
            idColumn.Unique = true; // столбец будет иметь уникальное значение
            idColumn.AllowDBNull = false; // не может принимать null
            idColumn.AutoIncrement = true; // будет автоинкрементироваться
            idColumn.AutoIncrementSeed = 0; // начальное значение
            idColumn.AutoIncrementStep = 16; // приращении при добавлении новой строки

            DataColumn nameColumn = new DataColumn("One1mas", Type.GetType("System.String"));
            nameColumn.AllowDBNull = true;
            DataColumn priceColumn = new DataColumn("One2mas", Type.GetType("System.String"));
            priceColumn.AllowDBNull = true;
            DataColumn discountColumn = new DataColumn("One3mas", Type.GetType("System.String"));
            discountColumn.AllowDBNull = true;

            booksTable.Columns.Add(idColumn);
            booksTable.Columns.Add(nameColumn);
            booksTable.Columns.Add(priceColumn);
            booksTable.Columns.Add(discountColumn);
            booksTable.PrimaryKey = new DataColumn[] { booksTable.Columns["offset"] };
          
          
        }
        public async void ShowModel()
        {
            
            var savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Тип открытого файла"+"|"+NameType, new List<string>() { NameType });
            savePicker.FileTypeChoices.Add("Text", new List<string>() {".txt"});
            savePicker.FileTypeChoices.Add("Bin", new List<string>() { ".bin" });
            savePicker.FileTypeChoices.Add("PNG", new List<string>() { ".png" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = Name;
            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                byte[] bb = new byte[Convert.ToInt32(Size)];
                int i = 0;
            /*    foreach(ClassData b in ClassDatas1)
                {
                    if (b.One1mas!=null)
                    {
                        bb[i] =Convert.ToByte(b.One1mas);
                        i++;
                    }
                    if (b.One2mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One2mas);
                        i++;
                    }
                    if (b.One3mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One3mas);
                        i++;
                    }
                    if (b.One4mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One4mas);
                        i++;
                    }
                    if (b.One5mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One5mas);
                        i++;
                    }
                    if (b.One6mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One6mas);
                        i++;
                    }
                    if (b.One7mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One7mas);
                        i++;
                    }
                    if (b.One8mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One8mas);
                        i++;
                    }
                    if (b.One9mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One9mas);
                        i++;
                    }
                    if (b.One10mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One10mas);
                        i++;
                    }
                    if (b.One11mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One11mas);
                        i++;
                    }
                    if (b.One12mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One12mas);
                        i++;
                    }
                    if (b.One13mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One13mas);
                        i++;
                    }
                    if (b.One14mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One14mas);
                        i++;
                    }
                    if (b.One15mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One15mas);
                        i++;
                    }
                    if (b.One16mas != null)
                    {
                        bb[i] = Convert.ToByte(b.One16mas);
                        i++;
                    }
                }
              
                await Windows.Storage.FileIO.WriteBytesAsync(file, bb1);

    */


            }
            else
            {

            }
           
            var mess = new MessageDialog("Сохранение завершено");
            await mess.ShowAsync();
        }
   
        public async void redact()
        {
          //  var f = (TextBlock)sender;
            MessageDialog messageDialog = new MessageDialog("hghg");
         await   messageDialog.ShowAsync();

        }
        public async Task OpenF()
        {

           // iniz();
            int i = 0;
            int ofs = 0;
            int ss = 0;
            int s = 0;
            var fd = new ClassData() { };
         //   string text;
          
            // text = String.Format("{0:X2}", ofs) + "\t";
            // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            // {
            fd.offset = ofs;
            DataRow row;
            row = booksTable.NewRow();
            fd.oneB = ss;
            //  text = String.Format("{0:X2}", ofs)+"\t";

            //   });
            while (i < bb1.Length)
            {
               
                if (s == 16)
                {
                  
                   // row.ItemArray = new object[] { null, String.Format("{0:X2}", bb1[i]), 200 };
                    // int x = booksTable.Columns.Count - 1;


                    // DataRow row1 = booksTable.NewRow();
                    // booksTable.Rows.Add(new object[] { null, String.Format("{0:X2}", bb1[i]) });
                    ClassDatas1.Add(row);
                    row = booksTable.NewRow();
                  
                    s = 0;
                 
                        fd.oneB = ss;
                   
                    ofs += 16;
                    //  text += "\n";
                    
                   // ClassDatas1.Add(fd);
            
                ss += 16;
                    fd = new ClassData() { };

                   
                    fd.offset =ofs;
                   

                }
                switch (s)
                {
                    case 0:
                      
                            fd.addByte(s, bb1[i]);
                        row[1] = String.Format("{0:X2}", bb1[i]);


                        break;
                    case 1:

                        fd.addByte(s, bb1[i]);
                        row[2] = String.Format("{0:X2}", bb1[i]);

                        break;
                    case 2:
                        fd.addByte(s, bb1[i]);
                        row[3] = String.Format("{0:X2}", bb1[i]);
                        break;
                    case 3:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 4:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 5:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 6:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 7:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 8:
                        fd.addByte(s, bb1[i]);

                        break;
                    case 9:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 10:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 11:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 12:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 13:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 14:
                        fd.addByte(s, bb1[i]);
                        break;
                    case 15:
                        fd.addByte(s, bb1[i]);
                        break;
                }
                s++;
                i++;
            }
            if(s!=0)
            {
                s = 0;

                fd.oneB = ss;
                ofs += 16;
                //  text += "\n";

              //  ClassDatas1.Add(fd);

                ss += 16;
               // fd = new ClassData() { };

                // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //  {
                //   text += String.Format("{0:X2}", ofs) + "\t";
               // fd.offset = ofs;
            }

           
        }
        public byte[] _bb1;
        public byte[] bb1
        {
            get
            {
                return _bb1;
            }
            set
            {
                _bb1 = value;
               // ClassDatas1.Clear();
             //  OpenF();
                 OnPropertyChanged("bb1");
            }
        }
      //  public byte[] bbuf { get; set; }
        public string Size { get; set; }
        public Visibility IsShow { get; set; }
        public Visibility IsShow1 { get; set; }
       
        
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
        ObservableCollection<DataRow> classDatas1;
        public ObservableCollection<DataRow> ClassDatas1
        {
            get
            {
                return classDatas1;
            }
            set
            {
                classDatas1 = value;
              
            }
        }
        ObservableCollection<byte> classDatas1B = new ObservableCollection<Byte>();
        public ObservableCollection<byte> ClassDatas1B
        {
            get
            {
                return classDatas1B;
            }
            set
            {
                classDatas1B = value;
           

            }
        }

        // public void iniz()
        //  {

        //  ClassDatas1 = new ObservableCollection<ClassData>();



        // }
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Raise the PropertyChanged event, passing the name of the property whose value has changed.
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
