using System.Collections.Generic;

namespace LibraryManagementSystem.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Archive(T entity);
    }
}
