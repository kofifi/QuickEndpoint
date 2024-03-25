using System.Linq; // Add this line at the top of your file
using ReactiveUI;
using Avalonia.Collections;
using System.Reactive;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System;

namespace QuickEndpoint.ViewModels;

public class Endpoint
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string Path { get; set; }
    public string ApiName { get; set; } // Add this line
}

public class EditApiDetailsViewModel : ViewModelBase
{
    private AvaloniaList<string> _availableEndpoints;
    private string _selectedEndpoint;
    private string _apiName;
    private const string EndpointsFilePath = "endpoints.json";

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
    

    public EditApiDetailsViewModel()
    {
        RefreshApiListCommand = ReactiveCommand.CreateFromTask(RefreshApiListAsync);
        AddEndpointCommand = ReactiveCommand.CreateFromTask(AddEndpointAsync);
        EditSelectedEndpointCommand = ReactiveCommand.CreateFromTask(EditSelectedEndpointAsync, this.WhenAnyValue(x => x.SelectedEndpoint, (selectedEndpoint) => !string.IsNullOrEmpty(selectedEndpoint)));
        DeleteSelectedEndpointCommand = ReactiveCommand.CreateFromTask(DeleteSelectedEndpointAsync, this.WhenAnyValue(x => x.SelectedEndpoint, (selectedEndpoint) => !string.IsNullOrEmpty(selectedEndpoint)));

        AvailableEndpoints = new AvaloniaList<string>();

        // Automatically refresh the list of endpoints when the ViewModel is created
        RefreshApiListAsync().ConfigureAwait(false);
    }


private async Task RefreshApiListAsync()
{
    if (File.Exists(EndpointsFilePath))
    {
        var json = await File.ReadAllTextAsync(EndpointsFilePath);
        var endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(json) ?? new List<Endpoint>();
        
        // Filter the endpoints list to only include those belonging to the selected API.
        var filteredEndpoints = endpoints
            .Where(endpoint => endpoint.ApiName == ApiName)
            .Select(endpoint => endpoint.Name)
            .ToList();
        
        AvailableEndpoints.Clear();
        foreach (var endpointName in filteredEndpoints)
        {
            AvailableEndpoints.Add(endpointName);
        }
    }
    else
    {
        AvailableEndpoints.Clear();
    }
}

private async Task AddEndpointAsync()
{
    string apiDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", ApiName);
    
    if (string.IsNullOrWhiteSpace(NewEndpointName))
    {
        ErrorMessage = "The name of the new endpoint is required.";
        return;
    }

    // Load existing endpoints from the JSON file.
    List<Endpoint> endpoints;
    if (File.Exists(EndpointsFilePath))
    {
        var json = await File.ReadAllTextAsync(EndpointsFilePath);
        endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(json) ?? new List<Endpoint>();
    }
    else
    {
        endpoints = new List<Endpoint>();
    }

    // Check if an endpoint with the same name already exists within the same API.
    if (endpoints.Any(ep => ep.Name.Equals(NewEndpointName, StringComparison.OrdinalIgnoreCase) && ep.ApiName == ApiName))
    {
        ErrorMessage = "An endpoint with this name already exists within the API. Please use a different name.";
        return;
    }

    ErrorMessage = "";

    // Create a new endpoint object, including the ApiName.
    var newEndpoint = new Endpoint
    {
        Name = NewEndpointName,
        Method = "GET", // Default method, this can be made dynamic as well.
        Path = "sample", // Default path, consider allowing user input for this too.
        ApiName = ApiName // Assign the API name to the endpoint.
    };

    // Add the new endpoint to the list and update the JSON file.
    endpoints.Add(newEndpoint);
    await File.WriteAllTextAsync(EndpointsFilePath, JsonConvert.SerializeObject(endpoints, Formatting.Indented));

    // Update the UI to reflect the new endpoint.
    AvailableEndpoints.Add(newEndpoint.Name);

    // Generate the controller code for the new endpoint.
    string controllerCode = $@"
using Microsoft.AspNetCore.Mvc;

namespace {ApiName}.Controllers
{{
    [ApiController]
    [Route(""[controller]"")]
    public class {newEndpoint.Name}Controller : ControllerBase
    {{
        [HttpGet]
        [Route(""{newEndpoint.Path}"")]
        public IActionResult Get()
        {{
            // Placeholder for the actual logic to be executed when the endpoint is called.
            return Ok(""{newEndpoint.Name} response"");
        }}
    }}
}}";

    // Ensure the Controllers directory exists and write the new controller file.
    string controllersDirectory = Path.Combine(apiDirectoryPath, "Controllers");
    Directory.CreateDirectory(controllersDirectory);
    string controllerFilePath = Path.Combine(controllersDirectory, $"{newEndpoint.Name}Controller.cs");
    await File.WriteAllTextAsync(controllerFilePath, controllerCode);

    NewEndpointName = string.Empty; // Clear the input field after adding the endpoint.
}


    private async Task EditSelectedEndpointAsync()
    {
        var endpoints = new List<Endpoint>();
        if (File.Exists(EndpointsFilePath))
        {
            var json = await File.ReadAllTextAsync(EndpointsFilePath);
            endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(json);
        }

        var endpointToEdit = endpoints.Find(e => e.Name == SelectedEndpoint);
        if (endpointToEdit != null)
        {
            // Te wartości powinny być aktualizowane na podstawie danych z interfejsu użytkownika
            endpointToEdit.Method = "POST";
            endpointToEdit.Path = "/updated/endpoint";

            await File.WriteAllTextAsync(EndpointsFilePath, JsonConvert.SerializeObject(endpoints, Formatting.Indented));
            await RefreshApiListAsync();
        }
    }


    private async Task DeleteSelectedEndpointAsync()
    {
        var endpoints = new List<Endpoint>();
        if (File.Exists(EndpointsFilePath))
        {
            var json = await File.ReadAllTextAsync(EndpointsFilePath);
            endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(json);
        }

        var endpointToDelete = endpoints.Find(e => e.Name == SelectedEndpoint);
        if (endpointToDelete != null)
        {
            // Attempt to delete the controller file
            try
            {
                string apiDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", ApiName);
                string controllersDirectory = Path.Combine(apiDirectoryPath, "Controllers");
                string controllerFileName = $"{endpointToDelete.Name}Controller.cs";
                string controllerFilePath = Path.Combine(controllersDirectory, controllerFileName);

                if (File.Exists(controllerFilePath))
                {
                    File.Delete(controllerFilePath); // Delete the controller file
                }
            }
            catch (Exception ex)
            {
                // Handle or log the error (e.g., insufficient permissions, file in use)
                // For simplicity, we're just printing the exception message to the console.
                // In a real application, consider logging this error or notifying the user.
                Console.WriteLine($"Error deleting controller file: {ex.Message}");
                return; // Optional: decide whether to stop the operation if the file cannot be deleted.
            }

            // Remove the endpoint from the list and update the JSON file
            endpoints.Remove(endpointToDelete);
            await File.WriteAllTextAsync(EndpointsFilePath, JsonConvert.SerializeObject(endpoints, Formatting.Indented));
            
            // Refresh the list of available endpoints in the UI
            await RefreshApiListAsync();
        }
    }
}