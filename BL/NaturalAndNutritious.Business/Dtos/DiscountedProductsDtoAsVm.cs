namespace NaturalAndNutritious.Business.Dtos
{
    public class DiscountedProductsDtoAsVm
    {
        public List<MainProductDto> DiscountedProducts { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
    }
}
