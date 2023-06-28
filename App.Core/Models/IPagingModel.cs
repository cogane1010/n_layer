using System;

namespace App.Core.Domain
{
    public interface IPagingModel
    {
        int PageIndex { get; set; }
        int PageSize { get; set; }
        string UserName { get; set; }
        string UserId { get; set; }
        string UserOrgId { get; set; }
    }
}
