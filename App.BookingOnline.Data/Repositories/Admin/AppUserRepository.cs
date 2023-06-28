using App.BookingOnline.Data.Identity;
using App.BookingOnline.Data.MailService;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core;
using App.Core.Domain;
using App.Core.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static App.Core.Enums;

namespace App.BookingOnline.Data.Repositories.Common
{
    public class AppUserRepository : BaseGridRepository<Customer, UserPagingModel>, IAppUserRepository
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
        public IConfiguration Configuration { get; }

        public AppUserRepository(IUnitOfWork unitOfWork, IMailService mailService, UserManager<AppUser> userManager,
                ILogger<AppUserRepository> logger, IConfiguration configuration) : base(unitOfWork)
        {
            _log = logger;
            _mailService = mailService;
            _userManager = userManager;
            _roleRepo = _unitOfWork.GetDataRepository<AspRole>();
            _userRepo = _unitOfWork.GetDataRepository<AppUser>();
            _userInRoleRepo = _unitOfWork.GetDataRepository<AspUserRole>();
            _userClaimRepo = _unitOfWork.GetDataRepository<UserClaim>();
            _userLoginRepo = _unitOfWork.GetDataRepository<UserLogin>();
            _customerRepo = _unitOfWork.GetDataRepository<Customer>();
            Configuration = configuration;
        }

