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

public class EndpointDisplay
{
    public string Name { get; set; }
    public string Method { get; set; }
    public string DisplayName => $"{Method} {Name}";
}

public class EditApiDetailsViewModel : ViewModelBase
{
    private AvaloniaList<EndpointDisplay> _availableEndpoints;
    private EndpointDisplay _selectedEndpoint;
    private string _apiName;
    private const string EndpointsFilePath = "endpoints.json";


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
    

    public EditApiDetailsViewModel()
    {
        RefreshApiListCommand = ReactiveCommand.CreateFromTask(RefreshApiListAsync);
        AddEndpointCommand = ReactiveCommand.CreateFromTask(AddEndpointAsync);
        EditSelectedEndpointCommand = ReactiveCommand.CreateFromTask(EditSelectedEndpointAsync, this.WhenAnyValue((EditApiDetailsViewModel x) => x.SelectedEndpoint, (EndpointDisplay selectedEndpoint) => selectedEndpoint != null));
        DeleteSelectedEndpointCommand = ReactiveCommand.CreateFromTask(DeleteSelectedEndpointAsync, this.WhenAnyValue((EditApiDetailsViewModel x) => x.SelectedEndpoint, (EndpointDisplay selectedEndpoint) => selectedEndpoint != null));

        AvailableEndpoints = new AvaloniaList<EndpointDisplay>();

        // Automatically refresh the list of endpoints when the ViewModel is created
        RefreshApiListAsync().ConfigureAwait(false);
    }


private async Task RefreshApiListAsync()
{
    if (File.Exists(EndpointsFilePath))
    {
        var json = await File.ReadAllTextAsync(EndpointsFilePath);
        var endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(json) ?? new List<Endpoint>();

        var filteredEndpoints = endpoints
            .Where(endpoint => endpoint.ApiName == ApiName)
            .Select(endpoint => new EndpointDisplay { Name = endpoint.Name, Method = endpoint.Method })
            .ToList();

        AvailableEndpoints.Clear();
        foreach (var endpoint in filteredEndpoints)
        {
            AvailableEndpoints.Add(endpoint);
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

    // Create a new endpoint object.
    var newEndpoint = new Endpoint
    {
        Name = NewEndpointName,
        Method = NewEndpointMethod, // Use the selected HTTP method.
        Path = "sample", // Consider allowing user input for this.
        ApiName = ApiName
    };

    // Add the new endpoint to the list and update the JSON file.
    endpoints.Add(newEndpoint);
    await File.WriteAllTextAsync(EndpointsFilePath, JsonConvert.SerializeObject(endpoints, Formatting.Indented));

    // Update the UI to reflect the new endpoint.
    AvailableEndpoints.Add(new EndpointDisplay { Name = newEndpoint.Name, Method = newEndpoint.Method });

    // Determine the correct HTTP method attribute and action method
    string httpMethodAttribute = newEndpoint.Method switch
    {
        "POST" => "[HttpPost]",
        "PUT" => "[HttpPut]",
        "DELETE" => "[HttpDelete]",
        "PATCH" => "[HttpPatch]",
        _ => "[HttpGet]"
    };

    string actionMethod = newEndpoint.Method switch
    {
        "POST" => "Post",
        "PUT" => "Put",
        "DELETE" => "Delete",
        "PATCH" => "Patch",
        _ => "Get"
    };

    // Generate the controller code dynamically
    string controllerCode = $@"
using Microsoft.AspNetCore.Mvc;

namespace {ApiName}.Controllers
{{
    [ApiController]
    [Route(""[controller]"")]
    public class {newEndpoint.Name}Controller : ControllerBase
    {{
        {httpMethodAttribute}
        [Route(""{newEndpoint.Path}"")]
        public IActionResult {actionMethod}()
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

        var endpointToEdit = endpoints.FirstOrDefault(e => e.Name == SelectedEndpoint.Name && e.ApiName == ApiName);
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
       if (File.Exists(EndpointsFilePath) && SelectedEndpoint != null)
    {
        var json = await File.ReadAllTextAsync(EndpointsFilePath);
        var endpoints = JsonConvert.DeserializeObject<List<Endpoint>>(json);

        var endpointToDelete = endpoints.FirstOrDefault(e => e.Name == SelectedEndpoint.Name && e.Method == SelectedEndpoint.Method);
        if (endpointToDelete != null)
        {
            {
                try
                {
                    string apiDirectoryPath = Path.Combine(Environment.CurrentDirectory, "Data", "CreatedApis", ApiName);
                    string controllersDirectory = Path.Combine(apiDirectoryPath, "Controllers");

                    // Construct the controller file name based on endpoint name and possibly method
                    string controllerFileName = $"{endpointToDelete.Name}Controller.cs";
                    string controllerFilePath = Path.Combine(controllersDirectory, controllerFileName);

                    if (File.Exists(controllerFilePath))
                    {
                        File.Delete(controllerFilePath); // Delete the controller file
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting controller file: {ex.Message}");
                    ErrorMessage = "Error deleting controller file. Please check logs for more details.";
                    return;
                }

                // Remove the endpoint from the list and update the JSON file
                endpoints.Remove(endpointToDelete);
                var updatedJson = JsonConvert.SerializeObject(endpoints, Formatting.Indented);
                await File.WriteAllTextAsync(EndpointsFilePath, updatedJson);
                
                // Refresh the list of available endpoints in the UI
                await RefreshApiListAsync();
            }
        }
    }
}
}