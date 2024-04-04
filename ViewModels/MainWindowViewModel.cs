using ReactiveUI;
using System.Reactive;

namespace QuickEndpoint.ViewModels
{
    using QuickEndpoint.Services;

    public class MainWindowViewModel : ReactiveObject
    {
        private readonly NavigationService _navigationService;
        public ReactiveCommand<Unit, Unit> CreateApiCommand { get; }
        public ReactiveCommand<Unit, Unit> EditApiCommand { get; }
        public ReactiveCommand<Unit, Unit> PublishApiCommand { get; }
        public ReactiveCommand<Unit, Unit> OpenSettingsCommand { get; }

        private static MainWindowViewModel _current = new MainWindowViewModel();
        public static MainWindowViewModel Current => _current;

        private ViewModelBase _currentViewModel;
        public ViewModelBase CurrentViewModel
        {
            get => _currentViewModel;
            set => this.RaiseAndSetIfChanged(ref _currentViewModel, value);
        }

        private MainWindowViewModel()
        {
            _navigationService = new NavigationService(this);

            CreateApiCommand = ReactiveCommand.Create(() => _navigationService.NavigateTo<CreateApiViewModel>());
            EditApiCommand = ReactiveCommand.Create(() => _navigationService.NavigateTo<EditApiViewModel>());
            PublishApiCommand = ReactiveCommand.Create(() => _navigationService.NavigateTo<PublishApiViewModel>());
            OpenSettingsCommand = ReactiveCommand.Create(() => _navigationService.NavigateTo<OpenSettingsViewModel>());

            _navigationService.NavigateTo<DashboardViewModel>();
        }

        public static void InitializeCurrent()
        {
            if (_current == null)
            {
                _current = new MainWindowViewModel();
            }
        }
    }
}
