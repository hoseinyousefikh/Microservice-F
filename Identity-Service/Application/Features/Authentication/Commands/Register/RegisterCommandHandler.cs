using Identity_Service.Application.Common.Events;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Common.Services;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using Identity_Service.Domain.Enums;
using Identity_Service.Presentation.Dtos.Responses.Authentication;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Common.Constants;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;
        private readonly IMediator _mediator;

        public RegisterCommandHandler(
            IIdentityService identityService,
            ITokenService tokenService,
            IMediator mediator)
        {
            _identityService = identityService;
            _tokenService = tokenService;
            _mediator = mediator;
        }

        public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var phoneNumber = request.PhoneNumber != null ? PhoneNumber.Create(request.PhoneNumber) : null;

            if (await _identityService.FindByEmailAsync(email.Value) != null)
                throw new BadRequestException(ErrorMessages.DuplicateEmail);

            if (await _identityService.FindByNameAsync(request.Username) != null)
                throw new BadRequestException(ErrorMessages.DuplicateUsername);

            var user = new User(
                request.Username,
                email,
                null, // Password hash will be handled by IIdentityService
                request.FirstName,
                request.LastName,
                phoneNumber);

            var result = await _identityService.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            await _mediator.Publish(new UserRegisteredEvent(
                user.Id,
                user.Username,
                user.Email.Value,
                user.FirstName,
                user.LastName,
                UserStatus.PendingVerification), cancellationToken);

            var roles = await _identityService.GetRolesAsync(user);
            var token = await _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            return new AuthResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                ExpiresIn = 3600 // 1 hour
            };
        }
    }
}