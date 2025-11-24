using Identity_Service.Application.Common.Events;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Common.Services;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Common.Constants;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediator _mediator;

        public ResetPasswordCommandHandler(
            IIdentityService identityService,
            IPasswordHasher passwordHasher,
            IMediator mediator)
        {
            _identityService = identityService;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var user = await _identityService.FindByEmailAsync(email.Value);

            if (user == null)
                throw new BadRequestException(ErrorMessages.UserNotFound);

            var result = await _identityService.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            user.ChangePassword(_passwordHasher.HashPassword(request.NewPassword));
            await _identityService.UpdateAsync(user);

            await _mediator.Publish(new UserPasswordChangedEvent(user.Id, user.Email.Value), cancellationToken);

            return Unit.Value;
        }
    }
}