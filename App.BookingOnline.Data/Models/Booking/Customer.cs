using App.Core.Domain;
using System;

namespace App.BookingOnline.Data.Models
{
    public class Customer : BaseEntity, IEntity
    {
        public string CustomerCode { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string LowerFullName { get; set; }
        public string Email { get; set; }
        public string MobilePhone { get; set; }
        public DateTime? DOB { get; set; }
        public int? Gender { get; set; }
        public bool IsMember { get; set; }
        public DateTime? VoucherDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsUpdateErrCode { get; set; }
        public string UserId { get; set; }
        public string FcmTokenDevice { get; set; }
        public string TokenResourseCode { get; set; }
        public string Languague { get; set; }
        public string Appversion { get; set; }
        public string VnVersion { get; set; }
        public string EnVersion { get; set; }
        public Guid? AvatarFileId { get; set; }
        public string Img_Url { get; set; }

      

    }


}