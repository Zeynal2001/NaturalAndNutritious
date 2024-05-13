using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Repositories
{
    public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
    {
        public SubCategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<Category> GetCategoryBySubCategoryId(Guid subCategoryId)
        {
            return await _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefaultAsync(c => c.SubCategories.Any(sc => sc.Id == subCategoryId));
        }

        public async Task<IQueryable<Product>> GetProductsBySubCategoryId(Guid subCategoryId)
        {
            return await Task.Run(() => 
                _context.Products
                .Where(p => p.SubCategory.Id == subCategoryId));
        }
    }
}
