using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using MediatR;
using SharedKernel.Common.Constants;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Users.Commands.DeleteMyAccount
{
    public class DeleteMyAccountCommandHandler : IRequestHandler<DeleteMyAccountCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserContextService _userContextService;
        private readonly ITokenService _tokenService;

        public DeleteMyAccountCommandHandler(
            IIdentityService identityService,
            IUserContextService userContextService,
            ITokenService tokenService)
        {
            _identityService = identityService;
            _userContextService = userContextService;
            _tokenService = tokenService;
        }

        public async Task<Unit> Handle(DeleteMyAccountCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedException();

            var user = await _identityService.FindByIdAsync(userId.Value);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            var passwordValid = await _identityService.CheckPasswordAsync(user, request.Password);
            if (!passwordValid)
                throw new BadRequestException(ErrorMessages.InvalidCurrentPassword);

            // Invalidate all tokens
            user.RefreshTokens.ToList().ForEach(rt => rt.Revoke());
            // After modifying the user entity (RefreshTokens), we need to update it in the database.
            await _identityService.UpdateAsync(user);

            var result = await _identityService.DeleteAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Unit.Value;
        }
    }
}