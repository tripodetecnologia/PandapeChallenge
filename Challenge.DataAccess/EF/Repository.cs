using Microsoft.EntityFrameworkCore;
using Challenge.DataAccess.Core;
using System.Linq.Expressions;

namespace Challenge.DataAccess.EF
{
    public class Repository<T> : IRepository<T> where T : class
    {

        #region Properties

        private readonly DbSet<T> _entitySet;

        private readonly DataBaseContext _context;

        public Repository(DataBaseContext context)
        {
            this._context = context;
            this._entitySet = context.Set<T>();
        }

        #endregion

        #region Methods

        public void AddOrUpdate(T entity)
        {
            var entry = _context.Entry(entity);
            switch (entry.State)
            {
                case EntityState.Detached:
                    _context.Add(entity);
                    break;
                case EntityState.Modified:
                    _context.Update(entity);
                    break;
                case EntityState.Added:
                    _context.Add(entity);
                    break;
                case EntityState.Unchanged:
                    //item already in db no need to do anything  
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void BulkInsert(IEnumerable<T> entity)
        {
            _context.BulkInsert(entity);
        }

        public async Task BulkInsertAsync(IEnumerable<T> entity)
        {
            await _context.BulkInsertAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAsync()
        {
            IEnumerable<T> query = null;
            await Task.Run(() =>
            {
                query = this._entitySet.AsNoTracking<T>();
            });
            return query;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where)
        {
            IEnumerable<T> query = null;
            await Task.Run(() =>
            {
                query = this._entitySet.Where<T>(where).AsNoTracking<T>();
            });
            return query;
        }

        public async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> where, params string[] includes)
        {
            IEnumerable<T> query = null;
            IQueryable<T> ts = this._entitySet.AsNoTracking<T>().Where(where);

            await Task.Run(() =>
            {
                var queryWithIncludes = ts;

                foreach (var include in includes)
                    queryWithIncludes = queryWithIncludes.Include(include);

                query = queryWithIncludes;

            });
            return query;
        }

        public void Remove(Expression<Func<T, bool>> where)
        {
            foreach (T t in this._entitySet.Where<T>(where).AsEnumerable<T>())
            {
                this._entitySet.Remove(t);
            }
        }

        public void Update(T entity)
        {
            this._context.Entry<T>(entity).State = EntityState.Modified;
        }

        #endregion
    }
}
