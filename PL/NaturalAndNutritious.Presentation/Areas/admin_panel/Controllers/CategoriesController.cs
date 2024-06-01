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
        public CategoriesController(ICategoryRepository categoryRepository, ICategoryService categoryService)
        {
            _categoryRepository = categoryRepository;
            _categoryService = categoryService;
        }

        private readonly ICategoryRepository _categoryRepository;
        private readonly ICategoryService _categoryService;

        public async Task<IActionResult> GetAllCategories(int page = 1, int pageSize = 5)
        {
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

            return View(vm);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _categoryService.CreateCategory(model);

            if (!result.Succeeded)
            {
                var error = new ErrorModel { ErrorMessage = result.Message };
                return View("AdminError", error);
            }

            return RedirectToAction(nameof(GetAllCategories));
        }

        public async Task<IActionResult> Update(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var guidId))
            {
                throw new ArgumentException($"The id '{categoryId}' is not a valid GUID.", nameof(categoryId));
            }

            var category = await _categoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such category"
                };

                return View("AdminError", errorModel);
            }

            var categoryDetails = new UpdateCategoryDto()
            {
                Id = category.Id.ToString(),
                CategoryName = category.CategoryName,
            };

            ViewData["hasError"] = false;

            return View(categoryDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateCategoryDto model)
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

            var category = await _categoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                ModelState.AddModelError("editError", "There isn't such category.");
                return View(model);
            }

            category.CategoryName = model.CategoryName;
            category.UpdatedAt = DateTime.UtcNow;

            var isUpdated = await _categoryRepository.UpdateAsync(category);
            affected = await _categoryRepository.SaveChangesAsync();

            if (isUpdated == false && affected == 0)
            {
                ModelState.AddModelError("updateError", "Category not updated.");

                return View(model);
            }

            return RedirectToAction(nameof(GetAllCategories));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var guidId))
            {
                throw new ArgumentException($"The id '{categoryId}' is not a valid GUID.", nameof(categoryId));
            }

            var isDeleted = await _categoryRepository.DeleteAsync(guidId);
            await _categoryRepository.SaveChangesAsync();

            if (isDeleted == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such category.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllCategories));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string categoryId)
        {
            if (!Guid.TryParse(categoryId, out var guidId))
            {
                throw new ArgumentException($"The id '{categoryId}' is not a valid GUID.", nameof(categoryId));
            }
            var category = await _categoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such category.";

                return View("AdminError", errorModel);
            }

            category.IsDeleted = true;

            var isUpdated = await _categoryRepository.UpdateAsync(category);
            await _categoryRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Category not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllCategories));
        }
    }
}
