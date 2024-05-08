using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Repositories.Abstractions
{
    public interface IOrderDetailRepository : IRepository<OrderDetail>
    {
        Task<IQueryable<OrderDetail>> GetOrderDetailsByProductId(Guid productId);
        Task<IQueryable<OrderDetail>> GetOrderDetailsByOrderId(Guid orderId);
    }
}
