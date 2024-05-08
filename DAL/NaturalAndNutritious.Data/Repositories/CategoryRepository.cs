using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;

namespace NaturalAndNutritious.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<IQueryable<Product>> GetProductsByCategoryId(Guid categoryId)
        {
            return await Task.Run(() => _context.Products
               //.Include(p => p.Category)
               .Where(p => p.Category.Id == categoryId));
        }

        public async Task<IQueryable<SubCategory>> GetSubCategoriesByCategoryId(Guid categoryId)
        {
            return await Task.Run(() => _context.SubCategories
                //.Include(sc => sc.Category)
                .Where(sc => sc.Category.Id == categoryId));
        }
    }
}
