using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public interface IGridRepository<TEntity, TPagingModel> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TPagingModel : class, IPagingModel
    {
        PagingResponseEntity<TEntity> GetPaging(TPagingModel pagingModel);
    }
}