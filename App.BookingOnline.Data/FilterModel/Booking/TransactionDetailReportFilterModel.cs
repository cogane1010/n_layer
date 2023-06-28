using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel
{
    public class TransactionDetailReportFilterModel : PagingModel
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
}
