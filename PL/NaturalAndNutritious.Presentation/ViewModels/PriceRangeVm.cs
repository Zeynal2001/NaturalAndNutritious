using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class PriceRangeVm
    {
        public double SelectedPrice { get; set; }
        public List<MainProductDto>? Products { get; set; }
    }
}
