using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IQueryable<Order>> GetOrdersByUserId(string userId);
        Task<IQueryable<OrderDetail>> GetOrderDetailsByOrderId(Guid orderId);
        Task<IQueryable<Product>> GetProductsByOrderId(Guid orderId);
        Task<IQueryable<Order>> GetOrdersByProductId(Guid productId);
    }
}
