using App.BookingOnline.Data.Identity;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.BookingOnline.Data.Repositories
{
    public interface IUserRoleRepository : ILinkRepository<AspUserRole>
    {
    }
}