using System.Linq.Expressions;

namespace Challenge.DataAccess.Core
{
    public interface IRepository<T> where T : class
    {
        void AddOrUpdate(T entity);
        
        Task<IEnumerable<T>> GetAsync();

        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where, params string[] includes);
                
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where);                      
        
        void Remove(Expression<Func<T, bool>> where);

        void Update(T entity);
             
    }
}
