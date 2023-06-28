using App.BookingOnline.Data.Identity;
using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.BookingOnline.Data.Models
{
    public class Role : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public IEnumerable<RoleMenu> RoleMenus { get; set; }
        //public IEnumerable<UserRole> UserRoles { get; set; }
        [NotMapped]
        public bool HasRole { get; set; }

    }

    public class RoleMenu : BaseEntity, IEntity
    {
        public string AspRoleId { get; set; }
        public Guid MenuId { get; set; }

        public AspRole Role { get; set; }
        public Menu Menu { get; set; }
    }
}