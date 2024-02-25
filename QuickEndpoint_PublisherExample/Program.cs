using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Ścieżki i nazwy
        var projectName = "QuickEndpoint_ApiExample";
        var projectBaseDir = "/home/kofi/Main/VSCProjects/QuickEndpoint";
        var projectDir = Path.Combine(projectBaseDir, projectName);
        var tempPublishDir = Path.Combine(projectBaseDir, "tempPublish"); // Tymczasowy folder do publikacji
        var archiveDir = Path.Combine(projectBaseDir, "Publish"); // Folder na archiwum
        var archivePath = Path.Combine(archiveDir, $"{projectName}.tar.gz");

        // Tworzenie folderu na archiwum, jeśli nie istnieje
        if (!Directory.Exists(archiveDir))
        {
            Directory.CreateDirectory(archiveDir);
        }

        // Usuwanie poprzednich wyników
        if (Directory.Exists(tempPublishDir))
        {
            Console.WriteLine($"Usuwanie istniejącego katalogu publikacji: {tempPublishDir}");
            Directory.Delete(tempPublishDir, true);
        }
        if (File.Exists(archivePath))
        {
            Console.WriteLine($"Usuwanie istniejącego archiwum: {archivePath}");
            File.Delete(archivePath);
        }

        // Publikowanie aplikacji
        Console.WriteLine("Publikowanie aplikacji...");
        var publishProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"publish {projectDir} -c Release -o {tempPublishDir}",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        publishProcess.WaitForExit();
        Console.WriteLine(publishProcess.StandardOutput.ReadToEnd());

        // Pakowanie do archiwum .tar.gz
        Console.WriteLine("Pakowanie aplikacji do archiwum .tar.gz...");
        var tarProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "tar",
            Arguments = $"-czvf {archivePath} -C {tempPublishDir} .",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        tarProcess.WaitForExit();
        Console.WriteLine(tarProcess.StandardOutput.ReadToEnd());

        // Usuwanie tymczasowego katalogu publikacji
        if (Directory.Exists(tempPublishDir))
        {
            Console.WriteLine($"Usuwanie tymczasowego katalogu publikacji: {tempPublishDir}");
            Directory.Delete(tempPublishDir, true);
        }

        Console.WriteLine($"Aplikacja została opublikowana i spakowana do {archivePath}");
    }
}
