using DBO.Data.Repositories.Contract;

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace DBO.Data.Repositories.Implementation
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
        }

        public T Create(T entity)
        {
            return _context.Set<T>().Add(entity);
        }

        public ICollection<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public async Task<ICollection<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }

        public IQueryable<T> Query()
        {
            return _context.Set<T>();
        }

        public void Remove(int id)
        {
            _context.Set<T>().Remove(GetById(id));
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public T Update(T entity)
        {
            if (_context.Entry(entity).State == EntityState.Detached)
            {
                _context.Set<T>().Add(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            return entity;
        }
    }
}
