
using Library.Infrastructure.Logging.Models;

namespace Library.Infrastructure.Logging.Interfaces
{
    public interface IMessageLoggerService
    {
        Task LogMessageAsync(MessageLog log);
        Task<MessageLog?> GetMessageLogAsync(Guid guid);
        Task<List<MessageLog>> GetAllMessageLogsAsync();
    }
}
