using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public interface IBaseGridService<TEntityDTO, TPagingModel> : IBaseDataService<TEntityDTO>
         where TEntityDTO : class, IEntityDTO
         where TPagingModel : class, IPagingModel
    {
        PagingResponseEntityDTO<TEntityDTO> GetPaging(TPagingModel pagingModel);
    }
}