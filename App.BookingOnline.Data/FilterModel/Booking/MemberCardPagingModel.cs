
using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Paging
{
    public class MemberCardPagingModel : PagingModel, IPagingModel
    {
        public string Golf_FullName { get; set; }
        public string Golf_CardNo { get; set; }
        public Guid? C_Org_Id { get; set; }
        public string OrgCode { get; set; }
    }

    public class MemberCardCoursePagingModel : PagingModel, IPagingModel
    {

    }



}