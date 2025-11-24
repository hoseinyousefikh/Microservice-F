using FluentValidation;
using MediatR;

namespace Identity_Service.Application.Features.Users.Commands.DeleteMyAccount
{
    public class DeleteMyAccountCommand : IRequest<Unit>
    {
        public string Password { get; set; }
    }

    public class DeleteMyAccountCommandValidator : AbstractValidator<DeleteMyAccountCommand>
    {
        public DeleteMyAccountCommandValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}