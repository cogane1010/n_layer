
using App.Core.Domain;


namespace App.BookingOnline.Data.Paging
{
    public class ContactSupportPagingModel : PagingModel, IPagingModel
    {
        public string CourseCode { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

}