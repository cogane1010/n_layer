using App.BookingOnline.Data.Identity;
using App.BookingOnline.Data.Models;
using App.BookingOnline.Data.Paging;
using App.Core.Domain;
using App.Core.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App.BookingOnline.Data.Repositories
{
    public interface IRoleRepository : IGenericGridRepository<AspRole, RolePagingModel, string>
    {
        List<Menu> GetMenuPermisstion(Guid Id);
    }
}