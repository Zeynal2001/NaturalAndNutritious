using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;

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
        Task<List<MainProductDto>> ProductsForBestsellerArea();
        Task<List<MainProductDto>> GetVegetablesForVegetablesArea();
    }
}
