using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    public class AuthController : Controller
    {
        public AuthController(IAdminAuthService adminAuthService)
        {
            _adminAuthService = adminAuthService;
        }

        private readonly IAdminAuthService _adminAuthService;

        [Area("admin_panel")]
        public IActionResult Login()
        {
            return View();
        }
        
        public IActionResult CreateAdmin()
        {
            return View();
        }

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
        [ValidateAntiForgeryToken]
        public IActionResult CreateAdmin(string passphrase)
        {
            return View();
        }
    }
}
