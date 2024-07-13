using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Presentation.Models;
using NaturalAndNutritious.Presentation.ViewModels;

namespace NaturalAndNutritious.Presentation.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(IBlogRepository blogRepository, ILogger<BlogsController> logger)
        {
            _blogRepository = blogRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            _logger.LogInformation("Blogs page requested. Page number: {Page}", page);

            try
            {
                ViewData["title"] = "Blogs";

                var blogsAsQueryable = await _blogRepository.FilterWithPagination(page, 9);
                var totalBlogs = blogsAsQueryable.Count();

                var blogs = await blogsAsQueryable.Select(bl => new BlogsModel()
                {
                    Id = bl.Id,
                    Title = bl.Title,
                    BlogPhotoUrl = bl.BlogPhotoUrl,
                    CreatedAt = bl.CreatedAt,
                }).ToListAsync();

                var vm = new BlogsVm()
                {
                    Blogs = blogs,
                    CurrentPage = page,
                    TotalPages = (int)Math.Ceiling(totalBlogs / (double)9),
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request: {Exception}", ex.ToString());
                return View("Error");
            }
        }

        public async Task<IActionResult> Details(Guid Id)
        {
            _logger.LogInformation("Blog Details action called with Id: {Id}", Id);

            try
            {
                ViewData["title"] = "Details";

                var blog = await _blogRepository.GetByIdAsync(Id);

                if (blog == null)
                {
                    _logger.LogWarning("Requested blog with ID {BlogId} not found", Id);
                    ViewData["msg"] = "Blog not found";
                    return View("Error");
                }

                var vm = new BlogDetailsVm()
                {
                    Id = blog.Id.ToString(),
                    Title = blog.Title,
                    Content = blog.Content,
                    BlogPhotoUrl = blog.BlogPhotoUrl,
                    AdditionalPhotoUrl1 = blog.AdditionalPhotoUrl1,
                    AdditionalPhotoUrl2 = blog.AdditionalPhotoUrl2,
                    CreatedDate = blog.CreatedAt
                };

                _logger.LogInformation("Blog Details page requested for blog ID {BlogId}", Id);

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred while processing the request for product ID {BlogId}: {Exception}", Id, ex.ToString());
                return View("Error");
            }
        }
    }
}