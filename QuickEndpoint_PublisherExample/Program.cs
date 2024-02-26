using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Paths and names
        var projectName = "QuickEndpoint_ApiExample";
        var projectBaseDir = "E:\\source_code\\QuickEndpoint";
        var projectDir = Path.Combine(projectBaseDir, projectName);
        var tempPublishDir = Path.Combine(projectBaseDir, "tempPublish"); // Temporary folder for publishing
        var archiveDir = Path.Combine(projectBaseDir, "Publish"); // Folder for the archive
        var archivePath = Path.Combine(archiveDir, $"{projectName}.tar.gz");

        // Create archive folder if it does not exist
        if (!Directory.Exists(archiveDir))
        {
            Directory.CreateDirectory(archiveDir);
        }

        // Removing previous results
        if (Directory.Exists(tempPublishDir))
        {
            Console.WriteLine($"Removing existing publication directory: {tempPublishDir}");
            Directory.Delete(tempPublishDir, true);
        }
        if (File.Exists(archivePath))
        {
            Console.WriteLine($"Removing existing archive: {archivePath}");
            File.Delete(archivePath);
        }

        // Publishing the application
        Console.WriteLine("Publishing the application...");
        var publishProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"publish {projectDir} -c Release -o {tempPublishDir}",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        publishProcess.WaitForExit();
        Console.WriteLine(publishProcess.StandardOutput.ReadToEnd());

        // Packing into a .tar.gz archive
        Console.WriteLine("Packing the application into a .tar.gz archive...");
        var tarProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "tar",
            Arguments = $"-czvf {archivePath} -C {tempPublishDir} .",
            RedirectStandardOutput = true,
            UseShellExecute = false
        });
        tarProcess.WaitForExit();
        Console.WriteLine(tarProcess.StandardOutput.ReadToEnd());

        // Removing the temporary publishing directory
        if (Directory.Exists(tempPublishDir))
        {
            Console.WriteLine($"Removing temporary publication directory: {tempPublishDir}");
            Directory.Delete(tempPublishDir, true);
        }

        Console.WriteLine($"The application has been published and packed into {archivePath}");
    }
}
