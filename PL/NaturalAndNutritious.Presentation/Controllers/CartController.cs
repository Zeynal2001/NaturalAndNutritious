using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NaturalAndNutritious.Presentation.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult AddToCart()
        {
            return View();
        }
        
        public IActionResult RemoveFromCart()
        {
            return View();
        }
    }
}
