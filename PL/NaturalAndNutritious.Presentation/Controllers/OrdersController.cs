using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Presentation.Models;
using NaturalAndNutritious.Presentation.ViewModels;

namespace NaturalAndNutritious.Presentation.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository orderRepository, UserManager<AppUser> userManager, ILogger<OrdersController> logger)
        {
            _orderRepository = orderRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> MyOrders()
        {
            ViewData["title"] = "Orders";

            try
            {
                var currentUserPrincipal = User;
                var user = await _userManager.GetUserAsync(currentUserPrincipal);

                var ordersAsQueryable = await _orderRepository.GetOrdersByUserId(user.Id);

                var orders = await ordersAsQueryable.Select(o => new OrdersModel()
                {
                    Id = o.Id,
                    OrderDate = o.OrderDate,
                    OrderNo = o.Id.ToString(),
                    OrderStatus = o.OrderStatus,
                    TotalAmount = o.Freight,
                    ProductQuantity = o.OrderDetails.Sum(od => od.Quantity)
                })
                .ToListAsync();

                return View(orders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MyOrders action");
                ViewData["msg"] = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        public async Task<IActionResult> OrderDetails(Guid Id)
        {
            ViewData["title"] = "OrderDetails";

            try
            {
                var orderDetailsAsQueryable = await _orderRepository.GetOrderDetailsByOrderId(Id);

                var orderDetails = await orderDetailsAsQueryable.Select(od => new OrderDetailsModel()
                {
                    Id = od.Id,
                    ProductId = od.ProductId,
                    Quantity = od.Quantity,
                    UnitPrice = od.UnitPrice,
                    ProductName = od.Product.ProductName,
                    ProductImageUrl = od.Product.ProductImageUrl,
                }).ToListAsync();

                var order = await orderDetailsAsQueryable.Select(od => od.Order).FirstOrDefaultAsync();

                if (order == null)
                {
                    _logger.LogWarning("Order with ID {OrderId} not found.", Id);
                    ViewData["msg"] = "Order not found";
                    return View("Error");
                }

                var vm = new OrderDetailsVm()
                {
                    OrderDetails = orderDetails,
                    OrderId = Id.ToString(),
                    OrderStatus = order.OrderStatus,
                    Shipper = order.Shipper.CompanyName,
                    ShipperTel = order.Shipper.PhoneNumber,
                    EstimatedDeliveryTime = order.RequiredDate
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in OrderDetails action for OrderId {OrderId}", Id);
                ViewData["msg"] = "An error occurred while processing your request.";
                return View("Error");
            }
        }
    }
}