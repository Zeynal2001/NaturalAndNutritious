using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class DiscountsController : Controller
    {
        public DiscountsController(IDiscountService discountService, IProductRepository productRepository)
        {
            _discountService = discountService;
            _productRepository = productRepository;
        }

        private readonly IDiscountService _discountService;
        private readonly IProductRepository _productRepository;

        [HttpGet]
        public async Task<IActionResult> CreateDiscount(string productId)
        {
            if (!Guid.TryParse(productId, out var Id))
            {
                throw new ArgumentException($"The id '{productId}' is not a valid GUID.", nameof(productId));
            }

            var product = await _productRepository.GetByIdAsync(Id);
            if (product == null)
            {
                var error = new ErrorModel { ErrorMessage = "Product is not found!" };
                return View("AdminError", error);
            }

            if (product.Discount != null)
            {
                var error = new ErrorModel { ErrorMessage = "The product already has a discount." };
                return View("AdminError", error);
            }

            var model = new CreateDiscountDto
            {
                ProductId = productId,
                ProductName = product.ProductName
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateDiscount(CreateDiscountDto model)
        {
            if (ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _discountService.CreateDiscount(model);

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

            return RedirectToAction("GetAllProducts", "Products");
        }
    }
}
