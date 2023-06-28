using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public interface IGenericGridRepository<TEntity, TPagingModel, TKey> : IGenericRepository<TEntity, TKey>
        where TKey : IEquatable<TKey>
        where TEntity : class, IGenericEntity<TKey>
        where TPagingModel : class, IPagingModel
    {
        PagingResponseGenericEntity<TEntity, TKey> GetPaging(TPagingModel pagingModel);
    }
}
