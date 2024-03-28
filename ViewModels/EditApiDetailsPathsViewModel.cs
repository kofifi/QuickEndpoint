using System.Linq; // Add this line at the top of your file
using ReactiveUI;
using Avalonia.Collections;
using System.Reactive;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;
using QuickEndpoint.Services; // Make sure to include the namespace for FileDataService

namespace QuickEndpoint.ViewModels;

public class Endpoint
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string ApiName { get; set; } // Identifies the API
    public string OriginName { get; set; } // NEW: Identifies the Origin within the API
}

public class EndpointDisplay
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; } // Add this line
    public string DisplayName => $"{Method} {Name} {Path}";
}
public class EditApiDetailsPathsViewModel : ViewModelBase
{
    private AvaloniaList<EndpointDisplay> _availableEndpoints;
    private EndpointDisplay _selectedEndpoint;
    private string _apiName;
    private string _originName; // NEW: Holds the selected origin's name
    private readonly FileDataService _fileDataService;


    // Add a property to set the origin name when an origin is selected
    public string OriginName
    {
        get => _originName;
        set
        {
            this.RaiseAndSetIfChanged(ref _originName, value);
            RefreshApiListAsync().ConfigureAwait(false); // Refresh list when origin changes
        }
    }
    
    private string _newEndpointPath;
    public string NewEndpointPath
    {
        get => _newEndpointPath;
        set => this.RaiseAndSetIfChanged(ref _newEndpointPath, value);
    }

    private string _newEndpointMethod = "GET"; // Default value
    public string NewEndpointMethod
    {
        get => _newEndpointMethod;
        set => this.RaiseAndSetIfChanged(ref _newEndpointMethod, value);
    }

    public List<string> HttpMethods { get; } = new List<string> { "GET", "POST", "PUT", "DELETE", "PATCH" };

    public AvaloniaList<EndpointDisplay> AvailableEndpoints
    {
        get => _availableEndpoints;
        set => this.RaiseAndSetIfChanged(ref _availableEndpoints, value);
    }

    public EndpointDisplay SelectedEndpoint
    {
        get => _selectedEndpoint;
        set => this.RaiseAndSetIfChanged(ref _selectedEndpoint, value);
    }

    public string ApiName
    {
        get => _apiName;
        set
        {
            this.RaiseAndSetIfChanged(ref _apiName, value);
            RefreshApiListAsync().ConfigureAwait(false);
        }
    }

    private string _newEndpointName;
    public string NewEndpointName
    {
        get => _newEndpointName;
        set => this.RaiseAndSetIfChanged(ref _newEndpointName, value);
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }



    public ReactiveCommand<Unit, Unit> RefreshApiListCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> AddEndpointCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> EditSelectedEndpointCommand { get; private set; }
    public ReactiveCommand<Unit, Unit> DeleteSelectedEndpointCommand { get; private set; }
    

