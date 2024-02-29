using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace QuickEndpoint_Gui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private bool _isPaneOpen;

    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set => SetProperty(ref _isPaneOpen, value);
    }

    [ICommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

}
