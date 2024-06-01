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
        public ProductsController(IProductRepository productRepository, IProductService productService, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository, IDiscountRepository discountRepository, ILogger<ProductsController> logger, IStorageService storageService)
        {
            _productRepository = productRepository;
            _productService = productService;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
            _discountRepository = discountRepository;
            _storageService = storageService;
            _logger = logger;
        }

        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductService _productService;
        private readonly IStorageService _storageService;
        private readonly ILogger<ProductsController> _logger;

        public async Task<IActionResult> GetAllProducts(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllProducts action called with page: {Page}, pageSize: {PageSize}", page, pageSize);

            try
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

                _logger.LogInformation("GetAllProducts action completed successfully for page: {Page}", page);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while getting products for page: {Page}", page);
                return View("Error");
            }
        }

        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create action called");

            try
            {
                var categoriesAsQueryable = await _categoryRepository.GetAllAsync();
                var categories = await categoriesAsQueryable
                   .Where(c => c.IsDeleted == false)
                   .ToListAsync();

                var subCategoriesAsQueryable = await _subCategoryRepository.GetAllAsync();
                var subCategories = await subCategoriesAsQueryable
                    .Include(c => c.Category)
                   .Where(c => c.IsDeleted == false)
                   .ToListAsync();

                var suppliersAsQueryable = await _supplierRepository.GetAllAsync();
                var suppliers = await suppliersAsQueryable
                   .Where(c => c.IsDeleted == false)
                   .ToListAsync();

                ViewData["categories"] = categories;
                ViewData["subcategories"] = subCategories;
                ViewData["suppliers"] = suppliers;

                _logger.LogInformation("Create action completed successfully");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Create view");
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductDto model)
        {
            _logger.LogInformation("Create POST action called");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                return View(model);
            }

            try
            {
                var result = await _productService.CreateProduct(model, "product-images");

                if (result.IsNull)
                {
                    var error = new ErrorModel { ErrorMessage = result.Message };
                    _logger.LogWarning("Product creation failed: {Message}", result.Message);
                    return View("AdminError", error);
                }

                if (!result.Succeeded)
                {
                    var error = new ErrorModel { ErrorMessage = result.Message };
                    _logger.LogWarning("Product creation failed: {Message}", result.Message);
                    return View("AdminError", error);
                }

                _logger.LogInformation("Product created successfully");
                return RedirectToAction(nameof(GetAllProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the product");
                return View("Error");
            }
        }

        public async Task<IActionResult> Update(string Id)
        {
            _logger.LogInformation("Update action called with Id: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMsg = $"The id '{Id}' is not a valid GUID.";
                _logger.LogWarning(errorMsg);
                throw new ArgumentException(errorMsg, nameof(Id));
            }

            try
            {
                var product = await _productRepository.GetByIdAsync(guidId);

                if (product == null)
                {
                    var errorModel = new ErrorModel()
                    {
                        ErrorMessage = "There isn't such product"
                    };
                    _logger.LogWarning("Product not found for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                //TODO: Sonra burada product un kategoriya, subcat, supplier ini de update etmeyi yaz.
                var productDetails = new UpdateProductDto()
                {
                    Id = product.Id.ToString(),
                    ProductName = product.ProductName,
                    ShortDescription = product.ShortDescription,
                    Description = product.Description,
                    ProductPrice = product.ProductPrice,
                    UnitsInStock = product.UnitsInStock,
                    ReorderLevel = product.ReorderLevel,
                    UnitsOnOrder = product.UnitsOnOrder,
                    ProductImageUrl = product.ProductImageUrl
                };

                _logger.LogInformation("Product found for Id: {Id}", Id);
                return View(productDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with Id: {Id}", Id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProductDto model)
        {
            _logger.LogInformation("Update action called with model: {@model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for model: {@model}", model);
                return View(model);
            }

            int affected = 0;

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                var errorMsg = $"The id '{model.Id}' is not a valid GUID.";
                _logger.LogError(errorMsg);
                throw new ArgumentException(errorMsg, nameof(model.Id));
            }

            try
            {
                var product = await _productRepository.GetByIdAsync(guidId);

                if (product == null)
                {
                    _logger.LogWarning("Product not found for Id: {Id}", model.Id);
                    ModelState.AddModelError("editError", "There isn't such product.");
                    return View(model);
                }

                var productImageUrl = await _productService.CompleteFileOperations(model);

                product.ProductName = model.ProductName;
                product.ShortDescription = model.ShortDescription;
                product.Description = model.Description;
                product.ProductPrice = model.ProductPrice;
                product.UnitsInStock = model.UnitsInStock;
                product.ReorderLevel = model.ReorderLevel;
                product.UnitsOnOrder = model.UnitsOnOrder;
                product.ProductImageUrl = productImageUrl;
                product.UpdatedAt = DateTime.UtcNow;

                var isUpdated = await _productRepository.UpdateAsync(product);
                affected = await _productRepository.SaveChangesAsync();

                if (!isUpdated && affected == 0)
                {
                    _logger.LogWarning("Product update failed for Id: {Id}", model.Id);
                    ModelState.AddModelError("updateError", "Product not updated.");
                    return View(model);
                }

                _logger.LogInformation("Product successfully updated for Id: {Id}", model.Id);
                return RedirectToAction(nameof(GetAllProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with Id: {Id}", model.Id);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string productId)
        {
            _logger.LogInformation("Delete action called with productId: {productId}", productId);

            if (!Guid.TryParse(productId, out var guidId))
            {
                var errorMsg = $"The id '{productId}' is not a valid GUID.";
                _logger.LogWarning(errorMsg);
                throw new ArgumentException(errorMsg, nameof(productId));
            }

            try
            {
                var isDeleted = await _productRepository.DeleteAsync(guidId);
                await _productRepository.SaveChangesAsync();

                if (!isDeleted)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "There isn't such product."
                    };

                    _logger.LogWarning("Product deletion failed for productId: {productId}", productId);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Product successfully deleted for productId: {productId}", productId);
                return RedirectToAction(nameof(GetAllProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the product with productId: {productId}", productId);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string productId)
        {
            _logger.LogInformation("AssumingDeleted action called with productId: {productId}", productId);

            if (!Guid.TryParse(productId, out var guidId))
            {
                var errorMsg = $"The id '{productId}' is not a valid GUID.";
                _logger.LogWarning(errorMsg);
                throw new ArgumentException(errorMsg, nameof(productId));
            }

            var product = await _productRepository.GetByIdAsync(guidId);

            if (product == null)
            {
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "There isn't such product."
                };

                _logger.LogWarning("Product not found for productId: {productId}", productId);
                return View("AdminError", errorModel);
            }

            product.IsDeleted = true;

            try
            {
                var isUpdated = await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                if (!isUpdated)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "Product not updated."
                    };

                    _logger.LogWarning("Product update failed for productId: {productId}", productId);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Product marked as deleted for productId: {productId}", productId);
                return RedirectToAction(nameof(GetAllProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with productId: {productId}", productId);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Discontinued(string productId)
        {
            _logger.LogInformation("Discontinued action called with productId: {productId}", productId);

            if (!Guid.TryParse(productId, out var guidId))
            {
                var errorMsg = $"The id '{productId}' is not a valid GUID.";
                _logger.LogWarning(errorMsg);
                throw new ArgumentException(errorMsg, nameof(productId));
            }

            var product = await _productRepository.GetByIdAsync(guidId);

            if (product == null)
            {
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "There isn't such product."
                };

                _logger.LogWarning("Product not found for productId: {productId}", productId);
                return View("AdminError", errorModel);
            }

            product.Discontinued = true;

            try
            {
                var isDiscontinued = await _productRepository.UpdateAsync(product);
                await _productRepository.SaveChangesAsync();

                if (!isDiscontinued)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "Product not updated."
                    };

                    _logger.LogWarning("Product update failed for productId: {productId}", productId);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Product marked as discontinued for productId: {productId}", productId);
                return RedirectToAction(nameof(GetAllProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the product with productId: {productId}", productId);
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDiscount(string productId)
        {
            _logger.LogInformation("RemoveDiscount action called with productId: {productId}", productId);

            if (!Guid.TryParse(productId, out var guidId))
            {
                var errorMsg = $"The id '{productId}' is not a valid GUID.";
                _logger.LogWarning(errorMsg);
                throw new ArgumentException(errorMsg, nameof(productId));
            }

            try
            {
                var product = await _productRepository.Table.Include(p => p.Discount).FirstOrDefaultAsync(p => p.Id == guidId);
                if (product == null)
                {
                    var error = new ErrorModel { ErrorMessage = "Product is not found!" };
                    _logger.LogWarning("Product not found for productId: {productId}", productId);
                    return View("AdminError", error);
                }

                var discount = product.Discount;

                if (discount == null)
                {
                    var error = new ErrorModel { ErrorMessage = "The product doesn't have a discount." };
                    _logger.LogWarning("Product doesn't have a discount for productId: {productId}", productId);
                    return View("AdminError", error);
                }

                var isDeleted = await _discountRepository.DeleteAsync(discount.Id);
                await _discountRepository.SaveChangesAsync();

                if (!isDeleted)
                {
                    var errorModel = new ErrorModel { ErrorMessage = "Discount could not be deleted." };
                    _logger.LogWarning("Discount could not be deleted for productId: {productId}", productId);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Discount removed successfully for productId: {productId}", productId);
                return RedirectToAction(nameof(GetAllProducts));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while removing the discount for productId: {productId}", productId);
                return View("Error");
            }
        }
    }
}
