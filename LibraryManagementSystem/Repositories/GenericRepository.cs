using LibraryManagementSystem.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace LibraryManagementSystem.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly LibraryContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(LibraryContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); //Access to tables
        }

        //Methods from IGenericRepository<T>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
                throw new Exception($"Entity of type {typeof(T).Name} with id {id} not found.");
            return entity;
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
