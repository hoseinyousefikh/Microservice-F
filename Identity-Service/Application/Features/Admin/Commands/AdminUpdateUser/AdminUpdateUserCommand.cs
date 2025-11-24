using FluentValidation;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;

namespace Identity_Service.Application.Features.Admin.Commands.AdminUpdateUser
{
    public class AdminUpdateUserCommand : IRequest<UserDetailResponseDto>
    {
        public Guid UserId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public List<string> Roles { get; set; } = new();
    }

    public class AdminUpdateUserCommandValidator : AbstractValidator<AdminUpdateUserCommand>
    {
        public AdminUpdateUserCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.FirstName)
                .MaximumLength(50);

            RuleFor(x => x.LastName)
                .MaximumLength(50);

            RuleFor(x => x.PhoneNumber)
                .Matches(@"^\+?[1-9]\d{1,14}$").When(x => !string.IsNullOrEmpty(x.PhoneNumber))
                .WithMessage("Invalid phone number format.");

            RuleFor(x => x.ProfileImageUrl)
                .Must(uri => Uri.TryCreate(uri, UriKind.Absolute, out _))
                .When(x => !string.IsNullOrEmpty(x.ProfileImageUrl))
                .WithMessage("Invalid profile image URL.");

            RuleForEach(x => x.Roles)
                .NotEmpty();
        }
    }
}