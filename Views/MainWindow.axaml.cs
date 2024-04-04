using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;

namespace QuickEndpoint.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // Usuwamy bezpośrednie tworzenie nowej instancji MainWindowViewModel
            // i zamiast tego korzystamy z już zainicjalizowanej instancji.
            DataContext = MainWindowViewModel.Current;
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
