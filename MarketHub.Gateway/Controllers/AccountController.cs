using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace MarketHub.Gateway.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AccountController> _logger;
        private const string IdentityServiceBaseUrl = "https://localhost:7053/api/account";

        public AccountController(IHttpClientFactory httpClientFactory, ILogger<AccountController> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        /// <summary>
        /// یک متد کمکی برای اضافه کردن هدر Authorization به درخواست‌های HttpClient
        /// این متد از تکرار کد جلوگیری می‌کند و مشکل فرمت هدر را اصلاح می‌کند.
        /// </summary>
        /// <param name="client">نمونه HttpClient</param>
        private void AddAuthorizationHeader(HttpClient client)
        {
            var authHeader = Request.Headers.Authorization.FirstOrDefault();
            if (authHeader != null && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        /// <summary>
        /// یک متد کمکی برای ارسال درخواست به سرویس پایین‌دستی و پردازش پاسخ
        /// این متد از تکرار کد برای مدیریت خطاها و پاسخ‌ها جلوگیری می‌کند.
        /// </summary>
        /// <param name="requestAction">تابعی که درخواست را اجرا می‌کند</param>
        /// <param name="operationName">نام عملیات برای لاگ‌گیری</param>
        /// <returns>نتیجه IActionResult</returns>
        private async Task<IActionResult> ForwardRequest(Func<Task<HttpResponseMessage>> requestAction, string operationName)
        {
            try
            {
                var response = await requestAction();

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("{OperationName} failed: {Error}", operationName, errorContent);

                    try
                    {
                        var errorJson = JsonSerializer.Deserialize<object>(errorContent);
                        return BadRequest(errorJson);
                    }
                    catch (JsonException)
                    {
                        return BadRequest(new { Message = $"{operationName} failed.", Details = errorContent });
                    }
                }

                var successResponse = await response.Content.ReadFromJsonAsync<object>();
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during {OperationName}", operationName);
                return StatusCode(500, new { Message = $"An error occurred during {operationName}." });
            }
        }
        // در پروژه Gateway، در AccountController

        [HttpGet("confirm-change-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmChangeEmail([FromQuery] string userId, [FromQuery] string token, [FromQuery] string newEmail)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    return client.GetAsync($"{IdentityServiceBaseUrl}/confirm-change-email?userId={userId}&token={token}&newEmail={newEmail}");
                },
                "Email change confirmation"
            );
        }
        [HttpGet("confirm-email")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    // این متد نیاز به احراز هویت ندارد، پس هدر اضافه نمی‌شود
                    return client.GetAsync($"{IdentityServiceBaseUrl}/confirm-email?userId={userId}&token={token}");
                },
                "Email confirmation"
            );
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client); // این متد نیاز به احراز هویت دارد
                    return client.PostAsJsonAsync($"{IdentityServiceBaseUrl}/change-password", request);
                },
                "Password change"
            );
        }

        [HttpPost("change-email")]
        [Authorize]
        public async Task<IActionResult> ChangeEmail([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client); // این متد نیاز به احراز هویت دارد
                    return client.PostAsJsonAsync($"{IdentityServiceBaseUrl}/change-email", request);
                },
                "Email change"
            );
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    // این متد نیاز به احراز هویت ندارد
                    return client.PostAsJsonAsync($"{IdentityServiceBaseUrl}/forgot-password", request);
                },
                "Forgot password"
            );
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string token, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    // این متد نیاز به احراز هویت ندارد
                    return client.PostAsJsonAsync($"{IdentityServiceBaseUrl}/reset-password?token={token}", request);
                },
                "Password reset"
            );
        }

        [HttpPut("edit-profile")]
        [Authorize]
        public async Task<IActionResult> EditProfile([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client); // این متد نیاز به احراز هویت دارد
                    return client.PutAsJsonAsync($"{IdentityServiceBaseUrl}/edit-profile", request);
                },
                "Profile edit"
            );
        }

        [HttpDelete("delete-account")]
        [Authorize]
        public async Task<IActionResult> DeleteAccount()
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client); // این متد نیاز به احراز هویت دارد
                    return client.DeleteAsync($"{IdentityServiceBaseUrl}/delete-account");
                },
                "Account deletion"
            );
        }
    }
}