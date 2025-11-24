using MediatR;

namespace Identity_Service.Application.Features.Admin.Commands.ActivateUser
{
    public class ActivateUserCommand : IRequest<Unit>
    {
        public Guid UserId { get; set; }
    }
}