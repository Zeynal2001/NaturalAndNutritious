using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class CategoryService : ICategoryService
    {
        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        private readonly ICategoryRepository _categoryRepository;

        public async Task<int> TotalCategories()
        {
            return await _categoryRepository.Table
                .OrderByDescending(c => c.CreatedAt)
                .CountAsync();
        }

        public async Task<CategoryServiceResult> CreateCategory(CreateCategoryDto model)
        {
            var result = 0;
            if (model != null)
            {
                var category = new Category()
                {
                    Id = Guid.NewGuid(),
                    CategoryName = model.CategoryName,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                };

                await _categoryRepository.CreateAsync(category);
                result = await _categoryRepository.SaveChangesAsync();

                if (result == 0)
                {
                    return new CategoryServiceResult
                    {
                        Succeeded = false,
                        IsNull = false,
                        Message = "Category is not created, The table is not affected."
                    };
                }

                return new CategoryServiceResult
                {
                    Succeeded = true,
                    IsNull = false,
                    Message = "Category created successfully."
                };
            }
            else
            {
                return new CategoryServiceResult { Succeeded = false, IsNull = true, Message = "Model is null." };
            }
        }
    }
}
