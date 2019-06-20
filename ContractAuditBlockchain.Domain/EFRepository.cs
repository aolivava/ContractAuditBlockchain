using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ContractAuditBlockchain.Domain
{
    public class EFRepository<T, TKey> : IRepository<T, TKey>, IDisposable where T : class
    {
        private readonly IApplicationDbContext db = null;
        private DbSet<T> table = null;

        public EFRepository(IApplicationDbContext db)
        {
            this.db = db;
            table = db.Set<T>();
        }

        public T GetById(TKey id)
        {
            try
            {
                return table.Find(id);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error retrieving entity by id", exc);
            }
        }

        public async Task<T> GetByIdAsync(TKey id)
        {
            try
            {
                return await table.FindAsync(id);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error retrieving entity by id", exc);
            }
        }

        public IQueryable<T> SearchFor(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return table.Where(predicate);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error searching for entities", exc);
            }
        }

        public IQueryable<T> GetAll()
        {
            try
            {
                return table;
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error getting all entities", exc);
            }
        }

        public void Insert(T obj)
        {
            try
            {
                table.Add(obj);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error inserting entity", exc);
            }
        }

        public void Update(T obj)
        {
            try
            {
                table.Attach(obj);
                db.Entry(obj).State = EntityState.Modified;
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error updating entity", exc);
            }
        }

        public void Save()
        {
            try
            {
                db.SaveChanges();
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error saving changes", exc);
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await db.SaveChangesAsync();
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error saving changes", exc);
            }
        }

        public void Delete(T entity)
        {
            try
            {
                if (db.Entry(entity).State == EntityState.Detached)
                {
                    table.Attach(entity);
                }
                table.Remove(entity);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error deleting entity", exc);
            }
        }

        public void Delete(TKey id)
        {
            try
            {
                var entity = table.Find(id);
                table.Remove(entity);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error deleting entity by id", exc);
            }
        }

        public async Task DeleteAsync(TKey id)
        {
            try
            {
                var entity = await table.FindAsync(id);
                table.Remove(entity);
            }
            catch (Exception exc)
            {
                throw new RepositoryException("Error deleting entity by id", exc);
            }
        }

        public void Dispose()
        {
            if (this.db != null)
            {
                this.db.Dispose();
            }
        }
    }
}
