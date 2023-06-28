using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public class GenericGridRepository<TEntity, TPagingModel, TKey>
           : GenericRepository<TEntity, TKey>, IGenericGridRepository<TEntity, TPagingModel, TKey>
       where TKey : IEquatable<TKey>
       where TEntity : class, IGenericEntity<TKey>
       where TPagingModel : class, IPagingModel
    {
        public GenericGridRepository(DbContext context) : base(context)
        {
        }

        public virtual PagingResponseGenericEntity<TEntity, TKey> GetPaging(TPagingModel pagingModel)
        {

            var result = new PagingResponseGenericEntity<TEntity, TKey>
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