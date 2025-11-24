using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using MediatR;

namespace Identity_Service.Application.Features.Authentication.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public LogoutCommandHandler(
            IIdentityService identityService,
            ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.RefreshToken);
            var username = principal.Identity?.Name;

            if (username != null)
            {
                var user = await _identityService.FindByNameAsync(username);
                if (user != null)
                {
                    // In a real implementation, you would invalidate the refresh token
                    // This would typically involve updating a token repository or similar
                    // For now, we'll just acknowledge the logout

                }
            }

            return Unit.Value;
        }
    }
}