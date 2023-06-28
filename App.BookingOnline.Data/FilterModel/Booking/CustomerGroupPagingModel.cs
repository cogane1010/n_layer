
using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Paging
{
    public class CustomerGroupPagingModel : PagingModel, IPagingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool? IsActive { get; set; }

        public Guid? C_Org_Id { get; set; }
    }

    public class CustomerGroupMappingPagingModel : PagingModel, IPagingModel
    {

    }



}