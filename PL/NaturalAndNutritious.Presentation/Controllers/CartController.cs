using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Presentation.Models;
using SessionMapper;
using System.Text.Json;

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
            return View();
        }

        [HttpPost]
        public IActionResult ProceedCheckout([FromBody] List<CheckoutModel> checkouts)
        {
            try
            {
                HttpContext.Session.SetAsJson<List<CheckoutModel>>("checkouts", checkouts);

                return Ok(new
                {
                    succeeded = true
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while proceeding checkout.");
                return StatusCode(500, "An error occurred while proceeding checkout.");
            }
        }
    }
}
