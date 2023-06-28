using App.BookingOnline.Data.Paging;
using App.BookingOnline.Service.Base;
using App.BookingOnline.Service.DTO;
using App.Core.Domain;
using System.Threading.Tasks;

namespace App.BookingOnline.Service.IService.Admin
{
    public interface IAppUserService : IBaseGridDataService<CustomerDTO, UserPagingModel>
    {
        Task<string> GetByPhoneAsync(string phoneNumber);
        Task<RespondData> ForgotPasswordAsync(string email);
        Task<RespondData> ChangePassword(string email, string password, string oldPassword);
        Task<RespondData> CreateAccount(CustomerDTO model);
        Task<RespondData> UpdateAccount(CustomerDTO model);
      
    }
}