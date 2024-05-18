using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
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

        public async Task<IActionResult> GetAllCategories(int page = 1 ,int pageSize = 5)
        {
            var categoriesQueryable =  await _categoryRepository.FilterWithPagination(page, pageSize);

            var categories = await categoriesQueryable
                .Select(c => new AllCategoriesDto()
                {
                    Id = c.Id.ToString(),
                    CategoryName = c.CategoryName,
                    CreatedAt = c.CreatedAt
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
    }
}
