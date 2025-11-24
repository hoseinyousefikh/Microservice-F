using AutoMapper;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Users.Queries.GetAllUsers
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, PagedList<UserSummaryResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetAllUsersQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<PagedList<UserSummaryResponseDto>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            // IIdentityService.GetAllUsersAsync() returns all users.
            // We perform the pagination in memory after fetching all users.
            // For a large number of users, it would be more efficient to add pagination parameters
            // (pageNumber, pageSize) to the IIdentityService.GetAllUsersAsync method and its implementation.
            var allUsers = await _identityService.GetAllUsersAsync();

            var totalCount = allUsers.Count;
            var pagedUsers = allUsers
                .OrderBy(u => u.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var userDtos = _mapper.Map<List<UserSummaryResponseDto>>(pagedUsers);

            return new PagedList<UserSummaryResponseDto>(
                userDtos.AsReadOnly(),
                request.PageNumber,
                request.PageSize,
                totalCount);
        }
    }
}