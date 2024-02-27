using System;
using System.Diagnostics;
using System.IO;

namespace CreateWebAppScript
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Ask the user for the web project name
                Console.Write("Enter the name of the web project: ");
                string projectName = Console.ReadLine();

                string parentDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).FullName;
                string projectDirectory = Path.Combine(parentDirectory, projectName);
                if (!Directory.Exists(projectDirectory))
                {
                    // Create the project directory
                    Directory.CreateDirectory(projectDirectory);

                    // Execute dotnet new webapp command
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = "dotnet",
                        Arguments = $"new webapp -n {projectName}",
                        WorkingDirectory = parentDirectory, // Set the parent directory as the working directory
                        RedirectStandardOutput = true,
                        UseShellExecute = false
                    };

                    using (Process process = Process.Start(startInfo))
                    {
                        process.WaitForExit();

                        // Display the output of the process
                        string output = process.StandardOutput.ReadToEnd();
                        Console.WriteLine(output);
                    }
                }
                else
                {
                    Console.WriteLine("A directory with this name already exists. Please choose a different name.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Program ended. Press any key to exit.");
            Console.ReadKey();
        }
    }
}
