using Identity_Service.Presentation.Dtos.Responses.Roles;
using MediatR;

namespace Identity_Service.Application.Features.Roles.Queries.GetUserRoles
{
    public class GetUserRolesQuery : IRequest<List<RoleSummaryResponseDto>>
    {
        public Guid UserId { get; set; }
    }
}