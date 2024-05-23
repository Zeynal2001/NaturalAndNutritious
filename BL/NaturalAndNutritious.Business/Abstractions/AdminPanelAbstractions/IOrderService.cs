using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface IOrderService
    {
        Task<int> TotalOrders();
        Task<int> TotalConfirmedOrders();
        Task<int> TotalUnconfirmedOrders();
    }
}
