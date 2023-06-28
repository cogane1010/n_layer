
using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Paging
{
    public class MemberRequestPagingModel : PagingModel, IPagingModel
    {
        public string FullName { get; set; }
        public string MobilePhone { get; set; }
        public string Email { get; set; }
        public DateTime? Request_Date { get; set; }
        public Guid? C_Org_Id { get; set; }
        public string Request_FullName { get; set; }
        public string Request_MobilePhone { get; set; }
    }

  


}