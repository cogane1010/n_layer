using App.Core.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.BookingOnline.Data.Paging
{
    public class MenuPagingModel : PagingModel, IPagingModel
    {
        public string Name { get; set; }
        public string ParentName { get; set; }

    }
}