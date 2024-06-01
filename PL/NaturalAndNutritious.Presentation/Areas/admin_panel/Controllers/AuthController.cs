using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Extensions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Enums;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    public class AuthController : Controller
    {
        public AuthController(IAdminAuthService adminAuthService, UserManager<AppUser> userManager)
        {
            _adminAuthService = adminAuthService;
            _userManager = userManager;
        }

        private readonly IAdminAuthService _adminAuthService;
        private readonly UserManager<AppUser> _userManager;

        [Area("admin_panel")]
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult CreateAdmin()
        {
            return View();
        }

        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(scheme: "AdminAuth");

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _adminAuthService.Login(model);

            if (result.IsNull)
            {
                ModelState.AddModelError("adminAuthError", result.Message);
                return View(model);
            }

            if (!result.Succeeded)
            {
                ModelState.AddModelError("adminLoginError", result.Message);
                return View(model);
            }

            var userClaimsPrinc = result.UserClaimsPrincipal;

            await HttpContext.SignInAsync("AdminAuth", userClaimsPrinc);

            return RedirectToActionPermanent("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(string passphrase)
        {
            if (string.IsNullOrWhiteSpace(passphrase))
            {
                ModelState.AddModelError("authError", "Passphrase can't be empty");
                return View();
            }

            if (passphrase == "<lM{5sdDJ02[")
            {
                var usr = new AppUser()
                {
                    Id = Guid.NewGuid().ToString(),
                    FName = "admin",
                    LName = "admin",
                    Email = "admin@gmail.com",
                    BirthDate = DateTime.UtcNow,
                    ProfilePhotoUrl = "",
                    UserName = "admin",
                };

                var res = await _userManager.CreateAsync(usr, "Admin123#");

                if (res.Succeeded)
                {
                    var roleRes = await _userManager.AddToRoleAsync(usr, nameof(RoleTypes.Admin));

                    if (roleRes.Succeeded)
                    {
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError("authError", roleRes.Errors.ErrorsToString());
                        return View();
                    }
                }

                ModelState.AddModelError("authError", res.Errors.ErrorsToString());
                return View();
            }
            else
            {
                ModelState.AddModelError("authError", "Passphrase is incorrect");
                return View();
            }
        }
    }
}
