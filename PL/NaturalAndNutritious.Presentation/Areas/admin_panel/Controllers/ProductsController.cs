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
        public ProductsController(IProductRepository productRepository, IProductService productService, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository, IDiscountRepository discountRepository, ILogger<ProductsController> logger)
        {
            _productRepository = productRepository;
            _productService = productService;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
            _discountRepository = discountRepository;
            _logger = logger;
        }

        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IProductService _productService;
        private readonly ILogger<ProductsController> _logger;

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

        public async Task<IActionResult> Update(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }

            var product = await _productRepository.GetByIdAsync(guidId);

            if (product == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such product"
                };

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

            return View(productDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateProductDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            int affected = 0;

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                throw new ArgumentException($"The id '{model.Id}' is not a valid GUID.", nameof(model.Id));
            }

            var product = await _productRepository.GetByIdAsync(guidId);

            if (product == null)
            {
                ModelState.AddModelError("editError", "There isn't such product.");
                return View(model);
            }

            product.ProductName = model.ProductName;
            product.ShortDescription = model.ShortDescription;
            product.Description = model.Description;
            product.ProductPrice = model.ProductPrice;
            product.UnitsInStock = model.UnitsInStock;
            product.ReorderLevel = model.ReorderLevel;
            product.UnitsOnOrder = model.UnitsOnOrder;
            product.ProductImageUrl = model.ProductImageUrl;
            product.UpdatedAt = DateTime.UtcNow;

            var isUpdated = await _productRepository.UpdateAsync(product);
            affected = await _productRepository.SaveChangesAsync();

            if (isUpdated == false && affected == 0)
            {
                ModelState.AddModelError("updateError", "Subcategory not updated.");

                return View(model);
            }

            return RedirectToAction(nameof(GetAllProducts));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string productId)
        {
            if (!Guid.TryParse(productId, out var guidId))
            {
                throw new ArgumentException($"The id '{productId}' is not a valid GUID.", nameof(productId));
            }

            var isDeleted = await _productRepository.DeleteAsync(guidId);
            await _productRepository.SaveChangesAsync();

            if (isDeleted == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such product.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllProducts));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string productId)
        {
            if (!Guid.TryParse(productId, out var guidId))
            {
                throw new ArgumentException($"The id '{productId}' is not a valid GUID.", nameof(productId));
            }
            var product = await _productRepository.GetByIdAsync(guidId);

            if (product == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such product.";

                return View("AdminError", errorModel);
            }

            product.IsDeleted = true;

            var isUpdated = await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Product not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllProducts));
        }

        [HttpPost]
        public async Task<IActionResult> Discontinued(string productId)
        {
            if (!Guid.TryParse(productId, out var guidId))
            {
                throw new ArgumentException($"The id '{productId}' is not a valid GUID.", nameof(productId));
            }
            var product = await _productRepository.GetByIdAsync(guidId);

            if (product == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such product.";

                return View("AdminError", errorModel);
            }

            product.Discontinued = true;

            var isDiscontinued = await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();

            if (isDiscontinued == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Product not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllProducts));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveDiscount(string productId)
        {
            if (!Guid.TryParse(productId, out var guidId))
            {
                throw new ArgumentException($"The id '{productId}' is not a valid GUID.", nameof(productId));
            }

            var product = await _productRepository.Table.Include(p => p.Discount).FirstOrDefaultAsync(p => p.Id == guidId);
            if (product == null)
            {
                var error = new ErrorModel { ErrorMessage = "Product is not found!" };
                return View("AdminError", error);
            }

            var discount = product.Discount;

            if (discount == null)
            {
                var error = new ErrorModel { ErrorMessage = "The product doesn't have a discount." };
                return View("AdminError", error);
            }

            var isDeleted = await _discountRepository.DeleteAsync(discount.Id);
            await _discountRepository.SaveChangesAsync();

            if (!isDeleted)
            {
                var errorModel = new ErrorModel { ErrorMessage = "Discount could not be deleted." };
                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllProducts));
        }
    }
}
