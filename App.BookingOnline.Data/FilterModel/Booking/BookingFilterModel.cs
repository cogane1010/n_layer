using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel
{
    public class BookingFilterModel : PagingModel
    {
        public Guid? BookingId { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime? TimeTo { get; set; }        
        public string PhoneNumber { get; set; }
        public string Mobilephone { get; set; }
        public string Fullname { get; set; }
        public string Email { get; set; }
        public int? NumberPlayers { get; set; }
        public string Status { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? BookingDate { get; set; }
        public DateTime? CreatedFrom { get; set; }
        public DateTime? CreatedTo { get; set; }
        public DateTime? BookingTo { get; set; }
        public DateTime? BookingFrom { get; set; }
        public string Teetime { get; set; }
        public Guid? C_Course_Id { get; set; }
        public Guid? C_Org_Id { get; set; }
        public string BookingCode { get; set; }
        public string CardNo { get; set; }
        public string CustomerCode { get; set; }
        public bool IsExcelExport { get; set; }
    }
}
