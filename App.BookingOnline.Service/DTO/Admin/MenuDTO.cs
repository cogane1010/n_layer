using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.BookingOnline.Service.DTO
{
    public class MenuDTO : IEntityDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string TranslateKey { get; set; }
        public Guid? ParentId { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int? DisplayOrder { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string ParentName { get; set; }
        public int Level { get; set; }
        public IEnumerable<MenuDTO> Sub { get; set; }
        public bool HasMenu { get; set; }
    }
}