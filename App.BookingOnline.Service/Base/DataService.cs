using App.Core.Domain;
using App.Core.Helper;
using App.Core.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace App.BookingOnline.Service.Base
{
    public interface IDataService<TEntityDTO> : IDisposable
        where TEntityDTO : class
    {
        TEntityDTO Get(Guid Id);
        IEnumerable<TEntityDTO> GetAll();
        TEntityDTO Add(TEntityDTO entityDTO);
        void Update(TEntityDTO entityDTO);
        void Delete(Guid Id);
        //IEnumerable<TEntityDTO> AddRange(IEnumerable<TEntityDTO> data);
    }

    public class DataService<TEntityDTO, TEntity, TRepository> : IDataService<TEntityDTO>
        where TEntityDTO : class, IEntityDTO
        where TEntity : class, IEntity
        where TRepository : class, IBaseRepository<TEntity>
    {
        protected TRepository _repo;

        public DataService(TRepository repo)
        {
            _repo = repo;
        }

        public TEntityDTO Add(TEntityDTO entityDTO)
        {
            var entity = AutoMapperHelper.Map<TEntityDTO, TEntity>(entityDTO);
            var result = _repo.AddAsync(entity).Result;
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
        }

        public void Delete(Guid Id)
        {
            _repo.Delete(Id);
        }

        public void Dispose()
        {
            if (_repo != null && _repo is IDisposable)
            {
                (_repo as IDisposable).Dispose();
            }
        }

        public TEntityDTO Get(Guid Id)
        {
            var result = _repo.GetByIdAsync(Id).Result;
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
        }

        public IEnumerable<TEntityDTO> GetAll()
        {
            var result = _repo.GetAllAsync().Result;

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TEntity, TEntityDTO>();
            });

            IMapper mapper = config.CreateMapper();

            var res = mapper.Map<List<TEntity>, List<TEntityDTO>>(result as List<TEntity>);
            return res;
        }

        public void Update(TEntityDTO entityDTO)
        {
            var entity = AutoMapperHelper.Map<TEntityDTO, TEntity>(entityDTO);
            _repo.Update(entity);
        }

    }
}
