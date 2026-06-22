using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace BinStudio.ViewModels
{
    public partial class HexRowViewModel : ObservableObject
    {
        [ObservableProperty] private string _address;
        // Каждая ячейка (байт) управляется отдельно
        public ObservableCollection<HexByteViewModel> Bytes { get; } = new();
    }
    public partial class HexByteViewModel : ObservableObject
    {
        [ObservableProperty] private string _hexValue;
        [ObservableProperty] private string _charValue;
        [ObservableProperty] private bool _isSelected;
        [ObservableProperty] private bool _isEditing;
    }
}
