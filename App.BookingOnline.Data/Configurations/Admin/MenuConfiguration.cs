using App.BookingOnline.Data.Models;
using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace App.BookingOnline.Data.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<Menu>
    {
        public void Configure(EntityTypeBuilder<Menu> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(m => m.Name)
                .IsRequired().HasMaxLength(250);

            builder
                .Property(m => m.TranslateKey)
                .HasMaxLength(250);
            builder
                .Property(m => m.Icon)
                .HasMaxLength(250);

            builder
               .Property(m => m.Url)
               .HasMaxLength(500);

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
                .ToTable("App_Menu");

            builder.HasData(new Menu
            {
                Id = new Guid("31f946a1-56cc-465d-a10d-20aae1d4f34e"),
                Name = "ROOT",
                Level = -1,
                CreatedUser = "admin",
                IsActive = true,
                DisplayOrder = -1,
            });
        }
    }
}