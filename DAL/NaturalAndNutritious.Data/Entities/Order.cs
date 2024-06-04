using System.ComponentModel.DataAnnotations.Schema;

namespace NaturalAndNutritious.Data.Entities
{
    public class Order : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public DateTime? ShippedDate { get; set; }
        public DateTime? RequiredDate { get; set; }
        public double Freight { get; set; }
        [NotMapped]
        public string ShipName { get => Shipper.CompanyName; }
        public string ShipAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MobileNumber { get; set; }
        public string ShipCity { get; set; }
        public string ShipRegion { get; set; }
        public string ShipPostalCode { get; set; }
        public string ShipCountry { get; set; }
        public string OrderStatus { get; set; }
        public bool Confirmed { get; set; }
        public bool CashOnDelivery { get; set; }

        public Shipper Shipper { get; set; }
        public AppUser AppUser { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
