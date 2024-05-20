using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IProductService
    {
        Task<List<AllProductsDto>> FilterProductsWithPagination(int page, int pageSize);
        Task<int> TotalProducts();
        Task<ProductServiceResult> CreateProduct(CreateProductDto model, string dirPath);
    }
}
