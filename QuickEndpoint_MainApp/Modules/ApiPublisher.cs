using System;
using System.Diagnostics;
using System.IO;

namespace QuickEndpoint_MainApp.Modules
{
    public static class ApplicationPublisher
    {
        public static void PublishApplication(string projectDir, string publishDir, string configuration)
        {
            Console.WriteLine("Publishing the application...");
            Utilities.ExecuteProcess("dotnet", $"publish \"{projectDir}\" -c {configuration} -o \"{publishDir}\"");
            Console.WriteLine($"The application has been published to {publishDir}");
        }
    }
}