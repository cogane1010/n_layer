using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public interface IGenericRepository<TEntity, TKey>
         where TKey : IEquatable<TKey>
         where TEntity : class, IGenericEntity<TKey>
    {
        ValueTask<TEntity> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        ValueTask<TEntity> AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Update(TEntity entity);
        void Delete(TKey Id);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void UpdateByProperties(TEntity entity, IEnumerable<Expression<Func<TEntity, object>>> properties);
    }
}