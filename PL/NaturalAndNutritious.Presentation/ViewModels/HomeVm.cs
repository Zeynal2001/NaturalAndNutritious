using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class HomeVm
    {
        public List<MainProductDto>? FilterProductsByCategories {  get; set; }
        public List<MainProductDto>? BestsellerProducts { get; set; }
        public List<MainProductDto>? Vegetables { get; set; }
        public List<CategoryDto>? Categories { get; set; }
        public Guid? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int TotalCustomers { get; set; }
        public int TotalProducts { get; set; }
        public List<ReviewDto> Reviews { get; set; }
    }
}
