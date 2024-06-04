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
    public class CategoriesController : Controller
    {
        public CategoriesController(ICategoryRepository categoryRepository, ICategoryService categoryService, ILogger<CategoriesController> logger)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
            _logger = logger;
        }

        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoriesController> _logger;

        public async Task<IActionResult> GetAllCategories(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllCategories action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var categoriesQueryable = await _categoryRepository.FilterWithPagination(page, pageSize);

            var categories = await categoriesQueryable
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new AllCategoriesDto()
                {
                    Id = c.Id.ToString(),
                    CategoryName = c.CategoryName,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                }).ToListAsync();

            var totalCategories = await _categoryService.TotalCategories();

            var vm = new GetAllCategoriesVm()
            {
                Categories = categories,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCategories / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalCategories} categories.", totalCategories);

            return View(vm);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Create GET action called.");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            _logger.LogInformation("Create POST action called.");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid.");
                return View(model);
            }

            var result = await _categoryService.CreateCategory(model);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Category creation failed: {Message}", result.Message);
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            _logger.LogInformation("Category created successfully.");
            return RedirectToAction(nameof(GetAllCategories));
        }

        public async Task<IActionResult> Update(string categoryId)
        {
            _logger.LogInformation("Update GET action called with categoryId: {CategoryId}", categoryId);

            if (!Guid.TryParse(categoryId, out var guidId))
            {
                var errorMessage = $"The id '{categoryId}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(categoryId));
            }

            var category = await _categoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such category." };
                _logger.LogWarning("Category with ID {CategoryId} not found.", categoryId);
                return View("AdminError", errorModel);
            }

            var categoryDetails = new UpdateCategoryDto
            {
                Id = category.Id.ToString(),
                CategoryName = category.CategoryName,
            };

            ViewData["hasError"] = false;
            _logger.LogInformation("Category details retrieved successfully for categoryId: {CategoryId}", categoryId);
            return View(categoryDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCategoryDto model)
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

            var category = await _categoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                _logger.LogWarning("Category with ID {CategoryId} not found.", model.Id);
                ModelState.AddModelError("editError", "There isn't such category.");
                return View(model);
            }

            category.CategoryName = model.CategoryName;
            category.UpdatedAt = DateTime.UtcNow;

            var isUpdated = await _categoryRepository.UpdateAsync(category);
            var affected = await _categoryRepository.SaveChangesAsync();

            if (!isUpdated || affected == 0)
            {
                _logger.LogWarning("Category update failed for categoryId: {CategoryId}", model.Id);
                ModelState.AddModelError("updateError", "Category not updated.");
                return View(model);
            }

            _logger.LogInformation("Category updated successfully for categoryId: {CategoryId}", model.Id);
            return RedirectToAction(nameof(GetAllCategories));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string categoryId)
        {
            _logger.LogInformation("Delete POST action called with categoryId: {CategoryId}", categoryId);

            if (!Guid.TryParse(categoryId, out var guidId))
            {
                var errorMessage = $"The id '{categoryId}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(categoryId));
            }

            var isDeleted = await _categoryRepository.DeleteAsync(guidId);
            await _categoryRepository.SaveChangesAsync();

            if (!isDeleted)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such category." };
                _logger.LogWarning("Category with ID {CategoryId} not found for deletion.", categoryId);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Category deleted successfully for categoryId: {CategoryId}", categoryId);
            return RedirectToAction(nameof(GetAllCategories));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string categoryId)
        {
            _logger.LogInformation("AssumingDeleted POST action called with categoryId: {CategoryId}", categoryId);

            if (!Guid.TryParse(categoryId, out var guidId))
            {
                var errorMessage = $"The id '{categoryId}' is not a valid GUID.";
                _logger.LogError(errorMessage);
                throw new ArgumentException(errorMessage, nameof(categoryId));
            }

            var category = await _categoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                var errorModel = new ErrorModel { ErrorMessage = "There isn't such category." };
                _logger.LogWarning("Category with ID {CategoryId} not found.", categoryId);
                return View("AdminError", errorModel);
            }

            category.IsDeleted = true;

            var isUpdated = await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            if (!isUpdated)
            {
                var errorModel = new ErrorModel { ErrorMessage = "Category not updated." };
                _logger.LogWarning("Category update to deleted failed for categoryId: {CategoryId}", categoryId);
                return View("AdminError", errorModel);
            }

            _logger.LogInformation("Category marked as deleted successfully for categoryId: {CategoryId}", categoryId);
            return RedirectToAction(nameof(GetAllCategories));
        }
    }
}
