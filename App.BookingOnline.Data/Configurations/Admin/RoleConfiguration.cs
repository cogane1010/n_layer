using App.BookingOnline.Data.Models;
using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace App.BookingOnline.Data.Configurations
{
    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(250);

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
                .Property(m => m.Description)
                .HasMaxLength(500);
            builder
                .ToTable("App_Role");


            builder.HasData(new Role
            {
                Id = new Guid("86c462e8-548a-45be-9142-d42f3c579b8c"),
                Name = "Admin",
                IsActive = true,
                CreatedUser= "admin"
            });
        }
    }
}