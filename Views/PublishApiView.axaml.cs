using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;

namespace QuickEndpoint.Views
{
    public partial class PublishApiView : UserControl
    {
        public PublishApiView()
        {
            InitializeComponent();
            DataContext = new PublishApiViewModel();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
