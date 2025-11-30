using Catalog_Service.src._01_Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> builder)
        {
            builder.ToTable("ProductAttributes");

            // تنظیمات کلید اصلی
            builder.HasKey(pa => pa.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(pa => pa.ProductId)
                .IsRequired();

            builder.Property(pa => pa.ProductVariantId)
                .IsRequired(false);

            builder.Property(pa => pa.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pa => pa.Value)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pa => pa.IsVariantSpecific)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pa => pa.CreatedAt)
                .IsRequired();

            builder.Property(pa => pa.UpdatedAt)
                .IsRequired(false);

            // تنظیمات روابط با تغییر رفتار حذف
            builder.HasOne(pa => pa.Product)
                .WithMany(p => p.Attributes)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // تغییر از Cascade به Restrict

            builder.HasOne(pa => pa.ProductVariant)
                .WithMany(pv => pv.Attributes)
                .HasForeignKey(pa => pa.ProductVariantId)
                .OnDelete(DeleteBehavior.Restrict); // تغییر از Cascade به Restrict

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(pa => pa.ProductId)
                .HasDatabaseName("IX_ProductAttribute_ProductId");

            builder.HasIndex(pa => pa.ProductVariantId)
                .HasDatabaseName("IX_ProductAttribute_ProductVariantId");

            builder.HasIndex(pa => pa.Name)
                .HasDatabaseName("IX_ProductAttribute_Name");

            builder.HasIndex(pa => new { pa.ProductId, pa.Name })
                .HasDatabaseName("IX_ProductAttribute_ProductId_Name");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(pa => pa.ProductVariantId).HasDefaultValue(null);
            builder.Property(pa => pa.UpdatedAt).HasDefaultValue(null);

            // اطمینان از اینکه ویژگی یا متعلق به محصول است یا به متغیر محصول
            builder.HasCheckConstraint("CK_ProductAttribute_SingleEntityReference",
                "(ProductVariantId IS NULL AND IsVariantSpecific = 0) OR " +
                "(ProductVariantId IS NOT NULL AND IsVariantSpecific = 1)");
        }
    }
}
