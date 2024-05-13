using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IQueryable<Review>> GetReviewsByProductId(Guid productId);
        Task<IQueryable<Review>> GetReviewsByUserId(string userId);
        Task<Product> GetProductByReviewId(Guid reviewId);
        Task<AppUser> GetUserByReviewId(Guid reviewId);
    }
}
