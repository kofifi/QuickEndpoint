using System;
using System.Diagnostics;
using System.IO;

namespace QuickEndpoint.Modules
{
    public class ApplicationPublisher
    {
        private string projectDir = @"E:\source_code\QuickEndpoint\QuickEndpoint_ApiExample";
        private string tempPublishDir = Path.Combine(@"E:\source_code\QuickEndpoint", "tempPublish");

        public void PublishApplication()
        {
            Console.WriteLine("Publishing the application...");
            Directory.CreateDirectory(tempPublishDir);

            var publishProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"publish {projectDir} -c Release -o {tempPublishDir}",
                RedirectStandardOutput = true,
                UseShellExecute = false
            });

            if (publishProcess == null)
            {
                Console.WriteLine("Failed to start the dotnet publish process.");
                return;
            }

            publishProcess.WaitForExit();

            // Check if StandardOutput is not null before accessing ReadToEnd
            // This is more to illustrate the pattern, as StandardOutput should not be null if process is not null
            // and RedirectStandardOutput = true
            if (publishProcess.StandardOutput != null)
            {
                Console.WriteLine(publishProcess.StandardOutput.ReadToEnd());
            }
            else
            {
                Console.WriteLine("No output received from the publishing process.");
            }
        }
    }
}
