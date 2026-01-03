using Catalog_Service.src.CrossCutting.Security.PolicyRequirements;
using Microsoft.AspNetCore.Authorization;

namespace Catalog_Service.src.CrossCutting.Security
{
    public static class AuthorizationPolicies
    {
        public const string AdminPolicy = "AdminPolicy";
        public const string VendorPolicy = "VendorPolicy";
        public const string ProductOwnerPolicy = "ProductOwnerPolicy";
        public const string CanManageProducts = "CanManageProducts";
        public const string CanManageCategories = "CanManageCategories";
        public const string CanManageBrands = "CanManageBrands";
        public const string CanViewProducts = "CanViewProducts";
        public const string CanCreateReview = "CanCreateReview";

        public static void Configure(AuthorizationOptions options)
        {
            // این پالیسی اکنون هر دو نقش Admin و SuperAdmin را می‌پذیرد
            options.AddPolicy(AdminPolicy, policy =>
                policy.RequireRole(RoleConstants.Administrator, RoleConstants.SuperAdministrator));

            // این پالیسی اکنون از نقش "Seller" استفاده می‌کند
            options.AddPolicy(VendorPolicy, policy =>
                policy.RequireRole(RoleConstants.Vendor));

            options.AddPolicy(ProductOwnerPolicy, policy =>
                policy.Requirements.Add(new ProductOwnerRequirement(0))); // Will be replaced with actual product ID

            // این پالیسی اکنون نقش‌های Admin, SuperAdmin و Seller را می‌پذیرد
            options.AddPolicy(CanManageProducts, policy =>
                policy.RequireRole(RoleConstants.Administrator, RoleConstants.SuperAdministrator, RoleConstants.Vendor));

            // این پالیسی اکنون هر دو نقش Admin و SuperAdmin را می‌پذیرد
            options.AddPolicy(CanManageCategories, policy =>
                policy.RequireRole(RoleConstants.Administrator, RoleConstants.SuperAdministrator));

            // این پالیسی اکنون هر دو نقش Admin و SuperAdmin را می‌پذیرد
            options.AddPolicy(CanManageBrands, policy =>
                policy.RequireRole(RoleConstants.Administrator, RoleConstants.SuperAdministrator));

            options.AddPolicy(CanViewProducts, policy =>
                policy.RequireRole(RoleConstants.Administrator, RoleConstants.SuperAdministrator, RoleConstants.Vendor, RoleConstants.Customer));

            options.AddPolicy(CanCreateReview, policy =>
                policy.RequireRole(RoleConstants.Customer));
        }
    }
}
