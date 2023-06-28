using App.Core.Domain;
using App.Core.Helper;
using App.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.BookingOnline.Service.Base
{
    public interface IBaseGridDataService<TEntityDTO, TPagingModel>
         where TEntityDTO : class, IEntityDTO
         where TPagingModel : class, IPagingModel
    {
        PagingResponseEntityDTO<TEntityDTO> GetPaging(TPagingModel pagingModel);
        ValueTask<TEntityDTO> GetByIdAsync(Guid Id);
        TEntityDTO GetById(Guid Id);
        Task<IEnumerable<TEntityDTO>> GetAll();
        ValueTask<TEntityDTO> AddAsync(TEntityDTO entityDTO);
        void Update(TEntityDTO entityDTO);
        void Delete(Guid Id);
    }

    public class BaseGridDataService<TEntityDTO, TEntity, TPagingModel, TRepository> : IBaseGridDataService<TEntityDTO, TPagingModel>
       where TEntityDTO : class, IEntityDTO
       where TEntity : class, IEntity
       where TPagingModel : class, IPagingModel
       where TRepository : class, IBaseGridRepository<TEntity, TPagingModel>
    {
        protected readonly TRepository _gridRepository;
        public BaseGridDataService(TRepository repo)
        {
            _gridRepository = repo;
        }

        public virtual async ValueTask<TEntityDTO> AddAsync(TEntityDTO entityDTO)
        {
            var entity = AutoMapperHelper.Map<TEntityDTO, TEntity>(entityDTO);
            var result = await _gridRepository.AddAsync(entity);
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
        }

        public void Delete(Guid Id)
        {
            _gridRepository.Delete(Id);
        }

        public virtual async ValueTask<TEntityDTO> GetByIdAsync(Guid Id)
        {
            var result = await _gridRepository.GetByIdAsync(Id);
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
        }

        public virtual TEntityDTO GetById(Guid Id)
        {
            var result = _gridRepository.SingleOrDefault(Id);
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
        }

        public virtual async Task<IEnumerable<TEntityDTO>> GetAll()
        {
            var result = await _gridRepository.GetAlls();
            return AutoMapperHelper.Map<TEntity, TEntityDTO, List<TEntity>, List<TEntityDTO>>(result.ToList());
        }

        public virtual PagingResponseEntityDTO<TEntityDTO> GetPaging(TPagingModel pagingModel)
        {
            var paging = _gridRepository.GetPaging(pagingModel);

            return new PagingResponseEntityDTO<TEntityDTO>
            {
                Count = paging.Count,
                Data = AutoMapperHelper.Map<TEntity, TEntityDTO, IEnumerable<TEntity>, IEnumerable<TEntityDTO>>(paging.Data)
            };
        }

        public virtual void Update(TEntityDTO entityDTO)
        {
            var entity = AutoMapperHelper.Map<TEntityDTO, TEntity>(entityDTO);
            _gridRepository.Update(entity);
        }
    }
}
