using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog_Service.src._02_Infrastructure.Configuration
{
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.ToTable("ProductReviews");

            // تنظیمات کلید اصلی
            builder.HasKey(pr => pr.Id);

            // تنظیمات ویژگی‌ها
            builder.Property(pr => pr.ProductId)
                .IsRequired();

            builder.Property(pr => pr.UserId)
                .IsRequired()
                .HasMaxLength(450); // طول استاندارد برای ASP.NET Core Identity

            builder.Property(pr => pr.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pr => pr.Comment)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(pr => pr.Rating)
                .IsRequired();

            builder.Property(pr => pr.Status)
                .IsRequired()
                .HasDefaultValue(ReviewStatus.Pending);

            builder.Property(pr => pr.CreatedAt)
                .IsRequired();

            builder.Property(pr => pr.UpdatedAt)
                .IsRequired(false);

            builder.Property(pr => pr.IsVerifiedPurchase)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pr => pr.HelpfulVotes)
                .IsRequired()
                .HasDefaultValue(0);

            // تنظیمات روابط
            builder.HasOne(pr => pr.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(pr => pr.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // تنظیمات ایندکس‌های اضافی
            builder.HasIndex(pr => pr.ProductId)
                .HasDatabaseName("IX_ProductReview_ProductId");

            builder.HasIndex(pr => pr.UserId)
                .HasDatabaseName("IX_ProductReview_UserId");

            builder.HasIndex(pr => pr.Rating)
                .HasDatabaseName("IX_ProductReview_Rating");

            builder.HasIndex(pr => pr.Status)
                .HasDatabaseName("IX_ProductReview_Status");

            builder.HasIndex(pr => pr.IsVerifiedPurchase)
                .HasDatabaseName("IX_ProductReview_IsVerifiedPurchase");

            builder.HasIndex(pr => new { pr.ProductId, pr.UserId })
                .IsUnique()
                .HasDatabaseName("IX_ProductReview_ProductId_UserId");

            // تنظیمات پیش‌فرض برای مقادیر اختیاری
            builder.Property(pr => pr.UpdatedAt).HasDefaultValue(null);

            // محدوده مجاز برای امتیاز
            builder.HasCheckConstraint("CK_ProductReview_ValidRating",
                "Rating >= 1 AND Rating <= 5");

            // محدوده مجاز برای تعداد رای‌های مفید
            builder.HasCheckConstraint("CK_ProductReview_ValidHelpfulVotes",
                "HelpfulVotes >= 0");
        }
    }
}
