using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Presentation.ViewModels;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly IMessageRepository _messageRepository;

        public HomeController(ILogger<HomeController> logger, IProductService productService, IProductRepository productRepository, ICategoryRepository categoryRepository, IUserRepository userRepository, IReviewRepository reviewRepository, IMessageRepository messageRepository)
        {
            _logger = logger;
            _productService = productService;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _userRepository = userRepository;
            _reviewRepository = reviewRepository;
            _messageRepository = messageRepository;
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                ViewData["title"] = "Home";
                var totalproducts = await _productRepository.GetAllAsync();
                var totoalusers = await _userRepository.GetAllUsers();

                var products = await _productService.GetProductsForHomePageAsync();
                var bestsellers = await _productService.GetBestSellers(10);
                var vegetables = await _productService.GetVegetablesForVegetablesArea();
                var categoriesAsQueryable = await _categoryRepository.GetAllAsync();

                var categories = await categoriesAsQueryable.Select(c => new CategoryDto()
                {
                    Id = c.Id,
                    CategoryName = c.CategoryName
                }).ToListAsync();

                var vm = new HomeVm()
                {
                    Categories = categories,
                    FilterProductsByCategories = products,
                    BestsellerProducts = bestsellers,
                    Vegetables = vegetables,
                    TotalCustomers = totoalusers.Count(),
                    TotalProducts = totalproducts.Count(),
                    Reviews = _reviewRepository.Table
                    .Where(r => r.Rating == 5 || r.Rating == 4)
                    .Select(r => new ReviewDto
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
                };

                _logger.LogInformation("Home page loaded successfully.");

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading home page.");
                ViewData["msg"] = "An unexpected error occurred.";
                return View("Error");
            }
        }

        public async Task<IActionResult> Search(string query, int page = 1)
        {
            try
            {
                ViewData["title"] = "Search";

                if (query == null)
                {
                    _logger.LogError("Query is empity.");
                    ViewData["msg"] = "Query can't be empty.";
                    return View("Error");
                }

                var searchVm = await _productService.ProductsForSearchFilter(query, page, 9);

                _logger.LogInformation("Search page loaded successfully.");

                return View(searchVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while loading search page.");
                ViewData["msg"] = "An unexpected error occurred.";
                return View("Error");
            }
        }

        public async Task<IActionResult> FilterByCategories(string categoryFilter, int page = 1)
        {
            try
            {
                ViewData["title"] = "Products By Category";
                ViewData["filter"] = categoryFilter;

                if (string.IsNullOrWhiteSpace(categoryFilter))
                {
                    _logger.LogError("Category filter is null or empity.");
                    ViewData["msg"] = "Category filter cannot be null or empty.";
                    return View("Error");
                }

                var products = await _productService.FilterProductsByCategories(categoryFilter, page, 8);

                _logger.LogInformation("Filtered products by category loaded successfully.");

                return View(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while filtering products by category.");
                ViewData["msg"] = "An unexpected error occurred.";
                return View("Error");
            }
        }

        public IActionResult Contact()
        {
            ViewData["title"] = "Contact";

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMessage(MessageDto dto)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid. Error count: {ErrorCount}", ModelState.ErrorCount);
                ViewData["hasError"] = ModelState.ErrorCount;
                return View(dto);
            }

            var message = new ContactMessage()
            {
                Id = Guid.NewGuid(),
                CustomerName = dto.CustomerName,
                CustomerEmailAddress = dto.CustomerEmailAddress,
                CustomerMessage = dto.CustomerMessage,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            try
            {
                await _messageRepository.CreateAsync(message);
                int affected = await _messageRepository.SaveChangesAsync();

                if (affected <= 0)
                {
                    _logger.LogError("The message could not be saved. No rows affected.");
                    ViewData["msg"] = "The message could not be saved.";
                    return View("Error");
                }

                _logger.LogInformation("Message was successfully delivered with ID: {MessageId}", message.Id);
                ViewData["successMsg"] = "The message was successfully delivered.";
                return View(nameof(Contact));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while saving the message.");
                ViewData["msg"] = "An unexpected error occurred.";
                return View("Error");
            }
        }

        public IActionResult Privacy()
        {
            ViewData["title"] = "Privacy";

            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
