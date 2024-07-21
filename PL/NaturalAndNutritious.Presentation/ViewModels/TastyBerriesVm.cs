using NaturalAndNutritious.Business.Dtos;

namespace NaturalAndNutritious.Presentation.ViewModels
{
    public class TastyBerriesVm
    {
        public List<MainProductDto>? Fruits { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
