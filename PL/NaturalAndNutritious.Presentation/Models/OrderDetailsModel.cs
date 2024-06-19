namespace NaturalAndNutritious.Presentation.Models
{
    public class OrderDetailsModel
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string? ProductImageUrl { get; set; }
        public int Quantity { get; set; }
        public double UnitPrice { get; set; }
    }
}
