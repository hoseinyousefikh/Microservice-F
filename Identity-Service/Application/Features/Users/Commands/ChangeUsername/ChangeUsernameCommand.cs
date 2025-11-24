using FluentValidation;
using MediatR;

namespace Identity_Service.Application.Features.Users.Commands.ChangeUsername
{
    public class ChangeUsernameCommand : IRequest<Unit>
    {
        public string NewUsername { get; set; }
        public string Password { get; set; }
    }

    public class ChangeUsernameCommandValidator : AbstractValidator<ChangeUsernameCommand>
    {
        public ChangeUsernameCommandValidator()
        {
            RuleFor(x => x.NewUsername)
                .NotEmpty()
                .MinimumLength(3)
                .MaximumLength(20);

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}