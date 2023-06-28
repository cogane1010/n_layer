using Newtonsoft.Json;
using System;

namespace App.Core.Domain
{
    public class BaseGenericEntity<TKey>
        where TKey : IEquatable<TKey>
    {
        public TKey Id { get; set; }
        [JsonIgnore]
        public string CreatedUser { get; set; }
        [JsonIgnore]
        public DateTime CreatedDate { get; set; }
        [JsonIgnore]
        public string UpdatedUser { get; set; }
        [JsonIgnore]
        public DateTime? UpdatedDate { get; set; }
    }
    public class BaseEntity: BaseGenericEntity<Guid>
    {
       
    }
}
