using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;

namespace QuickEndpoint.Views
{
    public partial class EditApiDetailsPathsView : UserControl
    {
        public EditApiDetailsPathsView()
        {
            InitializeComponent();
            DataContext = new EditApiDetailsPathsViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
