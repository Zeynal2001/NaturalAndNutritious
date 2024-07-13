using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class BlogsController : Controller
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IBlogService _blogService;
        private readonly ILogger<BlogsController> _logger;

        public BlogsController(ILogger<BlogsController> logger, IBlogRepository blogRepository, IBlogService blogService)
        {
            _logger = logger;
            _blogRepository = blogRepository;
            _blogService = blogService;
        }

        public async Task<IActionResult> GetAllBlogs(int page = 1, int pageSize = 5)
        {
            _logger.LogInformation("GetAllBlogs action called with page: {Page} and pageSize: {PageSize}", page, pageSize);

            var blogsQueryable = await _blogRepository.FilterWithPagination(page, pageSize);
            var blogs = await blogsQueryable
                .OrderByDescending(sc => sc.CreatedAt)
                .Select(sb => new AllBlogsDto()
                {
                    Id = sb.Id,
                    Title = sb.Title,
                    BlogPhoto = sb.BlogPhotoUrl,
                    CreatedAt = sb.CreatedAt,
                    IsDeleted = sb.IsDeleted,
                    UpdatedAt = sb.UpdatedAt
                }).ToListAsync();

            var totalBlogs = await _blogRepository.Table
                        .Where(b => !b.IsDeleted)
                        .OrderByDescending(sc => sc.CreatedAt)
                        .CountAsync();

            var vm = new GetAllBlogsVm()
            {
                Blogs = blogs,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalBlogs / (double)pageSize),
                PageSize = pageSize
            };

            _logger.LogInformation("Retrieved {TotalBlogs} blogs.", totalBlogs);

            return View(vm);
        }

        public IActionResult Create()
        {
            _logger.LogInformation("Blog Create GET action called");

            try
            {
                _logger.LogInformation("Blog Create Get action completed successfully");

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Blog Create view");
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while loading the Blog Create view." };
                return View("AdminError", errorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBlogDto model)
        {
            _logger.LogInformation("Blog Create POST action called");

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("ModelState is invalid");
                return View(model);
            }

            try
            {
                var result = await _blogService.CreateBlog(model, "blog-photos");

                if (result.IsNull)
                {
                    var error = new ErrorModel { ErrorMessage = result.Message };
                    _logger.LogWarning("Blog creation failed: {Message}", result.Message);
                    return View("AdminError", error);
                }

                if (!result.Succeeded)
                {
                    var error = new ErrorModel { ErrorMessage = result.Message };
                    _logger.LogWarning("Blog creation failed: {Message}", result.Message);
                    return View("AdminError", error);
                }

                _logger.LogInformation("Blog created successfully");
                return RedirectToAction(nameof(GetAllBlogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the blog");
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while while creating the blog." };
                return View("AdminError", errorMessage);
            }
        }

        public async Task<IActionResult> Update(Guid Id)
        {
            _logger.LogInformation("Blog Update action called with Id: {Id}", Id);

            try
            {
                var blog = await _blogRepository.GetByIdAsync(Id);

                if (blog == null)
                {
                    var errorModel = new ErrorModel()
                    {
                        ErrorMessage = "There isn't such blog"
                    };
                    _logger.LogWarning("Blog not found for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                var productDetails = new UpdateBlogDto()
                {
                    Id = blog.Id,
                    Title = blog.Title,
                    BlogPhotoUrl = blog.BlogPhotoUrl,
                    AdditionalPhotoUrl1 = blog.AdditionalPhotoUrl1,
                    AdditionalPhotoUrl2 = blog.AdditionalPhotoUrl2,
                    Content = blog.Content,
                };

                _logger.LogInformation("Blog found for Id: {Id}", Id);
                return View(productDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Blog with Id: {Id}", Id);
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while loading the Blog Update view." };
                return View("AdminError", errorMessage);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateBlogDto model)
        {
            _logger.LogInformation("Blog Update action called with model: {@model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for model: {@model}", model);
                return View(model);
            }

            int affected = 0;
            try
            {
                var blog = await _blogRepository.GetByIdAsync(model.Id);

                if (blog == null)
                {
                    _logger.LogWarning("Blog not found for Id: {Id}", model.Id);
                    ModelState.AddModelError("editError", "There isn't such blog.");
                    return View(model);
                }

                var blogPhotoUrl = await _blogService.CompleteFileOperations(model);
                if(model.AdditionalPhoto1 != null)
                    blog.AdditionalPhotoUrl1 = await _blogService.CompleteFileOperations2(model);
                if(model.AdditionalPhoto2 != null)
                    blog.AdditionalPhotoUrl2 = await _blogService.CompleteFileOperations3(model);

                blog.Title = model.Title;
                blog.Content = model.Content;
                blog.BlogPhotoUrl = blogPhotoUrl;
                blog.UpdatedAt = DateTime.UtcNow;

                var isUpdated = await _blogRepository.UpdateAsync(blog);
                affected = await _blogRepository.SaveChangesAsync();

                if (!isUpdated && affected == 0)
                {
                    _logger.LogWarning("Blog update failed for Id: {Id}", model.Id);
                    ModelState.AddModelError("updateError", "Blog not updated.");
                    return View(model);
                }

                _logger.LogInformation("Blog successfully updated for Id: {Id}", model.Id);
                return RedirectToAction(nameof(GetAllBlogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog with Id: {Id}", model.Id);
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while updating the blog." };
                return View("AdminError", errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid Id)
        {
            _logger.LogInformation("Blog Delete action called with blogId: {blogId}", Id);

            try
            {
                var isDeleted = await _blogRepository.DeleteAsync(Id);
                await _blogRepository.SaveChangesAsync();

                if (!isDeleted)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "There isn't such blog."
                    };

                    _logger.LogWarning("Blog deletion failed for blogId: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Blog successfully deleted for blogId: {Id}", Id);
                return RedirectToAction(nameof(GetAllBlogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the blog with blogId: {blogId}", Id);
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while deleting the blog." };
                return View("AdminError", errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(Guid Id)
        {
            _logger.LogInformation("Blog AssumingDeleted action called with Id: {Id}", Id);

            var subscriber = await _blogRepository.GetByIdAsync(Id);

            if (subscriber == null)
            {
                var errorModel = new ErrorModel
                {
                    ErrorMessage = "There isn't such blog."
                };

                _logger.LogWarning("Blog not found for Id: {Id}", Id);
                return View("AdminError", errorModel);
            }

            subscriber.IsDeleted = true;

            try
            {
                var isUpdated = await _blogRepository.UpdateAsync(subscriber);
                await _blogRepository.SaveChangesAsync();

                if (!isUpdated)
                {
                    var errorModel = new ErrorModel
                    {
                        ErrorMessage = "Blog not updated."
                    };

                    _logger.LogWarning("Blog update failed for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                _logger.LogInformation("Blog marked as deleted for Id: {Id}", Id);
                return RedirectToAction(nameof(GetAllBlogs));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the blog with Id: {Id}", Id);
                var errorMessage = new ErrorModel { ErrorMessage = "An error occurred while updating the blog as IsDeleted." };
                return View("AdminError", errorMessage);
            }
        }
    }
}