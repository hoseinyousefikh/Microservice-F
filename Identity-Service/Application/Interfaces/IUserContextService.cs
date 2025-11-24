using System.Security.Claims;

namespace Identity_Service.Application.Interfaces
{
    public interface IUserContextService
    {
        Guid? GetCurrentUserId();
        string? GetCurrentUsername();
        ClaimsPrincipal? GetUser();
    }
}