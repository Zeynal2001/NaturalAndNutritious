using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public Task<IQueryable<Order>> GetOrdersByUserId(string userId)
        {
            //Burada sinxron olarak Orders i alırıq
            var orders = _context.Orders
                        .Include(o => o.AppUser)
                        .Where(o => o.AppUser.Id == userId);

            //Və burada da asinxron bir task içində onları return eləyirik.
            return Task.FromResult(orders);
        }

        public Task<IQueryable<Review>> GetReviewsByUserId(string userId)
        {
            //Burada sinxron olarak Reviews i alırıq
            var reviews = _context.Reviews
                        .Include(r => r.AppUser)
                        .Where(r => r.AppUser.Id == userId);

            //Və burada da asinxron bir task içində onları return eləyirik.
            return Task.FromResult(reviews);
        }

        public async Task<AppUser> GetUserByIdAsync(string userId)
        {
            //if (user == null)
            //{
            //    throw new ArgumentNullException(nameof(user));
            //}
            
            return await _userManager.FindByIdAsync(userId); ;
        }

        public async Task<IdentityResult> UpdateUserAsync(AppUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> ChangeUserPasswordAsync(AppUser user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> DeleteUserAsync(AppUser user)
        {
            return await _userManager.DeleteAsync(user);
        }

        public async Task<IQueryable<AppUser>> GetAllUsers()
        {
            return await Task.FromResult(_context.Users);
        }

        public async Task<AppUser> FindUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        }
        /*
public IQueryable<Review> GetReviewsByUserId(string userId)
{
return _context.Reviews
.Include(r => r.AppUser)
.Where(r => r.AppUser.Id == userId).AsQueryable();
}
*/

        /*
        public async Task<IQueryable<Order>> GetOrdersByUserId(string userId)
        {
            return await Task.Run(() => _context.Orders
                        .Include(o => o.AppUser)
                        .Where(o => o.AppUser.Id == userId));
        }

        public async Task<IQueryable<Review>> GetReviewsByUserId(string userId)
        {
            return await Task.Run(() =>
                        _context.Reviews
                        .Include(r => r.AppUser)
                        .Where(r => r.AppUser.Id == userId));
        }
        */
    }
}
