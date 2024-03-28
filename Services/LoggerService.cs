using System;
using System.IO;
using System.Threading.Tasks;

namespace QuickEndpoint.Services;
    public class LoggerService : ILoggerService
    {
        private readonly string _logFilePath;

        public LoggerService()
        {
            string logDirectory = Path.Combine(Environment.CurrentDirectory, "Data", "Logs");
            Directory.CreateDirectory(logDirectory); // Ensure the directory exists.
            _logFilePath = Path.Combine(logDirectory, "debug.log");
        }

        public async Task LogAsync(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(_logFilePath, true))
                {
                    await writer.WriteLineAsync($"{DateTime.Now}: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }

        public void Log(string message)
        {
            try
            {
                File.AppendAllText(_logFilePath, $"{DateTime.Now}: {message}\n");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }