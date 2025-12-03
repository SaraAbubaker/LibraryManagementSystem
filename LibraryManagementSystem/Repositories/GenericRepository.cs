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
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }

        public T GetById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null)
                throw new Exception($"Entity of type {typeof(T).Name} with id {id} not found.");
            return entity;
        }

        public void Add(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }

        public void Archive(T entity)
        {
            dynamic e = entity;
            e.IsArchived = true;
            _dbSet.Update(e);
            _context.SaveChanges();
        }

    }
}
