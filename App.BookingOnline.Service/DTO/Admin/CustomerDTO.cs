using App.Core.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace App.BookingOnline.Service.DTO
{
    public class CustomerDTO : IEntityDTO
    {
        public Guid Id { get; set; }
        public string CustomerCode { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string LowerFullName { get; set; }
        public string Email { get; set; }
        public string Total_Golf_CardNo { get; set; }
        public string Total_Course { get; set; }
        public string Total_Org { get; set; }
        public IEnumerable<string> Total_Courseaa { get; set; }
        public string MobilePhone { get; set; }
        public Guid? OtpId { get; set; }
        public string OtpCode { get; set; }
        public bool? IsSendOtp { get; set; } = false;
        public DateTime? DOB { get; set; }
        public int? Gender { get; set; }
        public bool IsMember { get; set; }
        public bool IsActive { get; set; }
        public int? StausInt { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UserId { get; set; }
        public Guid? RegisterCourse { get; set; }
        public Guid? AvatarFileId { get; set; }
        public string Img_Url { get; set; }
        public string Full_Image_Url { get; set; }
        public string ImageData { get; set; }
        public string Appversion { get; set; }
        public string VnVersion { get; set; }
        public string EnVersion { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        public string Golf_CardNo { get; set; }
    }

}