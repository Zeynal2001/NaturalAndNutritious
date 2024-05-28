using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using SessionMapper;
using System.Security.Claims;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class ProductService : IProductService
    {
        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository, ISupplierRepository supplierRepository, IStorageService storageService, IDiscountRepository discountRepository, IReviewRepository reviewRepository, IShipperRepository shipperRepository, IOrderRepository orderRepository, IOrderRepository @object, IOrderDetailRepository orderDetailRepository, UserManager<AppUser> userManager)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _subCategoryRepository = subCategoryRepository;
            _supplierRepository = supplierRepository;
            _storageService = storageService;
            _discountRepository = discountRepository;
            _reviewRepository = reviewRepository;
            _shipperRepository = shipperRepository;
            _orderRepository = orderRepository;
            _orderDetailRepository = orderDetailRepository;
            _userManager = userManager;
        }

        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubCategoryRepository _subCategoryRepository;
        private readonly ISupplierRepository _supplierRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IStorageService _storageService;
        private readonly IReviewRepository _reviewRepository;
        private readonly IShipperRepository _shipperRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly UserManager<AppUser> _userManager;

        public async Task<List<AllProductsDto>> FilterProductsWithPagination(int page, int pageSize)
        {
            if (page == 0 && pageSize == 0)
            {
                throw new ArgumentException();
            }

            var products = await _productRepository.Table
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
                    UnitsOnOrder = model.UnitsOnOrder,
                    ReorderLevel = model.ReorderLevel,
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
                .Take(8)
                .Where(p => p.Category.IsDeleted == false)
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

        public async Task<DiscountedProductsDtoAsVm> GetDiscountedProducts(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and pageSize must be greater than 0.");
            }

            var discountedProductsAsQueryable = await _productRepository.GetProductsWithDiscounts();
                                                
            var totalProducts = await discountedProductsAsQueryable.CountAsync();

            var paginatedProducts = await discountedProductsAsQueryable
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

            var vm = new DiscountedProductsDtoAsVm()
            {
                DiscountedProducts = productDetailDtos,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
            };

            return vm;
        }

        public async Task<HomeFilterDtoAsVm> FilterProductsByCategories(string categoryFilter, int page, int pageSize)
        {
            categoryFilter = categoryFilter.ToLower();

            var productsAsQueryable = _productRepository.Table
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => !p.IsDeleted && 
                            (p.Category.CategoryName == categoryFilter))
                .OrderByDescending(p => p.CreatedAt);

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

            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

            var vm = new HomeFilterDtoAsVm()
            {
                Products = productDetailDtos,
                CurrentFilter = categoryFilter,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)pageSize),
                Categories = await categoriesAsQueryable
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryDto()
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                }).ToListAsync(),
            };

            return vm;
        }

        public async Task<List<MainProductDto>> FilterProductsByCategoriesProdcutsController(Guid categoryId, int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and pageSize must be greater than 0.");
            }

            var productsAsQueryable = _productRepository.Table
                                      .Include(p => p.Category)
                                      .Include(p => p.Discount)
                                      .Where(p => !p.IsDeleted && (p.Category.Id == categoryId))
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

            return productDetailDtos;
        }

        public async Task<int> TotalProductsForProductsByCategory(Guid categoryId)
        {
            var productsAsQueryable = _productRepository.Table
                                      .Include(p => p.Category)
                                      .Include(p => p.Discount)
                                      .Where(p => !p.IsDeleted && (p.Category.Id == categoryId))
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.ViewsCount);

            return await productsAsQueryable.CountAsync();
        }

        public async Task<List<MainProductDto>> ProductsForProductsController(int page, int pageSize)
        {
            if (page <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("Page and pageSize must be greater than 0.");
            }

            var productsAsQueryable = _productRepository.Table
                                      .Include(p => p.Category)
                                      .Include(p => p.Discount)
                                      .Where(p => !p.IsDeleted)
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

            return productDetailDtos;
        }

        public async Task<int> TotalProductsForProductsController()
        {
            var productsAsQueryable = _productRepository.Table
            .Include(p => p.Category)
            .Include(p => p.Discount)
            .Where(p => !p.IsDeleted)
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.ViewsCount);

            return await productsAsQueryable.CountAsync();
        }

        public Task<List<MainProductDto>> ProductsForBestsellerArea()
        {
            throw new NotImplementedException();
        }

        public async Task<List<MainProductDto>> GetVegetablesForVegetablesArea()
        {
            var vegetables = await _productRepository.Table
                .Include(p => p.Category)
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Where(p => p.Category.CategoryName == "Vegetable" && (!p.IsDeleted))
                .Take(8)
                .Select(p => new MainProductDto()
                {
                    Id = p.Id,
                    CategoryName = p.Category.CategoryName,
                    ProductName = p.ProductName,
                    ProductImageUrl = p.ProductImageUrl,
                    ShortDescription = p.ShortDescription,
                    OriginalPrice = p.ProductPrice,
                }).ToListAsync();

            return vegetables;
        }

        public async Task<List<MainProductDto>> GetAllDiscountedProducts()
        {
            var discountedProducts = new List<MainProductDto>();

            var featuredProducts = await _productRepository.Table
                .Include(p => p.Category)
                .Include(p => p.Discount)
                .Include(p => p.Reviews)
                .ThenInclude(reviews => reviews.AppUser)
                .Where(p => p.Discount != null && !p.IsDeleted)
                .Take(6)
                .ToListAsync();

            discountedProducts = featuredProducts.Select(p => new MainProductDto()
            {
                Id = p.Id,
                ProductName = p.ProductName,
                ShortDescription = p.ShortDescription,
                ProductImageUrl = p.ProductImageUrl,
                CategoryName = p.Category.CategoryName,
                OriginalPrice = p.ProductPrice,
                DiscountedPrice = ApplyDiscount(p.ProductPrice, p.Discount),
                Star = (int?)(p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0)
            }).ToList();

            return discountedProducts;
        }

        public async Task<List<RelatedProductsDto>> GetRelateProducts(Product product)
        {
            var relatedProducts = new List<RelatedProductsDto>();

            var related = await _productRepository.Table
                .Include(p => p.Category)
                .Take(8)
                .Where(p => p.Category.CategoryName == product.Category.CategoryName && product.Id != p.Id && !p.IsDeleted)
                .Select(p => new RelatedProductsDto()
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    ShortDescription = p.ShortDescription,
                    ProductPrice = p.ProductPrice,
                    ProductImageUrl = p.ProductImageUrl,
                    CategoryName = p.Category.CategoryName
                })
                .ToListAsync();

            relatedProducts.AddRange(related);
            return relatedProducts;
        }


        public async Task AddReviewAsync(ReviewDto reviewDto, AppUser user)
        {
            var review = new Review
            {
                ProductId = reviewDto.ProductId,
                AppUser = user,
                ReviewText = reviewDto.ReviewText,
                Rating = reviewDto.Rating,
                ReviewDate = DateTime.UtcNow
            };

          await _reviewRepository.CreateAsync(review);
          await _reviewRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceAsync(double price)
        {
            return await _productRepository.Table
                .Include(p => p.Category)
                .Include(p => p.Discount)
                                 .Where(p => p.ProductPrice <= price && !p.IsDeleted)
                                 .ToListAsync();
        }

        public async Task<(bool success, string message)> ProcessOrderAsync(CheckoutDto model, ClaimsPrincipal userPrincipal, ISession session)
        {
            var checkouts = session.Get<List<CheckoutModel>>("checkouts");
            var shipper = await _shipperRepository.Table.FirstOrDefaultAsync(sh => sh.CompanyName == "YasabKargo");
            var user = await _userManager.GetUserAsync(userPrincipal);

            if (user == null)
            {
                return (false, "Unauthorized");
            }

            if (checkouts == null || !checkouts.Any() || shipper == null)
            {
                return (false, "Order failed due to missing data.");
            }

            var totalSum = 0.0;
            var discountedPrices = new Dictionary<Guid, double>();

            foreach (var checkoutItem in checkouts)
            {
                var product = await _productRepository.GetByIdAsync(checkoutItem.ProductId);

                if (product == null)
                {
                    return (false, $"Order failed because product with ID {checkoutItem.ProductId} was not found.");
                }

                var discount = await _discountRepository.GetDiscountByProductId(product.Id);
                var discountedPrice = ApplyDiscount(checkoutItem.Price, discount);
                discountedPrices[product.Id] = discountedPrice;

                totalSum += discountedPrice * checkoutItem.Quantity;
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                AppUser = user,
                Shipper = shipper,
                Freight = totalSum,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ShipAddress = model.ShipAddress,
                ShipCity = model.ShipCity,
                ShipRegion = model.ShipRegion,
                ShipPostalCode = model.ShipPostalCode,
                ShipCountry = model.ShipCountry,
                CashOnDelivery = model.CashOnDelivery,
                Confirmed = false,
                IsDeleted = false
            };

            await _orderRepository.CreateAsync(order);

            foreach (var orderItem in checkouts)
            {
                var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                var discountedPrice = discountedPrices[orderItem.ProductId];

                var orderDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    UnitPrice = discountedPrice,
                    CreatedAt = DateTime.UtcNow,
                    OrderId = order.Id,
                    Product = product,
                    IsDeleted = false
                };

                await _orderDetailRepository.CreateAsync(orderDetail);
            }

            await _orderRepository.SaveChangesAsync();
            await _orderDetailRepository.SaveChangesAsync();

            var myOrder = await _orderRepository.GetByIdAsync(order.Id);

            if (myOrder.Confirmed == false)
            {
                return (true, "The order has been successfully processed, but it needs to be confirmed.");
            }

            return (true, "The order has been successfully processed and confirmed.");
        }
        
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

        //3)/ 66 - 33 = discountedPrice(33)   2)// originalPrice(66) * 0,5 = 33    1)//DiscountRate(50) / 100 = 0,5