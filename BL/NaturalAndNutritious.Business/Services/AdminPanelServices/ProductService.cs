using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class ProductService : IProductService
    {
        public ProductService(AppDbContext context, IProductRepository productRepository, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository, IStorageService storageService)
        {
            _context = context;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
            _storageService = storageService;
        }

        private readonly AppDbContext _context;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStorageService _storageService;

        public async Task<List<AllProductsDto>> FilterProductsWithPagination(int page, int pageSize)
        {
            if (page == 0 && pageSize == 0)
            {
                throw new ArgumentException();
            }

            var products = await  _context.Products 
                .Include(p => p.Discount)
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
                    ReOrderLevel = p.ReorderLevel,
                    Category = p.Category.CategoryName,
                    SubCategory = p.SubCategory.SubCategoryName,
                    Supplier = p.Supplier.CompanyName,
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    Discount = p.Discount.DiscountRate
                }).ToListAsync();

            return products;
        }

        public async Task<int> TotalProducts()
        {
            return await _context.Products.CountAsync();
        }

        public async Task<ProductServiceResult> CreateProduct(CreateProductDto model, string dirPath)
        {
            var affected = 0;
            if (model != null && dirPath != null)
            {
                if (!Guid.TryParse(model.Category, out var categoryId))
                {
                    throw new ArgumentException($"The id '{model.Category}' is not a valid GUID.", nameof(model.Category));
                }
                if (!Guid.TryParse(model.SubCategory, out var subCategoryId))
                {
                    throw new ArgumentException($"The id '{model.SubCategory}' is not a valid GUID.", nameof(model.SubCategory));
                }
                if (!Guid.TryParse(model.Supplier, out var supplierId))
                {
                    throw new ArgumentException($"The id '{model.Supplier}' is not a valid GUID.", nameof(model.Supplier));
                }

                var selectedCategory = await _categoryRepository.GetByIdAsync(categoryId);
                var selectedSubCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
                var selectedSupplier = await _supplierRepository.GetByIdAsync(supplierId);

                if (selectedCategory == null)
                {
                    return new ProductServiceResult { Succeeded = false, IsNull = true, Message = "Category not found!" };
                }
                if (selectedSubCategory == null)
                {
                    return new ProductServiceResult { Succeeded = false, IsNull = true, Message = "Subcategory not found!" };
                }
                if (selectedSupplier == null)
                {
                    return new ProductServiceResult { Succeeded = false, IsNull = true, Message = "Supplier not found!" };
                }

                var uploaded = await _storageService.UploadFileAsync(dirPath, model.ProductImage);
                var productImageUrl = uploaded.FullPath;

                if (_storageService is LocalStorageService)
                {
                    productImageUrl = $"uploads/{productImageUrl}";
                }

                var product = new Product()
                {
                    Id = Guid.NewGuid(),
                    ProductName = model.ProductName,
                    ProductImageUrl = productImageUrl,
                    ProductPrice = model.ProductPrice,
                    Description = model.Description,
                    ShortDescription = model.ShortDescription,
                    ViewsCount = 0,
                    Discontinued = false,
                    UnitsInStock = model.UnitsInStock,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    CategoryId = selectedCategory.Id,
                    SubCategoryId = selectedSubCategory.Id,
                    SupplierId = selectedSupplier.Id
                };

                await _productRepository.CreateAsync(product);
                affected = await _productRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    return new ProductServiceResult
                    {
                        Succeeded = false,
                        IsNull = false,
                        Message = "Product is not created, The table is not affected."
                    };
                }

                return new ProductServiceResult
                {
                    Succeeded = true,
                    IsNull = false,
                    Message = "Product created successfully."
                };
            }
            else
            {
                return new ProductServiceResult { Succeeded = false, IsNull = true, Message = "Model is null." };
            }
        }
    }
}
