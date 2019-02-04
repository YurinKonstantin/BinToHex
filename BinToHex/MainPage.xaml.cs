using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using Windows.UI.ViewManagement;
using Windows.UI;
using System.Runtime.CompilerServices;
using System.Data;
using System.Threading.Tasks;
using Windows.UI.Core;
using Microsoft.Toolkit.Uwp.UI.Controls;
using Windows.UI.Xaml.Controls.Primitives;

using System.Collections.Generic;
using System.IO;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace BinToHex
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        int cout=0;

        /// <summary>
        /// Коллекция вкладок
        /// </summary>
        public ModalViewViDoc ViewModel { get; set; }
        public MainPage()
        {
           
            this.InitializeComponent();
            //   var t = ApplicationView.GetForCurrentView().TitleBar;
            // t.BackgroundColor = Colors.Indigo;
            // t.ForegroundColor = Colors.White;
            // t.ButtonBackgroundColor = Colors.Indigo;
            // t.ButtonForegroundColor = Colors.White;
        

            this.ViewModel = new ModalViewViDoc();
        
    
            this.DataContext = this;
           
            ViewModel.ColTabs.Add(new VidDoc()
            {
                Name = "New File",
                ccc = 0,
                IsShow1 = Visibility.Visible,
                 IsShow = Visibility.Visible
                //  BiteFile=bb

            });
            Window.Current.Activated += Current_Activated;
            //   cout++;

            // Tabs.SelectedIndex = 0;
        }
        private void Current_Activated(object sender, WindowActivatedEventArgs e)
        {
            ClassSetUpUser.SetPush();
            if (ClassSetUpUser.Application=="Dark")
            {
                rb2.IsChecked = true;
            }
            else
            {
                rb1.IsChecked = true;
            }
          //  RadioButton_Checked(null, null);


        }
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            
            Ring.IsActive = true;
            try
            {
                
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
               // picker.FileTypeFilter.Add(".bin");
               // picker.FileTypeFilter.Add(".txt");
               picker.FileTypeFilter.Add("*");
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);



                    byte[] bb = buffer.ToArray();
                    Windows.Storage.FileProperties.BasicProperties basicProperties = await file.GetBasicPropertiesAsync();
                   // ViewModel.newVid();
               


                    int icol = 0;
                    if (ViewModel.ColTabs.Count > 0)
                    {
                        if (ViewModel.ColTabs[0].Name == "New File")
                        {
                            try
                            {


                                VidDoc vidDoc = new VidDoc()
                                {
                                    Name = file.DisplayName,
                                    Sppan = true,
                                    ccc = cout,
                                    IsShow = Visibility.Visible,
                                    IsShow1 = Visibility.Visible,

                                    Poz = 0,
                                    Path = file.Path,
                                    Size = basicProperties.Size.ToString(),
                                    NameType = file.FileType
                                    //  BiteFile=bb

                                };

                                ViewModel.ColTabs.Add(vidDoc);

                                //   cout++;
                                ViewModel.ColTabs.RemoveAt(0);
                                Tabs.SelectedIndex = 0;
                                vidDoc.bb1 = bb;
                               await vidDoc.OpenF();

                                //Tabs.SelectedIndex = 0;
                                cout++;
                            }
                            catch(Exception ex)
                            {
                                MessageDialog messageDialog = new MessageDialog(ex.ToString());
                                await messageDialog.ShowAsync();
                            }

                        }
                        else
                        {


                            icol = Tabs.SelectedIndex;
                            ViewModel.ColTabs.RemoveAt(icol);
                            VidDoc vidDoc = new VidDoc()
                            {
                                //uuu
                                //jjj
                                Name = file.DisplayName,
                                Sppan = true,
                                ccc = cout,
                                IsShow = Visibility.Visible,
                                IsShow1 = Visibility.Visible,

                                Poz = 0,
                                Path = file.Path,
                                Size = basicProperties.Size.ToString(),
                                NameType = file.FileType
                                //   bb1 = bb
                                //  BiteFile=bb

                            };
                            ViewModel.ColTabs.Insert(icol, vidDoc);
                            Tabs.SelectedIndex = icol;
                            vidDoc.bb1 = bb;
                           await vidDoc.OpenF();
                            cout++;
                        }


                    }
                    else
                    {
                        VidDoc vidDoc = new VidDoc()
                        {
                            //uuu
                            //jjj
                            Name = file.DisplayName,
                            Sppan = true,
                            ccc = cout,
                            IsShow = Visibility.Visible,
                            IsShow1 = Visibility.Visible,

                            Poz = 0,
                            Path = file.Path,
                            Size = basicProperties.Size.ToString(),
                            NameType = file.FileType
                            //   bb1 = bb
                            //  BiteFile=bb

                        };
                        ViewModel.ColTabs.Add(vidDoc);
                        cout++;
                        Tabs.SelectedIndex = ViewModel.ColTabs.Count - 1;
                        vidDoc.bb1 = bb;
                        await vidDoc.OpenF();
                    }
                    Ring.IsActive = false;

                }
                else
                {
                    Ring.IsActive = false;
                    MessageDialog f = new MessageDialog("Файл не открыт");
                    await f.ShowAsync();
                }

                
            }
            catch
            {
              
                Ring.IsActive = false;
                MessageDialog f = new MessageDialog("Произошла ошибка, возможно файл поврежден");
                await f.ShowAsync();
            }
            Ring.IsActive = false;
        }
        public async Task OpenF(byte[] bb1, VidDoc vidDoc)
        {

            // iniz();
            int i = 0;
            int ofs = 0;
            int ss = 0;
            int s = 0;
           // var fd = new ClassData() { };
            //   string text;

            // text = String.Format("{0:X2}", ofs) + "\t";
            // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            // {
         //   fd.offset = ofs;
         //   fd.oneB = ss;
            //  text = String.Format("{0:X2}", ofs)+"\t";

            //   });
            while (i < bb1.Length)
            {
                if (s == 16)
                {
                    s = 0;

                  //  fd.oneB = ss;

                    ofs += 16;
                    //  text += "\n";

                  //  ClassDatas1.Add(fd);

                  ss += 16;
                  //  fd = new ClassData() { };


                 //   fd.offset = ofs;


                }
                switch (s)
                {
                    case 0:
                        await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                        {
                            vidDoc.ClassDatas1B.Add(bb1[i]);
                        });
                       // fd.addByte(s, bb1[i]);

                        break;
                    case 1:

                        //fd.addByte(s, bb1[i]);

                        break;
                    case 2:
                       // fd.addByte(s, bb1[i]);
                        break;
                    case 3:
                      //  fd.addByte(s, bb1[i]);
                        break;
                    case 4:
                      //  fd.addByte(s, bb1[i]);
                        break;
                    case 5:
                      //  fd.addByte(s, bb1[i]);
                        break;
                    case 6:
                       // fd.addByte(s, bb1[i]);
                        break;
                    case 7:
                       // fd.addByte(s, bb1[i]);
                        break;
                    case 8:
                       // fd.addByte(s, bb1[i]);

                        break;
                    case 9:
                      //  fd.addByte(s, bb1[i]);
                        break;
                    case 10:
                       // fd.addByte(s, bb1[i]);
                        break;
                    case 11:
                      //  fd.addByte(s, bb1[i]);
                        break;
                    case 12:
                       // fd.addByte(s, bb1[i]);
                        break;
                    case 13:
                      //  fd.addByte(s, bb1[i]);
                        break;
                    case 14:
                       // fd.addByte(s, bb1[i]);
                        break;
                    case 15:
                       // fd.addByte(s, bb1[i]);
                        break;
                }
                s++;
                i++;
            }
            if (s != 0)
            {
                s = 0;

                // fd.oneB = ss;
                //  ofs += 16;
                //  text += "\n";
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                  {
                      //vidDoc.ClassDatas1B.Add(bb1[i]);
                  });
              //  ClassDatas1.Add(fd);

                ss += 16;
                // fd = new ClassData() { };

                // await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                //  {
                //   text += String.Format("{0:X2}", ofs) + "\t";
                // fd.offset = ofs;
            }


        }
        private async void OpenNewFile(object sender, RoutedEventArgs e)
        {
            // ViewModel.IsShowBar = Visibility.Visible;
            Ring.IsActive = true;
            try
            {


                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                //  picker.FileTypeFilter.Add(".bin");
                //  picker.FileTypeFilter.Add(".txt");
                // picker.FileTypeFilter.Add(".doc");
                picker.FileTypeFilter.Add("*");
                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                  
                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);

                    byte[] bb = buffer.ToArray();
                    Windows.Storage.FileProperties.BasicProperties basicProperties = await file.GetBasicPropertiesAsync();

                    VidDoc vidDoc = new VidDoc()
                    {
                        //uuu
                        //jjj
                        Name = file.DisplayName,
                        Sppan = true,
                        ccc = cout,
                        IsShow = Visibility.Visible,
                        IsShow1 = Visibility.Visible,
                        Poz = 0,
                        Path = file.Path,
                        Size = basicProperties.Size.ToString(),
                        NameType = file.FileType
                        //   bb1 = bb
                        //  BiteFile=bb

                    };
                  await  Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             ViewModel.ColTabs.Add(vidDoc
                             );
                         });
                    if (ViewModel.ColTabs[0].Name == "New File")
                    {
                      
                        ViewModel.ColTabs.RemoveAt(0);

                    }


                    cout++;
                   await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Tabs.SelectedIndex = ViewModel.ColTabs.Count - 1;
                    });
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        vidDoc.bb1 = bb;
                    });
                    
                       await vidDoc.OpenF();
                    await OpenF(bb, vidDoc);
                    //ViewModel.IsShowBar = Visibility.Collapsed;
                    Ring.IsActive = false;

                }
                else
                {
                    MessageDialog f = new MessageDialog("Файл не открыт");
                    await f.ShowAsync();
                    Ring.IsActive = false;
                }
            }
            catch
            {
                MessageDialog f = new MessageDialog("Произошла ошибка, возможно файл поврежден");
                await f.ShowAsync();
                Ring.IsActive = false;
            }
            Ring.IsActive = false;
        }



        private void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string Fio = (string)btn.Tag.ToString();

            foreach (VidDoc tes in ViewModel.ColTabs)
            {
                if (tes.ccc == Convert.ToInt32(Fio))
                {

                    tes.Sppan = !tes.Sppan;
                    break;
                }
                else
                {

                }

            }
            //  var x = PivotItemsContainer.SelectedIndex;

        }

        private  void TextBlock_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            TextBlock f = (TextBlock)sender;
            f.SelectAll();

           // int x = PivotItemsContainer.SelectedIndex;

            int ds = 0;
            foreach (var d in ViewModel.ColTabs)
            {
              //  if (ds == x)
              //  {
                //    d.Poz = Convert.ToInt32(f.Tag);
                 //   return;
             //   }
                ds++;
            }


        }
        public void StartSelectAndPoz(int seleStart, int tag, out int start, out int poz)
        {
            start = 0;
            poz = 0;
            switch (seleStart)
            {
                case 0:

                    start = seleStart;
                    poz = 1 + tag;


                    break;
                case 1:
                    start = seleStart - 1;
                    poz = 1 + tag;


                    break;
                case 2:

                    start = seleStart - 2;
                    poz = 1 + tag;

                    break;
                case 3:
                    start = seleStart;
                    poz = 2 + tag;
                    break;
                case 4:
                    start = seleStart - 1;
                    poz = 2 + tag;

                    break;
                case 5:
                    start = seleStart - 2;
                    poz = 2 + tag;
                    break;
                case 6:

                    start = seleStart;
                    poz = 3 + tag;

                    break;
                case 7:

                    start = seleStart - 1;
                    poz = 3 + tag;

                    break;
                case 8:

                    start = seleStart - 2;
                    poz = 3 + tag;

                    break;
                case 9:
                    start = seleStart;
                    poz = 4 + tag;


                    break;
                case 10:
                    start = seleStart - 1;
                    poz = 4 + tag;


                    break;
                case 11:

                    start = seleStart - 2;
                    poz = 4 + tag;

                    break;
                case 12:
                    start = seleStart;
                    poz = 5 + tag;


                    break;
                case 13:
                    start = seleStart - 1;
                    poz = 5 + tag;


                    break;
                case 14:

                    start = seleStart - 2;
                    poz = 5 + tag;

                    break;

                case 15:
                    start = seleStart;
                    poz = 6 + tag;


                    break;
                case 16:
                    start = seleStart - 1;
                    poz = 6 + tag;


                    break;
                case 17:

                    start = seleStart - 2;
                    poz = 6 + tag;

                    break;

                case 18:
                    start = seleStart;
                    poz = 7 + tag;


                    break;
                case 19:
                    start = seleStart - 1;
                    poz = 7 + tag;


                    break;
                case 20:

                    start = seleStart - 2;
                    poz = 7 + tag;

                    break;

                case 21:
                    start = seleStart;
                    poz = 8 + tag;


                    break;
                case 22:
                    start = seleStart - 1;
                    poz = 8 + tag;


                    break;
                case 23:

                    start = seleStart - 2;
                    poz = 8 + tag;

                    break;
                case 24:
                    start = seleStart;
                    poz = 9 + tag;


                    break;
                case 25:
                    start = seleStart - 1;
                    poz = 9 + tag;


                    break;
                case 26:

                    start = seleStart - 2;
                    poz = 9 + tag;

                    break;

                case 27:
                    start = seleStart;
                    poz = 10 + tag;


                    break;
                case 28:
                    start = seleStart - 1;
                    poz = 10 + tag;


                    break;
                case 29:

                    start = seleStart - 2;
                    poz = 10 + tag;

                    break;

                case 30:
                    start = seleStart;
                    poz = 11 + tag;


                    break;
                case 31:
                    start = seleStart - 1;
                    poz = 11 + tag;


                    break;
                case 32:

                    start = seleStart - 2;
                    poz = 11 + tag;

                    break;

                case 33:
                    start = seleStart;
                    poz = 12 + tag;


                    break;
                case 34:
                    start = seleStart - 1;
                    poz = 12 + tag;


                    break;
                case 35:

                    start = seleStart - 2;
                    poz = 12 + tag;

                    break;

                case 36:
                    start = seleStart;
                    poz = 13 + tag;


                    break;
                case 37:
                    start = seleStart - 1;
                    poz = 13 + tag;


                    break;
                case 38:

                    start = seleStart - 2;
                    poz = 13 + tag;

                    break;

                case 39:
                    start = seleStart;
                    poz = 14 + tag;


                    break;
                case 40:
                    start = seleStart - 1;
                    poz = 14 + tag;


                    break;
                case 41:

                    start = seleStart - 2;
                    poz = 14 + tag;

                    break;

                case 42:
                    start = seleStart;
                    poz = 15 + tag;


                    break;
                case 43:
                    start = seleStart - 1;
                    poz = 15 + tag;


                    break;
                case 44:

                    start = seleStart - 2;
                    poz = 15 + tag;

                    break;

            }
        }
  

        private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
           // MessageDialog messageDialog = new MessageDialog("событие");
          //  await messageDialog.ShowAsync();
          if(ClassSetUpUser.ShovH)
            {
                helpRed.Visibility = Visibility.Visible;
            }
       
            TextBox f = (TextBox)sender;
           // DataGrid dataGrid = sender as DataGrid;
    

            if (f != null & f.SelectionLength == 0)
            {
                int x = Tabs.SelectedIndex;
               // int ds = 0;
                // StartSelectAndPoz(f.SelectionStart, Convert.ToInt32(f.Tag), out int start, out int poz);

                //  f.Select(start, 2);
                ViewModel.poisc(x, Convert.ToInt32(f.Tag));
            }
        }

        private void Tabs_TabClosing(object sender, Microsoft.Toolkit.Uwp.UI.Controls.TabClosingEventArgs e)
        {
            if (ViewModel.ColTabs.Count==1)
            {
                byte[] bb = new byte[0];
                ViewModel.ColTabs.Add(new VidDoc()
                {
                    Name = "New File",
                    ccc = 0,
                    IsShow1 = Visibility.Collapsed
                    //  BiteFile=bb

                });

                //   cout++;

                Tabs.SelectedIndex = 0;
            }
        
            //  var x = PivotItemsContainer.SelectedIndex;
         //   ViewModel.ColTabs.RemoveAt(0);
            // TabViewNotification.Show("You're closing the '" + e.Tab.Header + "' tab.", 2000);
            //  var f = e.Tab.Tag;
            //TabView tabs1 = (TabView)sender;
            //    tabs1.SelectedIndex = -1;
            //   tabs1.SelectedItem = null;
            //   MessageDialog d = new MessageDialog(tabs1.SelectedIndex.ToString());
            //  await d.ShowAsync();

        }


        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            ClassSetUpUser.saveUseSet();
        }

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
           
           

            
            RadioButton rb = sender as RadioButton;
                if (rb != null )
                {
                    MessageDialog messageDialog = new MessageDialog("Параметры будут применены после перезагрузки приложения");
                    string colorName = rb.Tag.ToString();
                    switch (colorName)
                    {
                        case "Light":
                            try
                            {
                                ClassSetUpUser.Application = "Light";
                                ClassSetUpUser.saveUseSet();
                                if (!ClassSetUpUser.start)
                                {
                                    await messageDialog.ShowAsync();
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageDialog message = new MessageDialog(ex.ToString());
                                await message.ShowAsync();
                            }
                            break;
                        case "Dark":
                            ClassSetUpUser.Application = "Dark";
                            ClassSetUpUser.saveUseSet();
                            if (!ClassSetUpUser.start)
                            {
                           
                                await messageDialog.ShowAsync();
                            }

                            break;

                    }
               


            }
            ClassSetUpUser.start = false;

        }

        private void AppBarButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            FlyoutBase.ShowAttachedFlyout((FrameworkElement)sender);
        }


        private async void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            MessageDialog messageDialog = new MessageDialog("событие");
           await messageDialog.ShowAsync();
            helpRed.Visibility = Visibility.Visible;
            TextBox f = (TextBox)sender;
            if (f != null & f.SelectionLength == 0)
            {
                int x = Tabs.SelectedIndex;
                int i = 0;
               foreach(var dVid in ViewModel.ColTabs)
                {
                    if(i==x)
                    {
                     
                        dVid.bb1[Convert.ToInt32(f.Tag)-1] = Convert.ToByte(Convert.ToInt32(f.Text.ToString(), 16));
                        dVid.ClassDatas1.Clear();
                      await  dVid.OpenF();
                      //  ViewModel.ColTabs.Insert(x, dVid);
                      // ViewModel.ColTabs.RemoveAt(x+1);
                        break;
                    }
                    i++;
                }
                //MessageDialog messageDialog = new MessageDialog(f.Tag.ToString()+" "+f.Text+" "+x.ToString());
              //  messageDialog.ShowAsync();
                //ViewModel.poisc(x, Convert.ToInt32(f.Tag));
            }
        }

        private async void dataGrid1_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {


                DataGrid f = (DataGrid)sender;
                if (f != null)
                {
                    var textBlock = (TextBox)e.EditingElement;
                  //  MessageDialog messageDialog = new MessageDialog(textBlock.Tag.ToString() + " " + textBlock.Text + " ");
                  //  await messageDialog.ShowAsync();
                    int x = Tabs.SelectedIndex;
                    int i = 0;
                    foreach (var dVid in ViewModel.ColTabs)
                    {
                        if (i == x)
                        {

                            dVid.bb1[Convert.ToInt32(textBlock.Tag) - 1] = Convert.ToByte(Convert.ToInt32(textBlock.Text.ToString(), 16));
                            dVid.ClassDatas1.Clear();
                            await dVid.OpenF();
                            //  ViewModel.ColTabs.Insert(x, dVid);
                            // ViewModel.ColTabs.RemoveAt(x+1);
                            break;
                        }
                        i++;
                    }
                 //   MessageDialog messageDialog1 = new MessageDialog(textBlock.Tag.ToString() + " " + textBlock.Text + " " + x.ToString());
                   // await messageDialog1.ShowAsync();
                    //ViewModel.poisc(x, Convert.ToInt32(f.Tag));
                }
            }
            catch(Exception )
            {

            }
        }

        private async void TextBox_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            MessageDialog messageDialog = new MessageDialog("событие");
            await  messageDialog.ShowAsync();
            helpRed.Visibility = Visibility.Visible;
            TextBox f = (TextBox)sender;
            if (f != null & f.SelectionLength == 0)
            {
                int x = Tabs.SelectedIndex;
              //  int ds = 0;
                // StartSelectAndPoz(f.SelectionStart, Convert.ToInt32(f.Tag), out int start, out int poz);

                //  f.Select(start, 2);
                ViewModel.poisc(x, Convert.ToInt32(f.Tag));
            }
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            helpRed.Visibility = Visibility.Collapsed;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox rb = sender as CheckBox;
            if (rb != null)
            {
            
             if(Convert.ToBoolean(rb.IsChecked))
                {
                    ClassSetUpUser.ShovH = false;
                    ClassSetUpUser.saveUseSet();
                }
             else
                {
                    ClassSetUpUser.ShovH = true;
                    ClassSetUpUser.saveUseSet();
                }


            }
        }
        DataConverter DataConverter1 = new DataConverter(); 
        public DataTable booksTable=new DataTable();
        public void DataGrid1_Loading(FrameworkElement sender, object args)
        {
            DataGrid dataGrid = (DataGrid)sender;
            ViewModel.ColTabs[0].dataGrid = dataGrid;

            booksTable = new DataTable("name");
            DataColumn idColumn = new DataColumn("offset", Type.GetType("System.Int32"));
            idColumn.Unique = true; // столбец будет иметь уникальное значение
            idColumn.AllowDBNull = false; // не может принимать null
            idColumn.AutoIncrement = true; // будет автоинкрементироваться
            idColumn.AutoIncrementSeed = 1; // начальное значение
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
         


            if (booksTable != null) // table is a DataTable
            {
                int i = 0;
                foreach (DataColumn col in booksTable.Columns)
                {
                    // booksTable.Columns.Add(
                    //  new DataGridTextColumn
                    //  {
                    //    Header = col.ColumnName,

                    //   Binding = new Binding(string.Format("[{0}]", col.ColumnName))



                    //  });
                    dataGrid.Columns.Add(new DataGridTextColumn()
                    {
                        Header = col.ColumnName,
                        Binding = new Binding { Path = new PropertyPath("[" + col.ColumnName.ToString() + "]") },
                        IsReadOnly = false
                    });

                }

            }
        }
    }
}
