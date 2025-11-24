using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.Authentication;
using MediatR;
using SharedKernel.Common.Constants;

namespace Identity_Service.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IIdentityService identityService,
            ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            var username = principal.Identity?.Name;

            if (username == null)
                throw new BadRequestException(ErrorMessages.InvalidToken);

            var user = await _identityService.FindByNameAsync(username);
            if (user == null)
                throw new BadRequestException(ErrorMessages.UserNotFound);

            if (!_tokenService.ValidateToken(request.RefreshToken))
                throw new BadRequestException(ErrorMessages.InvalidToken);

            var newAccessToken = await _tokenService.GenerateJwtToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = 3600 // 1 hour
            };
        }
    }
}