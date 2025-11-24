using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;

namespace Identity_Service.Application.Features.Users.Queries.GetFilteredUsers
{
    public class GetFilteredUsersQuery : IRequest<PagedList<UserSummaryResponseDto>>
    {
        public string? SearchTerm { get; set; }
        public int? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Default constructor
        public GetFilteredUsersQuery() { }

        // Constructor with parameters
        public GetFilteredUsersQuery(string? searchTerm, int? status, int pageNumber, int pageSize)
        {
            SearchTerm = searchTerm;
            Status = status;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}