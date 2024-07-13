using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public DiscountsController(IDiscountService discountService, IProductRepository productRepository, ILogger<DiscountsController> logger)
        {
            _discountService = discountService;
            _productRepository = productRepository;
            _logger = logger;
        }

        private readonly IDiscountService _discountService;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<DiscountsController> _logger;

        [HttpGet]
        public async Task<IActionResult> CreateDiscount(string productId)
        {
            _logger.LogInformation("CreateDiscount GET action called with productId: {ProductId}", productId);

            if (!Guid.TryParse(productId, out var Id))
            {
                var errorMessage = $"The id '{productId}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(productId));
            }

            var product = await _productRepository.Table.Include(p => p.Discount).FirstOrDefaultAsync(p => p.Id == Id);
            if (product == null)
            {
                var error = new ErrorModel { ErrorMessage = "Product is not found!" };
                _logger.LogWarning("Product with ID {ProductId} not found", productId);
                return View("AdminError", error);
            }

            if (product.Discount != null)
            {
                var error = new ErrorModel { ErrorMessage = "The product already has a discount." };
                _logger.LogWarning("Product with ID {ProductId} already has a discount", productId);
                return View("AdminError", error);
            }

            var model = new CreateDiscountDto
            {
                ProductId = productId,
                ProductName = product.ProductName
            };

            _logger.LogInformation("Product with ID {ProductId} is valid for discount creation", productId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDiscount(CreateDiscountDto model)
        {
            _logger.LogInformation("CreateDiscount POST action called for productId: {ProductId}", model.ProductId);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid for productId: {ProductId}", model.ProductId);
                return View(model);
            }

            var result = await _discountService.CreateDiscount(model);

            if (result.IsNull)
            {
                var error = new ErrorModel { ErrorMessage = result.Message };
                _logger.LogWarning("Discount creation failed for productId: {ProductId}. Error: {ErrorMessage}", model.ProductId, result.Message);
                return View("AdminError", error);
            }

            if (!result.Succeeded)
            {
                var error = new ErrorModel { ErrorMessage = result.Message };
                _logger.LogWarning("Discount creation unsuccessful for productId: {ProductId}. Error: {ErrorMessage}", model.ProductId, result.Message);
                return View("AdminError", error);
            }

            _logger.LogInformation("Discount created successfully for productId: {ProductId}", model.ProductId);
            return RedirectToAction("GetAllProducts", "Products");
        }
    }
}