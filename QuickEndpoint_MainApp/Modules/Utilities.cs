using System;
using System.Diagnostics;
using System.IO;

namespace QuickEndpoint_MainApp.Modules
{
    public static class Utilities
    {
        public static string GetProjectBaseDirectory()
        {
            var currentDir = Directory.GetCurrentDirectory();
            return Directory.GetParent(currentDir)?.FullName ?? throw new InvalidOperationException("Cannot find the parent directory.");
        }

        public static string CombinePaths(params string[] paths)
        {
            return Path.Combine(paths);
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