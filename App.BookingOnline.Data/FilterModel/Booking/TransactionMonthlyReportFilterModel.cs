using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel
{
    public class TransactionMonthlyReportFilterModel : PagingModel
    {
        public DateTime? FilterDate { get; set; }
    }
}
