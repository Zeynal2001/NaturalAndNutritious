using NaturalAndNutritious.Business.Dtos;
using System.ComponentModel.DataAnnotations;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class PriceRangeVm
    {
        public double SelectedPrice { get; set; }
        public List<MainProductDto>? Products { get; set; }
    }
}
