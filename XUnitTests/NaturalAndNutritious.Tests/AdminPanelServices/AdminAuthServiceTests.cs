using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.AdminPanelServices;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Tests.AdminPanelServices
{
    public class AdminAuthServiceTests
    {
        private readonly Mock<UserManager<AppUser>> _userManagerMock;
        private readonly Mock<SignInManager<AppUser>> _signInManagerMock;
        private readonly AdminAuthService _adminAuthService;

        public AdminAuthServiceTests()
        {
            _userManagerMock = new Mock<UserManager<AppUser>>(MockBehavior.Strict, null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<AppUser>>(_userManagerMock.Object, new HttpContextAccessor(), new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object, null, null, null);
            _adminAuthService = new AdminAuthService(_userManagerMock.Object, _signInManagerMock.Object);
        }

        [Fact]
        public async Task Login_ValidAdmin_ReturnsSuccessResult()
        {
            // Arrange
            var adminLoginDto = new AdminLoginDto
            {
                Email = "admin@example.com",
                Password = "password"
            };
            var adminUser = new AppUser
            {
                Id = "adminId",
                Email = adminLoginDto.Email
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(adminLoginDto.Email))
                            .ReturnsAsync(adminUser);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(adminUser, adminLoginDto.Password, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            // Act
            var result = await _adminAuthService.Login(adminLoginDto);

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal("", result.Message);
            Assert.NotNull(result.UserClaimsPrincipal);
        }

        [Fact]
        public async Task Login_InvalidAdmin_ReturnsErrorResult()
        {
            // Arrange
            var adminLoginDto = new AdminLoginDto
            {
                Email = "admin@example.com",
                Password = "password"
            };

             _userManagerMock.Setup(m => m.FindByEmailAsync(adminLoginDto.Email))
                            .ReturnsAsync((AppUser)null);

            // Act
            var result = await _adminAuthService.Login(adminLoginDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsNull);
            Assert.Equal("There isn't such admin.", result.Message);
            Assert.Null(result.UserClaimsPrincipal);
        }

        [Fact]
        public async Task Login_IncorrectPassword_ReturnsErrorResult()
        {
            // Arrange
            var adminLoginDto = new AdminLoginDto
            {
                Email = "admin@example.com",
                Password = "password"
            };
            var adminUser = new AppUser
            {
                Id = "adminId",
                Email = adminLoginDto.Email
            };

            _userManagerMock.Setup(m => m.FindByEmailAsync(adminLoginDto.Email))
                            .ReturnsAsync(adminUser);
            _signInManagerMock.Setup(m => m.CheckPasswordSignInAsync(adminUser, adminLoginDto.Password, false))
                              .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _adminAuthService.Login(adminLoginDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal("Email or password is incorrect!", result.Message);
            Assert.Null(result.UserClaimsPrincipal);
        }
    }
}
