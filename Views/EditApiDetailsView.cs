using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;

namespace QuickEndpoint.Views
{
    public partial class EditApiDetailsView : UserControl
    {
        public EditApiDetailsView()
        {
            InitializeComponent();
            DataContext = new EditApiDetailsViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
