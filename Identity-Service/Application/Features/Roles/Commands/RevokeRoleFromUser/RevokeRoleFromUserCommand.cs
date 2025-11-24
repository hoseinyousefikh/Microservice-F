using MediatR;

namespace Identity_Service.Application.Features.Roles.Commands.RevokeRoleFromUser
{
    public class RevokeRoleFromUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}