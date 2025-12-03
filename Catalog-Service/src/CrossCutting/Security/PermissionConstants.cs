namespace Catalog_Service.src.CrossCutting.Security
{
    public static class PermissionConstants
    {
        // Product Permissions
        public const string CreateProduct = "products:create";
        public const string ReadProduct = "products:read";
        public const string UpdateProduct = "products:update";
        public const string DeleteProduct = "products:delete";

        // Category Permissions
        public const string CreateCategory = "categories:create";
        public const string ReadCategory = "categories:read";
        public const string UpdateCategory = "categories:update";
        public const string DeleteCategory = "categories:delete";

        // Brand Permissions
        public const string CreateBrand = "brands:create";
        public const string ReadBrand = "brands:read";
        public const string UpdateBrand = "brands:update";
        public const string DeleteBrand = "brands:delete";

        // Review Permissions
        public const string CreateReview = "reviews:create";
        public const string ReadReview = "reviews:read";
        public const string UpdateReview = "reviews:update";
        public const string DeleteReview = "reviews:delete";
        public const string ApproveReview = "reviews:approve";
    }
}
