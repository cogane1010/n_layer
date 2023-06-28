using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.BookingOnline.Data.Models
{
    public class Setting : BaseEntity, IEntity
    {
        public string Code { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}