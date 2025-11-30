using Catalog_Service.src._01_Domain.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class ProductTagConfiguration : IEntityTypeConfiguration<ProductTag>
    {
        public void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            builder.ToTable("ProductTags");

            // تنظیمات کلید اصلی
            builder.HasKey(pt => pt.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(pt => pt.ProductId)
                .IsRequired();

            builder.Property(pt => pt.TagText)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.CreatedAt)
                .IsRequired();

            // تنظیمات روابط
            builder.HasOne(pt => pt.Product)
                .WithMany(p => p.Tags)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // تنظیمات ایندکس‌ها
            builder.HasIndex(pt => pt.ProductId)
                .HasDatabaseName("IX_ProductTag_ProductId");

            builder.HasIndex(pt => pt.TagText)
                .HasDatabaseName("IX_ProductTag_TagText");

            // ایندکس منحصر به فرد برای جلوگیری از تگ‌های تکراری برای یک محصول
            builder.HasIndex(pt => new { pt.ProductId, pt.TagText })
                .IsUnique()
                .HasDatabaseName("IX_ProductTag_ProductId_TagText");

            // حذف Check Constraint که شامل subquery بود
            // builder.HasCheckConstraint("CK_ProductTag_UniqueTagPerProduct", ...);
        }
    }
}
