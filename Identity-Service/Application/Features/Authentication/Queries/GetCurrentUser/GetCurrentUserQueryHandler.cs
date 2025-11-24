using AutoMapper;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using System.Security.Claims;
using System;

namespace Identity_Service.Application.Features.Authentication.Queries.GetCurrentUser
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetCurrentUserQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<UserProfileResponseDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userIdString = request.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedException();

            var user = await _identityService.FindByIdAsync(userId);
            if (user == null)
                throw new NotFoundException("User not found");

            return _mapper.Map<UserProfileResponseDto>(user);
        }
    }
}