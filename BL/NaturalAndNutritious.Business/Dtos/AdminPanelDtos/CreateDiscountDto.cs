using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Business.Dtos.AdminPanelDtos
{
    public class CreateDiscountDto
    {
        [Required]
        public string DiscountType { get; set; } //Percentage, FixedAmount
        [Required]
        public double DiscountRate { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string ProductId { get; set; }
        public string? ProductName { get; set; }
    }
}
