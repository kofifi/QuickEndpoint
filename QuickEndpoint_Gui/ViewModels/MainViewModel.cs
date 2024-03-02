using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace QuickEndpoint_Gui.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private bool _isPaneOpen;
    public bool IsPaneOpen
    {
        get => _isPaneOpen;
        set
        {
            if (SetProperty(ref _isPaneOpen, value))
            {
                foreach (var button in ItemsButton)
                {
                    button.UpdateContent(value);
                }
            }
        }
    }


    [ICommand]
    private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    private string _apiUrl;

    public string ApiUrl
    {
        get => _apiUrl;
        set => SetProperty(ref _apiUrl, value);
    }


    [ICommand]
    private void CreateApi()
    {
        // Logika tworzenia API
    }

    [ICommand]
    private void DeployApi()
    {
        // Logika deployowania API
    }

    [ICommand]
    private void InstallApi()
    {
        // Logika instalowania API
    }

    public class ButtonViewModel : ObservableObject
    {
        private string _contentOpen;
        private string _contentClosed;
        private string _content;
        public string Content
        {
            get => _content;
            set => SetProperty(ref _content, value);
        }

        private ICommand _command;
        public ICommand Command
        {
            get => _command;
            set => SetProperty(ref _command, value);
        }

        public ButtonViewModel(string contentOpen, string contentClosed, ICommand command)
        {
            _contentOpen = contentOpen;
            _contentClosed = contentClosed;
            _command = command;
            _content = contentClosed; // Domyślnie ustaw na zawartość dla zamkniętego panelu
        }

        public void UpdateContent(bool isPaneOpen)
        {
            Content = isPaneOpen ? _contentOpen : _contentClosed;
        }
    }

    public ObservableCollection<ButtonViewModel> ItemsButton { get; } = new ObservableCollection<ButtonViewModel>();
    public MainViewModel()
    {
        // Dodaj przyciski do listy
        ItemsButton.Add(new ButtonViewModel("Create Api", "\uE710", new RelayCommand(CreateApi)));
        ItemsButton.Add(new ButtonViewModel("Deploy Api", "\uE710", new RelayCommand(DeployApi)));
        ItemsButton.Add(new ButtonViewModel("Install Api", "\uE710", new RelayCommand(InstallApi)));
    }

}
