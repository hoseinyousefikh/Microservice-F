using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Roles.Commands.RevokeRoleFromUser
{
    public class RevokeRoleFromUserCommandHandler : IRequestHandler<RevokeRoleFromUserCommand, Unit>
    {
        private readonly IIdentityService _identityService;

        public RevokeRoleFromUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Unit> Handle(RevokeRoleFromUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException("User not found");

            var role = await _identityService.FindRoleByIdAsync(request.RoleId);
            if (role == null)
                throw new NotFoundException("Role not found");

            var userRoles = await _identityService.GetRolesAsync(user);
            if (!userRoles.Contains(role.Name))
                throw new BadRequestException("User does not have this role");

            var result = await _identityService.RemoveFromRoleAsync(user, role.Name);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Unit.Value;
        }
    }
}