using System.Linq; 
using ReactiveUI;
using Avalonia.Collections;
using System.Reactive;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System;
using QuickEndpoint.Services; 

namespace QuickEndpoint.ViewModels;

public class Endpoint
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string ApiName { get; set; }
    public string OriginName { get; set; }
}

public class EndpointDisplay
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string DisplayName => $"{Method} {Name} {Path}";
}

public class EditApiDetailsPathsViewModel : ViewModelBase
{
    private AvaloniaList<EndpointDisplay> _availableEndpoints;
    private EndpointDisplay _selectedEndpoint;
    private string _apiName;
    private string _originName;
    private readonly IFileDataService _fileDataService;
    private readonly ILoggerService _logger;

    public string OriginName
    {
        get => _originName;
        set
        {
            this.RaiseAndSetIfChanged(ref _originName, value);
            RefreshApiListAsync().ConfigureAwait(false);
        }
    }
    
    private string _newEndpointPath;
    public string NewEndpointPath
    {
        get => _newEndpointPath;
        set => this.RaiseAndSetIfChanged(ref _newEndpointPath, value);
    }

    private string _newEndpointMethod = "GET";
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
        _fileDataService = new FileDataService();
        _logger = new LoggerService(); 
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
    _logger.Log($"Starting to add new endpoint: {NewEndpointName}");
    string apiDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", ApiName);

    if (string.IsNullOrWhiteSpace(NewEndpointName) || string.IsNullOrWhiteSpace(NewEndpointPath))
    {
        ErrorMessage = "Both the name and path of the new endpoint are required.";
        _logger.Log($"Failed to add endpoint due to missing name or path. Name: {NewEndpointName}, Path: {NewEndpointPath}");
        return;
    }

    try
    {
        var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints");

        var newEndpoint = new Endpoint
        {
            Name = NewEndpointName,
            Method = NewEndpointMethod,
            Path = NewEndpointPath,
            ApiName = ApiName,
            OriginName = OriginName
        };

        endpoints.Add(newEndpoint);
        await _fileDataService.SaveDataAsync("endpoints", endpoints);
        AvailableEndpoints.Add(new EndpointDisplay { Name = newEndpoint.Name, Method = newEndpoint.Method, Path = newEndpoint.Path });
        _logger.Log($"Successfully added new endpoint: {newEndpoint.Name} with method {newEndpoint.Method} and path {newEndpoint.Path}");

        string controllersDirectory = Path.Combine(apiDirectoryPath, "Controllers");
        Directory.CreateDirectory(controllersDirectory);
        string controllerFilePath = Path.Combine(controllersDirectory, $"{newEndpoint.OriginName}Controller.cs");

        if (File.Exists(controllerFilePath))
        {
            string existingControllerCode = await File.ReadAllTextAsync(controllerFilePath);
            // Remove the last two curly braces
            int lastCurlyBraceIndex = existingControllerCode.LastIndexOf("}");
            if (lastCurlyBraceIndex > 0)
            {
                existingControllerCode = existingControllerCode.Remove(lastCurlyBraceIndex);
                lastCurlyBraceIndex = existingControllerCode.LastIndexOf("}");
                if (lastCurlyBraceIndex > 0)
                {
                    existingControllerCode = existingControllerCode.Remove(lastCurlyBraceIndex);
                }
            }
            string newEndpointCode = CreateControllerCode(newEndpoint, appendMode: true);
            string updatedControllerCode = existingControllerCode + newEndpointCode + "\n}\n}";
            await File.WriteAllTextAsync(controllerFilePath, updatedControllerCode);
            _logger.Log($"Updated existing controller file at: {controllerFilePath}");
        }
        else
        {
            string controllerCode = CreateControllerCode(newEndpoint, appendMode: false);
            await File.WriteAllTextAsync(controllerFilePath, controllerCode);
            _logger.Log($"Created new controller file at: {controllerFilePath}");
        }

        NewEndpointName = string.Empty;
        NewEndpointPath = string.Empty;
        ErrorMessage = "";
    }
    catch (Exception ex)
    {
        ErrorMessage = "Failed to add the endpoint due to an internal error.";
        _logger.Log($"Exception encountered in AddEndpointAsync: {ex.Message}");
    }
}


