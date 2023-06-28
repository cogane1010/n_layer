using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel.Common
{
    public class SmsHistoryFilterModel : PagingModel
    {
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }
        public string Mobilephone { get; set; }
    }
}
