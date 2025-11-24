using AutoMapper;
using Identity_Service.Application.Common.Exceptions;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDetailResponseDto>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<UserDetailResponseDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _identityService.FindByIdAsync(request.Id);
            if (user == null)
                throw new NotFoundException("User not found");

            return _mapper.Map<UserDetailResponseDto>(user);
        }
    }
}