using Microsoft.AspNetCore.Mvc;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class BlogsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["title"] = "Blogs";

            return View();
        }
    }
}
