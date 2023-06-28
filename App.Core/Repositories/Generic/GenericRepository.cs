using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public class GenericRepository<TEntity, TKey> : IGenericRepository<TEntity, TKey>
         where TKey : IEquatable<TKey>
         where TEntity : class, IGenericEntity<TKey>
    {
        protected readonly DbContext Context;
        protected DbSet<TEntity> dbSet;

        public GenericRepository(DbContext context)
        {
            this.Context = context;
            this.dbSet = Context.Set<TEntity>();
        }

        public virtual async ValueTask<TEntity> AddAsync(TEntity entity)
        {
            var res = await Context.Set<TEntity>().AddAsync(entity);
            Context.SaveChanges();
            return res.Entity;
        }

        public virtual void Update(TEntity entity)
        {
            Context.Set<TEntity>().Update(entity);
            Context.SaveChanges();

        }

        public virtual async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Context.Set<TEntity>().AddRangeAsync(entities);
            Context.SaveChanges();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate);
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await Context.Set<TEntity>().ToListAsync();
        }


        public virtual ValueTask<TEntity> GetByIdAsync(TKey id)
        {
            return Context.Set<TEntity>().FindAsync(id);
        }

        public virtual void Delete(TKey Id)
        {
            var entity = Context.Set<TEntity>().Find(Id);
            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
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

        public virtual Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().AsNoTracking().SingleOrDefaultAsync(predicate);
        }

        public void UpdateByProperties(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> properties)
        {
            this.dbSet.Attach(entity);
            var entry = this.Context.Entry(entity);
            foreach (var prop in properties)
            {
                entry.Property(prop).IsModified = true;
            }

            Context.SaveChanges();

        }
    }
}