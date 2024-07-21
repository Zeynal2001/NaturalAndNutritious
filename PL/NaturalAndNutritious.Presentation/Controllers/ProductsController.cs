using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Presentation.ViewModels;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(ICategoryRepository categoryRepository, IProductService productService, IProductRepository productRepository, IDiscountRepository discountRepository, UserManager<AppUser> userManager, ILogger<ProductsController> logger, IOrderRepository orderRepository)
        {
            _categoryRepository = categoryRepository;
            _productService = productService;
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _userManager = userManager;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            _logger.LogInformation("Products page requested. Page number: {Page}", page);

            try
            {
                ViewData["title"] = "Products";

                var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

                var productAsPaginated = await _productService.ProductsForProductsController(page, 9);
                var totalProducts = await _productService.TotalProductsForProductsController();
                var discountedProducts = await _productService.GetAllDiscountedProducts();

                var vm = new ProductsVm()
                {
                    DiscountedProducts = discountedProducts,
                    Categories = await categoriesAsQueryable
                        .Select(c => new CategoryDto()
                        {
                            Id = c.Id,
                            CategoryName = c.CategoryName
                        }).ToListAsync(),
                    Products = productAsPaginated,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)9)
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request: {Exception}", ex.ToString());
                return View("Error");
            }
        }

        public async Task<IActionResult> ProductsByCategory(Guid Id, int page = 1)
        {
            _logger.LogInformation("Products by category page requested. Category ID: {CategoryId}, Page number: {Page}", Id, page);

            try
            {
                ViewData["title"] = "Products By Category";
                ViewData["filterCat"] = Id.ToString();

                var category = await _categoryRepository.Table
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == Id);

                if (category == null)
                {
                    _logger.LogWarning("Requested category with ID {CategoryId} not found", Id);
                    ViewData["msg"] = "Category not found!";
                    return View("Error");
                }

                var totalProducts = await _productService.TotalProductsForProductsByCategory(Id);
                var products = await _productService.FilterProductsByCategoriesProductsController(Id, page, 9);

                var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

                var vm = new ProductsByCategoryVm()
                {
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalProducts / (double)9),
                    Products = products,
                    Id = category.Id,
                    Categories = await categoriesAsQueryable
                        .Select(c => new CategoryDto()
                        {
                            Id = c.Id,
                            CategoryName = c.CategoryName
                        }).ToListAsync(),
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request: {Exception}", ex.ToString());
                return View("Error");
            }
        }

        public async Task<IActionResult> DiscountedProducts(int page = 1)
        {
            _logger.LogInformation("Discounted products page requested. Page number: {Page}", page);

            try
            {
                ViewData["title"] = "Discounted Products";

                var discountedProductsVm = await _productService.GetDiscountedProducts(page, 5);

                return View(discountedProductsVm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request: {Exception}", ex.ToString());
                return View("Error");
            }
        }

        public async Task<IActionResult> Detail(Guid Id)
        {
            _logger.LogInformation("Detail method called for product ID {ProductId}", Id);

            try
            {
                ViewData["title"] = "Product Details";
                var theUserHasOrder = false;

                var product = await _productRepository.Table
                                    .Include(p => p.Category)
                                    .Include(p => p.SubCategory)
                                    .Include(p => p.Discount)
                                    .Include(p => p.Reviews)
                                    .ThenInclude(review => review.AppUser)
                    .FirstOrDefaultAsync(p => p.Id == Id && !p.IsDeleted);

                if (product == null)
                {
                    _logger.LogWarning("Requested product with ID {ProductId} not found", Id);
                    ViewData["msg"] = "There isn't such product";
                    return View("Error");
                }

                product.ViewsCount++;
                await _productRepository.SaveChangesAsync();

                var discount = await _discountRepository.GetDiscountByProductId(product.Id);
                double discountedPrice = product.ProductPrice;

                if (discount != null)
                {
                    discountedPrice = _productService.ApplyDiscount(product.ProductPrice, discount);
                }

                //-------------We check whether the specific product is included in the user's orders.--------------
                if (User.Identity.IsAuthenticated)
                {
                    var currentUserPrincipal = User;
                    var user = await _userManager.GetUserAsync(currentUserPrincipal);

                    if (user != null)
                    {
                        var ordersAsQueryable = await _orderRepository.GetOrdersByUserId(user.Id);
                        theUserHasOrder = ordersAsQueryable.Any(o => o.OrderDetails.Any(od => od.ProductId == product.Id));
                    }
                }
                //--------------------------------------------------------------------------------------------------

                var details = new ProductDetailsDto()
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    TheUserHasOrder = theUserHasOrder,
                    ShortDescription = product.ShortDescription,
                    Description = product.Description,
                    OriginalPrice = product.ProductPrice,
                    DiscountedPrice = discountedPrice,
                    ViewsCount = product.ViewsCount,
                    CategoryName = product.Category.CategoryName,
                    SubCategoryName = product.SubCategory.SubCategoryName,
                    ProductImageUrl = product.ProductImageUrl,
                    Reviews = product.Reviews.Select(r => new ReviewDto
                    {
                        ReviewText = r.ReviewText,
                        Rating = r.Rating,
                        ReviewDate = r.ReviewDate,
                        User = new AppUserDto
                        {
                            FullName = r.AppUser.FullName,
                            ProfilePhotoUrl = r.AppUser.ProfilePhotoUrl
                        }
                    }).ToList(),
                    AverageStar = product.Reviews.Any() ? (int?)product.Reviews.Average(r => r.Rating) : null,
                    /*Əgər Reviews-də bu produktla bağlı ən azı bir şərh varsa,
                    * Bu Average funksiyası ilə hesablanır və int? olaraq təyin edilir.
                    * Əks halda null təyin olunur.*/
                };

                //------------------------------RelatedProducts-------------------------------------
                var relatedProducts = await _productService.GetRelateProducts(product);
                //-----------------------------DiscountedProducts-----------------------------------
                var discountedPoducts = await _productService.GetAllDiscountedProducts();
                //-------------------------------GetAllCategories-----------------------------------
                var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

                var vm = new ProductDetailVm()
                {
                    ProductDetails = details,
                    RelatedProducts = relatedProducts,
                    FeaturedPproducts = discountedPoducts,
                    Categories = await categoriesAsQueryable
                    .Select(c => new CategoryDto()
                    {
                        Id = c.Id,
                        CategoryName = c.CategoryName
                    }).ToListAsync(),
                };

                _logger.LogInformation("Product detail page requested for product ID {ProductId}", Id);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request for product ID {ProductId}: {Exception}", Id, ex.ToString());
                return View("Error");
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewDto reviewDto)
        {

            _logger.LogInformation("AddReview POST action called.");

            //if (!ModelState.IsValid)
            //{
            //    return View("_AddReview", reviewDto);
            //}

            if (reviewDto.ProductId == null)
            {
                ViewData["msg"] = "Product Id is required.";
                return View("Error");
            }

            if (reviewDto.Rating == null || reviewDto.Rating > 5 || reviewDto.Rating < 1)
            {
                ViewData["msg"] = "Rating must be between 1 and 5 stars.";
                return View("Error");
            }

            if (reviewDto.ReviewText == null)
            {
                ViewData["msg"] = "Review text is required.";
                return View("Error");
            }

            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    _logger.LogWarning("User is not authenticated while trying to add review.");
                    return Unauthorized();
                }

                var user = await _userManager.GetUserAsync(User);

                await _productService.AddReviewAsync(reviewDto, user);
                _logger.LogInformation("Review added for product ID {ProductId} by user {UserId}.", reviewDto.ProductId, user.Id);

                return RedirectToAction("Detail", new { id = reviewDto.ProductId });
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while adding review for product ID {ProductId}: {Exception}", reviewDto.ProductId, ex.ToString());
                return View("Error");
            }
        }

        public async Task<IActionResult> PriceRange()
        {
            _logger.LogInformation("PriceRange (GET) method called.");

            try
            {
                var products = await _productRepository.GetAllAsync();

                var vm = new PriceRangeVm()
                {
                    Products = await products.Select(p => new MainProductDto()
                    {
                        Id = p.Id,
                        ProductName = p.ProductName,
                        ProductImageUrl = p.ProductImageUrl,
                        OriginalPrice = p.ProductPrice,
                        CategoryName = p.Category.CategoryName
                    }).ToListAsync()
                };

                _logger.LogInformation("Price range page accessed. Retrieved {ProductCount} products.", vm.Products.Count);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while loading the price range page: {Exception}", ex.ToString());
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PriceRange(PriceRangeVm model)
        {
            _logger.LogInformation("PriceRange [HttpPost] method called with selected price: {SelectedPrice}.", model.SelectedPrice);

            try
            {
                var products = await _productService.GetProductsByPriceAsync(model.SelectedPrice);

                var productDetailDtos = new List<MainProductDto>();

                foreach (var product in products)
                {
                    var discount = await _discountRepository.GetDiscountByProductId(product.Id);
                    double discountedPrice = product.ProductPrice;

                    if (discount != null)
                    {
                        discountedPrice = _productService.ApplyDiscount(product.ProductPrice, discount);
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

                var vm = new PriceRangeVm()
                {
                    Products = productDetailDtos
                };

                _logger.LogInformation("Price range filter applied with selected price: {SelectedPrice}. Retrieved {ProductCount} products.", model.SelectedPrice, vm.Products.Count);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while filtering products by price range: {Exception}", ex.ToString());
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByIds([FromQuery] Guid id)
        {
            _logger.LogInformation("GetProductsByIds method called with ID: {ProductId}", id);
            
            try
            {
                var product = await _productRepository.Table
                    .Select(p => new
                    {
                        id = p.Id,
                        photo = p.ProductImageUrl,
                        name = p.ProductName,
                        price = p.ProductPrice
                    })
                    .FirstOrDefaultAsync(p => p.id == id);

                if (product is null)
                {
                    _logger.LogWarning("Product with ID {ProductId} not found.", id);
                    return NotFound();
                }

                _logger.LogInformation("Product with ID {ProductId} retrieved successfully.", id);
                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while retrieving product by ID {ProductId}: {Exception}", id, ex.ToString());
                return StatusCode(500);
            }
        }

        public async Task<IActionResult> ExoticFruits(int page = 1)
        {
            ViewData["title"] = "Exotic Fruits";

            var fruits = await _productService.GetExoticFruits(page, 8);

            var vm = new ExoticFruitsVm()
            {
                Fruits = fruits,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(await _productService.TotalExoticFruits() / (double)9)
            };

            return View(vm);
        }

        public async Task<IActionResult> ExoticVegetables(int page = 1)
        {
            ViewData["title"] = "Exotic Vegetables";

            var fruits = await _productService.GetExoticVegetables(page, 8);

            var vm = new ExoticVegetablesVm()
            {
                Fruits = fruits,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(await _productService.TotalExoticVegetables() / (double)9)
            };

            return View(vm);
        }

        public async Task<IActionResult> TastyBerries(int page = 1)
        {
            ViewData["title"] = "Tasty Berries";

            var fruits = await _productService.GetBerries(page, 8);

            var vm = new TastyBerriesVm()
            {
                Fruits = fruits,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(await _productService.TotalBerries() / (double)9)
            };

            return View(vm);
        }
        
        public async Task<IActionResult> FreshApples(int page = 1)
        {
            ViewData["title"] = "Fresh Apples";

            var fruits = await _productService.GetApples(page, 8);

            var vm = new FreshApplesVm()
            {
                Fruits = fruits,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(await _productService.TotalApples() / (double)9)
            };

            return View(vm);
        }
    }
}