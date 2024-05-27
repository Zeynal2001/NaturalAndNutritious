using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class ProductDetailVm
    {
        public ProductDetailsDto ProductDetails { get; set; }
        public List<CategoryDto>? Categories { get; set; }
        public List<RelatedProductsDto>? RelatedProducts { get; set; }
        public List<MainProductDto>? FeaturedPproducts { get; set; }
    }
}
