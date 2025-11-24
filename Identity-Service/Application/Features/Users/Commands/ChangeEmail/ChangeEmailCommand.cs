using FluentValidation;
using MediatR;

namespace Identity_Service.Application.Features.Users.Commands.ChangeEmail
{
    public class ChangeEmailCommand : IRequest<Unit>
    {
        public string NewEmail { get; set; }
        public string Password { get; set; }
    }

    public class ChangeEmailCommandValidator : AbstractValidator<ChangeEmailCommand>
    {
        public ChangeEmailCommandValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}