using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.FilterModel.Common
{
    public class CourseTemplateFilterModel : PagingModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public Guid? OrgId { get; set; }
        public bool? Status { get; set; }
    }
}
