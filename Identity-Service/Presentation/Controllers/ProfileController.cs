using Identity_Service.Application.Features.Users.Commands;
using Identity_Service.Application.Features.Users.Commands.ChangeEmail;
using Identity_Service.Application.Features.Users.Commands.ChangePassword;
using Identity_Service.Application.Features.Users.Commands.ChangeUsername;
using Identity_Service.Application.Features.Users.Commands.DeleteMyAccount;
using Identity_Service.Application.Features.Users.Commands.UpdateProfile;
using Identity_Service.Application.Features.Users.Queries;
using Identity_Service.Application.Features.Users.Queries.GetAllUsers;
using Identity_Service.Application.Features.Users.Queries.GetFilteredUsers;
using Identity_Service.Application.Features.Users.Queries.GetUserById;
using Identity_Service.Presentation.Dtos.Requests.UserManagement;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Service.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfileController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPut("change-password")]
        public async Task<ActionResult> ChangePassword([FromBody] ChangePasswordRequestDto request)
        {
            var command = new ChangePasswordCommand
            {
                CurrentPassword = request.CurrentPassword,
                NewPassword = request.NewPassword
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPut("change-email")]
        public async Task<ActionResult> ChangeEmail([FromBody] ChangeEmailRequestDto request)
        {
            var command = new ChangeEmailCommand
            {
                NewEmail = request.NewEmail,
                Password = request.Password
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPut("change-username")]
        public async Task<ActionResult> ChangeUsername([FromBody] ChangeUsernameRequestDto request)
        {
            var command = new ChangeUsernameCommand
            {
                NewUsername = request.NewUsername,
                Password = request.Password
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPut("update-profile")]
        public async Task<ActionResult> UpdateProfile([FromBody] UpdateUserProfileRequestDto request)
        {
            var command = new UpdateProfileCommand
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                ProfileImageUrl = request.ProfileImageUrl
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpDelete("delete-account")]
        public async Task<ActionResult> DeleteMyAccount([FromBody] DeleteMyAccountRequestDto request)
        {
            var command = new DeleteMyAccountCommand
            {
                Password = request.Password
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDetailResponseDto>> GetUserById(Guid id)
        {
            var query = new GetUserByIdQuery(id);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("all")]
        public async Task<ActionResult<PagedList<UserSummaryResponseDto>>> GetAllUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetAllUsersQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("filtered")]
        public async Task<ActionResult<PagedList<UserSummaryResponseDto>>> GetFilteredUsers(
     [FromQuery] string? searchTerm = null,
     [FromQuery] int? status = null,
     [FromQuery] int pageNumber = 1,
     [FromQuery] int pageSize = 10)
        {
            var query = new GetFilteredUsersQuery
            {
                SearchTerm = searchTerm,
                Status = status,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}