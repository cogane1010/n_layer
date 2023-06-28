using App.BookingOnline.AppApi.Controllers;
using App.BookingOnline.Data.Paging;
using App.BookingOnline.MobileApi.ViewModel;
using App.BookingOnline.Service.DTO;
using App.BookingOnline.Service.IService.Admin;
using App.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog.Context;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace App.BookingOnline.MobileApi.Controllers.User
{
    public class AccountController : GridController<CustomerDTO, UserPagingModel, IAppUserService>
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger _log;

        public AccountController(IAppUserService service, IConfiguration configuration, ILogger<AccountController> logger) : base(service)
        {
            Configuration = configuration;
            _log = logger;
        }

        /// <summary>
        /// quên mật khẩu
        /// </summary>
        /// <param name="forgotPasswordModel"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("ForgotPassword")]
        public async Task<RespondData> ForgotPassword(ForgotPasswordViewModel model)
        {
            //TODO: check 10' an quen mat khua
            if (!string.IsNullOrEmpty(model.Email))
            {
                return await _service.ForgotPasswordAsync(model.Email);
            }
            return new RespondData { IsSuccess = false, MsgCode = "42" };
        }

        /// <summary>
        /// thay đổi mật khẩu
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("ChangePassword")]
        public async Task<RespondData> ChangePassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var username = UserName;
                return await _service.ChangePassword(username, model.Password, model.OldPassword);
            }
            return new RespondData { IsSuccess = false, MsgCode = "01" };
        }
         /// <summary>
        /// tạo tài khoản user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("Create")]
        public async Task<RespondData> CreateAccount(CustomerDTO model)
        {
            if (ModelState.IsValid)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogInformation(JsonConvert.SerializeObject(model));
                }
                if (!string.IsNullOrEmpty(model.OtpCode) && model.OtpId != Guid.Empty && model.OtpId != null)
                {
                    
                    if (!string.IsNullOrEmpty(model.MobilePhone))
                    {
                        var phone = RemoveWhitespace(model.MobilePhone);
                        model.MobilePhone = phone;
                    }

                    RespondData result = await _service.CreateAccount(model);
                    return result;
                }
                else
                {
                    //return _service.CheckCreateAccount(model);
                }
            }
            return new RespondData { IsSuccess = false, MsgCode = "02" };
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public string RemoveWhitespace(string input)
        {
            if (input == null)
                return null;
            return new string(input.ToCharArray()
                .Where(c => !Char.IsWhiteSpace(c))
                .ToArray());
        }


        /// <summary>
        /// sửa thông tin tài khoản
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Update")]
        public async Task<RespondData> UpdateAccount(CustomerDTO model)
        {
            using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
            {
                _log.LogInformation(JsonConvert.SerializeObject(model));
            }
            if (ModelState.IsValid)
            {
                model.UpdatedDate = DateTime.Now;
                model.UpdatedUser = UserId;

                RespondData result = await _service.UpdateAccount(model);

                return result;
            }
            return new RespondData { IsSuccess = false, MsgCode = "03" };
        }
    }
}
