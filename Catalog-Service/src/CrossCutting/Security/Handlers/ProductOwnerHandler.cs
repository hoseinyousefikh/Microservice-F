using Catalog_Service.src._01_Domain.Core.Contracts.Repositories;
using Catalog_Service.src.CrossCutting.Security.PolicyRequirements;
using Microsoft.AspNetCore.Authorization;

namespace Catalog_Service.src.CrossCutting.Security.Handlers
{
    public class ProductOwnerHandler : AuthorizationHandler<ProductOwnerRequirement>
    {
        private readonly IProductRepository _productRepository;

        public ProductOwnerHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ProductOwnerRequirement requirement)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            var userRole = context.User.FindFirst("role")?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                context.Fail();
                return;
            }

            // Admins can access any product
            if (userRole == RoleConstants.Administrator)
            {
                context.Succeed(requirement);
                return;
            }

            // Get the product
            var product = await _productRepository.GetByIdAsync(requirement.ProductId);
            if (product == null)
            {
                context.Fail();
                return;
            }

            // Check if the user is the owner of the product through the brand
            // Since Brand doesn't have VendorId, we'll check if the user has permission to manage products
            if (userRole == RoleConstants.Vendor && context.User.HasClaim(c => c.Type == "permission" && c.Value == PermissionConstants.UpdateProduct))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
