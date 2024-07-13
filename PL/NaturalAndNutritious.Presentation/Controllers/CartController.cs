using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Dtos;
using SessionMapper;

namespace NaturalAndNutritious.Presentation.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ILogger<CartController> _logger;

        public CartController(ILogger<CartController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            _logger.LogInformation("CartController Index method called.");

            try
            {
                ViewData["title"] = "Cart";

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred in Cart Index method: {Exception}", ex.ToString());
                ViewData["msg"] = "An unexpected error occurred.";
                return View("Error");
            }
        }

        [HttpPost]
        public IActionResult ProceedCheckout([FromBody] List<CheckoutModel> checkouts)
        {
            _logger.LogInformation("ProceedCheckout method called.");

            try
            {
                // Set checkouts in session
                HttpContext.Session.SetAsJson<List<CheckoutModel>>("checkouts", checkouts);

                _logger.LogInformation("Checkouts saved in session.");

                return Ok(new
                {
                    succeeded = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while proceeding checkout: {Exception}", ex.ToString());
                return StatusCode(500, "An error occurred while proceeding checkout.");
            }
        }
    }
}
