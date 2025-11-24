//using Identity_Service.Presentation.Dtos.Requests.Authentication;
//using System.Net;
//using System.Text;
//using System.Text.Json;
//using Xunit;

//namespace Identity_Service.Tests.Application.UnitTests
//{
//    public class AccountControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
//    {
//        private readonly CustomWebApplicationFactory<Program> _factory;
//        private readonly HttpClient _client;

//        public AccountControllerIntegrationTests(CustomWebApplicationFactory<Program> factory)
//        {
//            _factory = factory;
//            _client = _factory.CreateClient();
//        }

//        [Fact]
//        public async Task Register_ValidRequest_ReturnsCreatedStatusCode()
//        {
//            // Arrange
//            var registerRequest = new RegisterRequestDto
//            {
//                Email = "testuser@example.com",
//                Password = "Password123!",
//                FirstName = "Test",
//                LastName = "User"
//            };

//            // استفاده از JsonSerializerOptions برای حروف کوچک در JSON
//            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
//            var content = new StringContent(JsonSerializer.Serialize(registerRequest, options), Encoding.UTF8, "application/json");

//            // Act
//            var response = await _client.PostAsync("/api/account/register", content);

//            // Assert
//            // EnsureSuccessStatusCode استثنا پرتاب می‌کند اگر کد وضعیت موفقیت‌آمیز نباشد.
//            // بهتر است از HttpStatusCode مستقیم استفاده کنیم.
//            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
//        }
//    }
//}