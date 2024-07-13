using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Enums;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using SessionMapper;
using System.Security.Claims;

namespace NaturalAndNutritious.Business.Services.AdminPanelServices
{
    public class OrderService : IOrderService
    {
        public OrderService(IOrderRepository orderRepository, IShipperRepository shipperRepository, UserManager<AppUser> userManager, IProductRepository productRepository, IDiscountRepository discountRepository, IProductService productService, IOrderDetailRepository orderDetailRepository)
        {
            _orderRepository = orderRepository;
            _shipperRepository = shipperRepository;
            _userManager = userManager;
            _productRepository = productRepository;
            _discountRepository = discountRepository;
            _productService = productService;
            _orderDetailRepository = orderDetailRepository;
        }

        private readonly IOrderRepository _orderRepository;
        private readonly IOrderDetailRepository _orderDetailRepository;
        private readonly IShipperRepository _shipperRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;
        private readonly IDiscountRepository _discountRepository;
        private readonly UserManager<AppUser> _userManager;

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

        public async Task<(bool success, string message)> ProcessOrderAsync(CheckoutDto model, ClaimsPrincipal userPrincipal, ISession session)
        {
            var checkouts = session.Get<List<CheckoutModel>>("checkouts");
            var shipper = await _shipperRepository.Table.FirstOrDefaultAsync(sh => sh.CompanyName == "YasabKargo");
            var user = await _userManager.GetUserAsync(userPrincipal);

            if (user == null)
            {
                return (false, "Unauthorized");
            }

            if (checkouts == null || !checkouts.Any() || shipper == null)
            {
                return (false, "Order failed due to missing data.");
            }

            var totalSum = 0.0;
            var discountedPrices = new Dictionary<Guid, double>();

            foreach (var checkoutItem in checkouts)
            {
                var product = await _productRepository.GetByIdAsync(checkoutItem.ProductId);

                if (product == null)
                {
                    return (false, $"Order failed because product with ID {checkoutItem.ProductId} was not found.");
                }

                var discount = await _discountRepository.GetDiscountByProductId(product.Id);
                var discountedPrice = _productService.ApplyDiscount(checkoutItem.Price, discount);
                discountedPrices[product.Id] = discountedPrice;

                totalSum += discountedPrice * checkoutItem.Quantity;
            }

            var order = new Order
            {
                Id = Guid.NewGuid(),
                AppUser = user,
                Shipper = shipper,
                Freight = totalSum,
                OrderDate = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                ShipAddress = model.ShipAddress,
                ShipCity = model.ShipCity,
                ShipRegion = model.ShipRegion,
                ShipPostalCode = model.ShipPostalCode,
                ShipCountry = model.ShipCountry,
                CashOnDelivery = model.CashOnDelivery,
                OrderStatus = nameof(StatusType.Pending),
                FirstName = model.FirstName,
                LastName = model.LastName,
                MobileNumber = model.MobileNumber,
                Confirmed = false,
                IsDeleted = false
            };

            await _orderRepository.CreateAsync(order);

            foreach (var orderItem in checkouts)
            {
                var product = await _productRepository.GetByIdAsync(orderItem.ProductId);
                var discountedPrice = discountedPrices[orderItem.ProductId];

                var orderDetail = new OrderDetail
                {
                    Id = Guid.NewGuid(),
                    ProductId = orderItem.ProductId,
                    Quantity = orderItem.Quantity,
                    UnitPrice = discountedPrice,
                    CreatedAt = DateTime.UtcNow,
                    OrderId = order.Id,
                    Product = product,
                    IsDeleted = false
                };

                await _orderDetailRepository.CreateAsync(orderDetail);
            }

            await _orderRepository.SaveChangesAsync();
            await _orderDetailRepository.SaveChangesAsync();

            var myOrder = await _orderRepository.GetByIdAsync(order.Id);

            if (myOrder.Confirmed == false)
            {
                return (true, "The order has been successfully processed, but it needs to be confirmed.");
            }

            return (true, "The order has been successfully processed and confirmed.");
        }
    }
}