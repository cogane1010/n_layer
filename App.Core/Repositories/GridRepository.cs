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
    public class GridRepository<TEntity, TPagingModel>
           : Repository<TEntity>, IGridRepository<TEntity, TPagingModel>
       where TEntity : class, IEntity
       where TPagingModel : class, IPagingModel
    {
        public GridRepository(DbContext context) : base(context)
        {
        }

        public virtual PagingResponseEntity<TEntity> GetPaging(TPagingModel pagingModel)
        {
           
            var result = new PagingResponseEntity<TEntity>
            {
                Data = this.dbSet.AsQueryable().OrderBy(x => x.Id)
                            .Skip(pagingModel.PageIndex * pagingModel.PageSize)
                            .Take(pagingModel.PageSize).ToList(),
                Count = this.dbSet.AsQueryable().Count()
            };
            return result;
        }
    }
}