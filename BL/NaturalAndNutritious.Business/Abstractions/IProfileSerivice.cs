using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IProfileService
    {
        Task<ProfileSeriviceResult> GetUserByIdAsync(string userId);
        Task<SettedModels> GetModelsForView(AppUser user);
        //Task<string> CompleteFileOperations(IFormFile? modelprofilePoto, string modelPotoUrl);
        Task<string> CompleteFileOperations(ProfileEditDto model);
        Task<EditResult> EditUserDetails(AppUser user, string profilePhotoUrl, ProfileEditDto model);
        Task<ChangePasswordResult> ChangeUserPasswordResultAsync(AppUser user, string currentPassword, string newPassword);
    }
}
