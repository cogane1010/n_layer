using App.Core.Domain;
using System;

namespace App.BookingOnline.Service.DTO.Common
{
    public class HomeDTO : IEntityDTO
    {
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public bool IsConnectSdk { get; set; } = false;
        public DateTime? UpdatedDate { get; set; }
        public Guid Id { get; set; }
        public CustomerDTO Customer { get; set; }
    }
}
