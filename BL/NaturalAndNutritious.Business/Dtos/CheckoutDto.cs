using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos
{
    public class CheckoutDto
    {
        public string ShipName { get; set; }
        [Required]
        public string ShipAddress { get; set; }
        [Required]
        public string ShipCity { get; set; }
        [Required]
        public string ShipRegion { get; set; }
        [Required]
        public string ShipPostalCode { get; set; }
        [Required]
        public string ShipCountry { get; set; }
        public bool CashOnDelivery { get; set; }
    }
}
