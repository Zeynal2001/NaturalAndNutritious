using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Repositories;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Presentation.ViewModels;
using System.Drawing.Printing;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly UserManager<AppUser> _userManager;

        public ProductsController(ICategoryRepository categoryRepository, IProductService productService, IProductRepository productRepository, IDiscountRepository discountRepository, UserManager<AppUser> userManager)
        {
            _categoryRepository = categoryRepository;
            _productService = productService;
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewData["title"] = "Products";

            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

            var productAsPaginated = await _productService.ProductsForProductsController(page, 9);
            var totalProducts = await _productService.TotalProductsForProductsController();
            var discountedPoducts = await _productService.GetAllDiscountedProducts();

            var vm = new ProductsVm()
            {
                DiscountedProducts = discountedPoducts,
                Categories = await categoriesAsQueryable
                .Where(c => !c.IsDeleted)
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

        public async Task<IActionResult> ProductsByCategory(Guid Id, int page = 1)
        {
            ViewData["title"] = "Products By Category";

            var category = await _categoryRepository.Table
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == Id);

            if (category == null)
            {
                ViewData["msg"] = "Category not found";
                return View("Error");
            }

            var totalProducts = await _productService.TotalProductsForProductsByCategory(Id);
            var products = await _productService.FilterProductsByCategoriesProdcutsController(Id, page, 9);

            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

            var vm = new ProductsByCategoryVm()
            {
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalProducts / (double)9),
                Products = products,
                Id = category.Id,
                Categories = await categoriesAsQueryable
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryDto()
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                }).ToListAsync(),
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(Guid Id)
        {
            ViewData["title"] = "Product Detail";

            var product = await _productRepository.Table
                .Include(p => p.Category)
                .Include(p => p.SubCategory)
                .Include(p => p.Discount)
                .Include(p => p.Reviews)
                .ThenInclude(review => review.AppUser)
                .FirstOrDefaultAsync(p => p.Id == Id && !p.IsDeleted);

            if (product == null)
            {
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

            var details = new ProductDetailsDto()
            {
                Id = product.Id,
                ProductName = product.ProductName,
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
            //-----------------------------GetAllCategories-------------------------------------
            var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

            var vm = new ProductDetailVm()
            {
                ProductDetails = details,
                RelatedProducts = relatedProducts,
                FeaturedPproducts = discountedPoducts,
                Categories = await categoriesAsQueryable
                .Where(c => !c.IsDeleted)
                .Select(c => new CategoryDto()
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                }).ToListAsync(),
            };

            return View(vm);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ReviewDto reviewDto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }
            await _productService.AddReviewAsync(reviewDto, user);
            return RedirectToAction("Detail", new { id = reviewDto.ProductId });
        }

        public async Task<IActionResult> PriceRange()
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

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> PriceRange(PriceRangeVm model)
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

            return View(vm);
        }
    }
}
//var dis = products.wh

//if (products != null)
//{
//    model.Products = products
//        .Select(p => new MainProductDto()
//        {
//            Id = p.Id,
//            ProductName = p.ProductName,
//            ProductImageUrl = p.ProductImageUrl,
//            DiscountedPrice = _productService.ApplyDiscount(p.ProductPrice, )
//            OriginalPrice = p.ProductPrice,
//            CategoryName = p.Category != null ? p.Category.CategoryName : null
//        }).ToList();
//}
//else
//{
//    model.Products = new List<MainProductDto>(); 
//}