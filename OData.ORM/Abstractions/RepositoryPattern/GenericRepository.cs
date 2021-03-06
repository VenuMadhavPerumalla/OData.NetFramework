﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OData.ORM.Abstractions.RepositoryPattern
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        private bool _disposed;

        public GenericRepository(DbContext context)
        {
            Context = context;
        }

        protected IDbSet<TEntity> DbSet
        {
            get
            {
                return Context.Set<TEntity>();
            }
        }        

        public TEntity Get<TDataType>(TDataType id) where TDataType : struct
        {
            return Context.Set<TEntity>().Find(id);
        }

        public TEntity Get<TDataType>(TDataType Id
            , Expression<Func<TEntity, object>> includes
            , Expression<Func<TEntity, bool>> predicate) where TDataType : struct
        {
            return Context.Set<TEntity>().Include(includes).SingleOrDefault(predicate);
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, object>> includes)
        {
            return Context.Set<TEntity>().Include(includes).ToList();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, object>> includes, Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Include(includes).Where(predicate).ToList();
        }

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
        }

        public virtual void Update(TEntity entity)
        {
            this.Context.Entry<TEntity>(entity).State = System.Data.Entity.EntityState.Modified;
        }
        public void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Attach(entity);
            Context.Entry(entity).State = EntityState.Deleted;
        }

        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.FirstOrDefaultAsync<TEntity>(predicate);
        }

        public async Task<bool> ExistAsync(Expression<Func<TEntity, bool>> predicate = null)
        {
            var set = this.Context.Set<TEntity>();
            return (predicate == null) ? await set.AnyAsync() : await set.AnyAsync(predicate);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    Context.Dispose();

            _disposed = true;
        }
    }
}
