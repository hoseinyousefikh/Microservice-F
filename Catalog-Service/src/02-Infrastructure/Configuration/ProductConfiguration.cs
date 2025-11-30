using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._02_Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");

            // تنظیمات کلید اصلی
            builder.HasKey(p => p.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.Sku)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Status)
                .IsRequired()
                .HasDefaultValue(ProductStatus.Draft);

            builder.Property(p => p.BrandId)
                .IsRequired();

            builder.Property(p => p.CategoryId)
                .IsRequired();

            builder.Property(p => p.ViewCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.CreatedAt)
                .IsRequired();

            builder.Property(p => p.UpdatedAt)
                .IsRequired(false);

            builder.Property(p => p.PublishedAt)
                .IsRequired(false);

            builder.Property(p => p.IsFeatured)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(p => p.MetaTitle)
                .HasMaxLength(200);

            builder.Property(p => p.MetaDescription)
                .HasMaxLength(500);

            builder.Property(p => p.StockQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(p => p.StockStatus)
                .IsRequired()
                .HasDefaultValue(StockStatus.OutOfStock);
            builder.Property(p => p.Price)
                .HasConversion<MoneyConverter>();

            builder.Property(p => p.OriginalPrice)
                .HasConversion<MoneyConverter>();
            // تنظیمات روابط
            builder.HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            // در ProductConfiguration.cs
            builder.Property(p => p.Slug)
                .HasConversion(
                    slug => slug.Value,
                    value => Slug.FromString(value));

            // در CategoryConfiguration.cs
            builder.Property(c => c.Slug)
                .HasConversion(
                    slug => slug.Value,
                    value => Slug.FromString(value));
            // تنظیمات مجموعه‌ها
            builder.HasMany(p => p.Variants)
                .WithOne(v => v.Product)
                .HasForeignKey(v => v.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Reviews)
                .WithOne(r => r.Product)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Tags)
                .WithOne(t => t.Product)
                .HasForeignKey(t => t.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Images)
                .WithOne() // ImageResource یک موجودیت مستقل است
                .HasForeignKey("ProductId") // Shadow Property
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Attributes)
                .WithOne(a => a.Product)
                .HasForeignKey(a => a.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(p => new { p.BrandId, p.CategoryId })
                .HasDatabaseName("IX_Product_BrandId_CategoryId");

            builder.HasIndex(p => new { p.Status, p.IsFeatured })
                .HasDatabaseName("IX_Product_Status_IsFeatured");

            builder.HasIndex(p => new { p.StockStatus, p.StockQuantity })
                .HasDatabaseName("IX_Product_StockStatus_StockQuantity");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(p => p.MetaTitle).HasDefaultValue(null);
            builder.Property(p => p.MetaDescription).HasDefaultValue(null);
            builder.Property(p => p.OriginalPrice).HasDefaultValue(null);
            builder.Property(p => p.PublishedAt).HasDefaultValue(null);
            builder.Property(p => p.UpdatedAt).HasDefaultValue(null);
        }
    }
}
