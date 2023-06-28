using App.BookingOnline.Data.Models;
using App.Core.Domain;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.BookingOnline.Data.Identity
{
    public class AppUser : IdentityUser, IEntity
    {
        // Add additional profile data for application users by adding properties to this class
        public string Name { get; set; }
        public string Department { get; set; }
        public DateTime EnrolledDate { get; set; }
        [MaxLength(200)]
        public string FullName { get; set; }
        public DateTime? Dob { get; set; }
        public int LockStatus { get; set; }
        [MaxLength(2000)]
        public string Address { get; set; }
        [MaxLength(200)]
        public string State { get; set; }
        [MaxLength(200)]
        public string City { get; set; }
        public int? Gender { get; set; }
        public bool IsActive { get; set; }
        public bool IsForgotPass { get; set; }
        public bool IsMember { get; set; }
        public Guid? C_Org_Id { get; set; }
        public string FcmTokenDevice { get; set; }
        [MaxLength(20)]
        public string Language { get; set; }
        [MaxLength(4000)]
        public string Img_Url { get; set; }
        [MaxLength(200)]
        public string AvatarFileId { get; set; }

        [NotMapped]
        Guid IGenericEntity<Guid>.Id { get; set; }

    }

    public partial class UserLogin : IdentityUserLogin<string>
    {
    }
    public partial class AspUserRole : IdentityUserRole<string>
    {
    }

    public partial class AspRole : IdentityRole<string>, IGenericEntity<string>
    {
        public AspRole() : base()
        {
        }

        public AspRole(string roleName)
        {
            Name = roleName;
        }
        public string DisplayName { get; set; }
        public string CreatedUser { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UpdatedUser { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public bool Protected { get; set; }
        public string Description { get; set; }

        public IEnumerable<RoleMenu> RoleMenus { get; set; }

        [NotMapped]
        public bool HasRole { get; set; }


    }
    public partial class UserClaim : IdentityUserClaim<string>
    {
    }

    public partial class RoleClaim : IdentityRoleClaim<string>
    {
    }
    public partial class UserToken : IdentityUserToken<string>
    {
    }





}
