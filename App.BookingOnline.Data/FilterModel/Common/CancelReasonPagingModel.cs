
using App.Core.Domain;


namespace App.BookingOnline.Data.Paging
{
    public class CancelReasonPagingModel : PagingModel, IPagingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    
    
}