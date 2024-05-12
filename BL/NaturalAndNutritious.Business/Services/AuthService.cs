using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.RepoServiceInterfaces;
using NaturalAndNutritious.Business.Services.Results;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services
{
    public class AuthService : IAuthService
    {
        public AuthService(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, IStorageService storageService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _storageService = storageService;
        }

        private readonly UserManager<AppUser> _userManager; //Cox vaxti register ucun istifade edilir
        private readonly SignInManager<AppUser> _signInManager; //Cox vaxti login ucun istifade edilir
        private readonly IStorageService _storageService;

        public Task<ServiceResult> Login(string username, string password)
        {
            throw new NotImplementedException();
        }

        public Task<ServiceResult> Register(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
