using FluentValidation;
using IdentityService._03_EndPoints.DTOs.Requests;

namespace IdentityService._03_EndPoints.Validators
{
    public class ChangeEmailRequestValidator : AbstractValidator<ChangeEmailRequestDto>
    {
        public ChangeEmailRequestValidator()
        {
            RuleFor(x => x.NewEmail)
                .NotEmpty().WithMessage("New email is required")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
