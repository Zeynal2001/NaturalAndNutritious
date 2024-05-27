namespace NaturalAndNutritious.Business.Dtos
{
    public class RelatedProductsDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string? ProductImageUrl { get; set; }
        public double ProductPrice { get; set; }
        public string? CategoryName { get; set; }
    }
}
