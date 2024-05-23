using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;

        public HomeController(ILogger<HomeController> logger, IProductService productService)
        {
            _logger = logger;
            _productService = productService;
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

        public async Task<IActionResult> Search(string query, int page = 1)
        {
            ViewData["title"] = "Search";

            if (query == null)
            {
                ViewData["msg"] = "Query cant't be empity.";
                return View("Error");
            }

            var searchVm = await _productService.ProductsForSearchFilter(query, page, 9);

            return View(searchVm);
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
