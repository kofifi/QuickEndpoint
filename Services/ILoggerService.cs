using System.Threading.Tasks;

namespace QuickEndpoint.Services
{
    public interface ILoggerService
    {
        Task LogAsync(string message);
        void Log(string message);
    }
}