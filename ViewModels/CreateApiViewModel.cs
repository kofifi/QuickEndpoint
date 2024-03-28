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
    CreateApiProgress = 0.0;
    LogDebugInfo("Starting API creation process.");

    try
    {
        await InitializeSetupAsync();

        // Setup directories
        var projectDirectory = await SetupProjectDirectoryAsync();
        if (string.IsNullOrEmpty(projectDirectory)) return;

        await CreateProjectAsync(projectDirectory);
        await OverwriteProgramFileAsync(projectDirectory);
        await CreateProjectStructureAsync(projectDirectory);

        LogDebugInfo("API creation process completed successfully.");
    }
    catch (Exception ex)
    {
        LogDebugInfo($"An error occurred during API creation: {ex.Message}");
        ErrorMessage = "An error occurred. Please check the log for details.";
    }
    finally
    {
        IsCreatingApi = false;
        CreateApiProgress = 1.0; // Ensure completion regardless of success.
    }
}

private async Task OverwriteProgramFileAsync(string projectDirectory)
{
    string programCsPath = Path.Combine(projectDirectory, "Program.cs");
    string programCsContent = @"
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();
";

    await File.WriteAllTextAsync(programCsPath, programCsContent);
    LogDebugInfo($"Program.cs file overwritten with custom content.");
    CreateApiProgress = 0.6; // Update progress.
}

private async Task<string> SetupProjectDirectoryAsync()
{
    string baseDirectory = Environment.CurrentDirectory;
    string apisDirectory = Path.Combine(baseDirectory, "Data", "CreatedApis");

    Directory.CreateDirectory(apisDirectory); // Ensure the base directory exists.

    string projectDirectory = Path.Combine(apisDirectory, NameOfApi);
    if (Directory.Exists(projectDirectory))
    {
        ErrorMessage = "API with this name already exists.";
        return null;
    }

    Directory.CreateDirectory(projectDirectory);
    LogDebugInfo("Project directory setup completed.");
    await Task.Delay(500); // Simulate directory setup time.
    CreateApiProgress = 0.2;

    return projectDirectory;
}

private async Task CreateProjectAsync(string projectDirectory)
{
    var startInfo = new ProcessStartInfo
    {
        FileName = "dotnet",
        Arguments = "new webapi --no-https",
        WorkingDirectory = projectDirectory,
        RedirectStandardOutput = true,
        RedirectStandardError = true,
        UseShellExecute = false,
        CreateNoWindow = true
    };

    using (var process = new Process { StartInfo = startInfo })
    {
        process.Start();
        await process.WaitForExitAsync();
        CreateApiProgress = 0.5;
        LogDebugInfo("dotnet new webapi process completed.");
    }
}

private async Task CreateProjectStructureAsync(string projectDirectory)
{
    string[] directories = { "Controllers", "Models", "Services", "Repositories" };
    foreach (var dir in directories)
    {
        Directory.CreateDirectory(Path.Combine(projectDirectory, dir));
    }
    LogDebugInfo("Project structure created.");
    await Task.Delay(500); // Simulate structure setup time.
    CreateApiProgress = 0.7;
}

private async Task InitializeSetupAsync()
{
    await Task.Delay(500); // Simulate initial setup work.
    LogDebugInfo("Initial setup completed.");
    CreateApiProgress = 0.1;
}


    private void LogDebugInfo(string message)
    {
        // Append the timestamp to every log entry for better tracking
        string logEntry = $"{DateTime.Now}: {message}\n";
        File.AppendAllText("debug.log", logEntry);
    }
}