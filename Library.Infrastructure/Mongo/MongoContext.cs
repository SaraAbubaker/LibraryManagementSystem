
using MongoDB.Driver;

namespace Library.Infrastructure.Mongo
{
    public class MongoContext
    {
        public IMongoDatabase Database { get; }

        public MongoContext(string connectionString)
        {
            var client = new MongoClient(connectionString);

            //Connect to the "LibraryLogs" database
            Database = client.GetDatabase("LibraryLogs");
        }
    }
}
