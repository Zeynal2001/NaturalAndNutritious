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
        //--------------------------------------------------
        public string RecipientFName { get; set; }
        public string RecipientLName { get; set; }
        public string MobileNumber { get; set; }
        public string ShipCountry { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipAddress { get; set; }
        public string ShipPostalCode { get; set; } 
        public bool CashOnDelivery { get; set; }
        public bool IsDelivered { get; set; }
        public bool IsCanceled { get; set; }
    }
}
