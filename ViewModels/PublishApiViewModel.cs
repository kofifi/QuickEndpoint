using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia.Collections;
using ReactiveUI;
using System.Runtime.InteropServices;
using QuickEndpoint_MainApp.Modules;
using System.Threading.Tasks;

namespace QuickEndpoint.ViewModels
{
    public class PublishApiViewModel : ViewModelBase
    {
        private string _apiName;
        private AvaloniaList<string> _availableApis = new AvaloniaList<string>();
        public ReactiveCommand<Unit, Unit> PublishApiCommand { get; }
        public ReactiveCommand<Unit, Unit> RefreshApiListCommand { get; }

        private bool _isPublishingApi;
        public bool IsPublishingApi
        {
            get => _isPublishingApi;
            set => this.RaiseAndSetIfChanged(ref _isPublishingApi, value);
        }

        private double _publishApiProgress;
        public double publishApiProgress
        {
            get => _publishApiProgress;
            set => this.RaiseAndSetIfChanged(ref _publishApiProgress, value);
        }

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
        public string ApiName1 { get => _apiName; set => _apiName = value; }
        public AvaloniaList<string> AvailableApis1 { get => _availableApis; set => _availableApis = value; }
        public bool IsPublishingApi1 { get => _isPublishingApi; set => _isPublishingApi = value; }
        public double PublishApiProgress { get => _publishApiProgress; set => _publishApiProgress = value; }

        public PublishApiViewModel()
        {
            PublishApiCommand = ReactiveCommand.CreateFromTask(PublishApiAsync);
            RefreshApiListCommand = ReactiveCommand.Create(RefreshAvailableApis);

            // Initial refresh to populate available APIs.
            RefreshAvailableApis();
        }

        private async Task PublishApiAsync()
        {
            if (string.IsNullOrEmpty(ApiName))
            {
                LogDebugInfo("No API selected for publishing.");
                return;
            }

            try
            {
                IsPublishingApi = true;
                LogDebugInfo("PublishApi method execution started.");

                // Step 1: Setup directories
                await Task.Delay(250);
                publishApiProgress = 0; // Indicate initial setup is starting

                string baseDirectory = Environment.CurrentDirectory;
                string sourceProjectDir = Path.Combine(baseDirectory, "Data", "CreatedApis", ApiName);
                string publishedApisDir = Path.Combine(baseDirectory, "Data", "PublishedApis", ApiName);
                string toolsDir = Path.Combine(baseDirectory, "Data", "Tools", "nssm", "win64");
                Directory.CreateDirectory(publishedApisDir);
                LogDebugInfo("Directories setup completed.");

                // Step 2: Publish the API
                await Task.Delay(500);
                publishApiProgress = 0.4; // Indicate publishing starts

                string configuration = "Release";
                ApplicationPublisher.PublishApplication(sourceProjectDir, publishedApisDir, configuration, LogDebugInfo);
                LogDebugInfo("API published.");

                // Step 3: Copy necessary tools
                await Task.Delay(500);
                publishApiProgress = 0.6; // Publishing completed
                string nssmSourcePath = Path.Combine(toolsDir, "nssm.exe");
                string nssmDestinationPath = Path.Combine(publishedApisDir, "nssm.exe");
                if (File.Exists(nssmSourcePath))
                {
                    File.Copy(nssmSourcePath, nssmDestinationPath, overwrite: true);
                    LogDebugInfo("nssm.exe has been copied to the published API directory.");
                }
                else
                {
                    LogDebugInfo("nssm.exe was not found in the tools directory.");
                }

                // Step 4: Generate install and uninstall scripts
                await Task.Delay(500);
                publishApiProgress = 0.8; // Tool copy completed

                if (OSHelper.IsWindows())
                {
                    InstallerScriptGenerator.GenerateBatchInstallScript(ApiName, publishedApisDir, LogDebugInfo);
                    InstallerScriptGenerator.GenerateBatchUninstallScript(ApiName, publishedApisDir, LogDebugInfo);
                }
                else if (OSHelper.IsLinux())
                {
                    InstallerScriptGenerator.GenerateBashInstallScript(ApiName, publishedApisDir, LogDebugInfo);
                    InstallerScriptGenerator.GenerateBashUninstallScript(ApiName, publishedApisDir, LogDebugInfo);
                }
                LogDebugInfo($"Scripts for '{ApiName}' generated.");

                // Setp 5: Finalize the process
                publishApiProgress = 1.0; // Update progress to 100% after completion
                await Task.Delay(500);

                LogDebugInfo("API publishing process completed.");
            }
            catch (Exception ex)
            {
                LogDebugInfo($"An error occurred while publishing API '{ApiName}': {ex.Message}");
            }
            finally
            {
                IsPublishingApi = false;
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

        private void LogDebugInfo(string message)
        {
            // Append the timestamp to every log entry for better tracking
            string logEntry = $"{DateTime.Now}: {message}\n";
            File.AppendAllText("debug.log", logEntry);
        }
    }
}

namespace QuickEndpoint_MainApp.Modules
{

