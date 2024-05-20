using NaturalAndNutritious.Data.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace NaturalAndNutritious.Data.Entities
{
    public class Discount : BaseEntity
    {
        public string DiscountType { get; set; } //Percentage, FixedAmount
        //public DiscountType DiscountTypes { get; set; }
        public double DiscountRate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        [ForeignKey(nameof(Product))]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
    }
}
