using App.BookingOnline.Data.Identity;
using App.BookingOnline.Data.MailService;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace App.BookingOnline.Data.Repositories.Common
{
    public class HomeRepository : BaseGridRepository<Customer, UserPagingModel>, IHomeRepository
    {
        private readonly IBaseRepository<AspRole> _roleRepo;
        private readonly IBaseRepository<AppUser> _userRepo;
        private readonly IBaseRepository<AspUserRole> _userInRoleRepo;
        private readonly IBaseRepository<UserClaim> _userClaimRepo;
        private readonly IBaseRepository<UserLogin> _userLoginRepo;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMailService _mailService;
        private readonly IBaseRepository<Customer> _customerRepo;
        private readonly ILogger _log;
        private readonly IConfiguration _config;

        public HomeRepository(IUnitOfWork unitOfWork, IMailService mailService, IConfiguration config, UserManager<AppUser> userManager
            , ILogger<HomeRepository> logger) : base(unitOfWork)
        {
            _mailService = mailService;
            _userManager = userManager;
            _roleRepo = _unitOfWork.GetDataRepository<AspRole>();
            _userRepo = _unitOfWork.GetDataRepository<AppUser>();
            _userInRoleRepo = _unitOfWork.GetDataRepository<AspUserRole>();
            _userClaimRepo = _unitOfWork.GetDataRepository<UserClaim>();
            _userLoginRepo = _unitOfWork.GetDataRepository<UserLogin>();
            _customerRepo = _unitOfWork.GetDataRepository<Customer>();
            _log = logger;
            _config = config;
        }

        public async Task<Customer> GetCustomer(string userId)
        {
            return _repo.SelectWhereNoTracking(x => x.UserId == userId).FirstOrDefault();
        }

        public void SettingLanguage(Customer customer)
        {
            try
            {
                _customerRepo.UpdateByProperties(customer, new List<Expression<Func<Customer, object>>>()
                                {
                                    x => x.Languague,
                                    x => x.UpdatedDate,
                                    x => x.UpdatedUser
                                });
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                }
            }
        }

        public void InsertFcmTokenDevice(Customer customer)
        {
            try
            {
                _customerRepo.UpdateByProperties(customer, new List<Expression<Func<Customer, object>>>()
                                {
                                    x => x.FcmTokenDevice,
                                    x => x.Languague,
                                    x => x.UpdatedDate,
                                    x => x.UpdatedUser
                                });
                _unitOfWork.SaveChanges();
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                }
            }
        }


        public bool GetStatusMessageVnByUser(string userId)
        {
            var cust = _customerRepo.SelectWhere(x => x.UserId == userId).FirstOrDefault();
            if (cust != null)
            {
                return cust.IsUpdateErrCode ? cust.IsUpdateErrCode : false;
            }
            else
            {
                return false;
            }
        }

        public bool UpdateStatusMessageVnByUser(string userId, bool v)
        {
            var cust = _customerRepo.SelectWhere(x => x.UserId == userId).FirstOrDefault();
            if (cust != null)
            {
                cust.IsUpdateErrCode = v;
            }
            _customerRepo.UpdateByProperties(cust, new List<Expression<Func<Customer, object>>>()
                                {
                                    x => x.IsUpdateErrCode
                                });
            _unitOfWork.SaveChanges();
            return true;
        }

        public RespondData UpdateAppUserVersion(string version, string userId)
        {
            try
            {
                var cust = _customerRepo.SelectWhere(x => x.UserId == userId).FirstOrDefault();
                if (cust != null)
                {
                    cust.Appversion = version;
                    _customerRepo.UpdateByProperties(cust, new List<Expression<Func<Customer, object>>>()
                                {
                                    x => x.Appversion
                                });
                    _unitOfWork.SaveChanges();
                }
            }
            catch(Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                    _log.LogError(e.StackTrace);
                }
            }                 
            return new RespondData { IsSuccess = true };
        }

      
    }
}
