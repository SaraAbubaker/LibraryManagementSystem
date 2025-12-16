using Library.Infrastructure.Logging.Interfaces;
using Library.Infrastructure.Logging.Models;
using Library.Infrastructure.Mongo;
using Library.Shared.Helpers;

namespace Library.Infrastructure.Logging.Services
{
    public class MessageLoggerService : IMessageLoggerService
    {
        private readonly MongoRepository<MessageLog> _repo;

        public MessageLoggerService(MongoContext context)
        {
            _repo = new MongoRepository<MessageLog>(context, "MessageLogs");
        }

        public async Task LogMessageAsync(MessageLog log)
        {
            log.Guid = Guid.NewGuid();
            log.CreatedAt = DateTime.Now;

            Validate.ValidateModel(log);

            await _repo.InsertAsync(log);
        }

        public async Task<MessageLog?> GetMessageLogAsync(Guid guid)
        {
            return await _repo.FindOneAsync(x => x.Guid == guid);
        }

        public async Task<List<MessageLog>> GetAllMessageLogsAsync()
        {
            return await _repo.FindAsync(_ => true);
        }
    }
}
