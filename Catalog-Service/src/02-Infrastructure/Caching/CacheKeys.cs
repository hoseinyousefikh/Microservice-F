namespace Catalog_Service.src._02_Infrastructure.Caching
{
    public static class CacheKeys
    {
        // Product keys
        public static class Products
        {
            public const string ProductById = "product:id:";
            public const string ProductBySlug = "product:slug:";
            public const string ProductBySku = "product:sku:";
            public const string FeaturedProducts = "product:featured";
            public const string NewestProducts = "product:newest";
            public const string BestSellingProducts = "product:bestselling";
            public const string ProductsByCategory = "product:category:";
            public const string ProductsByBrand = "product:brand:";
            public const string ProductSearch = "product:search:";
            public const string ProductVariants = "product:variants:";
            public const string ProductReviews = "product:reviews:";
            public const string ProductAverageRating = "product:rating:";
        }

        // Category keys
        public static class Categories
        {
            public const string CategoryById = "category:id:";
            public const string CategoryBySlug = "category:slug:";
            public const string RootCategories = "category:root";
            public const string CategoryTree = "category:tree";
            public const string CategoryPath = "category:path:";
            public const string SubCategories = "category:sub:";
        }

        // Brand keys
        public static class Brands
        {
            public const string BrandById = "brand:id:";
            public const string BrandBySlug = "brand:slug:";
            public const string AllBrands = "brand:all";
        }

        // Image keys
        public static class Images
        {
            public const string ImageById = "image:id:";
            public const string ProductImages = "image:product:";
            public const string VariantImages = "image:variant:";
        }

        // Attribute keys
        public static class Attributes
        {
            public const string ProductAttributes = "attribute:product:";
            public const string VariantAttributes = "attribute:variant:";
        }

        // Review keys
        public static class Reviews
        {
            public const string ReviewById = "review:id:";
            public const string ProductReviews = "review:product:";
            public const string UserReviews = "review:user:";
            public const string PendingReviews = "review:pending";
        }

        // Tag keys
        public static class Tags
        {
            public const string ProductTags = "tag:product:";
            public const string PopularTags = "tag:popular";
        }

        // General keys
        public static class General
        {
            public const string SiteSettings = "general:settings";
            public const string HomePageData = "general:homepage";
            public const string NavigationMenu = "general:navigation";
        }

        // Helper methods to generate cache keys
        public static string GenerateKey(string prefix, params object[] parameters)
        {
            return $"{prefix}{string.Join(":", parameters)}";
        }
    }
}
