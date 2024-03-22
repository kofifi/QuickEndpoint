using ReactiveUI;

namespace QuickEndpoint.ViewModels
{
    public class OpenSettingsViewModel : ViewModelBase
    {
        private string _apiName;
        public string ApiName
        {
            get => _apiName;
            set => this.RaiseAndSetIfChanged(ref _apiName, value);
        }

        public override string Greeting => "Hello from OpenSettingsViewModel";
    }
}
