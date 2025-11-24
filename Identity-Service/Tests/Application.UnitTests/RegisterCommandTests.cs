//using Identity_Service.Application.Features.Authentication.Commands.Register;
//using Identity_Service.Infrastructure.Persistence.Entities;
//using Microsoft.AspNetCore.Identity;
//using Moq;
//using System.Threading;
//using Xunit;

//namespace Identity_Service.Tests.Application.UnitTests
//{
//    public class RegisterCommandTests
//    {
//        private readonly Mock<UserManager<User>> _mockUserManager;
//        private readonly Mock<RoleManager<Role>> _mockRoleManager;

//        public RegisterCommandTests()
//        {
//            _mockUserManager = new Mock<UserManager<User>>(
//                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);

//            _mockRoleManager = new Mock<RoleManager<Role>>(
//                Mock.Of<IRoleStore<Role>>(), null, null, null, null);
//        }

//        [Fact]
//        public async Task Handle_ValidRequest_ReturnsSuccessResponse()
//        {
//            // Arrange
//            var command = new RegisterCommand("test@example.com", "Password123!", "Test", "User");

//            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
//                .ReturnsAsync(IdentityResult.Success);
//            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
//                .ReturnsAsync(IdentityResult.Success);
//            _mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
//                .ReturnsAsync(true);

//            var handler = new RegisterCommandHandler(_mockUserManager.Object, _mockRoleManager.Object);

//            // Act
//            var result = await handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.NotNull(result);
//            Assert.True(result.IsSuccessful);
//            Assert.Equal("User registered successfully.", result.Message);
//        }

//        [Fact]
//        public async Task Handle_InvalidPassword_ReturnsErrorResponse()
//        {
//            // Arrange
//            var command = new RegisterCommand("test@example.com", "weak", "Test", "User");
//            var identityError = new IdentityError { Description = "Password is too weak." };
//            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
//                .ReturnsAsync(IdentityResult.Failed(identityError));

//            var handler = new RegisterCommandHandler(_mockUserManager.Object, _mockRoleManager.Object);

//            // Act
//            var result = await handler.Handle(command, CancellationToken.None);

//            // Assert
//            Assert.NotNull(result);
//            Assert.False(result.IsSuccessful);
//            Assert.Contains("Password is too weak.", result.Message);
//        }
//    }
//}
