using System;
using QuickEndpoint_MainApp.Modules;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Starting application setup...");

        // Setup project details
        const string projectName = "QuickEndpoint_ApiExample";
        const string configuration = "Release";
        const string entryPoint = "QuickEndpoint_ApiExample.exe";

        var projectBaseDir = Utilities.GetProjectBaseDirectory();
        var projectDir = Utilities.CombinePaths(projectBaseDir, projectName);
        var installersDir = Utilities.CombinePaths(projectBaseDir, "Installers", projectName);
        var nssmPath = Utilities.CombinePaths(projectBaseDir, "Tools", "nssm-2.24", "win64", "nssm.exe");

        // Ensure necessary directories exist
        Utilities.EnsureDirectoryExists(installersDir);

        // Publish application
        ApplicationPublisher.PublishApplication(projectDir, installersDir, configuration);

        // Generate installer script
        InstallerScriptGenerator.GenerateScript(projectName, entryPoint, installersDir, nssmPath);

        // Generate uninstall script
        InstallerScriptGenerator.GenerateUninstallScript(projectName, installersDir, projectName);

        Console.WriteLine("Application setup completed successfully.");
    }
}
