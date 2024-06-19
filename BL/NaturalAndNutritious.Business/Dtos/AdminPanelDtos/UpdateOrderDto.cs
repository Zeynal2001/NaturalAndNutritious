using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class UpdateOrderDto
    {
        public string Id { get; set; }
        [Required]
        public string RecipientName { get; set; }
        [Required]
        public string RecipientLName { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public DateTime? ShippedDate { get; set; }
        [Required]
        public DateTime? RequiredDate { get; set; }
        [Required]
        public string ShipAddress { get; set; }
        [Required]
        public string ShipCity { get; set; }
        [Required]
        public string ShipCountry { get; set; }
        [Required]
        public string ShipRegion { get; set; }
        [Required]
        public string ShipPostalCode { get; set; }
    }
}
