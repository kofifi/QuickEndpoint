using System;
using ReactiveUI;
using System.Reactive;

namespace QuickEndpoint.ViewModels  
{
    public class MainWindowViewModel : ReactiveObject
    {
        public ReactiveCommand<Unit, Unit> CreateApiCommand { get; }
        public ReactiveCommand<Unit, Unit> EditApiCommand { get; }
        public ReactiveCommand<Unit, Unit> PublishApiCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

        private ReactiveObject _currentViewModel;
        public ReactiveObject CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }
        public MainWindowViewModel()
        {
            // Define the commands with their execution logic
            CreateApiCommand = ReactiveCommand.Create(() => ExecuteCreateApi());
            EditApiCommand = ReactiveCommand.Create(() => ExecuteEditApi());
            PublishApiCommand = ReactiveCommand.Create(() => ExecutePublishApi());
            OpenSettingsCommand = ReactiveCommand.Create(() => ExecuteOpenSettings());
            CurrentViewModel = new DashboardViewModel(); // Assuming you have a DashboardViewModel
        }

        private void ExecuteCreateApi()
        {
            // Implement the logic for creating an API
            Console.WriteLine("Creating API...");
            CurrentViewModel = new CreateApiViewModel(); // Switch to CreateApiViewModel
        }

        private void ExecuteEditApi()
        {
            // Implement the logic for editing an API
            Console.WriteLine("Editing API...");
            CurrentViewModel = new EditApiViewModel(); // Switch to EditApiViewModel
            Console.WriteLine("CurrentViewModel changed to EditApiViewModel.");
        }


        private void ExecutePublishApi()
        {
            // Implement the logic for publishing an API
            Console.WriteLine("Publishing API...");
            CurrentViewModel = new PublishApiViewModel();
        }

        private void ExecuteOpenSettings()
        {
            // Implement the logic for opening settings
            Console.WriteLine("Opening Settings...");
            CurrentViewModel = new OpenSettingsViewModel();
        }
    }
}