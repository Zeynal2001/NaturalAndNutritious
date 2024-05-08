using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Data.Repositories.Abstractions;

namespace NaturalAndNutritious.Data.Repositories
{
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository
    {
        public OrderDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public async Task<IQueryable<OrderDetail>> GetOrderDetailsByOrderId(Guid orderId)
        {
            return await Task.Run(() => _context.OrderDetails
                .Where(o => o.Order.Id == orderId));
        }

        public async Task<IQueryable<OrderDetail>> GetOrderDetailsByProductId(Guid productId)
        {
            return await Task.Run(() => _context.OrderDetails
                .Where(o => o.Product.Id == productId));
        }
    }
}
