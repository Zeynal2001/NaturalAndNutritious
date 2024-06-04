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
    public class SubCategoriesController : Controller
    {
        public SubCategoriesController(ISubCategoryService subcategoryService, ISubCategoryRepository subCategoryRepository, ICategoryRepository categoryRepository, ILogger<SubCategoriesController> logger)
        {
            _subcategoryService = subcategoryService;
            _subCategoryRepository = subCategoryRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        private readonly ISubCategoryService _subcategoryService;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<SubCategoriesController> _logger;

        public async Task<IActionResult> GetAllSubCategories(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllSubCategories action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var subCategoriesQueryable = await _subCategoryRepository.FilterWithPagination(page, pageSize);

            var subCategories = await subCategoriesQueryable
                .Include(sc => sc.Category)
                .OrderByDescending(sc => sc.CreatedAt)
                .Select(sc => new AllSubCategoriesDto()
                {
                    Id = sc.Id.ToString(),
                    SubCategoryName = sc.SubCategoryName,
                    CategoryName = sc.Category.CategoryName,
                    CreatedAt = sc.CreatedAt,
                    UpdatedAt = sc.UpdatedAt,
                }).ToListAsync();

            var totalSubCategories = await _subcategoryService.TotalSubcategories();

            var vm = new GetAllSubCategoriesVm()
            {
                SubCategories = subCategories,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalSubCategories / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalSubCategories} subcategories.", totalSubCategories);

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            _logger.LogInformation("Create GET action called.");

            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

            var categories = await categoriesAsQueryable
                .Where(c => c.IsDeleted == false)
                .ToListAsync();

            ViewData["categories"] = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubcategoryDto model)
        {
            _logger.LogInformation("Create POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            var result = await _subcategoryService.CreateSubcategory(model);

            if (result.IsNull)
            {
                _logger.LogWarning("Subcategory creation failed: {Message}", result.Message);
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            if (!result.Succeeded)
            {
                _logger.LogWarning("Subcategory creation failed: {Message}", result.Message);
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            _logger.LogInformation("Subcategory created successfully.");
            return RedirectToAction(nameof(GetAllSubCategories));
        }

        public async Task<IActionResult> Update(string Id)
        {
            _logger.LogInformation("Update GET action called with subcategoryId: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(Id));
            }

            var subCategory = await _subCategoryRepository.GetByIdAsync(guidId);

            if (subCategory == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such subcategory." };
                _logger.LogWarning("Subcategory with ID {Id} not found.", Id);
                return View("AdminError", errorModel);
            }

            var subCategoryDetails = new UpdateSubCategoryDto()
            {
                Id = subCategory.Id.ToString(),
                SubCategoryName = subCategory.SubCategoryName,
            };

            _logger.LogInformation("Subcategory details retrieved successfully for subcategoryId: {Id}", Id);
            return View(subCategoryDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateSubCategoryDto model)
        {
            _logger.LogInformation("Update POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                var errorMessage = $"The id '{model.Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(model.Id));
            }

            var subCategory = await _subCategoryRepository.GetByIdAsync(guidId);

            if (subCategory == null)
            {
                _logger.LogWarning("Subcategory with ID {Id} not found.", model.Id);
                ModelState.AddModelError("editError", "There isn't such subcategory.");
                return View(model);
            }

            subCategory.SubCategoryName = model.SubCategoryName;
            subCategory.UpdatedAt = DateTime.UtcNow;

            var isUpdated = await _subCategoryRepository.UpdateAsync(subCategory);
            var affected = await _subCategoryRepository.SaveChangesAsync();

            if (!isUpdated || affected == 0)
            {
                _logger.LogWarning("Subcategory update failed for subcategoryId: {Id}", model.Id);
                ModelState.AddModelError("updateError", "Subcategory not updated.");
                return View(model);
            }

            _logger.LogInformation("Subcategory updated successfully for subcategoryId: {Id}", model.Id);
            return RedirectToAction(nameof(GetAllSubCategories));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            _logger.LogInformation("Delete POST action called with subcategoryId: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(Id));
            }

            var isDeleted = await _subCategoryRepository.DeleteAsync(guidId);
            await _subCategoryRepository.SaveChangesAsync();

            if (!isDeleted)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such subcategory." };
                _logger.LogWarning("Subcategory with ID {Id} not found for deletion.", Id);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Subcategory deleted successfully for subcategoryId: {Id}", Id);
            return RedirectToAction(nameof(GetAllSubCategories));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string Id)
        {
            _logger.LogInformation("AssumingDeleted POST action called with subcategoryId: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(Id));
            }

            var subCategory = await _subCategoryRepository.GetByIdAsync(guidId);

            if (subCategory == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such subcategory." };
                _logger.LogWarning("Subcategory with ID {Id} not found.", Id);
                return View("AdminError", errorModel);
            }

            subCategory.IsDeleted = true;

            var isUpdated = await _subCategoryRepository.UpdateAsync(subCategory);
            await _subCategoryRepository.SaveChangesAsync();

            if (!isUpdated)
            {
                var errorModel = new ErrorModel { ErrorMessage = "Subcategory not updated." };
                _logger.LogWarning("Subcategory update to deleted failed for subcategoryId: {Id}", Id);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Subcategory marked as deleted successfully for subcategoryId: {Id}", Id);
            return RedirectToAction(nameof(GetAllSubCategories));
        }
    }
}
