using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public interface IRepository<TEntity> : IGenericRepository<TEntity, Guid>
         where TEntity : class, IEntity
    {
       
    }
}