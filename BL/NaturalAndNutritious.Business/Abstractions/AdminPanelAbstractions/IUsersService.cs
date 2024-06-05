using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface IUserService
    {
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<List<AllUsersDto>> GetAllUsersWithPaginate(int page = 0, int pageSize = 0);
        Task<int> TotalUers();
        Task<EditResult> EditUserDetails(AppUser user, string profilePhotoUrl, EditUserDto model);
        Task<string> CompleteFileOperations(EditUserDto model);
        Task<IList<string>> GetUserRolesAsync(AppUser user);
        Task<IdentityRole> GetRoleById(string roleId);
        Task<bool> IsInRoleAsync(AppUser user ,string roleName);
        Task<IdentityResult> AddToRoleAsync(AppUser user ,string roleName);
        Task<List<IdentityRole>> GetAllRoles();
        Task<UsersServiceResult> DeleteUser(string currentUserId, string userId);
    }
}
