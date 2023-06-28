
using App.Core;
using App.Core.Domain;
using App.Core.Helper;
using App.Core.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public class GenericBaseGridService<TEntityDTO, TEntity, TPagingModel, TRepository, TKey>
           : GenericBaseDataService<TEntityDTO, TEntity, TRepository, TKey>, IGenericBaseGridService<TEntityDTO, TPagingModel, TKey>
       where TKey : IEquatable<TKey>
       where TEntityDTO : class, IGenericEntityDTO<TKey>
       where TEntity : class, IGenericEntity<TKey>
       where TPagingModel : class, IPagingModel
       where TRepository : class, IGenericGridRepository<TEntity, TPagingModel, TKey>
    {
        public GenericBaseGridService(TRepository repo) : base(repo)
        {
            _repo = repo;
        }

        public virtual PagingResponseGenericEntityDTO<TEntityDTO, TKey> GetPaging(TPagingModel pagingModel)
        {
            var paging = _repo.GetPaging(pagingModel);
            return new PagingResponseGenericEntityDTO<TEntityDTO, TKey>
            {
                Count = paging.Count,
                Data = AutoMapperHelper.Map<TEntity, TEntityDTO, IEnumerable<TEntity>, IEnumerable<TEntityDTO>>(paging.Data)
            };
        }
    }
}
