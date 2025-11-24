using MediatR;

namespace Identity_Service.Application.Features.Roles.Commands.AssignRoleToUser
{
    public class AssignRoleToUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}