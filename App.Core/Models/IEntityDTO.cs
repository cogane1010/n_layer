using Newtonsoft.Json;
using System;

namespace App.Core.Domain
{

    public interface IGenericEntityDTO<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
    }
    public interface IEntityDTO : IGenericEntityDTO<Guid>
    {
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string UpdatedUser { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedDate { get; set; }
    }
}
