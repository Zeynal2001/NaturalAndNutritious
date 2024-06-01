using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IQueryable<OrderDetail>> GetOrderDetailsByProductId(Guid productId);
        Task<IQueryable<OrderDetail>> GetOrderDetailsByOrderId(Guid orderId);
    }
}
