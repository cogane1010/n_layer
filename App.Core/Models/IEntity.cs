using System;

namespace App.Core.Domain
{

    public interface IGenericEntity<TKey>
         where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
    public interface IEntity : IGenericEntity<Guid>
    {
    }
}
