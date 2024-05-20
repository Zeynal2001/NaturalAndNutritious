using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services.Results;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IAuthService
    {
        Task<ServiceResult> Login(LoginDto model);
        Task<ServiceResult> Register(RegisterDto model, string dirPath);
        Task LogOut();
    }
}
