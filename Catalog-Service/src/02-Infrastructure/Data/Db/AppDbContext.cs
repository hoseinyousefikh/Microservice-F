using Catalog_Service.src._01_Domain.Core;
using Catalog_Service.src._01_Domain.Core.Entities;
using Catalog_Service.src._01_Domain.Core.Enums;
using Catalog_Service.src._01_Domain.Core.Primitives;
using Catalog_Service.src._02_Infrastructure.Configuration;
using Catalog_Service.src._02_Infrastructure.Data.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging; // این using را اضافه کنید
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;

namespace Catalog_Service.src._02_Infrastructure.Data.Db
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // DbSet برای موجودیت‌های اصلی
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<ImageResource> ImageResources { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<ProductAttribute> ProductAttributes { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }
        public DbSet<ProductReviewReply> ProductReviewReplies { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var moneyConverter = new MoneyConverter();

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(Money))
                    {
                        property.SetValueConverter(moneyConverter);
                    }
                }
            }
            // اعمال تمام تنظیمات موجودیت‌ها
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new BrandConfiguration());
            modelBuilder.ApplyConfiguration(new ImageResourceConfiguration());
            modelBuilder.ApplyConfiguration(new ProductVariantConfiguration());
            modelBuilder.ApplyConfiguration(new ProductAttributeConfiguration());
            modelBuilder.ApplyConfiguration(new ProductReviewConfiguration());
            modelBuilder.ApplyConfiguration(new ProductTagConfiguration());

            // تنظیمات جهانی برای Value Objects
            ConfigureValueObjects(modelBuilder);

            // فیلترهای سراسری برای موجودیت‌های نرم‌حذف شده
            ConfigureGlobalQueryFilters(modelBuilder);

            // ایندکس‌های سراسری
            ConfigureGlobalIndexes(modelBuilder);
        }

        private void ConfigureValueObjects(ModelBuilder modelBuilder)
        {
            // تنظیمات برای Value Object: Money
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Product>()
                .Property(p => p.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariant>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<ProductVariant>()
                .Property(p => p.OriginalPrice)
                .HasColumnType("decimal(18,2)");

            // تنظیمات برای Value Object: Slug
            modelBuilder.Entity<Product>()
                .Property(p => p.Slug)
                .HasMaxLength(200);

            modelBuilder.Entity<Category>()
                .Property(c => c.Slug)
                .HasMaxLength(200);

            modelBuilder.Entity<Brand>()
                .Property(b => b.Slug)
                .HasMaxLength(200);
            modelBuilder.Entity<Product>()
                .Property(p => p.Slug)
                .HasMaxLength(200);

            modelBuilder.Entity<Category>()
                .Property(c => c.Slug)
                .HasMaxLength(200);

            modelBuilder.Entity<Brand>()
                .Property(b => b.Slug)
                .HasMaxLength(200);

            // تنظیمات برای Value Object: Dimensions
            modelBuilder.Entity<Product>()
                .OwnsOne(p => p.Dimensions, d =>
                {
                    d.Property(dim => dim.Length).HasColumnName("Length").HasColumnType("decimal(10,2)");
                    d.Property(dim => dim.Width).HasColumnName("Width").HasColumnType("decimal(10,2)");
                    d.Property(dim => dim.Height).HasColumnName("Height").HasColumnType("decimal(10,2)");
                    d.Property(dim => dim.Unit).HasColumnName("DimensionUnit").HasMaxLength(10);
                });

            modelBuilder.Entity<ProductVariant>()
                .OwnsOne(p => p.Dimensions, d =>
                {
                    d.Property(dim => dim.Length).HasColumnName("Length").HasColumnType("decimal(10,2)");
                    d.Property(dim => dim.Width).HasColumnName("Width").HasColumnType("decimal(10,2)");
                    d.Property(dim => dim.Height).HasColumnName("Height").HasColumnType("decimal(10,2)");
                    d.Property(dim => dim.Unit).HasColumnName("DimensionUnit").HasMaxLength(10);
                });

            // تنظیمات برای Value Object: Weight
            modelBuilder.Entity<Product>()
                .OwnsOne(p => p.Weight, w =>
                {
                    w.Property(weight => weight.Value).HasColumnName("WeightValue").HasColumnType("decimal(10,2)");
                    w.Property(weight => weight.Unit).HasColumnName("WeightUnit").HasMaxLength(10);
                });

            modelBuilder.Entity<ProductVariant>()
                .OwnsOne(p => p.Weight, w =>
                {
                    w.Property(weight => weight.Value).HasColumnName("WeightValue").HasColumnType("decimal(10,2)");
                    w.Property(weight => weight.Unit).HasColumnName("WeightUnit").HasMaxLength(10);
                });
        }

        private void ConfigureGlobalQueryFilters(ModelBuilder modelBuilder)
        {
            // فیلتر برای دسترسی به دسته‌بندی‌های فعال
            modelBuilder.Entity<Category>()
                .HasQueryFilter(c => c.IsActive);

            // فیلتر برای دسترسی به برندهای فعال
            modelBuilder.Entity<Brand>()
                .HasQueryFilter(b => b.IsActive);

            // فیلتر برای دسترسی به محصولات منتشر شده
            modelBuilder.Entity<Product>()
                .HasQueryFilter(p => p.Status == ProductStatus.Published);

            // فیلتر برای دسترسی به محصولات منتشر شده
            modelBuilder.Entity<ProductVariant>()
                .HasQueryFilter(pv => pv.IsActive);
        }

        private void ConfigureGlobalIndexes(ModelBuilder modelBuilder)
        {
            // ایندکس برای بهینه‌سازی جستجو بر اساس نام محصول
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Name)
                .HasDatabaseName("IX_Product_Name");

            // ایندکس برای بهینه‌سازی جستجو بر اساس Slug
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Product_Slug");

            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Category_Slug");

            modelBuilder.Entity<Brand>()
                .HasIndex(b => b.Slug)
                .IsUnique()
                .HasDatabaseName("IX_Brand_Slug");

            // ایندکس برای بهینه‌سازی جستجو بر اساس SKU
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Sku)
                .IsUnique()
                .HasDatabaseName("IX_Product_Sku");

            modelBuilder.Entity<ProductVariant>()
                .HasIndex(pv => pv.Sku)
                .IsUnique()
                .HasDatabaseName("IX_ProductVariant_Sku");

            // ایندکس برای بهینه‌سازی جستجو بر اساس دسته‌بندی
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CategoryId)
                .HasDatabaseName("IX_Product_CategoryId");

            // ایندکس برای بهینه‌سازی جستجو بر اساس برند
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.BrandId)
                .HasDatabaseName("IX_Product_BrandId");

            // ایندکس برای بهینه‌سازی جستجو بر اساس وضعیت موجودی
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.StockStatus)
                .HasDatabaseName("IX_Product_StockStatus");

            // ایندکس برای بهینه‌سازی جستجو بر اساس وضعیت محصول
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Status)
                .HasDatabaseName("IX_Product_Status");

            // ایندکس برای بهینه‌سازی جستجو بر اساس محصولات ویژه
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.IsFeatured)
                .HasDatabaseName("IX_Product_IsFeatured");

            // ایندکس برای بهینه‌سازی جستجو بر اساس تاریخ ایجاد
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_Product_CreatedAt");

            // ایندکس برای بهینه‌سازی جستجو بر اساس امتیاز بازبینی
            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => pr.Rating)
                .HasDatabaseName("IX_ProductReview_Rating");

            // ایندکس برای بهینه‌سازی جستجو بر اساس وضعیت بازبینی
            modelBuilder.Entity<ProductReview>()
                .HasIndex(pr => pr.Status)
                .HasDatabaseName("IX_ProductReview_Status");
        }

        // *** این متد را با نسخه تشخیصی جایگزین کنید ***
        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();

            // ⛔️ Domain Events نباید با Request Token کنسل شوند
            await ProcessDomainEventsAsync(CancellationToken.None);

            var logger = this.GetService<ILogger<AppDbContext>>();

            try
            {
                // ⛔️ SaveChanges هم با None
                return await base.SaveChangesAsync(CancellationToken.None);
            }
            catch (DbUpdateException ex)
            {
                logger.LogError(ex, "Database update failed");

                if (ex.InnerException != null)
                    logger.LogError("Inner Exception: {Message}", ex.InnerException.Message);

                throw;
            }
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries<Entity>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    // برای موجودیت‌های جدید، CreatedAt تنظیم می‌شود
                    if (entry.Property("CreatedAt") != null && entry.Property("CreatedAt").CurrentValue == null)
                    {
                        entry.Property("CreatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    // برای موجودیت‌های ویرایش شده، UpdatedAt به‌روزرسانی می‌شود
                    if (entry.Property("UpdatedAt") != null)
                    {
                        entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
                    }
                }
            }
        }

        private async Task ProcessDomainEventsAsync(CancellationToken cancellationToken)
        {
            var domainEntities = ChangeTracker.Entries<AggregateRoot>()
                .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any())
                .ToList();

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            // پاک کردن رویدادها از موجودیت‌ها
            domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

            // پردازش رویدادها (در اینجا می‌توانید رویدادها را به یک سرویس پیام‌رسان ارسال کنید)
            foreach (var domainEvent in domainEvents)
            {
                // در اینجا می‌توانید رویدادها را پردازش کنید
                // برای مثال: await _mediator.Publish(domainEvent, cancellationToken);
            }
        }
    }
}