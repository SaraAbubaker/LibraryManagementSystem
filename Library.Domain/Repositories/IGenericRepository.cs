
namespace Library.Domain.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        IQueryable<T> GetById(int id);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task ArchiveAsync(T entity);
        Task CommitAsync();
    }
}
