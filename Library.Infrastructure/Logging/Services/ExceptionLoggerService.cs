using Library.Infrastructure.Logging.Interfaces;
using Library.Infrastructure.Logging.Models;
using Library.Infrastructure.Mongo;

namespace Library.Infrastructure.Logging.Services
{
    public class ExceptionLoggerService : IExceptionLoggerService
    {
        private readonly MongoRepository<ExceptionLog> _repo;

        public ExceptionLoggerService(MongoContext context)
        {
            _repo = new MongoRepository<ExceptionLog>(context, "ExceptionLogs");
        }

        public async Task LogExceptionAsync(ExceptionLog exception)
        {
            exception.Guid = exception.Guid == Guid.Empty ? Guid.NewGuid() : exception.Guid;
            exception.CreatedAt = DateTime.UtcNow;
            exception.Level = LogLevel.Exception; // always Exception
            await _repo.InsertAsync(exception);
        }

        public async Task<ExceptionLog?> GetExceptionLogAsync(Guid guid)
        {
            return await _repo.FindOneAsync(x => x.Guid == guid);
        }

        public async Task<List<ExceptionLog>> GetAllExceptionLogsAsync()
        {
            return await _repo.FindAsync(_ => true); // fetch all
        }
    }
}
