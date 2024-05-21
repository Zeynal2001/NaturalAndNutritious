using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;

        public OrdersController(IOrderRepository orderRepository, IOrderService orderService)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
        }

        public async Task<IActionResult> GetAllOrders(int page = 1, int pageSize = 5)
        {
            var ordersAsQueryable = await _orderRepository.FilterWithPagination(page, pageSize);

            var orders = await ordersAsQueryable
                .Include(o => o.OrderDetails)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new AllOrdersDto()
                {
                    Id = o.Id.ToString(),
                    UserName = o.AppUser.FullName,
                    ShipName = o.Shipper.CompanyName,
                    OrderDate = o.OrderDate,
                    ShippedDate = o.ShippedDate,
                    RequiredDate = o.RequiredDate,
                    Freight = o.Freight,
                    ShipAddress = o.ShipAddress,
                    ShipCity = o.ShipCity,
                    ShipRegion = o.ShipRegion,
                    ShipPostalCode = o.ShipPostalCode,
                    ShipCountry = o.ShipCountry,
                    Confirmed = o.Confirmed,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                }).ToListAsync();

            var totalOrders = await _orderService.TotalOrders();

            var vm = new GetAllOrdersVm()
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize),
                PageSize = pageSize
            };

            return View(vm);
        }

        public async Task<IActionResult> GetComfirmedOrders(int page = 1, int pageSize = 5)
        {
            var ordersAsQueryable = await _orderRepository.FilterWithPagination(page, pageSize);

            var orders = await ordersAsQueryable
                .Include(o => o.OrderDetails)
                .Where(o => o.Confirmed == true)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new AllOrdersDto()
                {
                    Id = o.Id.ToString(),
                    UserName = o.AppUser.FullName,
                    ShipName = o.Shipper.CompanyName,
                    OrderDate = o.OrderDate,
                    ShippedDate = o.ShippedDate,
                    RequiredDate = o.RequiredDate,
                    Freight = o.Freight,
                    ShipAddress = o.ShipAddress,
                    ShipCity = o.ShipCity,
                    ShipRegion = o.ShipRegion,
                    ShipPostalCode = o.ShipPostalCode,
                    ShipCountry = o.ShipCountry,
                    Confirmed = o.Confirmed,
                    CreatedAt = o.CreatedAt,
                    UpdatedAt = o.UpdatedAt,
                }).ToListAsync();

            var totalOrders = await _orderService.TotalOrders();

            var vm = new GetAllOrdersVm()
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize),
                PageSize = pageSize
            };

            return View(vm);
        }
    }
}
