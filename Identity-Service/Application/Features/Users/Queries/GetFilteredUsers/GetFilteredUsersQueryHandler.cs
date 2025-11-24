using AutoMapper;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Enums;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Identity_Service.Application.Features.Users.Queries.GetFilteredUsers
{
    public class GetFilteredUsersQueryHandler : IRequestHandler<GetFilteredUsersQuery, PagedList<UserSummaryResponseDto>>
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public GetFilteredUsersQueryHandler(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService;
            _mapper = mapper;
        }

        public async Task<PagedList<UserSummaryResponseDto>> Handle(GetFilteredUsersQuery request, CancellationToken cancellationToken)
        {
            // IIdentityService.GetAllUsersAsync() returns all users.
            // We perform the filtering and pagination in memory after fetching all users.
            // For a large number of users, this is inefficient. It would be much better to
            // add filtering and pagination parameters to the IIdentityService method itself
            // and perform these operations in the database via the IdentityService implementation.
            var allUsers = await _identityService.GetAllUsersAsync();
            var query = allUsers.AsQueryable();

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(u =>
                    u.Username.Contains(request.SearchTerm) ||
                    u.Email.Value.Contains(request.SearchTerm) ||
                    (u.FirstName != null && u.FirstName.Contains(request.SearchTerm)) ||
                    (u.LastName != null && u.LastName.Contains(request.SearchTerm)));
            }

            if (request.Status.HasValue)
            {
                // Cast the int value from the request to the UserStatus enum for comparison
                // کد صحیح
                query = query.Where(u => u.Status.Id == request.Status.Value);
            }

            var totalCount = query.Count();
            var pagedUsers = query
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