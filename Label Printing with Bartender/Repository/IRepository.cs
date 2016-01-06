using System.Linq;

namespace Label_Printing_with_Bartender.Service
{
    public interface IRepository
    {
        void Add<T>(T entity) where T : class;

        void Delete<T>(T entity) where T : class;

        void Update<T>(T entity) where T : class;

        IQueryable<T> Get<T>() where T : class;
        
        int Save();
    }
}
