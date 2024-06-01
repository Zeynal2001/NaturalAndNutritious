using NaturalAndNutritious.Data.Entities;

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
