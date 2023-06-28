using App.Core.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.BookingOnline.Data.Paging
{
    public class SettingPagingModel : PagingModel, IPagingModel
    {
        public string Code { get; set; }
    }
}