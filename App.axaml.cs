using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;
using QuickEndpoint.Views;

namespace QuickEndpoint;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Upewnij się, że instancja MainWindowViewModel jest zainicjalizowana
            MainWindowViewModel.InitializeCurrent();

            desktop.MainWindow = new MainWindow
            {
                // Użyj istniejącej instancji MainWindowViewModel
                DataContext = MainWindowViewModel.Current,
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
