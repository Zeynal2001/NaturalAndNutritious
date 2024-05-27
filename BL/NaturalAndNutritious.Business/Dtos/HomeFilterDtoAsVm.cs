using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Business.Dtos
{
    public class HomeFilterDtoAsVm
    {
        public List<MainProductDto> Products { get; set; }
        public List<CategoryDto> Categories { get; set; }
        public string CurrentFilter { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
