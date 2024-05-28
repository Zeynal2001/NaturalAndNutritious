using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Enums;

namespace NaturalAndNutritious.Tests
{
    public class AuthServiceTests
    {
        [Fact]
        public async Task Login_SuccessfulLogin_ReturnsSuccessResult()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<AppUser>>(MockBehavior.Strict);
            var signInManagerMock = new Mock<SignInManager<AppUser>>(userManagerMock.Object,
                                                                     new Mock<IHttpContextAccessor>().Object,
                                                                     new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                                                                     null, null, null, null);

            var authService = new AuthService(userManagerMock.Object, signInManagerMock.Object, null);

            var user = new AppUser { Email = "test@example.com" };
            userManagerMock.Setup(m => m.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            signInManagerMock.Setup(m => m.PasswordSignInAsync(user, It.IsAny<string>(), false, false))
                             .ReturnsAsync(SignInResult.Success);

            var loginDto = new LoginDto
            {
                Email = "test@example.com",
                Password = "Password123",
                RememberMe = false
            };

            // Act
            var result = await authService.Login(loginDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Empty(result.Message);
        }

        [Fact]
        public async Task Register_SuccessfulRegistration_ReturnsSuccessResult()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<AppUser>>(MockBehavior.Strict);
            var signInManagerMock = new Mock<SignInManager<AppUser>>(userManagerMock.Object,
                                                                     new Mock<IHttpContextAccessor>().Object,
                                                                     new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                                                                     null, null, null, null);
            var storageServiceMock = new Mock<IStorageService>();
            var authService = new AuthService(userManagerMock.Object, signInManagerMock.Object, storageServiceMock.Object);

            storageServiceMock.Setup(s => s.UploadFileAsync(It.IsAny<string>(), It.IsAny<IFormFile>()))
                              .ReturnsAsync(new UploadedFile { FullPath = "uploaded/photo.jpg" });

            var user = new AppUser { Email = "john@example.com" };
            userManagerMock.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                           .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), nameof(RoleTypes.Client)))
                           .ReturnsAsync(IdentityResult.Success);

            var registerDto = new RegisterDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                BirthDate = new DateTime(1990, 1, 1),
                Password = "Password123",
                ProfilePhoto = null
            };

            // Act
            var result = await authService.Register(registerDto, "uploads");

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal("Registered!", result.Message);
        }

        [Fact]
        public async Task LogOut_LogoutUser()
        {
            // Arrange
            var userManagerMock = new Mock<UserManager<AppUser>>(MockBehavior.Strict);
            var signInManagerMock = new Mock<SignInManager<AppUser>>(userManagerMock.Object,
                                                                     new Mock<IHttpContextAccessor>().Object,
                                                                     new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                                                                     null, null, null, null);
            var authService = new AuthService(userManagerMock.Object, signInManagerMock.Object, null);

            signInManagerMock.Setup(m => m.SignOutAsync()).Returns(Task.CompletedTask);

            // Act
            await authService.LogOut();

            // Assert
            signInManagerMock.Verify(m => m.SignOutAsync(), Times.Once);
        }
    }
}