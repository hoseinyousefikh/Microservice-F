using Identity_Service.Presentation.Dtos.Responses.Roles;
using MediatR;

namespace Identity_Service.Application.Features.Roles.Queries.GetAllRoles
{
    public class GetAllRolesQuery : IRequest<List<RoleSummaryResponseDto>>
    {
    }
}