using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class ProductsController : Controller
    {
        public ProductsController(IProductRepository productRepository, IProductService productService, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository)
        {
            _productRepository = productRepository;
            _productService = productService;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
        }

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ISupplierRepository _supplierRepository;
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

        public async Task<IActionResult> Create()
        {
            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();
            var categories = await categoriesAsQueryable
               .Where(c => c.IsDeleted == false)
               .ToListAsync();

            var subCategoriesAsQueryable = await _subCategoryRepository.GetAllAsync();
            var subCategories = await subCategoriesAsQueryable
               .Where(c => c.IsDeleted == false)
               .ToListAsync();

            var suppliersAsQueryable = await _supplierRepository.GetAllAsync();
            var suppliers = await suppliersAsQueryable
               .Where(c => c.IsDeleted == false)
               .ToListAsync();

            ViewData["categories"] = categories;
            ViewData["subcategories"] = subCategories;
            ViewData["suppliers"] = suppliers;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _productService.CreateProduct(model, "product-images");

            if (result.IsNull)
            {
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            if (!result.Succeeded)
            {
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            return RedirectToAction(nameof(GetAllProducts));
        }
    }
}
