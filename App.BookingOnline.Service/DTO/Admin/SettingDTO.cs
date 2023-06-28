using App.Core.Domain;
using System;

namespace App.BookingOnline.Service.DTO
{
    public class SettingDTO : IEntityDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
    }
}