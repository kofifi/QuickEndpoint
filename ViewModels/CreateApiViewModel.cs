using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive;

namespace QuickEndpoint.ViewModels;
public class CreateApiViewModel : ViewModelBase
{

    private bool _isCreatingApi;
    public bool IsCreatingApi
    {
        get => _isCreatingApi;
        set => this.RaiseAndSetIfChanged(ref _isCreatingApi, value);
    }

    private string _errorMessage;
    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    private double _createApiProgress;
    public double CreateApiProgress
    {
        get => _createApiProgress;
        set => this.RaiseAndSetIfChanged(ref _createApiProgress, value);
    }

    private string _apiName;
    public string ApiName
    {
        get => _apiName;
        set => this.RaiseAndSetIfChanged(ref _apiName, value);
    }

    private string _nameOfApi;
    public string NameOfApi
    {
        get => _nameOfApi;
        set => this.RaiseAndSetIfChanged(ref _nameOfApi, value);
    }

    private string _descriptionOfApi;
    public string DescriptionOfApi
    {
        get => _descriptionOfApi;
        set => this.RaiseAndSetIfChanged(ref _descriptionOfApi, value);
    }

    // Command property
    public ReactiveCommand<Unit, Unit> CreateApiCommand { get; }

    public CreateApiViewModel()
    {
        // Initialize the command with a method to call
        CreateApiCommand = ReactiveCommand.CreateFromTask(CreateApiAsync);

        // Other initializations...
    }

    private async Task CreateApiAsync()
    {
        IsCreatingApi = true;
        CreateApiProgress = 0.0; // Start progress at 0%
        LogDebugInfo("CreateApi method execution started.");

        try
        {
            // Simulate initial setup work
            await Task.Delay(500); // Simulate some initial work
            CreateApiProgress = 0.1; // Update progress to 10%
            LogDebugInfo("Initial setup completed.");

            // Directory and project setup
            string baseDirectory = Environment.CurrentDirectory;
            string apisDirectory = Path.Combine(baseDirectory, "Data", "CreatedApis");
            if (!Directory.Exists(apisDirectory))
            {
                Directory.CreateDirectory(apisDirectory);
            }
            string projectDirectory = Path.Combine(apisDirectory, NameOfApi);
            if (Directory.Exists(projectDirectory))
            {
                ErrorMessage = "API with this name already exists.";
                return; // Wyjdź z metody, nie kontynuując tworzenia API
            }
            else
            {
                ErrorMessage = ""; // Wyczyść komunikat o błędzie, jeśli katalog nie istnieje
            }
            Directory.CreateDirectory(projectDirectory);
            LogDebugInfo("Project directory setup completed.");
            await Task.Delay(500); // Simulate time taken for directory setup
            CreateApiProgress = 0.3; // Update progress to 30%

            // Process start and execution
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "new webapp",
                WorkingDirectory = projectDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo })
            {
                process.Start();
                LogDebugInfo("dotnet process started.");
                while (!process.HasExited)
                {
                    await Task.Delay(1000); // Wait a bit before updating the progress
                    CreateApiProgress = Math.Min(CreateApiProgress + 0.2, 0.9); // Ensure progress does not exceed 90%
                }
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                if (!string.IsNullOrEmpty(output))
                {
                    File.WriteAllText(Path.Combine(projectDirectory, "api_creation_output.txt"), output);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    LogDebugInfo($"dotnet process error: {error}");
                }
            }

            await Task.Delay(500); // Finalize everything
            CreateApiProgress = 1.0; // Ensure progress is set to 100% after completion
            await Task.Delay(500); // Finalize everything
            LogDebugInfo("API creation process completed.");
        }
        catch (Exception ex)
        {
            LogDebugInfo($"An error occurred: {ex.Message}");
        }
        finally
        {
            IsCreatingApi = false;
        }
    }

    private void LogDebugInfo(string message)
    {
        // Append the timestamp to every log entry for better tracking
        string logEntry = $"{DateTime.Now}: {message}\n";
        File.AppendAllText("debug.log", logEntry);
    }
}