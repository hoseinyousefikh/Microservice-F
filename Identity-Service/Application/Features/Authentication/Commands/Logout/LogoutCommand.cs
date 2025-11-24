using MediatR;

namespace Identity_Service.Application.Features.Authentication.Commands.Logout
{
    public class LogoutCommand : IRequest<Unit>
    {
        public string RefreshToken { get; set; }
    }
}