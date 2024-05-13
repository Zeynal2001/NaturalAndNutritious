using Microsoft.EntityFrameworkCore;
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
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<Product> GetProductByReviewId(Guid reviewId)
        {
            var products = await _context.Products
                .Include(p => p.Reviews)
                .FirstOrDefaultAsync(p => p.Reviews.Any(r => r.Id == reviewId));

            return products;
        }

        public async Task<IQueryable<Review>> GetReviewsByProductId(Guid productId)
        {
            return await Task.Run(() => _context.Reviews
                .Where(r => r.Product.Id == productId));
        }

        public async Task<IQueryable<Review>> GetReviewsByUserId(string userId)
        {
            return await Task.Run(() => 
                        _context.Reviews
                        .Include(r => r.AppUser)
                        .Where(r => r.AppUser.Id == userId));
        }

        public async Task<AppUser> GetUserByReviewId(Guid reviewId)
        {
            var user = await _context.Users
                .Include(u => u.Reviews) // Kullanıcının incelemelerini de dahil et
                .FirstOrDefaultAsync(u => u.Reviews.Any(r => r.Id == reviewId));

            return user;

            //return await _context.Users
            //    .Where(usr => usr.Reviews.Any(r => r.Id == reviewId))
            //    .FirstOrDefaultAsync();
        }
    }
}
