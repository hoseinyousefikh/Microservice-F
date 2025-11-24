using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;

namespace Identity_Service.Application.Features.Users.Queries.GetUserById
{
    public class GetUserByIdQuery : IRequest<UserDetailResponseDto>
    {
        public Guid Id { get; }

        public GetUserByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}