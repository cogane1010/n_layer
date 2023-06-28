using App.BookingOnline.Data;
using App.BookingOnline.Data.MailService;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.BookingOnline.Service.Base;
using App.BookingOnline.Service.DTO;
using App.BookingOnline.Service.DTO.Common;
using App.Core.Configs;
using App.Core.Domain;
using App.Core.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace App.BookingOnline.Service.Service.Common
{
    public class HomeService : BaseGridDataService<HomeDTO, Customer, UserPagingModel, IHomeRepository>, IHomeService
    {
        private readonly IMailService _mailService;
        private readonly IAppUserRepository _appUserRepository;
        private readonly ILogger _log;
        private readonly IConfiguration _config;
        private string backendUrl;
        private string backendMobileUrl;

        public HomeService(IHomeRepository gridRepository, IAppUserRepository appUserRepository, IConfiguration config,
                            IMailService mailService, ILogger<HomeService> logger) : base(gridRepository)
        {
            _mailService = mailService;
            _appUserRepository = appUserRepository;
            _log = logger;
            _config = config;

            backendUrl = _config.GetSection("urlData").GetValue<string>("BackendUrl") + "/";
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                backendUrl = _config.GetSection("urlData").GetValue<string>("BackendUrlPro") + "/";
            }
            backendMobileUrl = _config.GetSection("urlData").GetValue<string>("SwaggerUrl");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
            {
                backendMobileUrl = _config.GetSection("urlData").GetValue<string>("SwaggerUrlPro");
            }
        }

        private string ConvertImageToBase64(string path)
        {
            var url = path.Replace("\\", "//");
            using (WebClient webClient = new WebClient())
            {
                try
                {
                    byte[] data = webClient.DownloadData(url);
                    if (data != null)
                    {
                        string base64String = Convert.ToBase64String(data);
                        return base64String;
                    }
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            return null;
        }


        public async Task<RespondData> GetHomeData(string userId, string lang, string userName)
        {

            var data = new RespondData();
            var homeData = new HomeDTO();
            try
            {
                var customer = await _gridRepository.GetCustomer(userId);
                var idCourse = new List<Guid>();
                if (customer != null)
                {
                    homeData.Customer = AutoMapperHelper.Map<Customer, CustomerDTO>(customer);
                    homeData.Customer.UserName = userName;
                    homeData.Customer.Full_Image_Url = backendMobileUrl + AppConfigs.UPLOAD_PATH + customer.Img_Url;
                    homeData.Customer.Full_Image_Url = homeData.Customer.Full_Image_Url.Replace("\\", "/");
                    //homeData.Customer.ImageData = ConvertImageToBase64(homeData.Customer.Full_Image_Url);
                    
                }
                
                data.IsSuccess = true;
                homeData.IsConnectSdk = _config.GetValue<bool>("IsConnectSdk");
                data.Data = homeData;
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                    _log.LogError(e.StackTrace);
                }
                data.Message = e.Message;
            }
            return data;
        }

        public async void SettingLanguage(string lang, string userId)
        {
            var customer = await _gridRepository.GetCustomer(userId);
            customer.Languague = lang;
            _gridRepository.SettingLanguage(customer);
        }

       

        public async void InsertFcmTokenDevice(string fcmToken, string lang, string userId)
        {
            var customer = await _gridRepository.GetCustomer(userId);
            if (!string.IsNullOrEmpty(fcmToken))
            {
                customer.FcmTokenDevice = fcmToken;
            }
            if (!string.IsNullOrEmpty(lang))
            {
                customer.Languague = lang;
            }
            customer.UpdatedUser = userId;
            customer.UpdatedDate = DateTime.Now;
            _gridRepository.InsertFcmTokenDevice(customer);
        }

        

        public string GetAppVersion(bool isAndroid)
        {
            if (isAndroid)
            {
                return _config.GetSection("versionApp").GetValue<string>("AndroidVersion");
            }
            else
            {
                return _config.GetSection("versionApp").GetValue<string>("IosVersion");
            }
        }

        public RespondData CheckVersionApp(int platform, string currVer, string userId)
        {
            //_gridRepository.UpdateAppUserVersion(currVer, userId);
            if (platform == 1)
            {
                var android = _config.GetSection("versionApp").GetValue<string>("AndroidVersion");
                var andPlit = android.Split('.');
                var curVerPlit = currVer.Split('.');
                if (Convert.ToInt32(andPlit[0]) == Convert.ToInt32(curVerPlit[0])
                    && Convert.ToInt32(andPlit[1]) == Convert.ToInt32(curVerPlit[1])
                    && Convert.ToInt32(andPlit[2]) > Convert.ToInt32(curVerPlit[2]))
                {
                    return new RespondData { IsSuccess = true, Data = 1 };
                }
                if (Convert.ToInt32(andPlit[0]) > Convert.ToInt32(curVerPlit[0])
                    || Convert.ToInt32(andPlit[1]) > Convert.ToInt32(curVerPlit[1]))
                {
                    return new RespondData { IsSuccess = true, Data = 2 };
                }
                return new RespondData { IsSuccess = true, Data = 0 };
            }
            if (platform == 0)
            {
                var ios = _config.GetSection("versionApp").GetValue<string>("IosVersion");
                var iosPlit = ios.Split('.');
                var curVerPlit = currVer.Split('.');
                if (iosPlit[0] == curVerPlit[0] && iosPlit[1] == curVerPlit[1]
                    && Convert.ToInt32(iosPlit[2]) > Convert.ToInt32(curVerPlit[2]))
                {
                    return new RespondData { IsSuccess = true, Data = 1 };
                }
                if (Convert.ToInt32(iosPlit[0]) > Convert.ToInt32(curVerPlit[0])
                    || Convert.ToInt32(iosPlit[1]) > Convert.ToInt32(curVerPlit[1]))
                {
                    return new RespondData { IsSuccess = true, Data = 2 };
                }
                return new RespondData { IsSuccess = true, Data = 0 };
            }
            return new RespondData { IsSuccess = true, Data = 0 };
        }

        public Task<RespondData> GetContactAllCourse()
        {
            throw new NotImplementedException();
        }

        public Task<RespondData> GetMemberCard(string userId)
        {
            throw new NotImplementedException();
        }

        public RespondData GetContactAllOrg()
        {
            throw new NotImplementedException();
        }

        public bool GetStatusMessageVnByUser(string userId)
        {
            throw new NotImplementedException();
        }

        public bool UpdateStatusMessageVnByUser(string userId, bool v)
        {
            throw new NotImplementedException();
        }

        public RespondData UpdateAppUserVersion(string version, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
