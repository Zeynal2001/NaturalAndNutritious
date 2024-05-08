using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;

namespace NaturalAndNutritious.Data.Repositories
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
            return await Task.Run(() => 
                _context.Categories
                .Include(c => c.SubCategories)
                .FirstOrDefault(c => c.SubCategories
                .Any(sc => sc.Id == subCategoryId)));
        }

        public async Task<IQueryable<Product>> GetProductsBySubCategoryId(Guid subCategoryId)
        {
            return await Task.Run(() => 
                _context.Products
                .Where(p => p.SubCategory.Id == subCategoryId));
        }
    }
}
