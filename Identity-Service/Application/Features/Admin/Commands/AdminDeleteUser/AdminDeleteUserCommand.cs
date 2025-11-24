using MediatR;

namespace Identity_Service.Application.Features.Admin.Commands.AdminDeleteUser
{
    public class AdminDeleteUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}