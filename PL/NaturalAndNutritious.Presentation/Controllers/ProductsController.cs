using Microsoft.AspNetCore.Mvc;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            ViewData["title"] = "Products";

            return View();
        }
    }
}
