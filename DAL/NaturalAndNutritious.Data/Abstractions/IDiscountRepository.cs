using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IDiscountRepository : IRepository<Discount>
    {
        Task<Discount> GetDiscountByProductId(Guid productId);
        Task<Product> GetProductByDiscountId(Guid discountId);
    }
}
