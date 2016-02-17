using System;
using System.Data.Entity;
using System.Linq;

namespace Label_Printing_with_Bartender.Repository
{
    public sealed class Repository : IRepository, IDisposable
    {
        private readonly DbContext context;

        public Repository(DbContext context)
        {
            this.context = context;
            context.Configuration.ProxyCreationEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
        }

        public void Add<T>(T entity) where T : class
        {
            context.Set<T>().Add(entity);
        }

        public void Delete<T>(T entity) where T : class
        {
            context.Set<T>().Remove(entity);
        }

        public void Update<T>(T entity) where T : class
        {
            context.Entry(entity).State = EntityState.Modified;
        }

        public IQueryable<T> Get<T>() where T : class
        {
            return context.Set<T>();
        }

        public int Save()
        {
            return context.SaveChanges();
        }

        #region IDisposable

        public void Dispose()
        {
            context.Dispose();
        }

        #endregion
    }
}
