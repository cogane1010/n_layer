using System;
using System.Collections.Generic;

namespace App.Core.Domain
{
    public class PagingResponseGenericEntity<TEntity, TKey>
         where TKey : IEquatable<TKey>
         where TEntity : class, IGenericEntity<TKey>
    {
        public IEnumerable<TEntity> Data { get; set; }
        public int Count { get; set; }
    }
    public class PagingResponseEntity<TEntity>: PagingResponseGenericEntity<TEntity, Guid>
         where TEntity : class, IEntity
    {
       
    }
}
