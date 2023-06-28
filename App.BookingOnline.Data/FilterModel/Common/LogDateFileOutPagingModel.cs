
using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Paging
{
    public class LogDateFileOutPagingModel : PagingModel, IPagingModel
    {
        public DateTime Date { get; set; }
        public string Status { get; set; }
    }
}