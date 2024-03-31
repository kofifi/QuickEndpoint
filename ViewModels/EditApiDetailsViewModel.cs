using System.Linq;
using ReactiveUI;
using Avalonia.Collections;
using System.Reactive;
using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic; // Add the missing using directive
using QuickEndpoint.Services; // Make sure to include the namespace for FileDataService


namespace QuickEndpoint.ViewModels;

public class Origin
{
    public string OriginName { get; set; }
    public string ApiName { get; set; }
}

public class OriginDisplay
{
    public string OriginName { get; set; }
    public int NumberOfEndpoints { get; set;}
    public string DisplayName => $"{OriginName}, {NumberOfEndpoints}";
}

public class EditApiDetailsViewModel : ViewModelBase
{
    private readonly IFileDataService _fileDataService;
    private readonly ILoggerService _logger;
    private AvaloniaList<OriginDisplay> _availableOrigins = new AvaloniaList<OriginDisplay>();
    private OriginDisplay _selectedOrigin;
    private string _apiName;
    private string _newOriginName;
    private string _errorMessage;

    public AvaloniaList<OriginDisplay> AvailableOrigins
    {
        get => _availableOrigins;
        set => this.RaiseAndSetIfChanged(ref _availableOrigins, value);
    }

    public OriginDisplay SelectedOrigin
    {
        get => _selectedOrigin;
        set
        {
            this.RaiseAndSetIfChanged(ref _selectedOrigin, value);
            SelectedOriginName = value?.OriginName;
        }
    }

    private string _selectedOriginName;
    public string SelectedOriginName
    {
        get => _selectedOriginName;
        set => this.RaiseAndSetIfChanged(ref _selectedOriginName, value);
    }

    public string ApiName
    {
        get => _apiName;
        set
        {
            if (_apiName != value)
            {
                _apiName = value;
                this.RaisePropertyChanged(nameof(ApiName));
                RefreshOriginListCommand.Execute().Subscribe();
            }
        }
    }

