using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class ProductsController : Controller
    {
        public ProductsController(IProductRepository productRepository, IProductService productService)
        {
            _productRepository = productRepository;
            _productService = productService;
        }

        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;

        public async Task<IActionResult> GetAllProducts(int page = 1, int pageSize = 5)
        {
            var products = await _productService.FilterProductsWithPagination(page, pageSize);

            var totalProducts = await _productService.TotalProducts();

            var vm = new GetAllProductsVm()
            {
                Products = products,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                PageSize = pageSize
            };

            return View(vm);
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(object _)
        {

            return View();
        }
    }
}
