
using App.Core.Repositories;
using App.BookingOnline.Data.Identity;

namespace App.BookingOnline.Data.Repositories
{
    public class UserRoleRepository : LinkRepository<AspUserRole>, IUserRoleRepository
    {
        public UserRoleRepository(BookingOnlineDbContext context)
            : base(context)
        { }


    }
}