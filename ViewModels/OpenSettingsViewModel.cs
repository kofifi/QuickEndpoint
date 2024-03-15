using ReactiveUI;
using System;

namespace QuickEndpoint.ViewModels
{
    public class OpenSettingsViewModel : ReactiveObject
    {
        // Add properties specific to the Create API view here
        private string _apiName;
        public string ApiName
        {
            get => _apiName;
            set => this.RaiseAndSetIfChanged(ref _apiName, value);
        }

        public OpenSettingsViewModel()
        {
            // Initialize your ViewModel properties and commands here
        }

        // Add commands and methods for CreateApiViewModel
    }
}
