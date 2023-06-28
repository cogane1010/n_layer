using App.BookingOnline.Data.Models;
using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.BookingOnline.Data.Configurations
{
    public class RoleMenuConfiguration : IEntityTypeConfiguration<RoleMenu>
    {
        public void Configure(EntityTypeBuilder<RoleMenu> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");
            builder.HasOne(x => x.Role)
                .WithMany(x => x.RoleMenus)
                .HasForeignKey(x => x.AspRoleId);
            builder.HasOne(x => x.Menu)
               .WithMany(x => x.RoleMenus)
               .HasForeignKey(x => x.MenuId);

            builder
                .Property(m => m.CreatedDate)
                .IsRequired().HasDefaultValueSql("GETDATE()");

            builder
                .Property(m => m.CreatedUser)
                .HasMaxLength(250);
            builder
                .Property(m => m.UpdatedUser)
                .HasMaxLength(250);
            builder
                .ToTable("App_Role_Menu_Ref");
        }
    }
}