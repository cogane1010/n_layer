using App.BookingOnline.Data;
using App.BookingOnline.Data.Identity;
using App.BookingOnline.Data.MailService;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.BookingOnline.Service.Base;
using App.BookingOnline.Service.DTO;
using App.BookingOnline.Service.IService.Admin;
using App.Core.Configs;
using App.Core.Domain;
using App.Core.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.IO;
using System.Threading.Tasks;

namespace App.BookingOnline.Service.Service.Admin
{
    public class AppUserService : BaseGridDataService<CustomerDTO, AppUser, UserPagingModel, IAppUserRepository>
        , IAppUserService
    {
        private readonly IMailService _mailService;
        private readonly ILogger _log;
        public IConfiguration Configuration { get; }
        private string backendMobileUrl;

        public AppUserService(IAppUserRepository gridRepository, IMailService mailService, IConfiguration configuration,
            ILogger<AppUserService> logger) : base(gridRepository)
        {
            _mailService = mailService;
            _log = logger;
            Configuration = configuration;
            backendMobileUrl = Configuration.GetSection("urlData").GetValue<string>("SwaggerUrl");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                backendMobileUrl = Configuration.GetSection("urlData").GetValue<string>("SwaggerUrlPro");
            }
        }

        public async Task<RespondData> ChangePassword(string email, string password, string oldPassword)
        {
            return await _gridRepository.ChangePassword(email, password, oldPassword);
        }

        public async Task<RespondData> CreateAccount(CustomerDTO model)
        {
            try
            {
                var folderName = string.Empty;
                var data = string.Empty;
                var pathToSave = string.Empty;

                var entity = AutoMapperHelper.Map<CustomerDTO, Customer>(model);
                entity.IsUpdateErrCode = true;

                if (!string.IsNullOrEmpty(model.ImageData))
                {
                    var imageData = model.ImageData.Split(';');
                    var nameExtenstion = imageData[0].Split('/')[1];
                    var fileName = "\\" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff") + "." + nameExtenstion;
                    string fileUrl = Configuration.GetSection("fileUpload").GetValue<string>("fileUrl");
                    if (!Directory.Exists(fileUrl))
                    {
                        Directory.CreateDirectory(fileUrl);
                    }
                    pathToSave = fileUrl + fileName;
                    entity.Img_Url = fileName;
                    data = imageData[1];
                }

                var result = await _gridRepository.CreateAccount(entity, model.Password);

                if (result != null && data != null && !string.IsNullOrEmpty(data)
                    && !string.IsNullOrEmpty(model.ImageData))
                {
                    var newBytes = Convert.FromBase64String(data.Split(',')[1]);
                    File.WriteAllBytes(pathToSave, newBytes);
                }
                return result;
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                    _log.LogError(e.StackTrace);
                }
                return new RespondData { IsSuccess = false, MsgCode = "09" };
            }
        }

        public RespondData GetAccountInfo(string userId)
        {
            var customer = new CustomerDTO();
            
            customer.Full_Image_Url = backendMobileUrl + "\\" + AppConfigs.UPLOAD_PATH + customer.Img_Url;
            

            return new RespondData { IsSuccess = true, Data = customer };
        }
        public async Task<RespondData> UpdateAccount(CustomerDTO model)
        {
            try
            {
                var fileName = string.Empty;
                var data = string.Empty;
                var pathToSave = string.Empty;
                if (!string.IsNullOrEmpty(model.ImageData))
                {
                    var imageData = model.ImageData.Split(';');
                    var nameExtenstion = imageData[0].Split('/')[1];
                    data = imageData[1];
                    if (string.IsNullOrEmpty(model.Img_Url))
                    {
                        fileName = "\\" + DateTime.Now.ToString("dd-MM-yyyy-HH-mm-ss-fff") + "." + nameExtenstion;
                        string fileUrl = Configuration.GetSection("fileUpload").GetValue<string>("fileUrl").ToString();
                        if (!Directory.Exists(fileUrl))
                        {
                            Directory.CreateDirectory(fileUrl);
                        }
                        pathToSave = fileUrl + fileName;

                    }
                    else
                    {
                        fileName = model.Img_Url;
                    }
                }
                else
                {
                    fileName = model.Img_Url;
                }

                var entity = AutoMapperHelper.Map<CustomerDTO, Customer>(model);

                entity.Img_Url = fileName;
                var result = await _gridRepository.UpdateAccount(entity);

                if (!string.IsNullOrEmpty(model.ImageData) && data != null && !string.IsNullOrEmpty(data))
                {
                    var newBytes = Convert.FromBase64String(data.Split(',')[1]);
                    File.WriteAllBytes(pathToSave, newBytes);
                }
                return result;
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                    _log.LogError(e.StackTrace);
                }
                return new RespondData { IsSuccess = false, MsgCode = "09" };
            }
        }

        public async Task<RespondData> ForgotPasswordAsync(string email)
        {
            var result = await _gridRepository.ForgotPasswordAsync(email);
            return result;
        }

        public async Task<string> GetByPhoneAsync(string phoneNumber)
        {
            var result = await _gridRepository.GetByPhoneForLoginAsync(phoneNumber);
            return result;
        }

        
    }
}
