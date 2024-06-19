using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;
using NaturalAndNutritious.Business.Enums;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Enums;
using NaturalAndNutritious.Presentation.Areas.admin_panel.Models;
using NaturalAndNutritious.Presentation.Models;
using NaturalAndNutritious.Presentation.ViewModels;

namespace NaturalAndNutritious.Presentation.Areas.admin_panel.Controllers
{
    [Area("admin_panel")]
    [Authorize(Roles = nameof(RoleTypes.Admin), AuthenticationSchemes = "AdminAuth")]
    public class OrdersController : Controller
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderService _orderService;
        private readonly IShipperRepository _shipperRepository;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IOrderRepository orderRepository, IOrderService orderService, ILogger<OrdersController> logger, IShipperRepository shipperRepository)
        {
            _orderRepository = orderRepository;
            _orderService = orderService;
            _shipperRepository = shipperRepository;
            _logger = logger;
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
                    OrderStatus = o.OrderStatus,
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
                    RecipientName = o.FirstName + " " + o.LastName,
                    MobileNumber = o.MobileNumber
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
            var ordersAsQueryable = await _orderRepository.GetAllAsync();

            var orders = await ordersAsQueryable
                .Include(o => o.OrderDetails)
                .Where(o => o.Confirmed == false)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AllOrdersDto()
                {
                    Id = o.Id.ToString(),
                    UserName = o.AppUser.FullName,
                    ShipName = o.Shipper.CompanyName,
                    OrderStatus = o.OrderStatus,
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
                    RecipientName = o.FirstName + " " + o.LastName,
                    MobileNumber = o.MobileNumber
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
            var ordersAsQueryable = await _orderRepository.GetAllAsync();

            var orders = await ordersAsQueryable
                .Include(o => o.OrderDetails)
                .Where(o => o.Confirmed == true)
                .OrderByDescending(o => o.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(o => new AllOrdersDto()
                {
                    Id = o.Id.ToString(),
                    UserName = o.AppUser.FullName,
                    ShipName = o.Shipper.CompanyName,
                    OrderStatus = o.OrderStatus,
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
                    RecipientName = o.FirstName + " " + o.LastName,
                    MobileNumber = o.MobileNumber
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
            order.OrderStatus = nameof(StatusType.Accepted);

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
            order.OrderStatus = nameof(StatusType.Pending);

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

        public async Task<IActionResult> OrderDetails(string Id)
        {
            ViewData["title"] = "OrderDetails";

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMsg = $"The id '{Id}' is not a valid GUID.";
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = errorMsg
                };
                _logger.LogError(errorMsg);
                return View("AdminError", errorModel);
            }

            try
            {
                var orderDetailsAsQueryable = await _orderRepository.GetOrderDetailsByOrderId(guidId);

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

        public async Task<IActionResult> Update(string Id)
        {
            _logger.LogInformation("Update order GET action called with Id: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMsg = $"The id '{Id}' is not a valid GUID.";
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = errorMsg
                };
                _logger.LogWarning(errorMsg);
                return View("AdminError", errorModel);
            }

            try
            {
                var order = await _orderRepository.GetByIdAsync(guidId);

                if (order == null)
                {
                    var errorModel = new ErrorModel()
                    {
                        ErrorMessage = "There isn't such order"
                    };
                    _logger.LogWarning("Order not found for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                var orderDetails = new UpdateOrderDto()
                {
                    Id = order.Id.ToString(),
                    RecipientName = order.FirstName,
                    RecipientLName = order.LastName,
                    MobileNumber = order.MobileNumber,
                    ShipCity = order.ShipCity,
                    ShipCountry = order.ShipCountry,
                    ShipRegion = order.ShipRegion,
                    ShipPostalCode = order.ShipPostalCode,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate,
                    ShipAddress = order.ShipAddress,
                };

                _logger.LogInformation("Order found for Id: {Id}", Id);
                return View(orderDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order with Id: {Id}", Id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(UpdateOrderDto model)
        {
            _logger.LogInformation("Update order POST action called with model: {@model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for model: {@model}", model);
                return View(model);
            }

            int affected = 0;

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                var errorMsg = $"The id '{model.Id}' is not a valid GUID.";
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = errorMsg
                };
                _logger.LogWarning(errorMsg);
                return View("AdminError", errorModel);
            }

            try
            {
                var order = await _orderRepository.GetByIdAsync(guidId);

                if (order == null)
                {
                    _logger.LogWarning("Order not found for Id: {Id}", model.Id);
                    ModelState.AddModelError("AdminError", "There isn't such order.");
                    return View(model);
                }

                order.FirstName = model.RecipientName;
                order.LastName = model.RecipientLName;
                order.MobileNumber = model.MobileNumber;
                order.ShipCity = model.ShipCity;
                order.ShipCountry = model.ShipCountry;
                order.ShipRegion = model.ShipRegion;
                order.ShipPostalCode = model.ShipPostalCode;
                order.RequiredDate = model.RequiredDate;
                order.ShippedDate = model.ShippedDate;
                order.ShipAddress = model.ShipAddress;
                order.UpdatedAt = DateTime.UtcNow;

                var isUpdated = await _orderRepository.UpdateAsync(order);
                affected = await _orderRepository.SaveChangesAsync();

                if (!isUpdated && affected == 0)
                {
                    _logger.LogWarning("Order update failed for Id: {Id}", model.Id);
                    ModelState.AddModelError("updateError", "order not updated.");
                    return View(model);
                }

                _logger.LogInformation("Order successfully updated for Id: {Id}", model.Id);
                return RedirectToAction(nameof(GetAllOrders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the order with Id: {Id}", model.Id);
                return View("Error");
            }
        }

        public async Task<IActionResult> ChangeStatus(string Id)
        {
            _logger.LogInformation("ChangeStatus GET action called with Id: {Id}", Id);

            if (!Guid.TryParse(Id, out var guidId))
            {
                var errorMsg = $"The id '{Id}' is not a valid GUID.";
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = errorMsg
                };
                _logger.LogError(errorMsg);
                return View("AdminError", errorModel);
            }

            try
            {
                var order = await _orderRepository.GetByIdAsync(guidId);

                if (order == null)
                {
                    var errorModel = new ErrorModel()
                    {
                        ErrorMessage = "There isn't such order"
                    };
                    _logger.LogWarning("Order not found for Id: {Id}", Id);
                    return View("AdminError", errorModel);
                }

                var productDetails = new ChangeOrderStatusDto()
                {
                    Id = order.Id.ToString(),
                    CurrentStatus = order.OrderStatus
                };

                _logger.LogInformation("Order found for Id: {Id}", Id);
                return View(productDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing status of the order with Id: {Id}", Id);
                return View("Error");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(ChangeOrderStatusDto model)
        {
            _logger.LogInformation("ChangeStatus POST action called with model: {@model}", model);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Model state is invalid for model: {@model}", model);
                return View(model);
            }

            if (!Guid.TryParse(model.Id, out var guidId))
            {
                var errorMsg = $"The id '{model.Id}' is not a valid GUID.";
                var errorModel = new ErrorModel()
                {
                    ErrorMessage = errorMsg
                };
                _logger.LogError(errorMsg);
                return View("AdminError", errorModel);
            }

            try
            {
                var order = await _orderRepository.GetByIdAsync(guidId);

                if (order == null)
                {
                    _logger.LogWarning("Order not found for Id: {Id}", model.Id);
                    ModelState.AddModelError("editError", "There isn't such order.");
                    return View(model);
                }

                order.OrderStatus = model.SelectedStatus;
                order.UpdatedAt = DateTime.UtcNow;

                int affected = await _orderRepository.SaveChangesAsync();

                if (affected == 0)
                {
                    _logger.LogWarning("Changing status of the order failed for Id: {Id}", model.Id);
                    ModelState.AddModelError("changingError", "Changing the order status failed.");
                    return View(model);
                }

                _logger.LogInformation("The order status has been successfully changed for Id: {Id}", model.Id);
                return RedirectToAction(nameof(GetAllOrders));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while changing status of the order with Id: {Id}", model.Id);
                return View("Error");
            }
        }
    }
}