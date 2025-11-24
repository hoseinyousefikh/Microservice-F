using AutoMapper;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;

namespace Identity_Service.Application.Features.Admin.Commands.AdminUpdateUser
{
    public class AdminUpdateUserCommandHandler : IRequestHandler<AdminUpdateUserCommand, UserDetailResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public AdminUpdateUserCommandHandler(
            IIdentityService identityService,
            IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<UserDetailResponseDto> Handle(AdminUpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var phoneNumber = request.PhoneNumber != null ? PhoneNumber.Create(request.PhoneNumber) : null;
            user.UpdateProfile(request.FirstName, request.LastName, phoneNumber, request.ProfileImageUrl);

            var result = await _identityService.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            // Update roles
            var currentRoles = await _identityService.GetRolesAsync(user);
            var rolesToRemove = currentRoles.Except(request.Roles).ToList();
            var rolesToAdd = request.Roles.Except(currentRoles).ToList();

            if (rolesToRemove.Any())
            {
                foreach (var roleName in rolesToRemove)
                {
                    var removeResult = await _identityService.RemoveFromRoleAsync(user, roleName);
                    if (!removeResult.Succeeded)
                        throw new BadRequestException($"Failed to remove role '{roleName}'");
                }
            }

            if (rolesToAdd.Any())
            {
                foreach (var roleName in rolesToAdd)
                {
                    var addResult = await _identityService.AddToRoleAsync(user, roleName);
                    if (!addResult.Succeeded)
                        throw new BadRequestException($"Failed to add role '{roleName}'");
                }
            }

            return _mapper.Map<UserDetailResponseDto>(user);
        }
    }
}