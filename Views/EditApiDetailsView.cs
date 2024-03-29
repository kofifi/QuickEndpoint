using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using QuickEndpoint.ViewModels;
using QuickEndpoint.Services; // Add this using directive

namespace QuickEndpoint.Views

{
    public partial class EditApiDetailsView : UserControl
    {
        public EditApiDetailsView()
        {
            InitializeComponent();
            DataContext = new EditApiDetailsViewModel(new FileDataService(), new LoggerService());
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