    public static class OSHelper
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }
    public static class ApplicationPublisher
    {
        public static void PublishApplication(string projectDir, string publishDir, string configuration, Action<string> logger)
        {
            Utilities.ExecuteProcess("dotnet", $"publish \"{projectDir}\" -c {configuration} -o \"{publishDir}\"", logger);
            logger($"The application has been published to {publishDir}");
        }
    }

    public static class InstallerScriptGenerator
    {
        public static void GenerateBatchInstallScript(string appName, string directory, Action<string> logger)
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
            logger($"Install script has been saved at: {installScriptPath}");
        }

        public static void GenerateBatchUninstallScript(string appName, string directory, Action<string> logger)
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
            logger($"Uninstall script has been saved at: {uninstallScriptPath}");
        }
        
        public static void GenerateBashInstallScript(string appName, string directory, Action<string> logger)
        {
            string installScriptPath = Path.Combine(directory, "install.sh");
            string serviceFilePath = Path.Combine(directory, $"{appName}.service");
            string executablePath = "/usr/bin/dotnet";  // Path to the dotnet executable
            string dllPath = Path.Combine(directory, $"{appName}.dll");  // Path to the main application DLL

            string serviceContent = @$"[Unit]
        Description={appName} .NET Application Service

        [Service]
        WorkingDirectory={directory}
        ExecStart={executablePath} {dllPath}
        Restart=always
        # Other configurations like User, Environment variables can be added here

        [Install]
        WantedBy=multi-user.target";

            File.WriteAllText(serviceFilePath, serviceContent);

            string bashContent = @$"#!/bin/bash
        echo 'Installing {appName}...'
        sudo cp {appName}.service /etc/systemd/system/
        if sudo systemctl enable {appName} && sudo systemctl start {appName}; then
            echo 'Installation completed successfully.'
        else
            echo 'Failed to install and start {appName}.'
        fi
        read -p 'Press any key to continue...'";

            File.WriteAllText(installScriptPath, bashContent);
            logger($"Install script for Linux has been saved at: {installScriptPath}");
        }


public static void GenerateBashUninstallScript(string appName, string directory, Action<string> logger)
{
    string uninstallScriptPath = Path.Combine(directory, "uninstall.sh");
    string bashContent = @$"#!/bin/bash
echo 'Uninstalling {appName}...'
if sudo systemctl stop {appName}; then
    echo '{appName} stopped successfully.'
else
    echo 'Failed to stop {appName}.'
fi

if sudo systemctl disable {appName}; then
    echo '{appName} disabled successfully.'
else
    echo 'Failed to disable {appName}.'
fi

SERVICE_FILE_PATH=""/etc/systemd/system/{appName}.service""
if [ -f ""$SERVICE_FILE_PATH"" ]; then
    sudo rm ""$SERVICE_FILE_PATH""
    echo 'Service file removed successfully.'
else
    echo 'Service file not found.'
fi

sudo systemctl daemon-reload
echo 'Systemd configuration reloaded.'
echo 'Uninstallation completed.'
read -p 'Press any key to continue...'";

    File.WriteAllText(uninstallScriptPath, bashContent);
    logger($"Uninstall script for Linux has been saved at: {uninstallScriptPath}");
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

        public static void ExecuteProcess(string fileName, string arguments, Action<string> logger)
        {
            using var process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            });

            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                logger(output);
            }

            if (!string.IsNullOrEmpty(error))
            {
                logger("Error: " + error);
            }

            if (process.ExitCode != 0)
            {
                throw new InvalidOperationException($"Process exited with code: {process.ExitCode}. Error: {output}");
            }
        }
    }
}
