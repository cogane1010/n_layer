using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.BookingOnline.Service.DTO
{
    public class RoleDTO : IEntityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }

        public List<MenuDTO> TreeMenuPermission { get; set; }
        public bool HasRole { get; set; }
        public bool Selected { get; set; }
    }


    public class AspRoleDTO : IGenericEntityDTO<string>
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public string DisplayName { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        
        public bool Protected { get; set; }
        public string NormalizedName { get; set; }
        public List<MenuDTO> TreeMenuPermission { get; set; }
        public bool HasRole { get; set; }
    }
}