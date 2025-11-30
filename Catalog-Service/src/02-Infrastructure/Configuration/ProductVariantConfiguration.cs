using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._02_Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> builder)
        {
            builder.ToTable("ProductVariants");

            // تنظیمات کلید اصلی
            builder.HasKey(pv => pv.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(pv => pv.ProductId)
                .IsRequired();

            builder.Property(pv => pv.Sku)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(pv => pv.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pv => pv.Price)
                .HasConversion<MoneyConverter>();

            builder.Property(pv => pv.OriginalPrice)
                .HasConversion<MoneyConverter>();

            builder.Property(pv => pv.StockQuantity)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(pv => pv.StockStatus)
                .IsRequired()
                .HasDefaultValue(StockStatus.OutOfStock);

            builder.Property(pv => pv.CreatedAt)
                .IsRequired();

            builder.Property(pv => pv.UpdatedAt)
                .IsRequired(false);

            builder.Property(pv => pv.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(pv => pv.ImageUrl)
                .HasMaxLength(500);

            // تنظیمات روابط
            builder.HasOne(pv => pv.Product)
                .WithMany(p => p.Variants)
                .HasForeignKey(pv => pv.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // تنظیمات مجموعه‌ها
            builder.HasMany(pv => pv.Attributes)
                .WithOne(a => a.ProductVariant)
                .HasForeignKey(a => a.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(pv => pv.ProductId)
                .HasDatabaseName("IX_ProductVariant_ProductId");

            builder.HasIndex(pv => pv.Sku)
                .IsUnique()
                .HasDatabaseName("IX_ProductVariant_Sku");

            builder.HasIndex(pv => pv.StockStatus)
                .HasDatabaseName("IX_ProductVariant_StockStatus");

            builder.HasIndex(pv => pv.IsActive)
                .HasDatabaseName("IX_ProductVariant_IsActive");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(pv => pv.OriginalPrice).HasDefaultValue(null);
            builder.Property(pv => pv.ImageUrl).HasDefaultValue(null);
            builder.Property(pv => pv.UpdatedAt).HasDefaultValue(null);

            // اطمینان از اینکه قیمت اصلی بزرگتر یا مساوی قیمت فعلی است
            builder.HasCheckConstraint("CK_ProductVariant_ValidOriginalPrice",
                "OriginalPrice IS NULL OR OriginalPrice >= Price");
        }
    }
}
