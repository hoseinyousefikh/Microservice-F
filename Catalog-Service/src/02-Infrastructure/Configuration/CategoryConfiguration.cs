using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");

            // تنظیمات کلید اصلی
            builder.HasKey(c => c.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(1000);

            builder.Property(c => c.CreatedByUserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(c => c.DisplayOrder)
                .IsRequired();

            builder.Property(c => c.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired(false);

            builder.Property(c => c.ImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.MetaTitle)
                .HasMaxLength(200);

            builder.Property(c => c.MetaDescription)
                .HasMaxLength(500);

            // تنظیمات روابط
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // در CategoryConfiguration.cs - داخل متد Configure
            builder.Property(c => c.Slug)
                .HasConversion(
                    slug => slug.Value,
                    value => Slug.FromString(value))
                .HasMaxLength(200);

            // تنظیمات مجموعه‌ها
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(c => c.SubCategories)
                .WithOne(c => c.ParentCategory)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(c => c.ParentCategoryId)
                .HasDatabaseName("IX_Category_ParentCategoryId");

            builder.HasIndex(c => c.DisplayOrder)
                .HasDatabaseName("IX_Category_DisplayOrder");

            builder.HasIndex(c => c.IsActive)
                .HasDatabaseName("IX_Category_IsActive");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(c => c.ParentCategoryId).HasDefaultValue(null);
            builder.Property(c => c.ImageUrl).HasDefaultValue(null);
            builder.Property(c => c.MetaTitle).HasDefaultValue(null);
            builder.Property(c => c.MetaDescription).HasDefaultValue(null);
            builder.Property(c => c.UpdatedAt).HasDefaultValue(null);

            // جلوگیری از ایجاد حلقه در دسته‌بندی‌ها
            builder.HasCheckConstraint("CK_Category_NoCircularReference",
                "Id <> ParentCategoryId OR ParentCategoryId IS NULL");
        }
    }
}