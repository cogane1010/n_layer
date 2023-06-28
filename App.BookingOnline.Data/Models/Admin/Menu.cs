using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.BookingOnline.Data.Models
{
    public class Menu : BaseEntity, IEntity
    {
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string TranslateKey { get; set; }
        public Guid? ParentId { get; set; }
        public string Url { get; set; }
        public string Icon { get; set; }
        public int? DisplayOrder { get; set; }
        
        public int Level { get; set; }

        public IEnumerable<RoleMenu> RoleMenus { get; set; }

        [NotMapped]
        public string ParentName { get; set; }
        [NotMapped]
        public IEnumerable<Menu> Sub { get; set; }
        [NotMapped]
        public bool HasMenu { get; set; }
    }
}