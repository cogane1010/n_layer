using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public interface IGenericBaseDataService<TEntityDTO, TKey> : IDisposable
        where TKey : IEquatable<TKey>
        where TEntityDTO : class, IGenericEntityDTO<TKey>
    {
        TEntityDTO Get(TKey Id);
        IEnumerable<TEntityDTO> GetAll();
        TEntityDTO Add(TEntityDTO entityDTO);
        void Update(TEntityDTO entityDTO);
        void Delete(TKey Id);
        //IEnumerable<TEntityDTO> AddRange(IEnumerable<TEntityDTO> data);
    }
}