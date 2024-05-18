using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class ProductService : IProductService
    {
        public ProductService(AppDbContext context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<List<AllProductsDto>> FilterProductsWithPagination(int page, int pageSize)
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

        public async Task<int> TotalProducts()
        {
            return await _context.Products.CountAsync();
        }
    }
}
