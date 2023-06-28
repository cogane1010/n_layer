using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.BookingOnline.Data.Models
{
    public class UploadFile : BaseEntity, IEntity
    {
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
    }
}