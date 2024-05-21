using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Data.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class OrderService : IOrderService
    {
        private readonly AppDbContext _appDbContext;

        public OrderService(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<int> TotalOrders()
        {
            return await _appDbContext.Orders.CountAsync();
        }
    }
}
