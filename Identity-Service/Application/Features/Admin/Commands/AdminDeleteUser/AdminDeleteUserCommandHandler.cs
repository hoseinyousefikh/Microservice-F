using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using MediatR;
using SharedKernel.Common.Constants;

namespace Identity_Service.Application.Features.Admin.Commands.AdminDeleteUser
{
    public class AdminDeleteUserCommandHandler : IRequestHandler<AdminDeleteUserCommand, Unit>
    {
        private readonly IIdentityService _identityService;

        public AdminDeleteUserCommandHandler(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<Unit> Handle(AdminDeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByIdAsync(request.UserId);
            if (user == null)
                throw new NotFoundException(ErrorMessages.UserNotFound);

            var result = await _identityService.DeleteAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(string.Join(", ", result.Errors.Select(e => e.Description)));

            return Unit.Value;
        }
    }
}