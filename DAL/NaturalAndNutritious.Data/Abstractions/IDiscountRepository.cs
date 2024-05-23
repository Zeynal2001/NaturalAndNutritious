using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Data.Abstractions
{
    public interface IDiscountRepository : IRepository<Discount>
    {
        Task<Discount> GetDiscountByProductId(Guid productId);
        Task<Product> GetProductByDiscountId(Guid discountId);
    }
}
