
using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Paging
{
    public class TeeSheetLockPagingModel : PagingModel, IPagingModel
    {
        public Guid? C_Org_Id { get; set; }
        public Guid? C_LockReason_Id { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

    }


    public class TeeSheetLockLinePagingModel : PagingModel, IPagingModel
    {
        public Guid? C_Org_Id { get; set; }
    }


}