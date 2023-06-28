using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel.Common
{
    public class PromotionFilterModel : PagingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid? CourseId { get; set; }
        public Guid? PromotionCustomerGroupId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsHot { get; set; }
        public string PromotionType { get; set; }
    }
}
