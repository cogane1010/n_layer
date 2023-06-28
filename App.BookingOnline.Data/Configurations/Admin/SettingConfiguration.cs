using App.BookingOnline.Data.Models;
using App.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace App.BookingOnline.Data.Configurations
{
    public class SettingConfiguration : IEntityTypeConfiguration<Setting>
    {
        public void Configure(EntityTypeBuilder<Setting> builder)
        {
            builder
                .HasKey(m => m.Id);

            builder
                .Property(m => m.Id)
                .HasDefaultValueSql("NEWID()");

            builder
                .Property(m => m.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder
              .Property(m => m.Value)
              .HasMaxLength(5000);

            builder
              .Property(m => m.Description)
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
                .ToTable("App_Setting");
        }
    }
}