using MediatR;

namespace Identity_Service.Application.Features.Roles.Commands.DeleteRole
{
    public class DeleteRoleCommand : IRequest<Unit>
    {
        public Guid RoleId { get; set; }
    }
}