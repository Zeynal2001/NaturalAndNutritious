using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Tests
{
    public class ProfileServiceTests
    {
        public ProfileServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _storageServiceMock = new Mock<IStorageService>();
            _profileService = new ProfileService(_userRepositoryMock.Object, _storageServiceMock.Object);
        }

        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IStorageService> _storageServiceMock;
        private readonly ProfileService _profileService;

        [Fact]
        public async Task GetUserByIdAsync_UserExists_ReturnsUser()
        {
            // Arrange
            var userId = "testId";
            var user = new AppUser { Id = userId, Email = "test@example.com" };
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync(user);

            // Act
            var result = await _profileService.GetUserByIdAsync(userId);

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal(user, result.FoundUser);
        }

        [Fact]
        public async Task GetUserByIdAsync_UserDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var userId = "testId";
            _userRepositoryMock.Setup(repo => repo.GetUserByIdAsync(userId)).ReturnsAsync((AppUser)null);

            // Act
            var result = await _profileService.GetUserByIdAsync(userId);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsNull);
            Assert.Equal("You aren't logged in or there isn't such user.", result.Message);
        }

        [Fact]
        public async Task GetModelsForView_UserNotNull_ReturnsProfileIndexModel()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "testId",
                FName = "Test",
                LName = "User",
                Email = "test@example.com",
                BirthDate = new System.DateTime(1990, 1, 1),
                ProfilePhotoUrl = "photo.jpg"
            };

            // Act
            var result = await _profileService.GetModelsForView(user);

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal(user.Id, result.ProfileIndexModel.Id);
            Assert.Equal(user.FullName, result.ProfileIndexModel.FullName);
            Assert.Equal(user.BirthDate, result.ProfileIndexModel.DateOfBirth);
            Assert.Equal(user.Email, result.ProfileIndexModel.Email);
            Assert.Equal(user.ProfilePhotoUrl, result.ProfileIndexModel.ProfilPhoto);
        }

        [Fact]
        public async Task GetModelsForView_UserIsNull_ReturnsError()
        {
            // Arrange
            AppUser user = null;

            // Act
            var result = await _profileService.GetModelsForView(user);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsNull);
            Assert.Equal("Something went wrong", result.Message);
        }

        [Fact]
        public async Task CompleteFileOperations_ProfilePhotoIsNull_ReturnsExistingUrl()
        {
            // Arrange
            var model = new ProfileEditDto
            {
                ProfilePhoto = null,
                ProfilePhotoUrl = "existingPhotoUrl.jpg"
            };

            // Act
            var result = await _profileService.CompleteFileOperations(model);

            // Assert
            Assert.Equal("existingPhotoUrl.jpg", result);
        }

        //Duzgun yazilmali olan
        [Fact]
        public async Task CompleteFileOperations_ProfilePhotoIsNotNull_DeletesOldAndUploadsNewPhoto()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var model = new ProfileEditDto
            {
                ProfilePhoto = mockFile.Object,
                ProfilePhotoUrl = "uploads/profile-photos/oldPhoto.jpg"
            };

            var photoName = Path.GetFileName(model.ProfilePhotoUrl);
            _storageServiceMock.Setup(s => s.HasFile("profile-photos", photoName)).Returns(true);
            _storageServiceMock.Setup(s => s.DeleteFileAsync("profile-photos", photoName)).Returns(Task.CompletedTask);
            _storageServiceMock.Setup(s => s.UploadFileAsync("profile-photos", model.ProfilePhoto))
                               .ReturnsAsync(new UploadFileDto { FullPath = "profile-photos/newPhoto.jpg" });

            // Act
            var result = await _profileService.CompleteFileOperations(model);

            // Assert
            Assert.Equal("profile-photos/newPhoto.jpg", result);
        }

        [Fact]
        public async Task EditUserDetails_ValidData_UpdatesUserDetails()
        {
            // Arrange
            var user = new AppUser { Id = "testId" };
            var profilePhotoUrl = "newPhotoUrl.jpg";
            var model = new ProfileEditDto
            {
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Email = "newemail@example.com",
                NickName = "NewNickName",
                BirthDate = new DateTime(1990, 1, 1)
            };

            var updateResult = IdentityResult.Success;

            _userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<AppUser>())).ReturnsAsync(updateResult);

            // Act
            var result = await _profileService.EditUserDetails(user, profilePhotoUrl, model);

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal("Profile successfully updated", result.Message);
        }

        [Fact]
        public async Task EditUserDetails_InvalidData_ReturnsError()
        {
            // Arrange
            AppUser user = null;
            var profilePhotoUrl = "newPhotoUrl.jpg";
            var model = new ProfileEditDto
            {
                FirstName = "NewFirstName",
                LastName = "NewLastName",
                Email = "newemail@example.com",
                NickName = "NewNickName",
                BirthDate = new DateTime(1990, 1, 1)
            };

            // Act
            var result = await _profileService.EditUserDetails(user, profilePhotoUrl, model);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsNull);
            Assert.Equal("Something went wrong", result.Message);
        }

        [Fact]
        public async Task ChangeUserPasswordAsync_ValidData_ChangesPassword()
        {
            // Arrange
            var user = new AppUser { Id = "testId" };
            var currentPassword = "currentPassword";
            var newPassword = "newPassword";

            //var updateResult = IdentityResult.Success;
            var changeResult = IdentityResult.Success;

            //_userRepositoryMock.Setup(repo => repo.UpdateUserAsync(It.IsAny<AppUser>())).ReturnsAsync(updateResult);

            _userRepositoryMock.Setup(repo => repo.ChangeUserPasswordAsync(It.IsAny<AppUser>(), currentPassword, newPassword))
                               .ReturnsAsync(changeResult);
            //_userRepositoryMock.Setup(repo => repo.ChangeUserPasswordAsync(user, currentPassword, newPassword))
            //                   .ReturnsAsync(new ChangePasswordResult { Succeeded = true });

            // Act
            var result = await _profileService.ChangeUserPasswordResultAsync(user, currentPassword, newPassword);

            // Assert
            Assert.True(result.Succeeded);
            Assert.False(result.IsNull);
            Assert.Equal("Password successfully changed, Profile successfully updated.", result.Message);
        }

        [Fact]
        public async Task ChangeUserPasswordAsync_InvalidData_ReturnsError()
        {
            // Arrange
            var user = new AppUser { Id = "testId" };
            var currentPassword = "currentPassword";
            var newPassword = "newPassword";

            var changeResult = IdentityResult.Failed(new IdentityError { Description = "Error changing password" });

            _userRepositoryMock.Setup(repo => repo.ChangeUserPasswordAsync(It.IsAny<AppUser>(), currentPassword, newPassword))
                               .ReturnsAsync(changeResult);
            //_userRepositoryMock.Setup(repo => repo.ChangeUserPasswordAsync(user, currentPassword, newPassword))
            //                   .ReturnsAsync(new ChangePasswordResult { Succeeded = false, Errors = new[] { "Error changing password" } });

            // Act
            var result = await _profileService.ChangeUserPasswordResultAsync(user, currentPassword, newPassword);

            // Assert
            Assert.False(result.Succeeded);
            Assert.True(result.IsNull);
            Assert.Equal("Error changing password", result.Message);
        }
    }
}
