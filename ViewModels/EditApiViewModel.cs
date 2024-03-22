using ReactiveUI;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Collections;

namespace QuickEndpoint.ViewModels
{
public class EditApiViewModel : ViewModelBase
{
    private string _apiName;
    private AvaloniaList<string> _availableApis = new AvaloniaList<string>();
    public ReactiveCommand<Unit, Unit> EditApiCommand { get; }
    public ReactiveCommand<Unit, Unit> RefreshApiListCommand { get; }

    public string ApiName
    {
        get => _apiName;
        set => this.RaiseAndSetIfChanged(ref _apiName, value);
    }

    public AvaloniaList<string> AvailableApis
    {
        get => _availableApis;
        set => this.RaiseAndSetIfChanged(ref _availableApis, value);
    }

        public EditApiViewModel()
        {
            EditApiCommand = ReactiveCommand.Create(EditApi);
            RefreshApiListCommand = ReactiveCommand.Create(RefreshAvailableApis);

            // Initial refresh to populate available APIs.
            RefreshAvailableApis(); 
        }

        private void EditApi()
        {
            // Logic for editing the selected API goes here.
        }

        public void RefreshAvailableApis()
        {
            try
            {
                LogDebugInfo("Starting to refresh available APIs...");

                string apisDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "CreatedApis");
                LogDebugInfo($"Looking for APIs in directory: {apisDirectory}");

                if (Directory.Exists(apisDirectory))
                {
                    var apiDirectories = Directory.GetDirectories(apisDirectory).Select(Path.GetFileName).ToList();
                    
                    LogDebugInfo($"Found {apiDirectories.Count} API directories.");
                    
                    AvailableApis.Clear();
                    foreach (var dir in apiDirectories)
                    {
                        AvailableApis.Add(dir);
                        LogDebugInfo($"Added API directory to list: {dir}");
                    }
                }
                else
                {
                    LogDebugInfo("APIs directory does not exist.");
                }

                LogDebugInfo("Finished refreshing available APIs.");
            }
            catch (Exception ex)
            {
                LogDebugInfo($"An error occurred while refreshing available APIs: {ex.Message}");
            }
        }

        private void LogDebugInfo(string message)
        {
            // Ensure the debug log file path is correct and accessible.
            string logFilePath = Path.Combine(Directory.GetCurrentDirectory(), "debug.log");

            // Append the timestamp to every log entry for better tracking.
            string logEntry = $"{DateTime.Now}: {message}\n";
            File.AppendAllText(logFilePath, logEntry);
        }
    }
}
