using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using System.Data;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class UserService : IUserService
    {
        public UserService(AppDbContext context, UserManager<AppUser> userManager, IUserRepository userRepository, IStorageService storageService, RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _userRepository = userRepository;
            _storageService = storageService;
            _roleManager = roleManager;
        }

        private readonly AppDbContext _context;
        private readonly IStorageService _storageService;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public async Task<List<AllUsersDto>> GetAllUsersWithPaginate(int page = 0, int pageSize = 0)
        {
            return await _context.Users
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Where(usr => usr.IsDeleted == false)
                .Select(usr => new AllUsersDto()
                {
                    Id = usr.Id,
                    FullName = usr.FName + " " + usr.LName,
                    UserName = usr.UserName,
                    BirthDate = usr.BirthDate,
                    Email = usr.Email,
                    ProfilePhoto = usr.ProfilePhotoUrl,
                }).ToListAsync();
        }

        public async Task<int> TotalUers()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<EditResult> EditUserDetails(AppUser user, string profilePhotoUrl, EditUserDto model)
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
                        Message = "User details successfully updated."
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
                    Message = "Something went wrong:/"
                };
            }
        }

        public async Task<string> CompleteFileOperations(EditUserDto model)
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

        public async Task<IList<string>> GetUserRolesAsync(AppUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<List<IdentityRole>> GetAllRoles()
        {
            return await _context.Roles.ToListAsync();
        }

        public async Task<IdentityRole> GetRoleById(string roleId)
        {
            return await _roleManager.FindByIdAsync(roleId);
        }

        public async Task<bool> IsInRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.IsInRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> AddToRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.AddToRoleAsync(user, roleName);
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(AppUser user, string roleName)
        {
            return await _userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task<UsersServiceResult> DeleteUser(string currentUserId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return new UsersServiceResult
                {
                    Succeeded = false,
                    IsNull = true,
                    Message = "The given Id is incorrect or null."
                };
            }

            if (userId == currentUserId)
            {
                return new UsersServiceResult
                {
                    Succeeded = false,
                    Message = "You can't delete yourself!"
                };
            }

            var foundUser = await _userManager.FindByIdAsync(userId);

            if (foundUser == null)
            {
                return new UsersServiceResult
                {
                    Succeeded = false,
                    IsNull = true,
                    Message = "User not found!"
                };
            }

            var result = await _userRepository.DeleteUserAsync(foundUser);

            if (!result.Succeeded)
            {
                return new UsersServiceResult
                {
                    Succeeded = false,
                    IsDeleted = false,
                    IsNull = false,
                    Message = result.Errors.ErrorsToString()
                };
            }
            else
            {
                return new UsersServiceResult
                {
                    Succeeded = true,
                    IsDeleted = true,
                    IsNull = false
                };
            }
        }
    }
}
