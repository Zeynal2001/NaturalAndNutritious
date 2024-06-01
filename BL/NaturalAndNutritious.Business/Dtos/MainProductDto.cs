namespace NaturalAndNutritious.Business.Dtos
{
    public class MainProductDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string ProductImageUrl { get; set; }
        public string? CategoryName { get; set; }

        public double OriginalPrice { get; set; }
        public double? DiscountedPrice { get; set; }
        public int? Star { get; set; }
        public int? TotalSold { get; set; }
    }
}
