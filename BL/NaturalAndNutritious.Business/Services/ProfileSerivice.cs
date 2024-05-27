using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Services
{
    public class ProfileService : IProfileService
    {
        public ProfileService(IUserRepository userRepository, IStorageService storageService)
        {
            _userRepository = userRepository;
            _storageService = storageService;
        }

        private readonly IUserRepository _userRepository;
        private readonly IStorageService _storageService;

        public async Task<ProfileSeriviceResult> GetUserByIdAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);

            if (user == null)
            {
                return new ProfileSeriviceResult
                {
                    IsNull = true,
                    Succeeded = false,
                    Message = "You aren't logged in or there isn't such user."
                };
            }
            else
            {
                return new ProfileSeriviceResult { Succeeded = true, IsNull = false, FoundUser = user };
            }
        }

        public Task<SettedModels> GetModelsForView(AppUser user)
        {
            if (user != null)
            {
                return Task.FromResult(new SettedModels
                {
                    IsNull = false,
                    Succeeded = true,
                    ProfileIndexModel = new Dtos.ProfileIndexDto()
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        DateOfBirth = user.BirthDate,
                        Email = user.Email,
                        ProfilPhoto = user.ProfilePhotoUrl
                    }
                });
            }
            else
            {
                return Task.FromResult(new SettedModels
                {
                    IsNull = true,
                    Succeeded = false,
                    Message = "Something went wrong"
                });
            }
        }

        /// <summary>
        /// Completes the file operations.
        /// </summary>
        /// <param name="model.ProfilePhoto">The profile photo to upload.</param>
        /// <param name="model.ProfilePhotoUrl">The URL of the profile photo to upload.</param>
        /// <returns>The URL of the uploaded profile photo.</returns>
        public async Task<string> CompleteFileOperations(ProfileEditDto model)
        {
            string profilePhoto = "";

            if (model.ProfilePhoto == null)
            {
                profilePhoto = model.ProfilePhotoUrl;
            }
            else
            {
                var photoName = Path.GetFileName(model.ProfilePhotoUrl);
                if (_storageService.HasFile("profile-photos", photoName))
                {
                    await _storageService.DeleteFileAsync("profile-photos", photoName);
                }

                var dto = await _storageService.UploadFileAsync("profile-photos", model.ProfilePhoto);
                profilePhoto = dto.FullPath;

                if (_storageService is LocalStorageService)
                {
                    profilePhoto = $"uploads/{dto.FullPath}";
                }
            }

            return profilePhoto;
        }

        public async Task<EditResult> EditUserDetails(AppUser user, string profilePhotoUrl, ProfileEditDto model)
        {
            if (user != null && model != null)
            {
                user.FName = model.FirstName;
                user.LName = model.LastName;
                user.Email = model.Email;
                user.UserName = model.NickName;
                user.BirthDate = model.BirthDate;
                user.ProfilePhotoUrl = profilePhotoUrl;

                var result = await _userRepository.UpdateUserAsync(user);

                if (result.Succeeded)
                {
                    return new EditResult
                    {
                        Succeeded = true,
                        IsNull = false,
                        Message = "Profile successfully updated"
                    };
                }

                var updateErrors = result.Errors.ErrorsToString();

                return new EditResult { Succeeded = false, IsNull = false, Message = updateErrors };
            }
            else
            {
                return new EditResult
                {
                    Succeeded = false,
                    IsNull = true,
                    Message = "Something went wrong"
                };
            }
        }

        public async Task<ChangePasswordResult> ChangeUserPasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            if (user != null && currentPassword != null && newPassword != null)
            {
                var result = await _userRepository.ChangeUserPasswordAsync(user, currentPassword, newPassword);

                if (result.Succeeded)
                {
                    return new ChangePasswordResult
                    {
                        Succeeded = true,
                        IsNull = false,
                        Message = "Password successfully changed, Profile successfully updated."
                    };
                }
                else
                {
                    return new ChangePasswordResult
                    {
                        Succeeded = false,
                        IsNull = true,
                        Message = result.Errors.ErrorsToString()
                    };
                }
            }
            else
            {
                return new ChangePasswordResult
                {
                    Succeeded = false,
                    IsNull = true,
                    Message = "Something went wrong."
                };
            }
        }
    }
}
