using Microsoft.AspNetCore.Http;
using NaturalAndNutritious.Business.Dtos;
using System.Security.Claims;

namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface IOrderService
    {
        Task<int> TotalOrders();
        Task<int> TotalConfirmedOrders();
        Task<int> TotalUnconfirmedOrders();
        Task<(bool success, string message)> ProcessOrderAsync(CheckoutDto model, ClaimsPrincipal userPrincipal, ISession session);
    }
}
