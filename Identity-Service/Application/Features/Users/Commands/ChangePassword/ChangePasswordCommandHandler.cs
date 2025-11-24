using Identity_Service.Application.Common.Events;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Common.Services;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using MediatR;
using SharedKernel.Common.Constants;

namespace Identity_Service.Application.Features.Users.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserContextService _userContextService;
        private readonly IMediator _mediator;

        public ChangePasswordCommandHandler(
            IIdentityService identityService,
            IPasswordHasher passwordHasher,
            IUserContextService userContextService,
            IMediator mediator)
        {
            _identityService = identityService;
            _passwordHasher = passwordHasher;
            _userContextService = userContextService;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedException();

            var user = await _identityService.FindByIdAsync(userId.Value);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            var result = await _identityService.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _identityService.UpdateAsync(user);

            await _mediator.Publish(new UserPasswordChangedEvent(user.Id, user.Email.Value), cancellationToken);

            return Unit.Value;
        }
    }
}