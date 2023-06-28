
using App.Core.Domain;


namespace App.BookingOnline.Data.Paging
{
    public class HotlinePagingModel : PagingModel, IPagingModel
    {
        public string PhoneNumber { get; set; }
    }
}