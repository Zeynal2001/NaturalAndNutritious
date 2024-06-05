namespace NaturalAndNutritious.Business.Abstractions.AdminPanelAbstractions
{
    public interface IOrderService
    {
        Task<int> TotalOrders();
        Task<int> TotalConfirmedOrders();
        Task<int> TotalUnconfirmedOrders();
    }
}
