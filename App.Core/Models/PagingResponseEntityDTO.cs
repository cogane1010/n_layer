using System;
using System.Collections.Generic;

namespace App.Core.Domain
{
    public class PagingResponseGenericEntityDTO<TEntityDTO, TKey>
        where TKey : IEquatable<TKey>
        where TEntityDTO : class, IGenericEntityDTO<TKey>
    {
        public IEnumerable<TEntityDTO> Data { get; set; }
        public int Count { get; set; }
    }
    public class PagingResponseEntityDTO<TEntityDTO> : PagingResponseGenericEntityDTO<TEntityDTO, Guid>
         where TEntityDTO : class, IEntityDTO
    {
        
    }
}
