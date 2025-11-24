using MediatR;

namespace Identity_Service.Application.Features.Admin.Commands.DeactivateUser
{
    public class DeactivateUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}