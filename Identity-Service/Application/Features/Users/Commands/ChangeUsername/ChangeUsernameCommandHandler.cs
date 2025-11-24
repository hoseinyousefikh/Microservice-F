using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Common.Constants;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Users.Commands.ChangeUsername
{
    public class ChangeUsernameCommandHandler : IRequestHandler<ChangeUsernameCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserContextService _userContextService;

        public ChangeUsernameCommandHandler(
            IIdentityService identityService,
            IUserContextService userContextService)
        {
            _identityService = identityService;
            _userContextService = userContextService;
        }

        public async Task<Unit> Handle(ChangeUsernameCommand request, CancellationToken cancellationToken)
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

            if (await _identityService.FindByNameAsync(request.NewUsername) != null)
                throw new BadRequestException(ErrorMessages.DuplicateUsername);

            user.ChangeUsername(request.NewUsername);
            var result = await _identityService.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Unit.Value;
        }
    }
}