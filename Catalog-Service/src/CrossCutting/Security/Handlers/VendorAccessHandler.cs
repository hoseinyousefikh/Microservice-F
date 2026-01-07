using Catalog_Service.src.CrossCutting.Security.PolicyRequirements;
using Microsoft.AspNetCore.Authorization;

namespace Catalog_Service.src.CrossCutting.Security.Handlers
{
    public class VendorAccessHandler : AuthorizationHandler<VendorRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            VendorRequirement requirement)
        {
            var userRole = context.User.FindFirst("role")?.Value;

            if (userRole == RoleConstants.Administrator || userRole == RoleConstants.Vendor)
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}