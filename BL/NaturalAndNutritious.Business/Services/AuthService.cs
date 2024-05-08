using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Business.Abstractions;
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
    }
}
