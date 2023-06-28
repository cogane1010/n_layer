using System;

namespace App.Core.Domain
{
    public class PagingModel : IPagingModel
    {
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string UserName { get; set; }
        public string UserOrgId { get; set; }
        public string UserId { get; set; }
        public string Status { get; set; }
    }
}