        public async Task<RespondData> ChangePassword(string name, string password, string oldPassword)
        {
            var user = await _userManager.FindByNameAsync(name);
            if (user == null)
            {
                return new RespondData { IsSuccess = false, MsgCode = "04" };
            }
            var checkPass = await _userManager.CheckPasswordAsync(user, oldPassword);
            if (!checkPass)
            {
                return new RespondData { IsSuccess = false, MsgCode = "35" };
            }
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, password);
            if (result.Succeeded)
            {
                user.IsForgotPass = false;
                await _userManager.UpdateAsync(user);
                _unitOfWork.SaveChanges();
                return new RespondData { IsSuccess = true, MsgCode = "05" };
            }
            else
            {
                return new RespondData { IsSuccess = false, MsgCode = "12" };
            }
        }

        public RespondData CheckCreateAccount(string Email, string PhoneNumber)
        {
            var checkEmail = _userRepo.SelectWhere(x => x.Email.Trim() == Email.Trim());
            if (checkEmail.Any())
            {
                return new RespondData { IsSuccess = false, MsgCode = "06" };
            }
            var checkPhone = _userRepo.SelectWhere(x => x.PhoneNumber == PhoneNumber);
            if (checkPhone.Any())
            {
                return new RespondData { IsSuccess = false, MsgCode = "07" };
            }

            return new RespondData { IsSuccess = true };
        }
        public async Task<RespondData> CreateAccount(Customer entity, string password)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var checkEmail = _userRepo.SelectWhere(x => x.Email == entity.Email);
                    if (checkEmail.Any())
                    {
                        return new RespondData { IsSuccess = false, MsgCode = "06" };
                    }
                    var checkPhone = _userRepo.SelectWhere(x => x.PhoneNumber == entity.MobilePhone);
                    if (checkPhone.Any())
                    {
                        return new RespondData { IsSuccess = false, MsgCode = "07" };
                    }
                    var user = new AppUser();
                    user.Name = entity.FullName;
                    user.UserName = entity.CustomerCode;
                    user.FullName = entity.FullName;
                    user.Email = entity.Email;
                    user.PhoneNumber = entity.MobilePhone.Trim();
                    user.PhoneNumber = Regex.Replace(user.PhoneNumber, @"\s", "");
                    if (entity.DOB.HasValue)
                    {
                        user.Dob = entity.DOB.Value;
                    }                    
                    user.EnrolledDate = DateTime.Now;

                    var data = await _userManager.CreateAsync(user, password);
                    if (data.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, Constants.Customer);
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("userName", user.UserName));
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("name", user.Name));
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("email", user.Email));
                        await _userManager.AddClaimAsync(user, new System.Security.Claims.Claim("role", Constants.Customer));

                        _unitOfWork.SaveChanges();
                        entity.CreatedDate = DateTime.Now;
                        var userId = await _userManager.FindByEmailAsync(user.Email);
                        entity.CreatedUser = userId.Id;
                        entity.UserId = userId.Id;
                        entity.LowerFullName = entity.FullName.ToLower();                    
                        entity.IsActive = true;
                        var result = await _customerRepo.AddAsync(entity);
                        if (result.Id != null || result.Id != Guid.Empty)
                        {
                            _unitOfWork.SaveChanges();
                            transaction.Commit();
                            return new RespondData { IsSuccess = true, MsgCode = "08" };
                        }
                    }
                    transaction.Rollback();
                    return new RespondData { IsSuccess = false, Message = data.Errors.FirstOrDefault()?.Code };
                }
                catch (Exception e)
                {
                    using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                    {
                        _log.LogError(e.Message);
                        _log.LogError(e.StackTrace);
                    }
                    transaction.Rollback();
                    return new RespondData { IsSuccess = false, MsgCode = "09" };
                }
            }
        }

        public override Customer SingleOrDefault(Guid id)
        {
            var cus = _repo.SingleOrDefault(x => x.UserId == id.ToString());
            return cus;
        }

        public RespondData GetAccountInfo(string userId)
        {
            throw new NotImplementedException();
        }
        public async Task<RespondData> UpdateAccount(Customer entity)
        {
            using (var transaction = _unitOfWork.BeginTransaction())
            {
                try
                {
                    var checkEmail = _userRepo.SelectWhere(x => x.Email == entity.Email);
                    if (checkEmail.Count() > 1)
                    {
                        return new RespondData { IsSuccess = false, MsgCode = "06" };
                    }
                    var checkPhone = _userRepo.SelectWhere(x => x.PhoneNumber == entity.MobilePhone);
                    if (checkPhone.Count() > 1)
                    {
                        return new RespondData { IsSuccess = false, MsgCode = "07" };
                    }

                    var customer = _customerRepo.SelectWhereNoTracking(x => x.Id == entity.Id).FirstOrDefault();
                    if (customer != null)
                    {
                        customer.FullName = entity.FullName;
                        customer.LowerFullName = entity.FullName.ToLower();
                        customer.Email = entity.Email;
                        customer.DOB = entity.DOB;
                        customer.Gender = entity.Gender;
                        customer.MobilePhone = entity.MobilePhone;
                        customer.Img_Url = entity.Img_Url;

                        _customerRepo.Update(customer);

                        var user = await _userManager.FindByIdAsync(customer.UserId);
                        if (user != null)
                        {
                            user.PhoneNumber = entity.MobilePhone;
                            user.FullName = entity.FullName;
                            user.Email = entity.Email;
                            await _userManager.UpdateAsync(user);
                            _unitOfWork.SaveChanges();
                            transaction.Commit();
                            return new RespondData { IsSuccess = true, MsgCode = "08" };
                        }
                    }
                    transaction.Rollback();
                    return new RespondData { IsSuccess = false, MsgCode = "10" };
                }
                catch (Exception e)
                {
                    using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                    {
                        _log.LogError(e.Message);
                        _log.LogError(e.StackTrace);
                    }
                    transaction.Rollback();
                    return new RespondData { IsSuccess = false, MsgCode = "09" };
                }
            }
        }

        public async Task<RespondData> ForgotPasswordAsync(string email)
        {
            var errorMessage = "12";
            try
            {
                var cust = GetByPhoneAsync(email);
                if (cust == null)
                {
                    return new RespondData { IsSuccess = false, MsgCode = "10" };
                }
                var user = await _userManager.FindByEmailAsync(cust.Email);
                if (user == null)
                {
                    user = await _userManager.FindByEmailAsync(cust.Email);
                }

                if (user == null)
                {
                    return new RespondData { IsSuccess = false, MsgCode = "10" };
                }

                if (user.LockoutEnd != null && user.LockoutEnd.HasValue)
                {
                    if (user.LockStatus == (int)AccountStatus.moreNoShow)
                    {
                        return new RespondData { IsSuccess = false, MsgCode = "70" };
                    }
                    if (user.LockStatus == (int)AccountStatus.locked)
                    {
                        return new RespondData { IsSuccess = false, MsgCode = "71" };
                    }
                    return new RespondData { IsSuccess = false, MsgCode = "40" };
                }

               

                var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
                var specChars = "0123456789";
                var stringChars = new char[8];
                var specStringChars = new char[1];
                var random = new Random();
                for (int i = 0; i < stringChars.Length; i++)
                {
                    stringChars[i] = chars[random.Next(chars.Length)];
                }
                specStringChars[0] = specChars[random.Next(specChars.Length)];

                var newPass = new String(stringChars) + new String(specStringChars);
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetPassResult = await _userManager.ResetPasswordAsync(user, token, newPass);
                if (!resetPassResult.Succeeded)
                {
                    return new RespondData { IsSuccess = false, Message = errorMessage };
                }
                var subject = "Thay đổi mật khẩu APP Golf Booking";
                var content = string.Format("Mật khẩu mới: {0}", newPass);
                await _mailService.SendMailAsync(user.Email, "", "", subject, content, user.UserName);

                user.IsForgotPass = true;
                await _userManager.UpdateAsync(user);
                _unitOfWork.SaveChanges();
                return new RespondData { IsSuccess = true, MsgCode = "13" };
            }
            catch (Exception e)
            {
                using (LogContext.PushProperty("MethodName", System.Reflection.MethodBase.GetCurrentMethod().Name))
                {
                    _log.LogError(e.Message);
                    _log.LogError(e.StackTrace);
                }
                return new RespondData { IsSuccess = false, MsgCode = "10" };
            }
        }

        private Customer GetByPhoneAsync(string telephone)
        {
            var user = _customerRepo.SelectWhere(x => x.MobilePhone == telephone || x.Email.ToLower() == telephone.ToLower()).FirstOrDefault();
            return user;
        }

        public async Task<string> GetByPhoneForLoginAsync(string phoneNumber)
        {
            var user = _customerRepo.SelectWhereNoTracking(x => x.MobilePhone == phoneNumber).FirstOrDefault();
            //var checkPass = await _userManager.CheckPasswordAsync(user, "Brg@123456");
            return user?.UserId;
        }

        public async ValueTask<IEnumerable<AspRole>> GetRolesExample()
        {
            try
            {
                var user = _userRepo.GetAllAsync().Result;
                var userInRole = _userInRoleRepo.GetAllAsync().Result;
                var roles = _roleRepo.GetAllAsync().Result;
                var userClaim = _userClaimRepo.GetAllAsync().Result;
                var userLogin = _userLoginRepo.GetAllAsync().Result;
            }
            catch (Exception e)
            {

            }


            return await _roleRepo.GetAllAsync();
        }

        public RespondData GetInfoMemberCard(string userId)
        {
            throw new NotImplementedException();
        }

        public RespondData ListMemberCard(string userId)
        {
            throw new NotImplementedException();
        }

        public Customer GetCustomerByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Customer> GetAllCustomer()
        {
            throw new NotImplementedException();
        }

        public AppUser GetUserByEmailOrPhone(string mobilePhone, string email)
        {
            throw new NotImplementedException();
        }

        public bool CheckMemberCardByUser(string golf_CardNo, string orgCode)
        {
            throw new NotImplementedException();
        }
    }
}
