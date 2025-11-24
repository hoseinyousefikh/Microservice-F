using FluentValidation;
using Identity_Service.Presentation.Dtos.Responses.Authentication;
using MediatR;

namespace Identity_Service.Application.Features.Authentication.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponseDto>
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(x => x.Username)
                .NotEmpty();

            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}