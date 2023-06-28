using App.BookingOnline.Data.Models;
using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace App.BookingOnline.Data.Configurations
{
    public class UploadFileConfiguration : IEntityTypeConfiguration<UploadFile>
    {
        public void Configure(EntityTypeBuilder<UploadFile> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(m => m.FileName)
                .IsRequired()
                .HasMaxLength(500);
            builder
               .Property(m => m.FileType)
               .IsRequired()
               .HasMaxLength(50);

            builder
                .Property(m => m.FilePath)
                .IsRequired()
                .HasMaxLength(2000);
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
                .ToTable("App_UploadFile");

           
        }
    }
}