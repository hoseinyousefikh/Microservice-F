using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class BrandConfiguration : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.ToTable("Brands");

            // تنظیمات کلید اصلی
            builder.HasKey(b => b.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(b => b.Description)
                .HasMaxLength(1000);

            builder.Property(b => b.CreatedByUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(b => b.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(b => b.CreatedAt)
                .IsRequired();

            builder.Property(b => b.UpdatedAt)
                .IsRequired(false);

            builder.Property(b => b.LogoUrl)
                .HasMaxLength(500);

            builder.Property(b => b.WebsiteUrl)
                .HasMaxLength(500);

            builder.Property(b => b.MetaTitle)
                .HasMaxLength(200);

            builder.Property(b => b.MetaDescription)
                .HasMaxLength(500);

            builder.Property(b => b.Slug)
                .HasConversion(
                 slug => slug.Value,
                 value => Slug.FromString(value));

            // تنظیمات مجموعه‌ها
            builder.HasMany(b => b.Products)
                .WithOne(p => p.Brand)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(b => b.IsActive)
                .HasDatabaseName("IX_Brand_IsActive");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(b => b.LogoUrl).HasDefaultValue(null);
            builder.Property(b => b.WebsiteUrl).HasDefaultValue(null);
            builder.Property(b => b.MetaTitle).HasDefaultValue(null);
            builder.Property(b => b.MetaDescription).HasDefaultValue(null);
            builder.Property(b => b.UpdatedAt).HasDefaultValue(null);
        }
    }
}