using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Presentation.Models;
using System.Diagnostics;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["title"] = "Home";

            return View();
        }

        public IActionResult Privacy()
        {
            ViewData["title"] = "Privacy";

            return View();
        }
        
        public IActionResult Contact()
        {
            ViewData["title"] = "Contact";

            return View();
        }



        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
