using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel
{
    public class BookingLineFilterModel : PagingModel
    {
        public DateTime? DateId { get; set; }
        public Guid? C_Org_Id { get; set; }
        public Guid? C_Course_Id { get; set; }
        public string Part { get; set; }
        public string Search { get; set; }
        public bool IsSearch { get; set; }
        public DateTime? Tee_Time { get; set; }
    }


    public class BookingStatisticFilterModel
    {
        public DateTime? DateId { get; set; }
        public Guid? C_Org_Id { get; set; }
    }
}
