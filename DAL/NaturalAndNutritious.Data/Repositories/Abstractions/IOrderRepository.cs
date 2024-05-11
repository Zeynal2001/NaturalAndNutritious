using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories.Abstractions
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IQueryable<Order>> GetOrdersByUserId(string userId);
        Task<IQueryable<OrderDetail>> GetOrderDetailsByOrderId(Guid orderId);
        Task<IQueryable<Product>> GetProductsByOrderId(Guid orderId);
        Task<IQueryable<Order>> GetOrdersByProductId(Guid productId);
    }
}
