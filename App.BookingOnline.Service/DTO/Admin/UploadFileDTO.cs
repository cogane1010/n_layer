using App.BookingOnline.Data.Models;
using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace App.BookingOnline.Service.DTO
{
    public class UploadFileDTO : IEntityDTO
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public int FileSize { get; set; }
        public string FileType { get; set; }
        public string FilePath { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }

       
    }

}