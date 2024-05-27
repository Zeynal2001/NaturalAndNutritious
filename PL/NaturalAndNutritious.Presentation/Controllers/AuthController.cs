using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(IAuthService authService, IStorageService storageService, IUsersService usersService)
        {
            _authService = authService;
            _storageService = storageService;
            _usersService = usersService;
        }

        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;
        private readonly IUsersService _usersService;

        public IActionResult Login(string? returnUrl)
        {
            ViewData["title"] = "Login";
            ViewData["hasError"] = false;

            return View();
        }

        public IActionResult Register()
        {
            ViewData["title"] = "Register";
            ViewData["hasError"] = false;

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            await _authService.LogOut();

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl)
        {
            ViewData["hasError"] = false;

            if (!ModelState.IsValid)
            {
                ViewData["hasError"] = true;
                return View(model);
            }

            var result = await _authService.Login(model);

            if (result.IsNull)
            {
                ViewData["hasError"] = true;
                ModelState.AddModelError("loginErors", result.Message);
                return View(model);
            }

            if (!result.Succeeded)
            {
                ViewData["hasError"] = true;
                ModelState.AddModelError("loginErors", result.Message);
                return View(model);
            }

            if (returnUrl != null)
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            ViewData["hasError"] = false;

            if (!ModelState.IsValid)
            {
                ViewData["hasError"] = true;
                return View(model);
            }

            var result = await _authService.Register(model, "profile-photos");

            if (!result.Succeeded)
            {
                ViewData["hasError"] = true;
                ModelState.AddModelError("registerErrors", result.Message);
                return View(model);
            }

            if (result.Succeeded)
            {
                TempData["successMsg"] = result.Message;
                return RedirectToAction(nameof(Login));
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            var currentUserId = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (currentUserId == null)
            {
                ViewData["msg"] = "You are not logged in!";

                return View("Error");
            }

            await _authService.LogOut();

            var result = await _usersService.DeleteUser(currentUserId, userId);

            if (result.IsNull)
            {
                ViewData["msg"] = result.Message;

                return View("Error");
            }

            if (!result.IsDeleted)
            {
                ViewData["msg"] = result.Message;

                return View("Error");
            }

            return RedirectToAction(nameof(Login));
        }
    }
}
