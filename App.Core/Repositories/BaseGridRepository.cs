using App.Core.Domain;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace App.Core.Repositories
{
    public interface IBaseGridRepository<TEntity, TPagingModel>
       where TEntity : class, IEntity
       where TPagingModel : class, IPagingModel
    {
        PagingResponseEntity<TEntity> GetPaging(TPagingModel pagingModel);
        ValueTask<TEntity> GetByIdAsync(Guid id);
        ValueTask<TEntity> SingleOrDefaultAsync(Guid id);
        TEntity SingleOrDefault(Guid id);
        ValueTask<IEnumerable<TEntity>> GetAlls();
        ValueTask<TEntity> AddAsync(TEntity entityDTo);
        IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entityDTOs);
        void Update(TEntity entityDto);
        void Delete(Guid id);
        void RemoveRange(IEnumerable<TEntity> entities);
    }

    public class BaseGridRepository<TEntity, TPagingModel>
           : BaseDataRepository<TEntity>, IBaseGridRepository<TEntity, TPagingModel>
       where TEntity : class, IEntity
       where TPagingModel : class, IPagingModel
    {
        protected readonly ILogger _logger;
        protected IBaseRepository<TEntity> _repo;
        public BaseGridRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _repo = unitOfWork.GetDataRepository<TEntity>();
        }

        public virtual async ValueTask<TEntity> AddAsync(TEntity entityDTo)
        {
            var res = await _repo.AddAsync(entityDTo);
            _unitOfWork.SaveChanges();
            return res;
        }

        public IEnumerable<TEntity> AddRange(IEnumerable<TEntity> entityDTOs)
        {
            throw new NotImplementedException();
        }

        public virtual void Delete(Guid id)
        {
            _repo.Delete(id);
            _unitOfWork.SaveChanges();
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            _repo.RemoveRange(entities);
            _unitOfWork.SaveChanges();
        }

        public virtual async ValueTask<TEntity> GetByIdAsync(Guid id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public ValueTask<IEnumerable<TEntity>> GetAlls()
        {
            return _repo.GetAllAsync();
        }

        public virtual PagingResponseEntity<TEntity> GetPaging(TPagingModel pagingModel)
        {
            var data = new PagingResponseEntity<TEntity>
            {
                Data = _repo.GetAllAsync().Result
                            .Skip(pagingModel.PageIndex * pagingModel.PageSize)
                            .Take(pagingModel.PageSize).ToList()
            };
            data.Count = data.Data.Count();
            return data;
        }

        public virtual void Update(TEntity entityDto)
        {
            _repo.Update(entityDto);
            _unitOfWork.SaveChanges();
        }

        public virtual async ValueTask<TEntity> SingleOrDefaultAsync(Guid id)
        {
            var aaa = await _repo.SingleOrDefaultAsync(x => x.Id == id);
            return aaa;
        }

        public virtual TEntity SingleOrDefault(Guid id)
        {
            var entity = _repo.SingleOrDefault(x => x.Id == id);
            return entity;
        }


    }
}