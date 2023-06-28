using App.BookingOnline.Data.Configurations;
using App.BookingOnline.Data.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace App.BookingOnline.Data
{
    public class BookingOnlineDbContext : IdentityDbContext<AppUser, AspRole, string, UserClaim, AspUserRole, UserLogin, RoleClaim, UserToken>
    {
        public BookingOnlineDbContext(DbContextOptions<BookingOnlineDbContext> options)
            : base(options)
        {
        }

        public BookingOnlineDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            //builder.Entity<IdentityUserRole<Guid>>().HasKey(p => new { p.UserId, p.RoleId });
            builder.Entity<AspRole>().Property(x => x.CreatedDate).HasDefaultValueSql("GETDATE()");
            builder.Entity<AspRole>().Property(x => x.CreatedUser).HasMaxLength(255);
            builder.Entity<AspRole>().Property(x => x.UpdatedUser).HasMaxLength(255);
            builder.Entity<AspRole>().Property(x => x.Description).HasMaxLength(255);
            builder.Entity<AspRole>().Property(x => x.DisplayName).HasMaxLength(255);

            #region admin
            builder
                .ApplyConfiguration(new MenuConfiguration());
            builder
                .ApplyConfiguration(new SettingConfiguration());
            //builder
            //    .ApplyConfiguration(new RoleConfiguration());
            builder
                .ApplyConfiguration(new RoleMenuConfiguration());
            //builder
            //   .ApplyConfiguration(new UserRoleConfiguration());

            builder
             .ApplyConfiguration(new UploadFileConfiguration());
            #endregion


            #region Seed Data Identity Roles  
            builder.Entity<AspRole>().HasData(new AspRole
            {
                Id = "672db3b8-c436-49bd-8172-bdb6ad6d6148",
                Name = Core.Constants.Admin,
                NormalizedName = Core.Constants.Admin.ToUpper(),
                DisplayName = "Admin",
                IsActive = true,
                Protected = true
            });
            builder.Entity<AspRole>().HasData(new AspRole
            {
                Id = "db29c853-03ea-4328-9553-83676192aeed",
                Name = Core.Constants.Employee,
                NormalizedName = Core.Constants.Employee.ToUpper(),
                DisplayName = "Nhân viên",
                IsActive = true,
                Protected = true
            });
            builder.Entity<AspRole>().HasData(new AspRole
            {
                Id = "3e1ce2a6-e835-41ff-ab54-11dc1e60e839",
                Name = Core.Constants.Customer,
                NormalizedName = Core.Constants.Customer.ToUpper(),
                DisplayName = "Khách hàng",
                IsActive = true,
                Protected = true
            });
            #endregion
        }
    }
}
