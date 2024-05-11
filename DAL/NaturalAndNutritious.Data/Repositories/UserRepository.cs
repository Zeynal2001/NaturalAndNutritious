using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

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
