using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions.RepoServiceInterfaces
{
    public interface IProductService2
    {
        //public Product Products { get; }
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> FilterWithPagination(int page = 0, int size = 0);
        Task<Product?> GetProductByIdAsync(Guid productId);
        Task CreateProductAsync(Product product);
        Task<bool> UpdateAsync(Product updatedProduct);
        Task<bool> DeleteProductAsync(Guid productId);
        Task<int> SaveChangesAsync();
        //---------------------------ADDITIONAL-----------------------------
        Task<IEnumerable<Product>> GetProductsByCategoryId(Guid categoryId);
        Task<IEnumerable<Product>> GetProductsWithDiscounts();
        Task<IEnumerable<Product>> GetProductsWithReviews();
        Task<Product> GetProductWithReviewsByProductId(Guid productId);
        Task<IEnumerable<Product>> GetProductsByOrderId(Guid orderId);
    }
}
