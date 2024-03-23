using ReactiveUI;
using System;
using Avalonia.Collections;
using System.Reactive;

namespace QuickEndpoint.ViewModels
{
    public class EditApiDetailsViewModel : ViewModelBase
    {
        private AvaloniaList<string> _availableEndpoints;
        private string _selectedEndpoint;

            private string _apiName;

        public string ApiName
        {
            get => _apiName;
            set => this.RaiseAndSetIfChanged(ref _apiName, value);
        }

        public EditApiDetailsViewModel()
        {
            // Initialize commands
            RefreshApiListCommand = ReactiveCommand.Create(RefreshApiList);

            // Placeholder data for AvailableEndpoints
            AvailableEndpoints = new AvaloniaList<string> { "Endpoint 1", "Endpoint 2", "Endpoint 3" };

            // Initialize other properties if necessary
        }

        public AvaloniaList<string> AvailableEndpoints
        {
            get => _availableEndpoints;
            set => this.RaiseAndSetIfChanged(ref _availableEndpoints, value);
        }

        public string SelectedEndpoint
        {
            get => _selectedEndpoint;
            set => this.RaiseAndSetIfChanged(ref _selectedEndpoint, value);
        }

        public ReactiveCommand<Unit, Unit> RefreshApiListCommand { get; }

        private void RefreshApiList()
        {
            // Simulate refreshing the list of endpoints. In a real application, this might involve fetching data from a database or an API.
            AvailableEndpoints.Clear();
            AvailableEndpoints.Add("Updated Endpoint 1");
            AvailableEndpoints.Add("Updated Endpoint 2");
            AvailableEndpoints.Add("Updated Endpoint 3");

            // Optionally, reset the selected endpoint or handle as needed.
        }

        // Include additional functionality as necessary, such as commands for editing specific endpoints, adding new ones, etc.
    }
}
