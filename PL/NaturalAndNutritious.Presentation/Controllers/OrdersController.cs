using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Enums;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;
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
            _logger.LogInformation("MyOrders page requested.");

            try
            {
                ViewData["title"] = "Orders";

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
                _logger.LogError("An error occurred while loading the MyOrders page: {Exception}", ex.ToString());
                ViewData["msg"] = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        public async Task<IActionResult> OrderDetails(Guid Id)
        {
            _logger.LogInformation("OrderDetails action called with Id: {Id}", Id);

            try
            {
                ViewData["title"] = "OrderDetails";
                bool IsDelivered = false;
                bool IsCanceled = false;

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

                if (order.OrderStatus == nameof(StatusType.Delivered) || order.OrderStatus == nameof(StatusType.Canceled) || order.OrderStatus == nameof(StatusType.Rejected))
                {
                    IsDelivered = true;
                }

                if (order.OrderStatus == nameof(StatusType.Canceled) || order.OrderStatus == nameof(StatusType.Rejected) || order.OrderStatus == nameof(StatusType.OnTheWay) || order.OrderStatus == nameof(StatusType.Delivered))
                {
                    IsCanceled = true;
                }

                var vm = new OrderDetailsVm()
                {
                    OrderDetails = orderDetails,
                    OrderId = Id.ToString(),
                    OrderStatus = order.OrderStatus,
                    Shipper = order.Shipper.CompanyName,
                    ShipperTel = order.Shipper.PhoneNumber,
                    EstimatedDeliveryTime = order.RequiredDate,
                    RecipientFName = order.FirstName,
                    RecipientLName = order.LastName,
                    MobileNumber = order.MobileNumber,
                    ShipCountry = order.ShipCountry,
                    ShipCity = order.ShipCity,
                    ShipRegion = order.ShipRegion,
                    ShipAddress = order.ShipAddress,
                    ShipPostalCode = order.ShipPostalCode,
                    CashOnDelivery = order.CashOnDelivery,
                    IsDelivered = IsDelivered,
                    IsCanceled = IsCanceled
                };

                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred in OrderDetails action for OrderId {OrderId}: {Exception}", Id, ex.ToString());
                ViewData["msg"] = "An error occurred while processing your request.";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Received(string Id)
        {
            _logger.LogInformation("Received method called with Id: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                string errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);

                ViewData["msg"] = errorMessage;
                return View("Error");
            }

            var order = await _orderRepository.GetByIdAsync(guidId);

            if (order == null)
            {
                _logger.LogWarning("Order with Id: {Id} not found.", Id);

                ViewData["msg"] = "Order not found!";
                return View("Error");
            }

            order.OrderStatus = nameof(StatusType.Delivered);

            int affected = await _orderRepository.SaveChangesAsync();

            if (affected == 0)
            {
                _logger.LogError("Failed to update order as Delivered with Id: {Id}", Id);

                ViewData["msg"] = "Order not updated.";
                return View("Error");
            }

            _logger.LogInformation("Order with Id: {Id} updated successfully as Delivered/Received.", Id);
            return RedirectToAction(nameof(MyOrders));
        }

        [HttpPost]
        public async Task<IActionResult> Canceled(string Id)
        {
            _logger.LogInformation("Canceled method called with Id: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                string errorMessage = $"The id '{Id}' is not a valid GUID.";
                _logger.LogError(errorMessage);

                ViewData["msg"] = errorMessage;
                return View("Error");
            }

            var order = await _orderRepository.GetByIdAsync(guidId);

            if (order == null)
            {
                _logger.LogWarning("Order with Id: {Id} not found.", Id);

                ViewData["msg"] = "Order not found!";
                return View("Error");
            }

            order.OrderStatus = nameof(StatusType.Canceled);

            int affected = await _orderRepository.SaveChangesAsync();

            if (affected == 0)
            {
                _logger.LogError("Failed to update order as Canceled with Id: {Id}", Id);

                ViewData["msg"] = "Order not updated.";
                return View("Error");
            }

            _logger.LogInformation("Order with Id: {Id} updated successfully as Canceled.", Id);
            return RedirectToAction(nameof(MyOrders));
        }
    }
}