using App.Core.Domain;
using Microsoft.AspNetCore.Mvc;
using System;
using App.BookingOnline.Service.Base;
using System.Threading.Tasks;

namespace App.BookingOnline.Api.Controllers
{

    public class GridController<TEntityDTO, TPagingModel, TBaseGridService> : BaseApiController
       where TEntityDTO : class, IEntityDTO
       where TPagingModel : class, IPagingModel
       where TBaseGridService : class, IBaseGridDataService<TEntityDTO, TPagingModel>

    {
        protected TBaseGridService _service;

        public GridController(TBaseGridService service)
        {
            _service = service;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("GetPaging")]
        public virtual RespondData GetPaging(TPagingModel filter)
        {
            try
            {
                filter.UserName = UserName;
                filter.UserOrgId = CurOrgId;
                filter.UserId = UserId;
                return Success(_service.GetPaging(filter));
            }
            catch (Exception e)
            {
                return Failure("",e.Message);
            }

        }

        [HttpPost("Get")]
        public virtual RespondData Get(Guid Id)
        {
            try
            {
                var data = _service.GetById(Id);
                return Success(data);
            }
            catch (Exception e)
            {
                return Failure("",e.Message);
            }

        }

        [HttpPost("GetAll")]
        public virtual RespondData GetAll()
        {
            try
            {
                return Success(_service.GetAll().Result);
            }
            catch (Exception e)
            {
                return Failure("",e.Message);
            }

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("AddOrEdit")]
        public async virtual ValueTask<RespondData> AddOrEdit(TEntityDTO entityDTO)
        {
            try
            {
                if (entityDTO.Id == null || entityDTO.Id == Guid.Empty)
                {
                    entityDTO.CreatedDate = DateTime.Now;
                    entityDTO.CreatedUser = this.UserName;
                    var entity = await _service.AddAsync(entityDTO);
                    return Success(entity);
                }
                else
                {
                    entityDTO.UpdatedDate = DateTime.Now;
                    entityDTO.UpdatedUser = this.UserName;
                    _service.Update(entityDTO);
                    return Success(entityDTO.Id);
                }
            }
            catch (Exception e)
            {
                return Failure("",e.Message);
            }

        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("Delete")]
        public virtual RespondData Delete(Guid Id)
        {
            try
            {
                _service.Delete(Id);
                return Success(null);
            }
            catch (Exception e)
            {
                return Failure("",e.Message);
            }

        }
    }
}