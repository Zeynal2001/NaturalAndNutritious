using NaturalAndNutritious.Data.Entities;

namespace NaturalAndNutritious.Business.Dtos
{
    public class ProductDetailsDto
    {
        public Guid Id { get; set; }
        public string ProductName { get; set; }
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public string ProductImageUrl { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }

        public double OriginalPrice { get; set; }
        public double? DiscountedPrice { get; set; }
        public int? AverageStar { get; set; }
        public int? UserStar { get; set; }
        public int? ViewsCount { get; set; }
        public int? Quantity { get; set; }
        public List<ReviewDto> Reviews { get; set; }
    }
}
