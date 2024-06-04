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
        public AuthController(IAdminAuthService adminAuthService, UserManager<AppUser> userManager, ILogger<AuthController> logger)
        {
            _adminAuthService = adminAuthService;
            _userManager = userManager;
            _logger = logger;
        }

        private readonly IAdminAuthService _adminAuthService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<AuthController> _logger;

        [Area("admin_panel")]
        public IActionResult Login()
        {
            _logger.LogInformation("Login GET action called.");
            return View();
        }

        public IActionResult CreateAdmin()
        {
            _logger.LogInformation("CreateAdmin GET action called.");
            return View();
        }

        [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
        public async Task<IActionResult> LogOut()
        {
            _logger.LogInformation("LogOut action called.");
            await HttpContext.SignOutAsync(scheme: "AdminAuth");
            _logger.LogInformation("User logged out successfully.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AdminLoginDto model)
        {
            _logger.LogInformation("Login POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            var result = await _adminAuthService.Login(model);

            if (result.IsNull)
            {
                _logger.LogWarning("Login failed: {Message}", result.Message);
                ModelState.AddModelError("adminAuthError", result.Message);
                return View(model);
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Login unsuccessful: {Message}", result.Message);
                ModelState.AddModelError("adminLoginError", result.Message);
                return View(model);
            }

            var userClaimsPrinc = result.UserClaimsPrincipal;
            await HttpContext.SignInAsync("AdminAuth", userClaimsPrinc);

            _logger.LogInformation("User logged in successfully.");
            return RedirectToActionPermanent("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAdmin(string passphrase)
        {
            _logger.LogInformation("CreateAdmin POST action called.");

            if (string.IsNullOrWhiteSpace(passphrase))
            {
                _logger.LogWarning("Passphrase is empty.");
                ModelState.AddModelError("authError", "Passphrase can't be empty");
                return View();
            }

            if (passphrase == "<lM{5sdDJ02[")
            {
                _logger.LogInformation("Passphrase is correct. Creating admin user.");

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
                        _logger.LogInformation("Admin user created and role assigned successfully.");
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        var errorString = roleRes.Errors.ErrorsToString();
                        _logger.LogError("Role assignment failed: {Errors}", errorString);
                        ModelState.AddModelError("authError", errorString);
                        return View();
                    }
                }

                var createErrorString = res.Errors.ErrorsToString();
                _logger.LogError("User creation failed: {Errors}", createErrorString);
                ModelState.AddModelError("authError", createErrorString);
                return View();
            }
            else
            {
                _logger.LogWarning("Incorrect passphrase.");
                ModelState.AddModelError("authError", "Passphrase is incorrect");
                return View();
            }
        }
    }
}
