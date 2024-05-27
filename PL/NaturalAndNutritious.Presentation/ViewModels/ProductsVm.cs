using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class ProductsVm
    {
        public List<CategoryDto> Categories { get; set; }
        public List<MainProductDto>? Products { get; set; }
        public List<MainProductDto>? DiscountedProducts { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
