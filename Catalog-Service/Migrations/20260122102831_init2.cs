using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Catalog_Service.Migrations
{
    /// <inheritdoc />
    public partial class init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. ایندکس اصلی برای جستجوی سریع
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Search_Main')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Products_Search_Main 
                    ON [dbo].[Products] 
                    (
                        [Status] ASC,
                        [CategoryId] ASC,
                        [BrandId] ASC
                    )
                    INCLUDE 
                    (
                        [Name],
                        [Slug],
                        [Price],
                        [OriginalPrice],
                        [StockQuantity],
                        [StockStatus],
                        [IsFeatured],
                        [CreatedAt],
                        [ViewCount]
                    )
                    WHERE [Status] = 1;
                END
            ");

            // 2. ایندکس برای جستجو در نام
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Name_Search')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Products_Name_Search 
                    ON [dbo].[Products] 
                    (
                        [Status] ASC,
                        [Name] ASC
                    )
                    INCLUDE 
                    (
                        [Slug],
                        [CategoryId],
                        [BrandId],
                        [Price],
                        [StockStatus],
                        [CreatedAt]
                    )
                    WHERE [Status] = 1;
                END
            ");

            // 3. ایندکس برای جستجو در اسلاگ
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Slug_Search')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Products_Slug_Search 
                    ON [dbo].[Products] 
                    (
                        [Status] ASC,
                        [Slug] ASC
                    )
                    INCLUDE 
                    (
                        [Name],
                        [CategoryId],
                        [BrandId],
                        [Price],
                        [CreatedAt]
                    )
                    WHERE [Status] = 1;
                END
            ");

            // 4. ایندکس ترکیبی برای فیلترهای متداول
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Filtered_Search')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Products_Filtered_Search 
                    ON [dbo].[Products] 
                    (
                        [Status] ASC,
                        [CategoryId] ASC,
                        [BrandId] ASC,
                        [StockStatus] ASC,
                        [IsFeatured] ASC
                    )
                    INCLUDE 
                    (
                        [Name],
                        [Slug],
                        [Price],
                        [CreatedAt],
                        [ViewCount]
                    )
                    WHERE [Status] = 1;
                END
            ");

            // 5. ایندکس برای مرتب‌سازی قیمت
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Price_Sort')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Products_Price_Sort 
                    ON [dbo].[Products] 
                    (
                        [Status] ASC,
                        [Price] ASC
                    )
                    INCLUDE 
                    (
                        [Name],
                        [Slug],
                        [CategoryId],
                        [BrandId],
                        [StockStatus],
                        [IsFeatured]
                    )
                    WHERE [Status] = 1;
                END
            ");

            // 6. ایندکس برای مرتب‌سازی تاریخ
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Products_Date_Sort')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Products_Date_Sort 
                    ON [dbo].[Products] 
                    (
                        [Status] ASC,
                        [CreatedAt] DESC
                    )
                    INCLUDE 
                    (
                        [Name],
                        [Slug],
                        [Price],
                        [CategoryId],
                        [BrandId],
                        [StockStatus],
                        [IsFeatured]
                    )
                    WHERE [Status] = 1;
                END
            ");

            // 7. ایندکس برای برندها
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Brands_Search')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Brands_Search 
                    ON [dbo].[Brands] 
                    (
                        [IsActive] ASC,
                        [Name] ASC
                    )
                    INCLUDE ([Id], [Slug])
                    WHERE [IsActive] = 1;
                END
            ");

            // 8. ایندکس برای دسته‌بندی‌ها
            migrationBuilder.Sql(@"
                IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Categories_Search')
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_Categories_Search 
                    ON [dbo].[Categories] 
                    (
                        [IsActive] ASC,
                        [ParentCategoryId] ASC,
                        [Name] ASC
                    )
                    INCLUDE ([Id], [Slug])
                    WHERE [IsActive] = 1;
                END
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // حذف ایندکس‌ها
            migrationBuilder.Sql(@"
                DECLARE @sql NVARCHAR(MAX) = N'';
                
                SELECT @sql += N'DROP INDEX ' + QUOTENAME(name) + ' ON [dbo].[Products];'
                FROM sys.indexes 
                WHERE object_id = OBJECT_ID('[dbo].[Products]') 
                AND name IN (
                    'IX_Products_Search_Main',
                    'IX_Products_Name_Search',
                    'IX_Products_Slug_Search',
                    'IX_Products_Filtered_Search',
                    'IX_Products_Price_Sort',
                    'IX_Products_Date_Sort'
                );
                
                SELECT @sql += N'DROP INDEX ' + QUOTENAME(name) + ' ON [dbo].[Brands];'
                FROM sys.indexes 
                WHERE object_id = OBJECT_ID('[dbo].[Brands]') 
                AND name = 'IX_Brands_Search';
                
                SELECT @sql += N'DROP INDEX ' + QUOTENAME(name) + ' ON [dbo].[Categories];'
                FROM sys.indexes 
                WHERE object_id = OBJECT_ID('[dbo].[Categories]') 
                AND name = 'IX_Categories_Search';
                
                EXEC sp_executesql @sql;
            ");
        }

    }
}
