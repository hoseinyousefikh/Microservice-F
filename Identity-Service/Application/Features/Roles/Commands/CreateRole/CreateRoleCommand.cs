using FluentValidation;
using Identity_Service.Presentation.Dtos.Responses.Roles;
using MediatR;

namespace Identity_Service.Application.Features.Roles.Commands.CreateRole
{
    public class CreateRoleCommand : IRequest<RoleDetailResponseDto>
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }

    public class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(50);

            RuleFor(x => x.Description)
                .MaximumLength(200);
        }
    }
}