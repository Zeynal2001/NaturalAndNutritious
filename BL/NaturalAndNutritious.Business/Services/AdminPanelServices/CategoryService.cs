using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class CategoryService : ICategoryService
    {
        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<int> TotalCategories()
        {
            return await _context.Categories.CountAsync();
        }
    }
}
