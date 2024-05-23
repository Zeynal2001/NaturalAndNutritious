using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class ProductService : IProductService
    {
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository, IStorageService storageService, IDiscountRepository discountRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
            _storageService = storageService;
            _discountRepository = discountRepository;
        }

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IStorageService _storageService;
        private readonly IDiscountRepository _discountRepository;

        public async Task<List<AllProductsDto>> FilterProductsWithPagination(int page, int pageSize)
        {
            if (page == 0 && pageSize == 0)
            {
                throw new ArgumentException();
            }

            var products = await _productRepository.Table
                .Include(p => p.Discount)
                .Where(p => p.IsDeleted == false)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    Discount = p.Discount.DiscountRate,
                    DiscountType = p.Discount.DiscountType
                }).ToListAsync();

            return products;
        }

        public async Task<int> TotalProducts()
        {
            return await _productRepository.Table
                        .Include(p => p.Discount)
                        .Where(p => p.IsDeleted == false)
                        .OrderByDescending(p => p.CreatedAt)
                        .CountAsync();
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

        public async Task<string> CompleteFileOperations(UpdateProductDto model)
        {
            string productImageUrl = "";

            if (model.ProductImage == null)
            {
                productImageUrl = model.ProductImageUrl;
            }
            else
            {
                var photoName = Path.GetFileName(model.ProductImageUrl);
                if (_storageService.HasFile("product-images", photoName))
                {
                    await _storageService.DeleteFileAsync("product-images", photoName);
                }

                var dto = await _storageService.UploadFileAsync("product-images", model.ProductImage);
                productImageUrl = dto.FullPath;

                if (_storageService is LocalStorageService)
                {
                    productImageUrl = $"uploads/{dto.FullPath}";
                }
            }

            return productImageUrl;
        }

        public async Task<List<MainProductDto>> GetProductsForHomePageAsync()
        {
            var productsAsQueryable = await _productRepository.GetAllAsync();
            //var reviewStar = productsAsQueryable.Where(p => p.Reviews.Any(r => r.Rating)).ToList();
            var products = await productsAsQueryable
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                .Include(p => p.Discount)
                .ToListAsync();

            var productDetailDtos = new List<MainProductDto>();
            

            foreach (var product in products)
            {
                var discount = await _discountRepository.GetDiscountByProductId(product.Id);
                double discountedPrice = product.ProductPrice;

                if (discount != null)
                {
                    discountedPrice = ApplyDiscount(product.ProductPrice, discount);
                }

                var productDetailDto = new MainProductDto
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    ProductImageUrl = product.ProductImageUrl,
                    ShortDescription = product.ShortDescription,
                    OriginalPrice = product.ProductPrice,
                    DiscountedPrice = discountedPrice,
                    CategoryName = product.Category.CategoryName
                };

                productDetailDtos.Add(productDetailDto);
            }

            return productDetailDtos;
        }

        public async Task<SearchDtoAsVm> ProductsForSearchFilter(string query, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and pageSize must be greater than 0.");
            }

            query = query.ToLower();

            var productsAsQueryable = _productRepository.Table
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => !p.IsDeleted &&
                            (p.ProductName.ToLower().Contains(query) ||
                             p.ShortDescription.ToLower().Contains(query) ||
                             p.Description.ToLower().Contains(query) ||
                             p.Category.CategoryName.ToLower().Contains(query) ||
                             p.SubCategory.SubCategoryName.ToLower().Contains(query)))
                .OrderByDescending(p => p.CreatedAt)
                .ThenByDescending(p => p.ViewsCount);

            var totalProducts = await productsAsQueryable.CountAsync();

            var paginatedProducts = await productsAsQueryable
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var productDetailDtos = new List<MainProductDto>();

            foreach (var product in paginatedProducts)
            {
                var discount = await _discountRepository.GetDiscountByProductId(product.Id);
                double discountedPrice = product.ProductPrice;

                if (discount != null)
                {
                    discountedPrice = ApplyDiscount(product.ProductPrice, discount);
                }

                var productDetailDto = new MainProductDto
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    ProductImageUrl = product.ProductImageUrl,
                    ShortDescription = product.ShortDescription,
                    OriginalPrice = product.ProductPrice,
                    DiscountedPrice = discountedPrice,
                    CategoryName = product.Category.CategoryName,
                };

                productDetailDtos.Add(productDetailDto);
            }

            var sm = new SearchDtoAsVm()
            {
                FoundProducts = productDetailDtos,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                Query = query
            };

            return sm;
        }


        //public async Task<SearchDtoAsVm> ProductsForSearchFilter(string query, int page, int pageSize)
        //{
        //    if (page <= 0 || pageSize <= 0)
        //    {
        //        throw new ArgumentException("Page and pageSize must be greater than 0.");
        //    }

        //    var productsAsQueryable = await _productRepository.FilterWithPagination(page, pageSize);

        //    query = query.ToLower();

        //    var filteredProductsQuery = productsAsQueryable
        //        .Include(p => p.Category)
        //        .Include(p => p.Discount)
        //        .Where(p => !p.IsDeleted &&
        //                    (p.ProductName.ToLower().Contains(query) ||
        //                     p.ShortDescription.ToLower().Contains(query) ||
        //                     p.Description.ToLower().Contains(query) ||
        //                     p.Category.CategoryName.ToLower().Contains(query) ||
        //                     p.SubCategory.SubCategoryName.ToLower().Contains(query)))
        //        .OrderByDescending(p => p.CreatedAt)
        //        .ThenByDescending(p => p.ViewsCount);

        //    var products = await filteredProductsQuery.ToListAsync();
        //    var totalProducts = await filteredProductsQuery.CountAsync();

        //    var productDetailDtos = new List<MainProductDto>();

        //    foreach (var product in products)
        //    {
        //        var discount = await _discountRepository.GetDiscountByProductId(product.Id);
        //        double discountedPrice = product.ProductPrice;

        //        if (discount != null)
        //        {
        //            discountedPrice = ApplyDiscount(product.ProductPrice, discount);
        //        }

        //        var productDetailDto = new MainProductDto
        //        {
        //            Id = product.Id,
        //            ProductName = product.ProductName,
        //            ProductImageUrl = product.ProductImageUrl,
        //            ShortDescription = product.ShortDescription,
        //            OriginalPrice = product.ProductPrice,
        //            DiscountedPrice = discountedPrice,
        //            CategoryName = product.Category.CategoryName,
        //        };

        //        productDetailDtos.Add(productDetailDto);
        //    }

        //    var sm = new SearchDtoAsVm()
        //    {
        //        FoundProducts = productDetailDtos,
        //        CurrentPage = page,
        //        TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
        //        Query = query
        //    };

        //    return sm;
        //}


        public Task<List<MainProductDto>> ProductsForBestsellerArea()
        {
            throw new NotImplementedException();
        }

        public Task<List<MainProductDto>> GetVegetablesForVegetablesArea()
        {
            throw new NotImplementedException();
        }

        //3)/ 66 - 33 = discountedPrice(33)   2)// originalPrice(66) * 0,5 = 33    1)//DiscountRate(50) / 100 = 0,5
        public double ApplyDiscount(double originalPrice, Discount discount)
        {
            if (discount == null) return originalPrice;

            if (discount.DiscountType == "Percentage")
            {
                return originalPrice - (originalPrice * discount.DiscountRate / 100);
            }
            else if (discount.DiscountType == "FixedAmount")
            {
                return originalPrice - discount.DiscountRate;
            }
            return originalPrice;
        }
    }
}
