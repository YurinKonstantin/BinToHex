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
            byte[] bb = new byte[0];
            ViewModel.ColTabs.Add(new VidDoc(bb)
            {
                Name = "Новый документ",
                ccc = 0,
                IsShow1 = Visibility.Collapsed,
                 IsShow = Visibility.Visible
                //  BiteFile=bb

            });

            //   cout++;

           // Tabs.SelectedIndex = 0;
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
                        if (ViewModel.ColTabs[0].Name == "Новый документ")
                        {
                            try
                            {


                               
                                  
                                    ViewModel.ColTabs.Add(new VidDoc(bb)
                                    {
                                        Name = file.DisplayName,
                                        Sppan = true,
                                        ccc = cout,
                                        IsShow = Visibility.Visible,
                                        IsShow1 = Visibility.Visible,

                                        Poz = 0,
                                        Path = file.Path,
                                        Size = basicProperties.Size.ToString(),
                                        //  BiteFile=bb

                                    });

                                //   cout++;
                                ViewModel.ColTabs.RemoveAt(0);
                                Tabs.SelectedIndex = 0;
                                

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
                            ViewModel.ColTabs.Insert(icol, new VidDoc(bb)
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
                                //   bb1 = bb
                                //  BiteFile=bb

                            });

                            Tabs.SelectedIndex = icol;
                            cout++;
                        }


                    }
                    else
                    {
                        ViewModel.ColTabs.Add(new VidDoc(bb)
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
                            //   bb1 = bb
                            //  BiteFile=bb

                        });
                        cout++;
                        Tabs.SelectedIndex = ViewModel.ColTabs.Count - 1;
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


                  await  Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                         {
                             ViewModel.ColTabs.Add(new VidDoc(bb)
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
                                 //   bb1 = bb
                                 //  BiteFile=bb

                             });
                         });
                    if (ViewModel.ColTabs[0].Name == "Новый документ")
                    {
                      
                        ViewModel.ColTabs.RemoveAt(0);

                    }

                    cout++;
                   await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        Tabs.SelectedIndex = ViewModel.ColTabs.Count - 1;
                    });
                 
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



        private async void AppBarButton_Click_2(object sender, RoutedEventArgs e)
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

        private async void TextBlock_Tapped_1(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
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
        private async void listViewer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MessageDialog d = new MessageDialog("ggfhg");
            await d.ShowAsync();

        }

        private async void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
        {
            TextBox f = (TextBox)sender;
            if (f != null & f.SelectionLength == 0)
            {
                int x = Tabs.SelectedIndex;
                int ds = 0;
                // StartSelectAndPoz(f.SelectionStart, Convert.ToInt32(f.Tag), out int start, out int poz);

                //  f.Select(start, 2);
                ViewModel.poisc(x, Convert.ToInt32(f.Tag));
            }
        }

        private async void Tabs_TabClosing(object sender, Microsoft.Toolkit.Uwp.UI.Controls.TabClosingEventArgs e)
        {
            if (ViewModel.ColTabs.Count==1)
            {
                byte[] bb = new byte[0];
                ViewModel.ColTabs.Add(new VidDoc(bb)
                {
                    Name = "Новый документ",
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

        private async void AppBarButton_Click_3(object sender, RoutedEventArgs e)
        {



        }
    }
}
