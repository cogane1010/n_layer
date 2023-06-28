
using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Paging
{
    public class CustomerPagingModel : PagingModel, IPagingModel
    {
        public string CustomerCode { get; set; }
        public string Name { get; set; }
        public DateTime? DOB { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public string MemberCard { get; set; }
        public bool? IsActive { get; set; }
    }
}