private string CreateControllerCode(Endpoint endpoint, bool appendMode)
{
    _logger.Log($"Generating code for endpoint: {endpoint.Name}, Method: {endpoint.Method}, Path: {endpoint.Path}");

    string httpMethodAttribute = endpoint.Method switch
    {
        "POST" => "HttpPost",
        "PUT" => "HttpPut",
        "DELETE" => "HttpDelete",
        "PATCH" => "HttpPatch",
        _ => "HttpGet"
    };

    // Generate a unique method name based on the endpoint path
    string methodName = "Get" + endpoint.Path.Replace("/", string.Empty).Replace("{", string.Empty).Replace("}", string.Empty);

    if (appendMode)
    {
        return $@"
        [{httpMethodAttribute}(""{endpoint.Name}/{endpoint.Path}"")]
        public IActionResult {methodName}()
        {{
            // Logic for {endpoint.Path}
            return Ok(""Response from {endpoint.Path}"");
        }}";
    }
    else
    {
        return $@"
using Microsoft.AspNetCore.Mvc;

namespace {endpoint.ApiName}.Controllers
{{
    [ApiController]
    [Route(""[controller]"")]
    public class {endpoint.OriginName}Controller : ControllerBase
    {{
        [{httpMethodAttribute}(""{endpoint.Name}/{endpoint.Path}"")]
        public IActionResult {methodName}()
        {{
            // Logic for {endpoint.Path}
            return Ok(""Response from {endpoint.Path}"");
        }}
    }}
}}";
    }
}



private async Task EditSelectedEndpointAsync()
{
    _logger.Log("Starting to edit an endpoint.");
    List<Endpoint> endpoints = await LoadEndpointsAsync();

    var endpointToEdit = endpoints.FirstOrDefault(e => e.Name == SelectedEndpoint.Name && e.ApiName == ApiName && e.OriginName == OriginName);
    if (endpointToEdit != null)
    {
        _logger.Log($"Editing endpoint: {endpointToEdit.Name}, API: {endpointToEdit.ApiName}, Origin: {endpointToEdit.OriginName}");

        endpointToEdit.Method = NewEndpointMethod;
        endpointToEdit.Path = NewEndpointPath;

        await SaveEndpointsAsync(endpoints);
        await RefreshApiListAsync().ConfigureAwait(false);

        _logger.Log($"Successfully edited endpoint: {endpointToEdit.Name}");
    }
    else
    {
        _logger.Log($"No matching endpoint found to edit for Name: {SelectedEndpoint.Name}, API: {ApiName}, Origin: {OriginName}");
    }
}


private async Task DeleteSelectedEndpointAsync()
{
    if (SelectedEndpoint == null)
    {
        _logger.Log("No selected endpoint to delete.");
        return;
    }

    _logger.Log($"Attempting to delete endpoint: {SelectedEndpoint.Name}");
    var endpoints = await _fileDataService.LoadDataAsync<Endpoint>("endpoints");

    var endpointToDelete = endpoints.FirstOrDefault(e => e.Name == SelectedEndpoint.Name && e.ApiName == ApiName && e.OriginName == OriginName);
    if (endpointToDelete != null)
    {
        string controllerFileName = $"{endpointToDelete.OriginName}{endpointToDelete.Name}Controller.cs";
        string controllersDirectory = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", endpointToDelete.ApiName, "Controllers");
        string controllerFilePath = Path.Combine(controllersDirectory, controllerFileName);

        if (File.Exists(controllerFilePath))
        {
            try
            {
                File.Delete(controllerFilePath);
                _logger.Log($"Deleted controller file: {controllerFilePath}");
            }
            catch (Exception ex)
            {
                _logger.Log($"Error deleting controller file '{controllerFileName}': {ex.Message}");
            }
        }

        endpoints.Remove(endpointToDelete);
        await _fileDataService.SaveDataAsync("endpoints", endpoints);

        await RefreshApiListAsync();

        _logger.Log($"Successfully deleted endpoint: {endpointToDelete.Name}");
    }
    else
    {
        _logger.Log($"No matching endpoint found to delete for Name: {SelectedEndpoint.Name}, API: {ApiName}, Origin: {OriginName}");
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