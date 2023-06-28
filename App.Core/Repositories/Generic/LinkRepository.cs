using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;



namespace App.Core.Repositories
{
    public interface ILinkRepository<TEntity>
         where TEntity : class
    {
        IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TEntity>> GetAllAsync();
        TEntity Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
    }

    public class LinkRepository<TEntity> : ILinkRepository<TEntity>
        where TEntity : class
    {
        protected readonly DbContext Context;
        protected DbSet<TEntity> dbSet;

        public LinkRepository(DbContext context)
        {
            this.Context = context;
            this.dbSet = Context.Set<TEntity>();
        }

        public virtual TEntity Add(TEntity entity)
        {
            var res = Context.Set<TEntity>().Add(entity).Entity;
            Context.SaveChanges();
            return res;
        }

        public virtual void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            Context.SaveChanges();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }

        public virtual void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
        }

        public virtual void RemoveRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            Context.SaveChanges();
        }

        public virtual IEnumerable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        
    }

}