    public string NewOriginName
    {
        get => _newOriginName;
        set => this.RaiseAndSetIfChanged(ref _newOriginName, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }
    private int _numberOfEndpoints;
    public int NumberOfEndpoints
    {
        get => _numberOfEndpoints;
        set => this.RaiseAndSetIfChanged(ref _numberOfEndpoints, value);
    }


    public ReactiveCommand<Unit, Unit> RefreshOriginListCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> AddOriginCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> EditSelectedOriginCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedOriginCommand { get; private set; }


    public EditApiDetailsViewModel(IFileDataService fileDataService, ILoggerService logger)
    {
        _fileDataService = fileDataService;
        _logger = logger;
        InitializeCommands();
        RefreshOriginListAsync().ConfigureAwait(false);
    }
    private void InitializeCommands()
    {
        RefreshOriginListCommand = ReactiveCommand.CreateFromTask(RefreshOriginListAsync);
        AddOriginCommand = ReactiveCommand.CreateFromTask(AddOriginAsync);
        EditSelectedOriginCommand = ReactiveCommand.CreateFromTask(EditSelectedOriginAsync, this.WhenAnyValue((EditApiDetailsViewModel x) => x.SelectedOrigin, (OriginDisplay selectedOrigin) => selectedOrigin != null));
        DeleteSelectedOriginCommand = ReactiveCommand.CreateFromTask(DeleteSelectedOriginAsync, this.WhenAnyValue((EditApiDetailsViewModel x) => x.SelectedOrigin, (OriginDisplay selectedOrigin) => selectedOrigin != null));
        AvailableOrigins = new AvaloniaList<OriginDisplay>();
    }


    private async Task RefreshOriginListAsync()
    {
        var origins = await _fileDataService.LoadDataAsync<Origin>("origins");
        var filteredOrigins = origins
            .Where(origin => origin.ApiName == ApiName)
            .GroupBy(origin => origin.OriginName)
            .ToList();

        var tempOrigins = new List<OriginDisplay>();
        foreach (var group in filteredOrigins)
        {
            int numberOfEndpoints = await CountEndpointsAsync(group.Key);
            var originDisplay = new OriginDisplay
            {
                OriginName = group.Key,
                NumberOfEndpoints = numberOfEndpoints
            };
            tempOrigins.Add(originDisplay);
        }

        AvailableOrigins = new AvaloniaList<OriginDisplay>(tempOrigins);
    }



    private async Task AddOriginAsync()
    {
        string apiDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", ApiName);

        await _logger.LogAsync($"Dodawanie nowego pochodzenia: {NewOriginName}");
        // Check if the Origin name is provided
        if (string.IsNullOrWhiteSpace(NewOriginName))
        {
            ErrorMessage = "The name of the new Origin is required.";
            await _logger.LogAsync("Nie podano nazwy nowego pochodzenia.");
            return;
        }

        // Use FileDataService to load existing origins
        var origins = await _fileDataService.LoadDataAsync<Origin>("origins");

        // Check if an Origin with the same name already exists within the API
        if (origins.Any(origin => origin.OriginName.Equals(NewOriginName, StringComparison.OrdinalIgnoreCase) && origin.ApiName == ApiName))
        {
            ErrorMessage = "An Origin with this name already exists within the API. Please use a different name.";
            return;
        }

        ErrorMessage = ""; // Clear the error message if all checks pass

        // Create a new Origin instance
        var newOrigin = new Origin
        {
            OriginName = NewOriginName,
            ApiName = ApiName
        };

        // Add the new Origin to the list
        origins.Add(newOrigin);

        // Use FileDataService to save the updated list of Origins
        await _fileDataService.SaveDataAsync("origins", origins);

        // Update the UI to reflect the new Origin
        AvailableOrigins.Add(new OriginDisplay { OriginName = newOrigin.OriginName });
        await _logger.LogAsync($"Nowe pochodzenie dodane pomyślnie: {NewOriginName}");
        NewOriginName = ""; // Reset the input fields for the next addition
        await RefreshOriginListAsync();
    }


    private async Task EditSelectedOriginAsync()
    {
        // Assuming you have a way to pass or set ApiName in EditApiDetailsPathsViewModel,
        // otherwise just instantiate it directly.
        var editApiDetailsPathsViewModel = new EditApiDetailsPathsViewModel
        {
            // Set properties as needed, for example:
            ApiName = this.ApiName,
            OriginName = this.SelectedOriginName
        };

        // Update the current ViewModel in MainWindowViewModel
        MainWindowViewModel.Current.CurrentViewModel = editApiDetailsPathsViewModel;
    }


    private async Task DeleteSelectedOriginAsync()
    {
        if (SelectedOrigin != null)
        {
            // Use FileDataService to load existing origins
            var origins = await _fileDataService.LoadDataAsync<Origin>("origins");

            var originToDelete = origins.FirstOrDefault(o => o.OriginName == SelectedOrigin.OriginName);
            if (originToDelete != null)
            {
                // Delete associated endpoints first
                await DeleteAssociatedEndpointsAsync(originToDelete.OriginName);

                // Now remove the origin from the origins list
                origins.Remove(originToDelete);

                // Use FileDataService to save the updated list of Origins
                await _fileDataService.SaveDataAsync("origins", origins);

                // Refresh the list of available origins in the UI
                await RefreshOriginListAsync();
            }
        }
    }

    private async Task DeleteAssociatedEndpointsAsync(string originName)
    {
        // Use FileDataService to load existing endpoints
        var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints"); 

        // Identify endpoints associated with the origin
        var endpointsToDelete = endpoints.Where(endpoint => endpoint.OriginName == originName).ToList();

        foreach (var endpoint in endpointsToDelete)
        {
            // Construct the controller file path for each endpoint
            string controllerFileName = $"{endpoint.OriginName}{endpoint.Name}Controller.cs";
            string controllersDirectory = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", endpoint.ApiName, "Controllers");
            string controllerFilePath = Path.Combine(controllersDirectory, controllerFileName);

            if (File.Exists(controllerFilePath))
            {
                try
                {
                    File.Delete(controllerFilePath); // Attempt to delete the controller file
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error deleting controller file for '{endpoint.Name}': {ex.Message}");
                    // Optionally, log this error or handle it as needed
                }
            }
        }

        // Filter out and update the endpoints list after deletion
        endpoints = endpoints.Except(endpointsToDelete).ToList();

        // Use FileDataService to save the updated list of endpoints
        await _fileDataService.SaveDataAsync("endpoints", endpoints);
    }

    private async Task<int> CountEndpointsAsync(string originName)
    {
        var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints");
        _logger.Log($"Loaded {endpoints.Count} endpoints:");

        foreach (var endpoint in endpoints)
        {
            _logger.Log($"Endpoint: {endpoint.Name}, Origin: {endpoint.OriginName}");
        }

        return endpoints.Count(endpoint => endpoint.OriginName == originName);
    }
}