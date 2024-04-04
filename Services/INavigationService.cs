using QuickEndpoint.ViewModels; // Add the missing using directive

namespace QuickEndpoint.Services
{
    // Definiuje kontrakt dla serwisu nawigacyjnego.
    public interface INavigationService
    {
        // Nawiguje do określonego ViewModelu, opcjonalnie przekazując parametr.
        void NavigateTo<TViewModel>(object parameter = null) where TViewModel : ViewModelBase, new();

        // Cofa do poprzedniego ViewModelu w historii nawigacji.
        void GoBack();
    }
}
