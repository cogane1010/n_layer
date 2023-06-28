using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public interface IBaseRepository<TEntity>
    {
        public IQueryable<TEntity> AsQueryable();
        ValueTask<TEntity> GetByIdAsync(Guid id);
        ValueTask<IEnumerable<TEntity>> GetAllAsync();
        IEnumerable<TEntity> GetAll();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        ValueTask<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(Guid Id);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void UpdateByProperties(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> properties);
        Task<T> AddAsyncWithChild<T>(T entity, params Expression<Func<T, object>>[] navigations) where T : BaseEntity;
        TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> SelectWhere(Expression<Func<TEntity, bool>> predicate);
        IQueryable<TEntity> SelectWhereNoTracking(Expression<Func<TEntity, bool>> predicate);
    }

    public class BaseRepository<TEntity> : IBaseRepository<TEntity>, IDisposable where TEntity : class
    {
        protected readonly DbContext _context;
        protected DbSet<TEntity> dbSet;

        public BaseRepository(DbContext context)
        {
            this._context = context;
            this.dbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return this.dbSet.AsQueryable<TEntity>();
        }

        public async ValueTask<TEntity> AddAsync(TEntity entity)
        {
            var res = await _context.Set<TEntity>().AddAsync(entity);
            _context.Entry(entity).State = EntityState.Added;
            _context.SaveChanges();
            return res.Entity;
        }

        public void Update(TEntity entity)
        {
            //_context.Entry(entity).State = EntityState.Detached;
            _context.Set<TEntity>().Update(entity);
            _context.Entry(entity).State = EntityState.Modified;
            _context.SaveChanges();

        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await _context.Set<TEntity>().AddRangeAsync(entities);
            _context.SaveChanges();
        }

        public IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate);
        }

        public async ValueTask<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _context.Set<TEntity>().ToListAsync();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        public async ValueTask<TEntity> GetByIdAsync(Guid id)
        {
            return await _context.Set<TEntity>().FindAsync(id);
        }

        public void Delete(Guid Id)
        {
            var entity = _context.Set<TEntity>().Find(Id);
            _context.Set<TEntity>().Remove(entity);
            _context.Entry(entity).State = EntityState.Deleted;
            _context.SaveChanges();
        }
        public void Remove(TEntity entity)
        {
            _context.Set<TEntity>().Remove(entity);
            _context.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
            _context.SaveChanges();
        }

        public Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var aa = _context.Set<TEntity>().SingleOrDefaultAsync(predicate);
            return aa;
        }
        public TEntity SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {            
            var entity = _context.Set<TEntity>().SingleOrDefault(predicate);
            return entity;
        }

        public IQueryable<TEntity> SelectWhere(Expression<Func<TEntity, bool>> predicate)
        {            
            var entity = _context.Set<TEntity>().Where(predicate);
            //DetachAllEntities();
            return entity;
        }
        public IQueryable<TEntity> SelectWhereNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = _context.Set<TEntity>().Where(predicate).AsNoTracking();           
            return entity;
        }
        public void DetachAllEntities()
        {
            var changedEntriesCopy = _context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added ||
                            e.State == EntityState.Modified ||
                            e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in changedEntriesCopy)
                entry.State = EntityState.Detached;
        }
        public void UpdateByProperties(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> properties)
        {
            this.dbSet.Attach(entity);
            var entry = this._context.Entry(entity);
            foreach (var prop in properties)
            {
                entry.Property(prop).IsModified = true;
            }

            _context.SaveChanges();

        }

        public async Task<T> AddAsyncWithChild<T>(T entity, params Expression<Func<T, object>>[] navigations) where T : BaseEntity
        {
            var dbEntity = _context.Add<T>(entity);

            var dbEntry = _context.Entry(dbEntity);
            dbEntry.CurrentValues.SetValues(entity);

            foreach (var property in navigations)
            {
                var propertyName = property.GetPropertyAccess().Name;
                var dbItemsEntry = dbEntry.Collection(propertyName);
                var accessor = dbItemsEntry.Metadata.GetCollectionAccessor();

                await dbItemsEntry.LoadAsync();
                var dbItemsMap = ((IEnumerable<BaseEntity>)dbItemsEntry.CurrentValue)
                    .ToDictionary(e => e.Id);

                var items = (IEnumerable<BaseEntity>)accessor.GetOrCreate(entity, true);

                foreach (var item in items)
                {
                    if (!dbItemsMap.TryGetValue(item.Id, out var oldItem))
                        accessor.Add(dbEntity, item, true);
                    else
                    {
                        _context.Entry(oldItem).CurrentValues.SetValues(item);
                        dbItemsMap.Remove(item.Id);
                    }
                }

                foreach (var oldItem in dbItemsMap.Values)
                    accessor.Remove(dbEntity, oldItem);
            }
            await _context.SaveChangesAsync();
            return entity;
        }
               

        public void Dispose()
        {
            _context.Dispose();
        }

        
    }
}