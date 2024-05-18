using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        new public async Task<List<AllProductsDto>> FilterWithPagination(int page, int pageSize)
        {
            if (page == 0 && pageSize == 0)
            {
                throw new ArgumentException();
            }

            var products = await _context.Products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Where(p => p.IsDeleted == false)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new AllProductsDto()
                {
                    Id = p.Id.ToString(),
                    ProductName = p.ProductName,
                    ProductImageUrl = p.ProductImageUrl,
                    ProductPrice = p.ProductPrice,
                    Discontinued = p.Discontinued,
                    UnitsInStock = p.UnitsInStock,
                    UnitsOnOrder = p.UnitsOnOrder,
                    ReOrderLevel = p.ReorderLevel

                    //CategoryName = p.Category.CategoryName,
                    //CategoryId = p.Category.Id.ToString(),
                    //Price = p.Price,
                    //ImageUrl = p.ImageUrl,
                    //IsDeleted = p.IsDeleted,
                }).ToListAsync();

            return products;
        }

        public Task<IQueryable<Product>> GetProductsByCategoryId(Guid categoryId)
        {
            //Burada sinxron olarak Pruductları alırıq
            var products = _context.Products
                .Where(p => p.Category.Id == categoryId);

            //Və burada da asinxron bir task içində onları return eləyirik.
            return Task.FromResult(products);
        }

        public Task<IQueryable<Product>> GetProductsByOrderId(Guid orderId)
        {
            var products = _context.Products
                .Where(p => p.Orders.Any(o => o.Id == orderId));

            return Task.FromResult(products);
        }

        public async Task<IQueryable<Product>> GetProductsWithDiscounts()
        {
            return await Task.Run(() => _context.Products
                .Include(p => p.Discount));
            //.Where(p => p.Discount != null)
        }

        public async Task<IQueryable<Product>> GetProductsWithReviews()
        {
            return await Task.Run(() => _context.Products
                .Include(p => p.Reviews));
        }


        public async Task<Product> GetProductWithReviewsByProductId(Guid productId)
        {
            var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product;
        }

        public Task<int> TotalProducts()
        {
            throw new NotImplementedException();
        }

        /*
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
            return await Task.Run(() => _context.Products
                .Include(p => p.Reviews));
        }

        public async Task<Product> GetProductWithReviewsByProductId(Guid productId)
        {
           var product = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Id == productId);

            return product;
        }
        */
    }
}
