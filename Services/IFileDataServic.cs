using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuickEndpoint.Services;
    public interface IFileDataService
    {
        Task<List<T>> LoadDataAsync<T>(string fileType);
        Task SaveDataAsync<T>(string fileType, List<T> data);
        Task<bool> DeleteDataAsync<T>(string fileType, Func<T, bool> predicate);
    }
