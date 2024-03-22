using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;

namespace QuickEndpoint.Views
{
    public partial class EditApiView : UserControl
    {
        public EditApiView()
        {
            InitializeComponent();
            DataContext = new EditApiViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
