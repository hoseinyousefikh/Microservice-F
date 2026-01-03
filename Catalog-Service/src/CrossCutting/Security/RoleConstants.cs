namespace Catalog_Service.src.CrossCutting.Security
{
    public static class RoleConstants
    {
        // این مقادیر باید دقیقاً با enum در IdentityService مطابقت داشته باشند
        public const string Guest = "Guest";
        public const string Customer = "Customer";
        public const string Vendor = "Seller"; // "Vendor" را به "Seller" تغییر دادیم
        public const string Moderator = "Moderator"; // نقش جدید اضافه شد
        public const string Administrator = "Admin"; // "Administrator" را به "Admin" تغییر دادیم
        public const string SuperAdministrator = "SuperAdmin"; // نقش جدید اضافه شد
    }
}
