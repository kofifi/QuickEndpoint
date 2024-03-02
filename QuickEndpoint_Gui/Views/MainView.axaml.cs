using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint_Gui.ViewModels;

namespace QuickEndpoint_Gui.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
        DataContext = new MainViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
