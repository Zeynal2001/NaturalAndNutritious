using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Data;
using NaturalAndNutritious.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Repositories
{
    public class DiscountRepository : Repository<Discount>, IDiscountRepository
    {
        public DiscountRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        private readonly AppDbContext _context;

        public  async Task<Discount> GetDiscountByProductId(Guid productId)
        {
            return await _context.Discounts.FirstOrDefaultAsync(d => d.Product.Id == productId);
        }

        public async Task<Product> GetProductByDiscountId(Guid discountId)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Discount.Id == discountId);
                //Task.Run(() => )
        }
    }
}
