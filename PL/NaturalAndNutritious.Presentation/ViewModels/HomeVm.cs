using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class HomeVm
    {
        public List<MainProductDto> FilterByCategries {  get; set; }
        public List<MainProductDto> BestsellerProducts { get; set; }
        public List<CategoryDto> Categories { get; set; }
    }
}
