using FluentValidation;
using IdentityService._03_EndPoints.DTOs.Requests;

namespace IdentityService._03_EndPoints.Validators
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequestDto>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.FirstName)
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.FirstName));

            RuleFor(x => x.LastName)
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.LastName));

            RuleFor(x => x.Roles)
                .NotEmpty().WithMessage("At least one role must be specified")
                .When(x => x.Roles != null);
        }
    }
}
