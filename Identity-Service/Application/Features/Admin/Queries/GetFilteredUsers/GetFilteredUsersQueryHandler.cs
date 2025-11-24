using AutoMapper;
using Identity_Service.Application.Interfaces;
using Identity_Service.Domain.Entities;
using Identity_Service.Domain.Enums;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;

namespace Identity_Service.Application.Features.Admin.Queries.GetFilteredUsers
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
            // Get all users from the identity service
            var allUsers = await _identityService.GetAllUsersAsync();

            // Apply search filter if provided
            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                allUsers = allUsers.Where(u =>
                    u.Username.Contains(request.SearchTerm) ||
                    u.Email.Value.Contains(request.SearchTerm) ||
                    (u.FirstName != null && u.FirstName.Contains(request.SearchTerm)) ||
                    (u.LastName != null && u.LastName.Contains(request.SearchTerm))).ToList();
            }

            // Apply status filter if provided
            if (request.Status.HasValue)
            {
                allUsers = allUsers.Where(u => u.Status.Id == request.Status.Value).ToList();
            }

            // Apply pagination
            var totalCount = allUsers.Count;
            var users = allUsers
                .OrderBy(u => u.CreatedAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            var userDtos = _mapper.Map<List<UserSummaryResponseDto>>(users);

            return new PagedList<UserSummaryResponseDto>(
                userDtos.AsReadOnly(),
                request.PageNumber,
                request.PageSize,
                totalCount);
        }
    }
}