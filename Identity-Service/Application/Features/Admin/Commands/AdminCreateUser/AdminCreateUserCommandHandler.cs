using AutoMapper;
using Identity_Service.Application.Common.Events;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Common.Services;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using Identity_Service.Domain.Enums;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SharedKernel.Common.Constants;

namespace Identity_Service.Application.Features.Admin.Commands.AdminCreateUser
{
    public class AdminCreateUserCommandHandler : IRequestHandler<AdminCreateUserCommand, UserDetailResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AdminCreateUserCommandHandler(
            IIdentityService identityService,
            IPasswordHasher passwordHasher,
            IMediator mediator,
            IMapper mapper)
        {
            _identityService = identityService;
            _passwordHasher = passwordHasher;
            _mediator = mediator;
            _mapper = mapper;
        }

        public async Task<UserDetailResponseDto> Handle(AdminCreateUserCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var phoneNumber = request.PhoneNumber != null ? PhoneNumber.Create(request.PhoneNumber) : null;

            if (await _identityService.FindByEmailAsync(email.Value) != null)
                throw new BadRequestException(ErrorMessages.DuplicateEmail);

            if (await _identityService.FindByNameAsync(request.Username) != null)
                throw new BadRequestException(ErrorMessages.DuplicateUsername);

            var user = new Domain.Entities.User(
                request.Username,
                email,
                _passwordHasher.HashPassword(request.Password),
                request.FirstName,
                request.LastName,
                phoneNumber);

            var result = await _identityService.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Assign roles
            foreach (var roleName in request.Roles)
            {
                var role = await _identityService.FindRoleByNameAsync(roleName);
                if (role == null)
                    throw new BadRequestException($"Role '{roleName}' not found");

                var roleResult = await _identityService.AddToRoleAsync(user, roleName);
                if (!roleResult.Succeeded)
                    throw new BadRequestException($"Failed to assign role '{roleName}'");
            }

            await _mediator.Publish(new UserRegisteredEvent(
                user.Id,
                user.Username,
                user.Email.Value,
                user.FirstName,
                user.LastName,
                UserStatus.Active), cancellationToken);

            return _mapper.Map<UserDetailResponseDto>(user);
        }
    }
}