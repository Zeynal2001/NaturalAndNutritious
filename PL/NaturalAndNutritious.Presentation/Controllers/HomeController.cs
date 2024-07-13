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
            _logger.LogInformation("Home Index method called.");
            
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
                _logger.LogError("An error occurred while loading Home page: {Exception}", ex.ToString());
                ViewData["msg"] = "An unexpected error occurred while loading Home page.";
                return View("Error");
            }
        }

        /*
        public async Task<IActionResult> Search(string search, int page = 1)
        {
            int TotalCount = _context.Books.Where(x => !x.IsDeleted && x.Name.Trim().ToLower().Contains(search.Trim().ToLower())).Count();
            ViewBag.TotalPage = (int)Math.Ceiling((decimal)TotalCount / 3);
            ViewBag.CurrentPage = page;

            List<Book> books = await _context.Books.Where(x => !x.IsDeleted && x.Name.Trim().ToLower().Contains(search.Trim().ToLower()))
                    //.Include(x => x.BookLanguages)
                    //.ThenInclude(x => x.Language)
                    .Skip((page - 1) * 3).Take(3)
                .ToListAsync();
            return Json(books);
        }
        */

        public async Task<IActionResult> Search(string query, int page = 1)
        {
            _logger.LogInformation("Search method called. Query: {Query}, Page: {Page}", query, page);

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
                _logger.LogError("An error occurred while loading Search page: {Exception}", ex.ToString());
                ViewData["msg"] = "An unexpected error occurred while loading Search page.";
                return View("Error");
            }
        }

        public async Task<IActionResult> FilterByCategories(string categoryFilter, int page = 1)
        {
            _logger.LogInformation("FilterByCategories method called. CategoryFilter: {CategoryFilter}, Page: {Page}", categoryFilter, page);

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
                _logger.LogError("An error occurred while filtering products by category: {Exception}", ex.ToString());
                ViewData["msg"] = "An unexpected error occurred while loading FilterByCategories page.";
                return View("Error");
            }
        }

        public IActionResult Contact()
        {
            try
            {
                ViewData["title"] = "Contact";
                _logger.LogInformation("Contact page accessed.");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while loading the Contact page: {Exception}", ex.ToString());
                ViewData["msg"] = "An unexpected error occurred while loading Contact page.";
                return View("Error");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddMessage(MessageDto dto)
        {
            _logger.LogInformation("AddMessage method called.");

            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Model state is invalid. Error count: {ErrorCount}", ModelState.ErrorCount);
                    ViewData["msg"] = "Model state is invalid.";
                    return View("Error");
                }

                var message = new ContactMessage()
                {
                    Id = Guid.NewGuid(),
                    CustomerName = dto.CustomerName,
                    CustomerEmailAddress = dto.CustomerEmailAddress,
                    CustomerMessage = dto.CustomerMessage,
                    CreatedAt = DateTime.UtcNow,
                    IsAnswered = false,
                    IsDeleted = false
                };

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
                return RedirectToAction(nameof(Contact));
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while saving the message: {Exception}", ex.ToString());
                ViewData["msg"] = "An unexpected error occurred while adding message.";
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
