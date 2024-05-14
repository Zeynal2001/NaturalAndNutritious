using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IProfileService
    {
        Task<ProfileSeriviceResult> GetUserByIdAsync(string userId);
        Task<SettedModels> GetModelsForView(AppUser user);
        //Task<string> CompleteFileOperations(IFormFile? modelprofilePoto, string modelPotoUrl);
        Task<string> CompleteFileOperations(ProfileEditDto model);
        Task<EditResult> EditUserDetails(AppUser user, string profilePhotoUrl, ProfileEditDto model);
        Task<ChangePasswordResult> ChangeUserPasswordAsync(AppUser user, string currentPassword, string newPassword);
    }
}
