using Microsoft.AspNetCore.Authorization;

namespace Catalog_Service.src.CrossCutting.Security.PolicyRequirements
{
    public class ProductOwnerRequirement : IAuthorizationRequirement
    {
        public int ProductId { get; }

        public ProductOwnerRequirement(int productId)
        {
            ProductId = productId;
        }
    }
}
