using App.BookingOnline.Data.Identity;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.BookingOnline.Data
{
    public interface IAppUserRepository : IBaseGridRepository<AppUser, UserPagingModel>
    {
        ValueTask<IEnumerable<AspRole>> GetRolesExample();
        Task<string> GetByPhoneForLoginAsync(string phoneNumber);
        Task<RespondData> ForgotPasswordAsync(string email);
        Task<RespondData> ChangePassword(string email, string password, string oldPassword);
        Task<RespondData> CreateAccount(Customer entity, string password);
        Task<RespondData> UpdateAccount(Customer entity);
    }
}