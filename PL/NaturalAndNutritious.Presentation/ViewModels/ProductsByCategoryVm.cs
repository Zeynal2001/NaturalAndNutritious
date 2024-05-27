using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class ProductsByCategoryVm
    {
        public Guid Id { get; set; }
        public List<MainProductDto> Products { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public string? CategoryName { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
