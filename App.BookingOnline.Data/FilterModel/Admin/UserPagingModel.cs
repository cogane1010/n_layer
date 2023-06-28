
using App.Core.Domain;


namespace App.BookingOnline.Data.Paging
{
    public class UserPagingModel : PagingModel, IPagingModel
    {
        public string UserName { get; set; }
        public string Name { get; set; }
        public string OrgId { get; set; }

    }
}