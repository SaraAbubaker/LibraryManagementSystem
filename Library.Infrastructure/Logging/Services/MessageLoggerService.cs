
using Library.Infrastructure.Logging.Interfaces;
using Library.Infrastructure.Logging.Models;
using Library.Infrastructure.Mongo;

namespace Library.Infrastructure.Logging.Services
{
    public class MessageLoggerService : IMessageLoggerService
    {
        //private readonly MongoRepository<MessageLog> _repo;

        public MessageLoggerService(MongoContext context)
        {
            //_repo = new MongoRepository<MessageLog>(context, "MessageLogs");
        }

        public async Task LogRequestAsync(MessageLog log)
        {
            //await _repo.InsertAsync(log);
        }

        public async Task LogResponseAsync(Guid guid, string response)
        {
            // update logic
        }
    }
}
