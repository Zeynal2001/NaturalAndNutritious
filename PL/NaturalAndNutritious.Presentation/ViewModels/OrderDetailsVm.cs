using NaturalAndNutritious.Presentation.Models;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class OrderDetailsVm
    {
        public List<OrderDetailsModel> OrderDetails {  get; set; }
        public string OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string? Shipper { get; set; }
        public string? ShipperTel { get; set; }
        public DateTime? EstimatedDeliveryTime { get; set; }
    }
}
