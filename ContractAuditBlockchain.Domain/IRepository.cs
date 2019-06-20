using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.Domain
{
    public interface IRepository<T, TKey> where T : class
    {

        T GetById(TKey id);
        Task<T> GetByIdAsync(TKey id);

        IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate);

        IQueryable<T> GetAll();

        void Insert(T entity);

        void Update(T entity);

        void Delete(T entity);

        void Delete(TKey id);

        Task DeleteAsync(TKey id);

        void Save();

        Task SaveAsync();
    }
}
