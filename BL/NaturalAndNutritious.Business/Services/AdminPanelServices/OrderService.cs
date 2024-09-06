using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NaturalAndNutritious.Business.Abstractions;
using NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions;
using NaturalAndNutritious.Business.Dtos;
using NaturalAndNutritious.Business.Enums;
using NaturalAndNutritious.Data.Abstractions;
using NaturalAndNutritious.Data.Entities;
using SessionMapper;
using System.Security.Claims;
using System.Transactions;

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
        private readonly ILogger<OrderService> _logger;
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
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled)) 
            {
                try
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

                        if (product.UnitsInStock < checkoutItem.Quantity)
                        {
                            return (false, $"Order failed because the product {product.ProductName} does not have enough stock.");
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

                        if (product == null)
                        {
                            return (false, $"Order failed because product with ID {orderItem.ProductId} was not found.");
                        }

                        var discountedPrice = discountedPrices[product.Id];
                        //var discountedPrice = discountedPrices[orderItem.ProductId];

                        var orderDetail = new OrderDetail
                        {
                            Id = Guid.NewGuid(),
                            ProductId = product.Id, //orderItem.ProductId, 
                            Quantity = orderItem.Quantity,
                            UnitPrice = discountedPrice,
                            CreatedAt = DateTime.UtcNow,
                            OrderId = order.Id,
                            Product = product,
                            IsDeleted = false
                        };

                        product.UnitsInStock -= orderItem.Quantity;
                        await _productRepository.UpdateAsync(product);

                        await _orderDetailRepository.CreateAsync(orderDetail);
                    }

                    await _orderRepository.SaveChangesAsync();
                    await _orderDetailRepository.SaveChangesAsync();

                    transaction.Complete();

                    var myOrder = await _orderRepository.GetByIdAsync(order.Id);

                    if (myOrder.Confirmed == false)
                    {
                        return (true, "The order has been successfully processed, but it needs to be confirmed.");
                    }

                    return (true, "The order has been successfully processed and confirmed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("An error occurred while processing the order: {Exception}", ex.ToString());
                    return (false, "An error occurred while processing the order.");
                }
            }
        }
    }
}