using App.BookingOnline.Data.Models;
using App.Core.Repositories;

namespace App.BookingOnline.Data.Repositories
{
    public class RoleMenuRepository : Repository<RoleMenu>, IRoleMenuRepository
    {
        public RoleMenuRepository(BookingOnlineDbContext context)
            : base(context)
        { }

    }
}