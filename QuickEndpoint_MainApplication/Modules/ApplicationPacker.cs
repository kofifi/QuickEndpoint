using System;
using System.Diagnostics;
using System.IO;

namespace QuickEndpoint.Modules
{
    public class ApplicationPacker
    {
        private string installersDirPath = Path.Combine(@"E:\source_code\QuickEndpoint", "Installers");
        private string tempPublishDir = Path.Combine(@"E:\source_code\QuickEndpoint", "tempPublish");
        private string projectName = "QuickEndpoint_ApiExample";

        public void PackFinalApplication()
        {
            Console.WriteLine("Packing the application and installer script into a final archive...");
            Directory.CreateDirectory(installersDirPath);

            string finalArchivePath = Path.Combine(installersDirPath, $"{projectName}_Final.tar.gz");

            var tarProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "tar",
                Arguments = $"-czvf {finalArchivePath} -C {tempPublishDir} .",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            if (tarProcess == null)
            {
                Console.WriteLine("Failed to start the tar process. The application and installer script could not be packed.");
                return;
            }

            tarProcess.WaitForExit();

            // Using the null-forgiveness operator to address CS8600
            string output = tarProcess.StandardOutput!.ReadToEnd();
            Console.WriteLine(output);

            Console.WriteLine($"The application and installer script have been packed into {finalArchivePath}");

            if (Directory.Exists(tempPublishDir))
            {
                Directory.Delete(tempPublishDir, true);
            }
        }
    }
}
