using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class HomeController : Controller
    {
        public HomeController(IProductRepository productRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _userRepository = userRepository;
        }

        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;

        public async Task<IActionResult> Index()
        {
            var products = await _productRepository.GetAllAsync();
            var users = await _userRepository.GetAllUsers();

            var budget = products.Sum(p => p.ProductPrice);
            var views = products.Sum(p => p.ViewsCount);

            var vm = new HomeVm()
            {
                TotalBudget = budget,
                TotalViews = views,
                TotalProducts = products.Count(),
                TotalUsers = users.Count()
            };

            return View(vm);
        }
    }
}
