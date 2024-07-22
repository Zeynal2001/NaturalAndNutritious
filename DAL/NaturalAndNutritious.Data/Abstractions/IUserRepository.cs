using Microsoft.AspNetCore.Identity;
using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IUserRepository
    {
        Task<IQueryable<Order>> GetOrdersByUserId(string userId);
        Task<IQueryable<AppUser>> GetAllUsers();
        Task<IQueryable<Review>> GetReviewsByUserId(string userId);
        Task<AppUser> GetUserByIdAsync(string userId);
        Task<IdentityResult> UpdateUserAsync(AppUser user);
        Task<IdentityResult> DeleteUserAsync(AppUser user);
        Task<AppUser> FindUserByEmailAsync(string email);
        Task<IdentityResult> ChangeUserPasswordAsync(AppUser user, string currentPassword, string newPassword);
    }
}
