using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions.RepoServiceInterfaces;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class AuthController : Controller
    {
        //public AuthController(IAuthService authService)
        //{
        //    _authService = authService;
        //}

        //private readonly IAuthService _authService;

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

        public IActionResult LogOut()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(object _)
        {
            ViewData["hasError"] = false;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(object _)
        {
            ViewData["hasError"] = false;

            return View();
        }
    }
}
