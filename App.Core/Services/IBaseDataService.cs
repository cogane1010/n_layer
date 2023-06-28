using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public interface IBaseDataService<TEntityDTO> : IDisposable
        where TEntityDTO : class, IEntityDTO
    {
        TEntityDTO Get(Guid Id);
        IEnumerable<TEntityDTO> GetAll();
        TEntityDTO Add(TEntityDTO entityDTO);
        void Update(TEntityDTO entityDTO);
        void Delete(Guid Id);
        //IEnumerable<TEntityDTO> AddRange(IEnumerable<TEntityDTO> data);
    }
}