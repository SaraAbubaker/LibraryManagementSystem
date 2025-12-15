
using MongoDB.Driver;

namespace Library.Infrastructure.Mongo
{
    public class MongoRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(MongoContext context, string collectionName)
        {
            _collection = context.Database.GetCollection<T>(collectionName);
        }

        //Insert
        public async Task InsertAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        //Update
        public async Task UpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
        {
            await _collection.UpdateOneAsync(filter, update);
        }

        //Find
        public async Task<List<T>> FindAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).ToListAsync();
        }

        //Find One
        public async Task<T> FindOneAsync(FilterDefinition<T> filter)
        {
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }

        //Delete
        public async Task DeleteAsync(FilterDefinition<T> filter)
        {
            await _collection.DeleteOneAsync(filter);
        }
    }
}