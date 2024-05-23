using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Data.Abstractions;
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
        public OrderService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        private readonly IOrderRepository _orderRepository;

        public async Task<int> TotalOrders()
        {
            return await _orderRepository.Table
                                         .Include(o => o.OrderDetails)
                                         .OrderByDescending(o => o.CreatedAt)
                                         .CountAsync();
        }

        public async Task<int> TotalConfirmedOrders()
        {
            return await _orderRepository.Table
                                         .Include(o => o.OrderDetails)
                                         .Where(o => o.Confirmed == true)
                                         .OrderByDescending(o => o.CreatedAt)
                                         .CountAsync();
        }
        
        public async Task<int> TotalUnconfirmedOrders()
        {
            return await _orderRepository.Table
                                         .Include(o => o.OrderDetails)
                                         .Where(o => o.Confirmed == false)
                                         .OrderByDescending(o => o.CreatedAt)
                                         .CountAsync();
        }
    }
}
