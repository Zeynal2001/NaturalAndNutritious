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
                    CustomerName = o.FirstName + " " + o.LastName
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

        public async Task<IActionResult> GetUnconfirmedOrders(int page = 1, int pageSize = 5)
        {
            var ordersAsQueryable = await _orderRepository.FilterWithPagination(page, pageSize);

            var orders = await ordersAsQueryable
                .Include(o => o.OrderDetails)
                .Where(o => o.Confirmed == false)
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
                    CustomerName = o.FirstName + " " + o.LastName
                }).ToListAsync();

            var totalOrders = await _orderService.TotalUnconfirmedOrders();

            var vm = new GetAllOrdersVm()
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize),
                PageSize = pageSize
            };

            return View(vm);
        }

        public async Task<IActionResult> GetConfirmedOrders(int page = 1, int pageSize = 5)
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
                    CustomerName = o.FirstName + " " + o.LastName
                }).ToListAsync();

            var totalOrders = await _orderService.TotalConfirmedOrders();

            var vm = new GetAllOrdersVm()
            {
                Orders = orders,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalOrders / (double)pageSize),
                PageSize = pageSize
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AssumingDeleted(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }
            var order = await _orderRepository.GetByIdAsync(guidId);

            if (order == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such order.";

                return View("AdminError", errorModel);
            }
            order.IsDeleted = true;

            var isUpdated = await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Order not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllOrders));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }

            var isDeleted = await _orderRepository.DeleteAsync(guidId);
            await _orderRepository.SaveChangesAsync();

            if (isDeleted == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such order.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetAllOrders));
        }

        [HttpPost]
        public async Task <IActionResult> ConfirmOrder(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }
            var order = await _orderRepository.GetByIdAsync(guidId);

            if (order == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such order.";

                return View("AdminError", errorModel);
            }
            order.Confirmed = true;

            var isUpdated = await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Order not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetConfirmedOrders));
        }

        [HttpPost]
        public async Task<IActionResult> UnconfirmOrder(string Id)
        {
            if (!Guid.TryParse(Id, out var guidId))
            {
                throw new ArgumentException($"The id '{Id}' is not a valid GUID.", nameof(Id));
            }
            var order = await _orderRepository.GetByIdAsync(guidId);

            if (order == null)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "There isn't such order.";

                return View("AdminError", errorModel);
            }
            order.Confirmed = false;

            var isUpdated = await _orderRepository.UpdateAsync(order);
            await _orderRepository.SaveChangesAsync();

            if (isUpdated == false)
            {
                var errorModel = new ErrorModel();
                errorModel.ErrorMessage = "Order not updated.";

                return View("AdminError", errorModel);
            }

            return RedirectToAction(nameof(GetUnconfirmedOrders));
        }
    }
}
