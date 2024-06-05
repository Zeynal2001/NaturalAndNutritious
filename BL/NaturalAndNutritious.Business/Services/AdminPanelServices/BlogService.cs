using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class BlogService : IBlogService
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IStorageService _storageService;

        public BlogService(IStorageService storageService, IBlogRepository blogRepository)
        {
            _storageService = storageService;
            _blogRepository = blogRepository;
        }

        public async Task<ServiceResult> CreateBlog(CreateBlogDto model, string dirPath)
        {
            int affected = 0;
            if (model != null && dirPath != null)
            {
                var uploaded = await _storageService.UploadFileAsync(dirPath, model.BlogPhoto);
                var blogPhotoUrl = uploaded.FullPath;

                if (_storageService is LocalStorageService)
                {
                    blogPhotoUrl = $"uploads/{blogPhotoUrl}";
                }

                var product = new Blog()
                {
                    Id = Guid.NewGuid(),
                    Title = model.Title,
                    BlogPhotoUrl = blogPhotoUrl,
                    Description = model.Description,
                    ShortDescription = model.ShortDescription,
                    ViewsCount = 0,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                };

                await _blogRepository.CreateAsync(product);
                affected = await _blogRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    return new ServiceResult
                    {
                        Succeeded = false,
                        IsNull = false,
                        Message = "Blog is not created, The table is not affected."
                    };
                }

                return new ServiceResult
                {
                    Succeeded = true,
                    IsNull = false,
                    Message = "Blog created successfully."
                };
            }
            else
            {
                return new ServiceResult { Succeeded = false, IsNull = true, Message = "Model or direction path is null." };
            }
        }

        public async Task<string> CompleteFileOperations(UpdateBlogDto model)
        {
            string blogPhotoUrl = "";

            if (model.BlogPhoto == null)
            {
                blogPhotoUrl = model.BlogPhotoUrl;
            }
            else
            {
                var photoName = Path.GetFileName(model.BlogPhotoUrl);
                if (_storageService.HasFile("blog-photos", photoName))
                {
                    await _storageService.DeleteFileAsync("blog-photos", photoName);
                }

                var dto = await _storageService.UploadFileAsync("blog-photos", model.BlogPhoto);
                blogPhotoUrl = dto.FullPath;

                if (_storageService is LocalStorageService)
                {
                    blogPhotoUrl = $"uploads/{dto.FullPath}";
                }
            }

            return blogPhotoUrl;
        }
    }
}