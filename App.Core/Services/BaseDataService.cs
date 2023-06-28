
using App.Core.Domain;
using App.Core.Helper;
using App.Core.Repositories;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.Core.Service
{
    public class BaseDataService<TEntityDTO, TEntity, TRepository> : IBaseDataService<TEntityDTO>
        where TEntityDTO : class, IEntityDTO
        where TEntity : class, IEntity
        where TRepository : class, IRepository<TEntity>
    {
        protected TRepository _repo;

        public BaseDataService(TRepository repo)
        {
            _repo = repo;
        }

        public void Dispose()
        {
            if (_repo != null && _repo is IDisposable)
            {
                (_repo as IDisposable).Dispose();
            }
        }

        public virtual TEntityDTO Get(Guid Id)
        {
            var result = _repo.GetByIdAsync(Id).Result;
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
          
        }

        public virtual TEntityDTO Add(TEntityDTO entityDTO)
        {
            var entity = AutoMapperHelper.Map<TEntityDTO, TEntity>(entityDTO);
            var result = _repo.AddAsync(entity).Result;
            return AutoMapperHelper.Map<TEntity, TEntityDTO>(result);
        }

        public virtual void Update(TEntityDTO entityDTO)
        {
            var entity = AutoMapperHelper.Map<TEntityDTO, TEntity>(entityDTO);
            _repo.Update(entity);
        }

        public virtual void Delete(Guid Id)
        {
            _repo.Delete(Id);
        }

        public virtual IEnumerable<TEntityDTO> GetAll()
        {
            var result = _repo.GetAllAsync().Result;

            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TEntity, TEntityDTO>();
            });

            IMapper mapper = config.CreateMapper();

            var res = mapper.Map<List<TEntity>, List<TEntityDTO>>(result as List<TEntity>);
            return res;
        }

        //public IEnumerable<TEntityDTO> AddRange(IEnumerable<TEntityDTO> entitiesDTO)
        //{
        //    var config = new MapperConfiguration(cfg => {
        //        cfg.CreateMap<TEntityDTO, TEntity>();
        //    });

        //    IMapper mapper = config.CreateMapper();

        //    var entities = mapper.Map<List<TEntityDTO>, List<TEntity>>(entitiesDTO as List<TEntityDTO>);
        //    var result = _repo.AddRange(entities);

        //    config = new MapperConfiguration(cfg => {
        //        cfg.CreateMap<TEntity, TEntityDTO>();
        //    });

        //    mapper = config.CreateMapper();
        //    return mapper.Map<List<TEntity>, List<TEntityDTO>>(entitiesDTO as List<TEntity>);
        //}
    }
}
