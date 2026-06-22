using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace BinViewer.MVVM
{
    public class ModalMain : INotifyPropertyChanged
    {
        private readonly ResourceManager _resourceManager;
        private readonly ResourceContext _resourceContext;
        public ModalMain(ResourceManager resourceManager, ResourceContext resourceContex)
        {
            _resourceManager = resourceManager;
            _resourceContext = resourceContex;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        // Храним ссылку на текущую активную ViewModel, чтобы вовремя отписываться от её событий
        HexDocumentViewModel activeViewModel;
        public HexDocumentViewModel ActiveViewModel
        {
            get
            {
                return activeViewModel;
            }
            set
            {
                activeViewModel = value;
                OnPropertyChanged("ActiveViewModel");

            }
        }
        public ObservableCollection<HexDocumentViewModel> TabViewDocuments = new ObservableCollection<HexDocumentViewModel>();
        public void OpenFile(string path)
        {
            var newTab = new HexDocumentViewModel(path);
            TabViewDocuments.Add(newTab);
            ActiveViewModel = newTab;
        }
        public void FileTabView_TabCloseRequested(TabView sender, TabViewTabCloseRequestedEventArgs args)
        {
            //if (args.Item is TabViewItem tab && tab.Tag is HexDocumentViewModel vm)
            //{
            if (args.Item is HexDocumentViewModel tabToRemove)
            {
                TabViewDocuments.Remove(tabToRemove);
                tabToRemove.Buffer.Dispose();
            }

            {
               

                //// Если закрываемая вкладка была активной, сбрасываем ссылку
                //if (_activeViewModel == vm)
                //{
                //    vm.PropertyChanged -= ViewModel_StatusChanged;
                //    _activeViewModel = null;
                //}

                // ОСВОБОЖДАЕМ ФАЙЛ: закрываем FileStream, чтобы Windows сняла блокировку доступа
               // vm.Buffer.Dispose();

              //  FileTabView.TabItems.Remove(tab);
             //   UpdateStatusBar();
              //  UpdateWindowTitle();
            }
        }
        //public void FileTabView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //   UpdateStatusBar();
        //    UpdateWindowTitle(); // Добавьте вызов сюда
        //}
        public async Task FileTabView_AddTabButtonClick(TabView sender, object args)
        {
            try
            {

                var picker = new FileOpenPicker();
                InitializeWithWindow.Initialize(picker, WindowNative.GetWindowHandle(this));
                picker.FileTypeFilter.Add("*");

                var file = await picker.PickSingleFileAsync();
                if (file != null)
                {

                    OpenFile(file.Path);
                    //// 1. Создаем ViewModel данных для файла
                    //var tabViewModel = new HexDocumentViewModel(file.Path);

                    //// 2. Создаем визуальный элемент (UI) для этой вкладки и передаем ему данные
                    //var editorView = new HexEditorView(tabViewModel);


                    //// 3. Собираем вкладку воедино
                    //var newTab = new TabViewItem
                    //{
                    //    Header = tabViewModel.FileName,
                    //    Content = editorView, // Помещаем UI внутрь вкладки
                    //    Tag = file.Path   // Сохраняем ссылку на ViewModel для поиска и сохранения
                    //};

                    //FileTabView.TabItems.Add(newTab);
                    //FileTabView.SelectedItem = newTab;
                    //UpdateStatusBar();
                    //UpdateWindowTitle(); // Добавьте вызов сюда
                }
            }
            catch (Exception ex)
            {
                // Запись в файл на рабочем столе, так как UI может не работать
                //File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "hex_error.txt"), ex.ToString());
            }
        }
    }
}
