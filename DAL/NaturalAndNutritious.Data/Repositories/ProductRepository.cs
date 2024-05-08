using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;

namespace NaturalAndNutritious.Data.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<IQueryable<Product>> GetProductsByCategoryId(Guid categoryId)
        {
            return await Task.Run(() => _context.Products
                .Where(p => p.Category.Id == categoryId));
        }

        public async Task<IQueryable<Product>> GetProductsByOrderId(Guid orderId)
        {
            return await Task.Run(() => _context.Products
            //TODO: Include etmek istesen onlari Service lerde ele
                .Where(p => p.Orders.Any(o => o.Id == orderId)));
        }

        public async Task<IQueryable<Product>> GetProductsWithDiscounts()
        {
            return await Task.Run(() => _context.Products
                .Include(p => p.Discount));
                //.Where(p => p.Discount != null)
        }

        public async Task<IQueryable<Product>> GetProductsWithReviews()
        {
            return await Task.Run(() => _context.Products.Include(p => p.Reviews));
        }
    }
}
