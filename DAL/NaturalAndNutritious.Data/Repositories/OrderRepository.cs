using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;

namespace NaturalAndNutritious.Data.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<IQueryable<Order>> GetOrdersByUserId(string userId)
        {
            return await Task.Run(() => _context.Orders
                .Include(o => o.AppUser)
                .Where(o => o.AppUser.Id == userId));
        }

        public async Task<IQueryable<OrderDetail>> GetOrderDetailsByOrderId(Guid orderId)
        {
            return await Task.Run(() => _context.OrderDetails
                .Include(o => o.Order)
                .Where(o => o.Order.Id == orderId));
        }

        public async Task<IQueryable<Product>> GetProductsByOrderId(Guid orderId)
        {
            return await Task.Run(() => _context.Products
               .Include(p => p.Orders)
               .Where(p => p.Orders.Any(o => o.Id == orderId)));
        }

        public async Task<IQueryable<Order>> GetOrdersByProductId(Guid productId)
        {
            return await Task.Run(() => _context.Orders
                .Include(o => o.Products)
                .Where(o => o.Products.Any(o => o.Id == productId)));
        }
    }
}
