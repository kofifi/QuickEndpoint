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

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }

        public MainWindowViewModel()
        {
            CreateApiCommand = ReactiveCommand.Create(() => ExecuteCreateApi());
            EditApiCommand = ReactiveCommand.Create(() => ExecuteEditApi());
            PublishApiCommand = ReactiveCommand.Create(() => ExecutePublishApi());
            OpenSettingsCommand = ReactiveCommand.Create(() => ExecuteOpenSettings());

            // Ustawienie początkowego ViewModelu
            CurrentViewModel = new DashboardViewModel();
        }

        private void ExecuteCreateApi()
        {
            CurrentViewModel = new CreateApiViewModel();
        }

        private void ExecuteEditApi()
        {
            CurrentViewModel = new EditApiViewModel();
        }

        private void ExecutePublishApi()
        {
            CurrentViewModel = new PublishApiViewModel();
        }

        private void ExecuteOpenSettings()
        {
            CurrentViewModel = new OpenSettingsViewModel();
        }
    }
}
