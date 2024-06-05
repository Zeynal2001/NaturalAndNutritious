using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Services.Results;

namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface IBlogService
    {
        Task<ServiceResult> CreateBlog(CreateBlogDto model, string dirPath);
        Task<string> CompleteFileOperations(UpdateBlogDto model);
    }
}
