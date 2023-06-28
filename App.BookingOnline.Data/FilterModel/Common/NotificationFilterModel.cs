using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel.Common
{
    public class NotificationFilterModel : PagingModel
    {
        public string Code { get; set; }
        public string Messange_title { get; set; }
        public int? Status { get; set; }
        public string SendUser { get; set; }
        public DateTime? SendDate { get; set; }
    }
}
