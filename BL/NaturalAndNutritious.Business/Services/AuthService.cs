using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Enums;

namespace NaturalAndNutritious.Business.Services
{
    public class AuthService : IAuthService
    {
        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IStorageService storageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _storageService = storageService;
        }

        private readonly UserManager<AppUser> _userManager; //Cox vaxti register ucun istifade edilir
        private readonly SignInManager<AppUser> _signInManager; //Cox vaxti login ucun istifade edilir
        private readonly IStorageService _storageService;

        public async Task<ServiceResult> Login(LoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new ServiceResult { Succeeded = false, IsNull = true, Message = "Email or password is incorrect." };
            }

            var res = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);

            if (!res.Succeeded)
            {
                return new ServiceResult { Succeeded = false, IsNull = false, Message = "Email or password is incorrect." };
            }
            else
            {
                return new ServiceResult { Succeeded = true, IsNull = false, Message = "" };
            }
        }

        public async Task<ServiceResult> Register(RegisterDto model, string dirPath)
        {
            var profilePhoto = "";
            if (model.ProfilePhoto == null)
            {
                profilePhoto = "img/userr=.png";
            }
            else
            {
                var uploaded = await _storageService.UploadFileAsync(dirPath, model.ProfilePhoto);
                profilePhoto = uploaded.FullPath;

                if (_storageService is LocalStorageService)
                {
                    profilePhoto = $"uploads/{profilePhoto}";
                }
            }

            var user = new AppUser()
            {
                FName = model.FirstName,
                LName = model.LastName,
                Email = model.Email,
                BirthDate = model.BirthDate,
                UserName = model.FirstName + model.LastName,
                ProfilePhotoUrl = profilePhoto,
                IsDeleted = false,
                IsSubscribed = false,
            };

            var res = await _userManager.CreateAsync(user, model.Password);

            if (!res.Succeeded)
            {
                var regErrors = res.Errors.ErrorsToString();

                return new ServiceResult { Succeeded = false, IsNull = false, Message = regErrors };
            }

            var roleRes = await _userManager.AddToRoleAsync(user, nameof(RoleTypes.Client));

            if (roleRes.Succeeded)
            {
                return new ServiceResult { Succeeded = true, IsNull = false, Message = "Registered!" };
            }
            else
            {
                var errors = roleRes.Errors.ErrorsToString();

                return new ServiceResult { Succeeded = false, IsNull = false, Message = errors };
            }
        }


        public async Task LogOut()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
