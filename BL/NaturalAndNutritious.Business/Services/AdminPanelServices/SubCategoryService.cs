using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Repositories;
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
    public class SubCategoryService : ISubCategoryService
    {
        public SubCategoryService(AppDbContext context, ISubCategoryRepository subCategoryRepository, ICategoryRepository categoryRepository)
        {
            _context = context;
            _subCategoryRepository = subCategoryRepository;
            _categoryRepository = categoryRepository;
        }

        private readonly AppDbContext _context;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ICategoryRepository _categoryRepository;


        public async Task<SubCategoryServiceResult> CreateSubcategory(CreateSubcategoryDto model)
        {
            var affected = 0;
            if (model != null)
            {
                if (!Guid.TryParse(model.Category, out var categoryId))
                {
                    throw new ArgumentException($"The id '{model.Category}' is not a valid GUID.", nameof(model.Category));
                }

                //var selectedCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

                var selectedCategory = await _categoryRepository.GetByIdAsync(categoryId);

                if (selectedCategory == null)
                {
                    return new SubCategoryServiceResult { Succeeded = false, IsNull = true, Message = "Category not found!" };
                }

                var subCategory = new SubCategory()
                {
                    Id = Guid.NewGuid(),
                    SubCategoryName = model.SubCategoryName,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    CategoryId = selectedCategory.Id
                };

                await _subCategoryRepository.CreateAsync(subCategory);
                affected = await _subCategoryRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    return new SubCategoryServiceResult
                    {
                        Succeeded = false,
                        IsNull = false,
                        Message = "Subcategory is not created, The table is not affected."
                    };
                }

                return new SubCategoryServiceResult
                {
                    Succeeded = true,
                    IsNull = false,
                    Message = "Subcategory created successfully."
                };
            }
            else
            {
                return new SubCategoryServiceResult { Succeeded = false, IsNull = true, Message = "Model is null." };
            }
        }

        public async Task<int> TotalSubcategories()
        {
            return await _context.SubCategories.CountAsync();
        }
    }
}
