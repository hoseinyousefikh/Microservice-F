using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.Authentication;
using MediatR;
using SharedKernel.Common.Constants;

namespace Identity_Service.Application.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            IIdentityService identityService,
            ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }

        public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByNameAsync(request.Username);
            if (user == null)
                throw new BadRequestException(ErrorMessages.InvalidCredentials);

            if (!await _identityService.IsEmailConfirmedAsync(user))
                throw new BadRequestException(ErrorMessages.EmailNotConfirmed);

            var isValidPassword = await _identityService.CheckPasswordAsync(user, request.Password);
            if (!isValidPassword)
                throw new BadRequestException(ErrorMessages.InvalidCredentials);

            if (await _identityService.IsLockedOutAsync(user))
                throw new BadRequestException(ErrorMessages.AccountLocked);

            var token = await _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            // Record login
            await _identityService.SetLastLoginDateAsync(user);

            return new AuthResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600 // 1 hour
            };
        }
    }
}