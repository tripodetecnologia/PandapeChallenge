﻿using Challenge.DataAccess.Core;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace Challenge.DataAccess.EF
{
    public class UnitOfWork : IUnitOfWork
    {

        #region Properties
        
        private readonly Dictionary<string, object> _repositories;

        private readonly DataBaseContext _context;

        private bool _disposed;

        #endregion


        public UnitOfWork(DataBaseContext context)
        {
            this._context = context;
            this._repositories = new Dictionary<string, object>();
        }

        #region Methods

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed && disposing)
            {
                this._context.Dispose();
            }
            this._disposed = true;
        }

        public IRepository<T> Repository<T>()
        where T : class
        {
            string name = typeof(T).Name;
            if (this._repositories.ContainsKey(name))
            {
                return (Repository<T>)this._repositories[name];
            }
            object obj = Activator.CreateInstance(typeof(Repository<>).MakeGenericType(new Type[] { typeof(T) }), new object[] { this._context });
            this._repositories.Add(name, obj);
            return (Repository<T>)this._repositories[name];
        }

        public async Task SaveChangesAsync()
        {
            try
            {
                await this._context.SaveChangesAsync();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw e;
            }
        }

        #endregion
    }
}
