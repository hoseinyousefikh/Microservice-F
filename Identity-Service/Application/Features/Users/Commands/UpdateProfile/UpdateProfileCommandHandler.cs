using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Entities.ValueObjects;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Users.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, Unit>
    {
        private readonly IIdentityService _identityService;
        private readonly IUserContextService _userContextService;

        public UpdateProfileCommandHandler(
            IIdentityService identityService,
            IUserContextService userContextService)
        {
            _identityService = identityService;
            _userContextService = userContextService;
        }

        public async Task<Unit> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            var userId = _userContextService.GetCurrentUserId();
            if (userId == null)
                throw new UnauthorizedException();

            var user = await _identityService.FindByIdAsync(userId.Value);
            if (user == null)
                throw new NotFoundException("User not found");

            var phoneNumber = request.PhoneNumber != null ? PhoneNumber.Create(request.PhoneNumber) : null;
            user.UpdateProfile(request.FirstName, request.LastName, phoneNumber, request.ProfileImageUrl);

            var result = await _identityService.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Unit.Value;
        }
    }
}