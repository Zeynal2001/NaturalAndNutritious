using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;

namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface IBlogService
    {
        Task<ServiceResult> CreateBlog(CreateBlogDto model, string dirPath);
        Task<string> CompleteFileOperations(UpdateBlogDto model);
        Task<string> CompleteFileOperations2(UpdateBlogDto model);
        Task<string> CompleteFileOperations3(UpdateBlogDto model);
    }
}
