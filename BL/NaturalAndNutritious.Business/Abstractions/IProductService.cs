using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;
using System.Security.Claims;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IProductService
    {
        Task<List<AllProductsDto>> FilterProductsWithPagination(int page, int pageSize);
        Task<int> TotalProducts();
        Task<ProductServiceResult> CreateProduct(CreateProductDto model, string dirPath);
        Task<string> CompleteFileOperations(UpdateProductDto model);
        double ApplyDiscount(double originalPrice, Discount discount);
        Task<List<MainProductDto>> GetProductsForHomePageAsync();
        Task<SearchDtoAsVm> ProductsForSearchFilter(string query, int page, int pageSize);
        Task<List<MainProductDto>> GetBestSellers(int topN);
        Task<List<MainProductDto>> GetVegetablesForVegetablesArea();
        Task<HomeFilterDtoAsVm> FilterProductsByCategories(string categoryFilter, int page, int pageSize);
        Task<List<MainProductDto>> ProductsForProductsController(int page, int pageSize);
        Task<int> TotalProductsForProductsController();
        Task<List<MainProductDto>> FilterProductsByCategoriesProductsController(Guid categoryId, int page, int pageSize);
        Task<List<MainProductDto>> GetAllDiscountedProducts();
        Task<List<RelatedProductsDto>> GetRelateProducts(Product product);
        Task<int> TotalProductsForProductsByCategory(Guid categoryId);
        Task AddReviewAsync(ReviewDto reviewDto, AppUser user);
        Task<IEnumerable<Product>> GetProductsByPriceAsync(double price);
        Task<DiscountedProductsDtoAsVm> GetDiscountedProducts(int page, int pageSize);
    }
}
