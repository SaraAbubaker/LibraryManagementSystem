using Library.Domain.Data;
using Library.Entities.Base;
using Library.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace Library.Domain.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : AuditBase
    {
        private readonly LibraryContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(LibraryContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); //Access to tables
        }

        //Methods from IGenericRepository<T>
        public IQueryable<T> GetAll()
        {
            IQueryable<T> query = _dbSet.AsQueryable();

            if (typeof(IArchivable).IsAssignableFrom(typeof(T)))
            {
                query = query.Where(e => !e.IsArchived);
            }

            return query;
        }


        public IQueryable<T> GetById(int id)
        {
            return _dbSet.Where(e => EF.Property<int>(e, "Id") == id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveAsync(T entity)
        {
            dynamic e = entity;
            try
            {
                e.IsArchived = true;
                _dbSet.Update(e);
                await _context.SaveChangesAsync();
            }
            catch (Microsoft.CSharp.RuntimeBinder.RuntimeBinderException)
            {
                throw new Exception($"Entity of type {typeof(T).Name} does not have an IsArchived property.");
            }
        }


    }
}
