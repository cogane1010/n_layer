
using App.Core.Domain;


namespace App.BookingOnline.Data.Paging
{
    public class OrganizationTypePagingModel : PagingModel, IPagingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class OrganizationPagingModel : PagingModel, IPagingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class OrganizationInfoPagingModel : PagingModel, IPagingModel
    {

    }
}