
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
    public class BaseGridService<TEntityDTO, TEntity, TPagingModel, TRepository>
           : BaseDataService<TEntityDTO, TEntity, TRepository>, IBaseGridService<TEntityDTO, TPagingModel>
       where TEntityDTO : class, IEntityDTO
       where TEntity : class, IEntity
       where TPagingModel : class, IPagingModel
       where TRepository : class, IGridRepository<TEntity, TPagingModel>
    {
        public BaseGridService(TRepository repo) : base(repo)
        {
            _repo = repo;
        }

        public virtual PagingResponseEntityDTO<TEntityDTO> GetPaging(TPagingModel pagingModel)
        {
            var paging = _repo.GetPaging(pagingModel);
            return new PagingResponseEntityDTO<TEntityDTO>
            {
                Count = paging.Count,
                Data = AutoMapperHelper.Map<TEntity, TEntityDTO, IEnumerable<TEntity>, IEnumerable<TEntityDTO>>(paging.Data)
            };
        }
    }
}
