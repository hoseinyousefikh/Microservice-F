using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using MediatR;
using SharedKernel.Application.Abstractions.Services;
using SharedKernel.Common.Constants;

namespace Identity_Service.Application.Features.Authentication.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(
            IIdentityService identityService,
            IEmailService emailService)
        {
            _identityService = identityService;
            _emailService = emailService;
        }

        public async Task<Unit> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var user = await _identityService.FindByEmailAsync(email.Value);

            if (user == null || !await _identityService.IsEmailConfirmedAsync(user))
                return Unit.Value; // Don't reveal if user exists

            var token = await _identityService.GeneratePasswordResetTokenAsync(user);
            var resetLink = $"https://yourapp.com/reset-password?token={token}&email={email.Value}";

            await _emailService.SendPasswordResetEmailAsync(email.Value, resetLink);

            return Unit.Value;
        }
    }
}