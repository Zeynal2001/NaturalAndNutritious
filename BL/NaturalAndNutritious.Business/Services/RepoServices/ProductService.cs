using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.RepoServiceInterfaces;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;

namespace NaturalAndNutritious.Business.Services.RepoServices
{
    public class ProductService : IProductService
    {
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        private readonly IProductRepository _productRepository;

        //bunu ProductRepository de new ile kohnesini hide eleyib yenisini yaradabilersen
        //public Product Products => _productRepository.Table; 

        public async Task CreateProductAsync(Product product)
        {
             await _productRepository.CreateAsync(product);
        }

        public async Task<bool> UpdateAsync(Product updatedProduct)
        {
            var updated = await _productRepository.UpdateAsync(updatedProduct);

            return updated;
        }

        public async Task<bool> DeleteProductAsync(Guid productId)
        {
            var deleted = await _productRepository.DeleteAsync(productId);

            return deleted;
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            var products = await _productRepository.GetAllAsync();

            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> FilterWithPagination(int page = 0, int size = 0)
        {
            var paginatedProducts = _productRepository.FilterWithPagination(page, size);

            return await paginatedProducts.ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(Guid productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);

            return product;
        }
        
        public async Task<int> SaveChangesAsync()
        {
            var savechanges = await _productRepository.SaveChangesAsync();

            return savechanges;
        }

        //---------------------------ADDITIONAL-----------------------------
        public async Task<IEnumerable<Product>> GetProductsByCategoryId(Guid categoryId)
        {
            var products = await _productRepository.GetProductsByCategoryId(categoryId);
                //products.Include(p => p.Category);
            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByOrderId(Guid orderId)
        {
            var products = await _productRepository.GetProductsByOrderId(orderId);
                //products.Include(p => p.Orders);
            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithDiscounts()
        {
            var products =  await _productRepository.GetProductsWithDiscounts();

            return await products.ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsWithReviews()
        {
            var products = await _productRepository.GetProductsWithReviews();

            return await products.ToListAsync();
        }

        public async Task<Product> GetProductWithReviewsByProductId(Guid productId)
        {
            var product = await _productRepository.GetProductWithReviewsByProductId(productId);

            return product;
        }
    }
}
