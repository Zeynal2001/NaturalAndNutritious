﻿using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IUserRepository
    {
        Task<IQueryable<Order>> GetOrdersByUserId(string userId);
        Task<IQueryable<Review>> GetReviewsByUserId(string userId);
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(AppUser user);
        Task<IdentityResult> ChangeUserPasswordAsync(AppUser user, string currentPassword, string newPassword);
    }
}
