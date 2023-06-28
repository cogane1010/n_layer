using App.Core.Domain;
using System;
using System.Collections.Generic;

namespace App.BookingOnline.Data.FilterModel
{
    public class TransactionFilterModel : PagingModel
    {
        public DateTime? FindDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public FtpConfiguration FptConfig { get; set; }

        // For logging
        public string FileName { get; set; }
        public string FilePath { get; set; }
        
        public byte[] FileContent { get; set; }
        public string HeaderId { get; set; }
        public Guid? OutTransHeaderId { get; set; }

        // gui email file out theo mot danh sach ngay
        public List<DateTime> EmailFilterDates { get; set; }
    }

    public class FtpConfiguration
    {
        public string Url { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class TransactionNotCompareFilter : PagingModel
    {

    }

    public class TransactionNotApproveFilter : PagingModel
    {

    }
}
