using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class ImageResourceConfiguration : IEntityTypeConfiguration<ImageResource>
    {
        public void Configure(EntityTypeBuilder<ImageResource> builder)
        {
            builder.ToTable("ImageResources");

            // تنظیمات کلید اصلی
            builder.HasKey(i => i.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(i => i.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(i => i.FileExtension)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(i => i.StoragePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.PublicUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(i => i.FileSize)
                .IsRequired();

            builder.Property(i => i.Width)
                .IsRequired();

            builder.Property(i => i.Height)
                .IsRequired();

            builder.Property(i => i.ImageType)
                .IsRequired()
                .HasDefaultValue(ImageType.Product);

            builder.Property(i => i.CreatedByUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(i => i.CreatedAt)
                .IsRequired();

            builder.Property(i => i.UpdatedAt)
                .IsRequired(false);

            builder.Property(i => i.IsPrimary)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(i => i.AltText)
                .HasMaxLength(200);

            // تنظیمات روابط با سایر موجودیت‌ها با استفاده از Shadow Properties
            // تغییر رفتار حذف به Restrict برای جلوگیری از cascade paths
            builder.HasOne<Product>()
                .WithMany(p => p.Images)
                .HasForeignKey("ProductId")
                .OnDelete(DeleteBehavior.Restrict); // تغییر از Cascade به Restrict

            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey("CategoryId")
                .OnDelete(DeleteBehavior.Restrict); // تغییر از Cascade به Restrict

            builder.HasOne<Brand>()
                .WithMany()
                .HasForeignKey("BrandId")
                .OnDelete(DeleteBehavior.Restrict); // تغییر از Cascade به Restrict

            builder.HasOne<ProductVariant>()
                .WithMany()
                .HasForeignKey("ProductVariantId")
                .OnDelete(DeleteBehavior.Restrict); // تغییر از Cascade به Restrict

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(i => i.ImageType)
                .HasDatabaseName("IX_ImageResource_ImageType");

            builder.HasIndex(i => i.IsPrimary)
                .HasDatabaseName("IX_ImageResource_IsPrimary");

            builder.HasIndex("ProductId")
                .HasDatabaseName("IX_ImageResource_ProductId");

            builder.HasIndex("CategoryId")
                .HasDatabaseName("IX_ImageResource_CategoryId");

            builder.HasIndex("BrandId")
                .HasDatabaseName("IX_ImageResource_BrandId");

            builder.HasIndex("ProductVariantId")
                .HasDatabaseName("IX_ImageResource_ProductVariantId");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(i => i.AltText).HasDefaultValue(null);
            builder.Property(i => i.UpdatedAt).HasDefaultValue(null);

            // Shadow Properties برای روابط
            builder.Property<int?>("ProductId");
            builder.Property<int?>("CategoryId");
            builder.Property<int?>("BrandId");
            builder.Property<int?>("ProductVariantId");

            // اطمینان از اینکه هر تصویر فقط به یک موجودیت مرتبط است
            builder.HasCheckConstraint("CK_ImageResource_SingleEntityReference",
                "(ProductId IS NOT NULL AND CategoryId IS NULL AND BrandId IS NULL AND ProductVariantId IS NULL) OR " +
                "(ProductId IS NULL AND CategoryId IS NOT NULL AND BrandId IS NULL AND ProductVariantId IS NULL) OR " +
                "(ProductId IS NULL AND CategoryId IS NULL AND BrandId IS NOT NULL AND ProductVariantId IS NULL) OR " +
                "(ProductId IS NULL AND CategoryId IS NULL AND BrandId IS NULL AND ProductVariantId IS NOT NULL)");
        }
    }
}