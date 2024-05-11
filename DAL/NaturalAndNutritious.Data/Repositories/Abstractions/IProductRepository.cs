using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories.Abstractions
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IQueryable<Product>> GetProductsByCategoryId(Guid categoryId);
        Task<IQueryable<Product>> GetProductsWithDiscounts();
        Task<IQueryable<Product>> GetProductsWithReviews();
        Task<Product> GetProductWithReviewsByProductId(Guid productId);
        Task<IQueryable<Product>> GetProductsByOrderId(Guid orderId);
    }
}
