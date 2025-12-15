
using Library.Infrastructure.Logging.Models;

namespace Library.Infrastructure.Logging.Interfaces
{
    public interface IMessageLoggerService
    {
        Task LogRequestAsync(MessageLog log);
        Task LogResponseAsync(Guid guid, string response);
    }
}
