using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DBO.Data.Repositories.Contract
{
    public interface IRepository<T>
    {
        T Create(T entity);
        IQueryable<T> Query();
        ICollection<T> GetAll();
        Task<ICollection<T>> GetAllAsync();
        T GetById(int id);
        void Remove(int id);
        T Update(T entity);
        void SaveChanges();
    }
}
