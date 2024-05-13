using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Services.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions
{
    public interface IAuthService
    {
        Task<ServiceResult> Login(LoginDto model);
        Task<ServiceResult> Register(RegisterDto model, string dirPath);
        Task LogOut();
    }
}
