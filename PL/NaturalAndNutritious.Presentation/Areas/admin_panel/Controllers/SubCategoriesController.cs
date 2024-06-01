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
        public SubCategoriesController(ISubCategoryService subcategoryService, ISubCategoryRepository subCategoryRepository, ICategoryRepository categoryRepository)
        {
            _subcategoryService = subcategoryService;
            _subCategoryRepository = subCategoryRepository;
            _categoryRepository = categoryRepository;
        }

        private readonly ISubCategoryService _subcategoryService;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;

        public async Task<IActionResult> GetAllSubCategories(int page = 1, int pageSize = 5)
        {
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

            return View(vm);
        }

        public async Task<IActionResult> Create()
        {
            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

             var categories =  await categoriesAsQueryable
                .Where(c => c.IsDeleted == false)
                .ToListAsync();

            ViewData["categories"] = categories;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSubcategoryDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _subcategoryService.CreateSubcategory(model);

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

            return RedirectToAction(nameof(GetAllSubCategories));
        }

        public async Task<IActionResult> Update(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }

            var category = await _subCategoryRepository.GetByIdAsync(guidId);

            if (category == null)
            {
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = "There isn't such subcategory"
                };

                return View("AdminError", errorModel);
            }

            var subCategoryDetails = new UpdateSubCategoryDto()
            {
                Id = category.Id.ToString(),
                SubCategoryName = category.SubCategoryName,
            };

            return View(subCategoryDetails);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateSubCategoryDto model)
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

            var subCategory = await _subCategoryRepository.GetByIdAsync(guidId);

            if (subCategory == null)
            {
                ModelState.AddModelError("editError", "There isn't such category.");
                return View(model);
            }

            subCategory.SubCategoryName = model.SubCategoryName;
            subCategory.UpdatedAt = DateTime.UtcNow;

            var isUpdated = await _subCategoryRepository.UpdateAsync(subCategory);
            affected = await _subCategoryRepository.SaveChangesAsync();

            if (isUpdated == false && affected == 0)
            {
                ModelState.AddModelError("updateError", "Subcategory not updated.");

                return View(model);
            }

            return RedirectToAction(nameof(GetAllSubCategories));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }

            var isDeleted = await _subCategoryRepository.DeleteAsync(guidId);
            await _subCategoryRepository.SaveChangesAsync();

            if (isDeleted == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such subcategory.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllSubCategories));
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }
            var subCategory = await _subCategoryRepository.GetByIdAsync(guidId);

            if (subCategory == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such category.";

                return View("AdminError", errorModel);
            }

            subCategory.IsDeleted = true;

            var isUpdated = await _subCategoryRepository.UpdateAsync(subCategory);
            await _subCategoryRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Category not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllSubCategories));
        }
    }
}
