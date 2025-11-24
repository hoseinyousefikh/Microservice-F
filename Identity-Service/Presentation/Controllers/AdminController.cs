using Identity_Service.Application.Features.Admin.Commands;
using Identity_Service.Application.Features.Admin.Commands.ActivateUser;
using Identity_Service.Application.Features.Admin.Commands.AdminCreateUser;
using Identity_Service.Application.Features.Admin.Commands.AdminDeleteUser;
using Identity_Service.Application.Features.Admin.Commands.AdminUpdateUser;
using Identity_Service.Application.Features.Admin.Commands.DeactivateUser;
using Identity_Service.Application.Features.Admin.Queries;
using Identity_Service.Application.Features.Admin.Queries.GetFilteredUsers;
using Identity_Service.Application.Features.Roles.Commands;
using Identity_Service.Application.Features.Roles.Commands.AssignRoleToUser;
using Identity_Service.Application.Features.Roles.Commands.RevokeRoleFromUser;
using Identity_Service.Presentation.Dtos.Requests.Admin;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Service.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("users")]
        public async Task<ActionResult<UserDetailResponseDto>> AdminCreateUser([FromBody] AdminCreateUserRequestDto request)
        {
            var command = new AdminCreateUserCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Roles = request.Roles
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPut("users")]
        public async Task<ActionResult<UserDetailResponseDto>> AdminUpdateUser([FromBody] AdminUpdateUserRequestDto request)
        {
            var command = new AdminUpdateUserCommand
            {
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                ProfileImageUrl = request.ProfileImageUrl,
                Roles = request.Roles
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("users/{userId}")]
        public async Task<ActionResult> AdminDeleteUser(Guid userId)
        {
            var command = new AdminDeleteUserCommand { UserId = userId };
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("users/{userId}/activate")]
        public async Task<ActionResult> ActivateUser(Guid userId)
        {
            var command = new ActivateUserCommand { UserId = userId };
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("users/{userId}/deactivate")]
        public async Task<ActionResult> DeactivateUser(Guid userId)
        {
            var command = new DeactivateUserCommand { UserId = userId };
            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("roles/assign")]
        public async Task<ActionResult> AssignRoleToUser([FromBody] AssignRoleRequestDto request)
        {
            var command = new AssignRoleToUserCommand
            {
                UserId = request.UserId,
                RoleId = request.RoleId
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("roles/revoke")]
        public async Task<ActionResult> RevokeRoleFromUser([FromBody] AssignRoleRequestDto request)
        {
            var command = new RevokeRoleFromUserCommand
            {
                UserId = request.UserId,
                RoleId = request.RoleId
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("users/filtered")]
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