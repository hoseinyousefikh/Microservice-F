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
    [Route("api/admin")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<AdminController> _logger;
        private const string IdentityServiceBaseUrl = "https://localhost:7053/api/admin";

        public AdminController(IHttpClientFactory httpClientFactory, ILogger<AdminController> logger)
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

        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var queryString = Request.QueryString.ToUriComponent();
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{IdentityServiceBaseUrl}/users{queryString}");
                },
                "Get all users"
            );
        }

        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.GetAsync($"{IdentityServiceBaseUrl}/users/{id}");
                },
                "Get user by ID"
            );
        }

        [HttpPost("users")]
        public async Task<IActionResult> CreateUser([FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsJsonAsync($"{IdentityServiceBaseUrl}/users", request);
                },
                "Create user"
            );
        }

        [HttpPut("users/{id}")]
        public async Task<IActionResult> UpdateUser(string id, [FromBody] object request)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PutAsJsonAsync($"{IdentityServiceBaseUrl}/users/{id}", request);
                },
                "Update user"
            );
        }

        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.DeleteAsync($"{IdentityServiceBaseUrl}/users/{id}");
                },
                "Delete user"
            );
        }

        [HttpPost("users/{id}/activate")]
        public async Task<IActionResult> ActivateUser(string id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{IdentityServiceBaseUrl}/users/{id}/activate", null);
                },
                "Activate user"
            );
        }

        [HttpPost("users/{id}/deactivate")]
        public async Task<IActionResult> DeactivateUser(string id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{IdentityServiceBaseUrl}/users/{id}/deactivate", null);
                },
                "Deactivate user"
            );
        }

        [HttpPost("users/{id}/confirm-email")]
        public async Task<IActionResult> ConfirmUserEmail(string id)
        {
            return await ForwardRequest(
                () => {
                    var client = _httpClientFactory.CreateClient();
                    AddAuthorizationHeader(client);
                    return client.PostAsync($"{IdentityServiceBaseUrl}/users/{id}/confirm-email", null);
                },
                "Confirm user email"
            );
        }
    }
}