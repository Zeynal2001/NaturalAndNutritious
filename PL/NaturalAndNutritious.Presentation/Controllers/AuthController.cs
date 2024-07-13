using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(IAuthService authService, IStorageService storageService, IUserService usersService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _storageService = storageService;
            _usersService = usersService;
            _logger = logger;
        }

        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;
        private readonly IUserService _usersService;
        private readonly ILogger<AuthController> _logger;

        public IActionResult Login(string? returnUrl)
        {
            ViewData["title"] = "Login";
            ViewData["hasError"] = false;

            _logger.LogInformation("Login page accessed.");

            return View();
        }

        public IActionResult Register()
        {
            ViewData["title"] = "Register";
            ViewData["hasError"] = false;

            _logger.LogInformation("Register page accessed.");

            return View();
        }

        public async Task<IActionResult> LogOut()
        {
            try
            {
                await _authService.LogOut();
                _logger.LogInformation("User logged out successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while logging out: {Exception}", ex.ToString());
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginDto model, string? returnUrl)
        {
            _logger.LogInformation("Login attempt.");

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
                _logger.LogError("Login failed: {ErrorMessage}", result.Message);
                return View(model);
            }

            if (!result.Succeeded)
            {
                ViewData["hasError"] = true;
                ModelState.AddModelError("loginErors", result.Message);
                _logger.LogError("Login failed: {ErrorMessage}", result.Message);
                return View(model);
            }

            if (returnUrl != null)
            {
                _logger.LogInformation("User logged in successfully.");
                return Redirect(returnUrl);
            }

            _logger.LogInformation("User logged in successfully.");
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterDto model)
        {
            _logger.LogInformation("Registration attempt.");

            ViewData["hasError"] = false;

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                ViewData["hasError"] = true;
                return View(model);
            }

            var result = await _authService.Register(model, "profile-photos");

            if (!result.Succeeded)
            {
                ViewData["hasError"] = true;
                ModelState.AddModelError("registerErrors", result.Message);
                _logger.LogError("Registration failed: {ErrorMessage}", result.Message);
                return View(model);
            }

            if (result.Succeeded)
            {
                TempData["successMsg"] = result.Message;
                _logger.LogInformation("User registered successfully.");
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
                _logger.LogError("Delete failed: {ErrorMessage}", "User is not logged in.");
                return View("Error");
            }

            await _authService.LogOut();

            var result = await _usersService.DeleteUser(currentUserId, userId);

            if (result.IsNull)
            {
                ViewData["msg"] = result.Message;
                _logger.LogError("Delete failed: {ErrorMessage}", result.Message);
                return View("Error");
            }

            if (!result.IsDeleted)
            {
                ViewData["msg"] = result.Message;
                _logger.LogError("Delete failed: {ErrorMessage}", result.Message);
                return View("Error");
            }

            _logger.LogInformation("User deleted successfully.");
            return RedirectToAction(nameof(Login));
        }
    }
}
