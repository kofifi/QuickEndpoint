using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;

namespace QuickEndpoint.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            DataContext = new DashboardViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
