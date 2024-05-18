using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(IAuthService authService, IStorageService storageService)
        {
            _authService = authService;
            _storageService = storageService;
        }

        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;

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
                return RedirectToAction("Login");
            }

            return View(model);
        }
    }
}
