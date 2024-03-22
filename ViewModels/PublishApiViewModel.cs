using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Collections;
using ReactiveUI;
using QuickEndpoint_MainApp.Modules;

namespace QuickEndpoint.ViewModels
{
    public class PublishApiViewModel : ViewModelBase
    {
        private string _apiName;
        private AvaloniaList<string> _availableApis = new AvaloniaList<string>();
        public ReactiveCommand<Unit, Unit> PublishApiCommand { get; }
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

        public PublishApiViewModel()
        {
            PublishApiCommand = ReactiveCommand.Create(PublishApi);
            RefreshApiListCommand = ReactiveCommand.Create(RefreshAvailableApis);

            // Initial refresh to populate available APIs.
            RefreshAvailableApis();
        }

        private void PublishApi()
{
    if (string.IsNullOrEmpty(ApiName))
    {
        Console.WriteLine("No API selected for publishing.");
        return;
    }

    try
    {
        string baseDirectory = Environment.CurrentDirectory;
        string sourceProjectDir = Path.Combine(baseDirectory, "Data", "CreatedApis", ApiName);
        string publishedApisDir = Path.Combine(baseDirectory, "Data", "PublishedApis", ApiName);
        string toolsDir = Path.Combine(baseDirectory, "Data", "Tools", "nssm", "win64"); // Path to nssm.exe

        // Ensure the directories exist
        Directory.CreateDirectory(publishedApisDir);

        // Specify the configuration, e.g., "Release" or "Debug"
        string configuration = "Release";

        // Publish the API
        ApplicationPublisher.PublishApplication(sourceProjectDir, publishedApisDir, configuration);

        // Copy nssm.exe to the PublishedApis directory
        string nssmSourcePath = Path.Combine(toolsDir, "nssm.exe");
        string nssmDestinationPath = Path.Combine(publishedApisDir, "nssm.exe");
        if (File.Exists(nssmSourcePath))
        {
            File.Copy(nssmSourcePath, nssmDestinationPath, overwrite: true);
            Console.WriteLine("nssm.exe has been copied to the published API directory.");
        }
        else
        {
            Console.WriteLine("nssm.exe was not found in the tools directory.");
        }

        // Assuming the publication was successful, generate install and uninstall scripts
        InstallerScriptGenerator.GenerateBatchInstallScript(ApiName, publishedApisDir);
        InstallerScriptGenerator.GenerateBatchUninstallScript(ApiName, publishedApisDir);

        Console.WriteLine($"API '{ApiName}' has been published and scripts have been generated in {publishedApisDir}.");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while publishing API '{ApiName}': {ex.Message}");
    }
}


        public void RefreshAvailableApis()
        {
            try
            {
                string apisDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Data", "CreatedApis");
                if (Directory.Exists(apisDirectory))
                {
                    var apiDirectories = Directory.GetDirectories(apisDirectory).Select(Path.GetFileName).ToList();
                    AvailableApis.Clear();
                    AvailableApis.AddRange(apiDirectories);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while refreshing available APIs: {ex.Message}");
            }
        }
    }
}

namespace QuickEndpoint_MainApp.Modules
{
    public static class ApplicationPublisher
    {
        public static void PublishApplication(string projectDir, string publishDir, string configuration)
        {
            Utilities.ExecuteProcess("dotnet", $"publish \"{projectDir}\" -c {configuration} -o \"{publishDir}\"");
            Console.WriteLine($"The application has been published to {publishDir}");
        }
    }

    public static class InstallerScriptGenerator
    {
        public static void GenerateBatchInstallScript(string appName, string directory)
        {
            string installScriptPath = Path.Combine(directory, "install.bat");
            string batchContent = @$"@echo off
    echo Installing {appName}...
    REM Example command to install service using NSSM and current directory for paths
    cd /d %~dp0
    nssm.exe install {appName} ""%~dp0{appName}.exe""
    echo Installation completed.
    pause";

            File.WriteAllText(installScriptPath, batchContent);
            Console.WriteLine($"Install script has been saved at: {installScriptPath}");
        }

        public static void GenerateBatchUninstallScript(string appName, string directory)
        {
            string uninstallScriptPath = Path.Combine(directory, "uninstall.bat");
            string batchContent = @$"@echo off
    echo Uninstalling {appName}...
    REM Example command to uninstall service using NSSM
    cd /d %~dp0
    nssm.exe remove {appName} confirm
    echo Uninstallation completed.
    pause";

            File.WriteAllText(uninstallScriptPath, batchContent);
            Console.WriteLine($"Uninstall script has been saved at: {uninstallScriptPath}");
        }
    }


    public static class Utilities
    {
        public static string GetProjectBaseDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();
            return Directory.GetParent(currentDir)?.FullName ?? throw new InvalidOperationException("Cannot find the parent directory.");
        }

        public static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void ExecuteProcess(string fileName, string arguments)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                UseShellExecute = false
            });
            process.WaitForExit();

            var output = process.StandardOutput.ReadToEnd();
            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine(output);
            }

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Process exited with code: {process.ExitCode}. Error: {output}");
            }
        }
    }
}