    public EditApiDetailsPathsViewModel()
    {
        _fileDataService = new FileDataService(); // Initialize FileDataService here
        RefreshApiListCommand = ReactiveCommand.CreateFromTask(RefreshApiListAsync);
        AddEndpointCommand = ReactiveCommand.CreateFromTask(AddEndpointAsync);
        EditSelectedEndpointCommand = ReactiveCommand.CreateFromTask(EditSelectedEndpointAsync, this.WhenAnyValue((EditApiDetailsPathsViewModel x) => x.SelectedEndpoint, (EndpointDisplay selectedEndpoint) => selectedEndpoint != null));
        DeleteSelectedEndpointCommand = ReactiveCommand.CreateFromTask(DeleteSelectedEndpointAsync, this.WhenAnyValue((EditApiDetailsPathsViewModel x) => x.SelectedEndpoint, (EndpointDisplay selectedEndpoint) => selectedEndpoint != null));

        AvailableEndpoints = new AvaloniaList<EndpointDisplay>();

        // Automatically refresh the list of endpoints when the ViewModel is created
        RefreshApiListAsync().ConfigureAwait(false);
    }


private async Task RefreshApiListAsync()
{
    var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints");

    var filteredEndpoints = endpoints
        .Where(endpoint => endpoint.ApiName == ApiName && endpoint.OriginName == OriginName)
        .Select(endpoint => new EndpointDisplay
        { 
            Name = endpoint.Name, 
            Method = endpoint.Method,
            Path = endpoint.Path
        })
        .ToList();

    AvailableEndpoints.Clear();
    foreach (var endpoint in filteredEndpoints)
    {
        AvailableEndpoints.Add(endpoint);
    }
}

private async Task AddEndpointAsync()
{
    string apiDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", ApiName);

    if (string.IsNullOrWhiteSpace(NewEndpointName) || string.IsNullOrWhiteSpace(NewEndpointPath))
    {
        ErrorMessage = "Both the name and path of the new endpoint are required.";
        return;
    }

    // Ładowanie istniejących endpointów za pomocą FileDataService
    var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints");

    // Sprawdzanie duplikatów endpointów w obrębie tego samego API i origin
    if (endpoints.Any(ep => ep.Name.Equals(NewEndpointName, StringComparison.OrdinalIgnoreCase)
                            && ep.ApiName == ApiName && ep.OriginName == OriginName))
    {
        ErrorMessage = "An endpoint with this name already exists within the selected origin. Please use a different name.";
        return;
    }

    // Tworzenie nowego endpointu
    var newEndpoint = new Endpoint
    {
        Name = NewEndpointName,
        Method = NewEndpointMethod,
        Path = NewEndpointPath,
        ApiName = ApiName,
        OriginName = OriginName
    };

    // Dodawanie nowego endpointu do listy i zapisywanie zmian
    endpoints.Add(newEndpoint);
    await _fileDataService.SaveDataAsync("endpoints", endpoints);

    // Aktualizacja UI
    AvailableEndpoints.Add(new EndpointDisplay { Name = newEndpoint.Name, Method = newEndpoint.Method, Path = newEndpoint.Path });

    // Tworzenie kodu kontrolera i zapisywanie pliku kontrolera
    string controllerCode = CreateControllerCode(newEndpoint);
    string controllersDirectory = Path.Combine(apiDirectoryPath, "Controllers");
    Directory.CreateDirectory(controllersDirectory);
    string controllerFilePath = Path.Combine(controllersDirectory, $"{newEndpoint.OriginName}{newEndpoint.Name}Controller.cs");
    await File.WriteAllTextAsync(controllerFilePath, controllerCode);

    // Resetowanie pól wejściowych
    NewEndpointName = string.Empty;
    NewEndpointPath = string.Empty;
    ErrorMessage = "";
}

private string CreateControllerCode(Endpoint endpoint)
{
    string httpMethodAttribute = endpoint.Method switch
    {
        "POST" => "[HttpPost]",
        "PUT" => "[HttpPut]",
        "DELETE" => "[HttpDelete]",
        "PATCH" => "[HttpPatch]",
        _ => "[HttpGet]"
    };

    string actionMethod = endpoint.Method switch
    {
        "POST" => "Post",
        "PUT" => "Put",
        "DELETE" => "Delete",
        "PATCH" => "Patch",
        _ => "Get"
    };

    return $@"
using Microsoft.AspNetCore.Mvc;

namespace {endpoint.ApiName}.Controllers
{{
    [ApiController]
    [Route(""[controller]"")]
    public class {endpoint.OriginName}{endpoint.Name}Controller : ControllerBase
    {{
        {httpMethodAttribute}
        [Route(""{endpoint.OriginName}{endpoint.Path}"")]
        public IActionResult {actionMethod}()
        {{
            return Ok(""{endpoint.OriginName}{endpoint.Name} response"");
        }}
    }}
}}";
}



    private async Task EditSelectedEndpointAsync()
    {
        List<Endpoint> endpoints = await LoadEndpointsAsync();

        var endpointToEdit = endpoints.FirstOrDefault(e => e.Name == SelectedEndpoint.Name 
                                                        && e.ApiName == ApiName 
                                                        && e.OriginName == OriginName);
        if (endpointToEdit != null)
        {
            // Update endpoint details based on user input
            endpointToEdit.Method = NewEndpointMethod;
            endpointToEdit.Path = NewEndpointPath;

            await SaveEndpointsAsync(endpoints);
            RefreshApiListAsync().ConfigureAwait(false);
        }
    }

private async Task DeleteSelectedEndpointAsync()
{
    if (SelectedEndpoint != null)
    {
        var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints");

        // Znajdź i usuń wybrany endpoint
        var endpointToDelete = endpoints.FirstOrDefault(e => e.Name == SelectedEndpoint.Name && e.ApiName == ApiName && e.OriginName == OriginName);
        if (endpointToDelete != null)
        {
            // Usuń plik kontrolera (jeśli to konieczne, zaimplementuj logikę tutaj)
            // Należy rozważyć przeniesienie logiki usuwania plików kontrolera do dedykowanej metody wewnątrz FileDataService lub innego serwisu
            string controllerFileName = $"{endpointToDelete.OriginName}{endpointToDelete.Name}Controller.cs";
            string controllersDirectory = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", endpointToDelete.ApiName, "Controllers");
            string controllerFilePath = Path.Combine(controllersDirectory, controllerFileName);

            if (File.Exists(controllerFilePath))
            {
                try
                {
                    File.Delete(controllerFilePath);
                    Console.WriteLine($"Deleted controller file: {controllerFilePath}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error deleting controller file '{controllerFileName}': {ex.Message}");
                }
            }

            // Usuń endpoint z listy i zapisz zmiany
            endpoints.Remove(endpointToDelete);
            await _fileDataService.SaveDataAsync("endpoints", endpoints);

            // Odśwież listę endpointów
            await RefreshApiListAsync();
        }
    }
}
        private async Task<List<Endpoint>> LoadEndpointsAsync()
        {
            return await _fileDataService.LoadDataAsync<Endpoint>("endpoints");
        }

        private async Task SaveEndpointsAsync(List<Endpoint> endpoints)
        {
            await _fileDataService.SaveDataAsync("endpoints", endpoints);
        }
}