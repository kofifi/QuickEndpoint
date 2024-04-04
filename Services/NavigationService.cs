using QuickEndpoint.ViewModels;
using System.Collections.Generic;

namespace QuickEndpoint.Services
{
    public class NavigationService : INavigationService
    {
        private readonly MainWindowViewModel _mainWindowViewModel;
        private readonly Stack<ViewModelBase> _navigationStack = new Stack<ViewModelBase>();

        public NavigationService(MainWindowViewModel mainWindowViewModel)
        {
            _mainWindowViewModel = mainWindowViewModel;
        }

        public void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase, new()
        {
            var viewModel = new TViewModel();

            if (viewModel is IParameterizedNavigation parameterizedViewModel)
            {
                parameterizedViewModel.SetParameter(parameter);
            }

            _mainWindowViewModel.CurrentViewModel = viewModel; // Bezpieczne użycie przez przekazaną referencję
            _navigationStack.Push(viewModel);
        }

        public void GoBack()
        {
            if (_navigationStack.Count > 1) // Nie cofaj się do początkowego ViewModelu
            {
                _navigationStack.Pop();
                var previousViewModel = _navigationStack.Peek();
                MainWindowViewModel.Current.CurrentViewModel = previousViewModel;
            }
        }
    }
}
