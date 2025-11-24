using Identity_Service.Application.Features.Authentication.Commands;
using Identity_Service.Application.Features.Authentication.Commands.ForgotPassword;
using Identity_Service.Application.Features.Authentication.Commands.Login;
using Identity_Service.Application.Features.Authentication.Commands.Logout;
using Identity_Service.Application.Features.Authentication.Commands.RefreshToken;
using Identity_Service.Application.Features.Authentication.Commands.Register;
using Identity_Service.Application.Features.Authentication.Commands.ResetPassword;
using Identity_Service.Application.Features.Authentication.Queries;
using Identity_Service.Application.Features.Authentication.Queries.GetCurrentUser;
using Identity_Service.Presentation.Dtos.Requests.Authentication;
using Identity_Service.Presentation.Dtos.Responses.Authentication;
using Identity_Service.Presentation.Dtos.Responses.UserManagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Service.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody] LoginRequestDto request)
        {
            var command = new LoginCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
        {
            var command = new RefreshTokenCommand
            {
                AccessToken = request.AccessToken,
                RefreshToken = request.RefreshToken
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult> Logout([FromBody] RefreshTokenRequestDto request)
        {
            var command = new LogoutCommand
            {
                RefreshToken = request.RefreshToken
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            var command = new ForgotPasswordCommand
            {
                Email = request.Email
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            var command = new ResetPasswordCommand
            {
                Email = request.Email,
                Token = request.Token,
                NewPassword = request.NewPassword
            };

            await _mediator.Send(command);
            return Ok();
        }

        [HttpGet("current-user")]
        [Authorize]
        public async Task<ActionResult<UserProfileResponseDto>> GetCurrentUser()
        {
            var query = new GetCurrentUserQuery(User);
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}