using System;
using System.Data.Entity;

namespace App.Core.Repositories
{
    public interface IBaseDataRepository<TEntity> 
        where TEntity : class
    {
    }

    public class BaseDataRepository<TEntity> : IBaseDataRepository<TEntity>, IDisposable where TEntity : class
    {
        protected readonly IUnitOfWork _unitOfWork;
        
        protected IBaseRepository<TEntity> _repo;
        public BaseDataRepository(IUnitOfWork unitOfWork) 
        {
            _unitOfWork = unitOfWork;
            _repo = unitOfWork.GetDataRepository<TEntity>();
        }

        public virtual void Dispose()
        {
            if (_unitOfWork != null && _unitOfWork is IDisposable)
            {
                (_unitOfWork as IDisposable).Dispose();
            }
        }

        
    }
}
