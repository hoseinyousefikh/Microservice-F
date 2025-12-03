using Microsoft.AspNetCore.Authorization;

namespace Catalog_Service.src.CrossCutting.Security.PolicyRequirements
{
    public class AdminRequirement : IAuthorizationRequirement
    {
    }
}
