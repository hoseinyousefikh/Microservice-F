using Catalog_Service.src.CrossCutting.Security;
using System.Security.Claims;

namespace Catalog_Service.src.CrossCutting.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.NameIdentifier) ??
                   user?.FindFirstValue("sub") ??
                   string.Empty;
        }

        public static string GetUserName(this ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.Name) ??
                   user?.FindFirstValue("unique_name") ??
                   string.Empty;
        }

        public static string GetUserEmail(this ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.Email) ??
                   user?.FindFirstValue("email") ??
                   string.Empty;
        }

        public static string GetUserRole(this ClaimsPrincipal user)
        {
            return user?.FindFirstValue(ClaimTypes.Role) ??
                   user?.FindFirstValue("role") ??
                   RoleConstants.Guest;
        }

        public static bool IsInRole(this ClaimsPrincipal user, string role)
        {
            if (user == null) return false;
            return user.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role) ||
                   user.Claims.Any(c => c.Type == "role" && c.Value == role);
        }

        public static bool IsAdmin(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoleConstants.Administrator);
        }

        public static bool IsVendor(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoleConstants.Vendor);
        }

        public static bool IsCustomer(this ClaimsPrincipal user)
        {
            return user.IsInRole(RoleConstants.Customer);
        }

        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (user == null) return false;
            return user.Claims.Any(c => c.Type == "permission" && c.Value == permission);
        }

        public static int? GetTenantId(this ClaimsPrincipal user)
        {
            var tenantIdClaim = user?.FindFirstValue("tenant_id");
            if (int.TryParse(tenantIdClaim, out var tenantId))
            {
                return tenantId;
            }
            return null;
        }
    }
}
