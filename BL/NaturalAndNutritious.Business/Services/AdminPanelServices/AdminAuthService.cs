using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class AdminAuthService : IAdminAuthService
    {
        public AdminAuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public async Task<AdminServicesResult> Login(AdminLoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new AdminServicesResult { Succeeded = false, IsNull = true, Message = "There isn't such admin." };
            }

            var res = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);

            if (!res.Succeeded)
            {
                return new AdminServicesResult { Succeeded = false, IsNull = false, Message = "Email or password is incorrect!" };
            }
            else
            {
                var userClaimsPrinc = await _signInManager.CreateUserPrincipalAsync(user);

                return new AdminServicesResult { Succeeded = true, IsNull = false, Message = "", UserClaimsPrincipal = userClaimsPrinc };
            }
        }
    }
}
