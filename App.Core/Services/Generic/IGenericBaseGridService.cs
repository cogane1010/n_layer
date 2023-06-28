using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public interface IGenericBaseGridService<TEntityDTO, TPagingModel, TKey> : IGenericBaseDataService<TEntityDTO, TKey>
         where TKey : IEquatable<TKey>
         where TEntityDTO : class, IGenericEntityDTO<TKey>
         where TPagingModel : class, IPagingModel
    {
        PagingResponseGenericEntityDTO<TEntityDTO, TKey> GetPaging(TPagingModel pagingModel);
    }
}