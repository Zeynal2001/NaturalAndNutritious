using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Presentation.ViewModels;
using NuGet.Common;
using System.Security.Claims;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class AuthController : Controller
    {
        public AuthController(IAuthService authService, IStorageService storageService, IUserService usersService, ILogger<AuthController> logger, UserManager<AppUser> userManager, IEmailService emailService, IUserRepository userRepository)
        {
            _authService = authService;
            _storageService = storageService;
            _usersService = usersService;
            _logger = logger;
            _userManager = userManager;
            _emailService = emailService;
            _userRepository = userRepository;
        }

        private readonly IAuthService _authService;
        private readonly IStorageService _storageService;
        private readonly IUserService _usersService;
        private readonly IEmailService _emailService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<AppUser> _userManager;

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

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} was not found.", model.Email);
                ViewData["msg"] = "No users with this email address could be found.";
                return View("Error");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Auth", new { token, email = model.Email }, protocol: Request.Scheme);

            await _emailService.SendPasswordResetEmailAsync(model.Email, resetLink);
            _logger.LogInformation("Password reset email sent to {Email}.", model.Email);

            return RedirectToAction(nameof(ForgotPasswordConfirmation));
        }

        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        public IActionResult ResetPassword(string token = null)
        {
            if (token == null)
            {
                _logger.LogError("A code must be supplied for password reset.");
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Token = token };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userRepository.FindUserByEmailAsync(model.Email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} was not found.", model.Email);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("Password reset successful for user with email {Email}.", model.Email);
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }

            foreach (var error in result.Errors)
            {
                _logger.LogError("Error resetting password for user with email {Email}: {Error}", model.Email, error.Description);
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }
    }
}
