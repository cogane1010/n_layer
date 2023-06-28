using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.BookingOnline.Data
{
    public interface IHomeRepository : IBaseGridRepository<Customer, UserPagingModel>
    {
        Task<Customer> GetCustomer(string userId);
        void SettingLanguage(Customer customer);
        void InsertFcmTokenDevice(Customer customer);      
    }
}