using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using System.Security.Claims;

namespace Identity_Service.Application.Features.Authentication.Queries.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<UserProfileResponseDto>
    {
        public ClaimsPrincipal User { get; }

        public GetCurrentUserQuery(ClaimsPrincipal user)
        {
            User = user;
        }
    }
}