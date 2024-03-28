using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace QuickEndpoint.Services;
    public class FileDataService : IFileDataService // Implement the interface
    {
        private readonly string _originsFilePath;
        private readonly string _endpointsFilePath;

        public FileDataService(string originsFileName = "origins.json", string endpointsFileName = "endpoints.json")
        {
            string baseDirectory = Path.Combine(Environment.CurrentDirectory, "Data", "JsonStorage");
            _originsFilePath = Path.Combine(baseDirectory, originsFileName);
            _endpointsFilePath = Path.Combine(baseDirectory, endpointsFileName);

            Directory.CreateDirectory(baseDirectory); // Ensure the directories exist
        }

        public async Task<List<T>> LoadDataAsync<T>(string fileType)
        {
            string filePath = fileType switch
            {
                "origins" => _originsFilePath,
                "endpoints" => _endpointsFilePath,
                _ => throw new ArgumentException("Invalid file type specified.")
            };

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);
                return JsonConvert.DeserializeObject<List<T>>(json) ?? new List<T>();
            }
            return new List<T>();
        }

        public async Task SaveDataAsync<T>(string fileType, List<T> data)
        {
            string filePath = fileType switch
            {
                "origins" => _originsFilePath,
                "endpoints" => _endpointsFilePath,
                _ => throw new ArgumentException("Invalid file type specified.")
            };

            var json = JsonConvert.SerializeObject(data, Formatting.Indented);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task<bool> DeleteDataAsync<T>(string fileType, Func<T, bool> predicate)
        {
            var data = await LoadDataAsync<T>(fileType);
            var itemToRemove = data.FirstOrDefault(predicate);

            if (itemToRemove != null)
            {
                data.Remove(itemToRemove);
                await SaveDataAsync(fileType, data);
                return true;
            }

            return false;
        }
    }
