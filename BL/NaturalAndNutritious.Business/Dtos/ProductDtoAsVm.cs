
namespace NaturalAndNutritious.Business.Dtos
{
    public class ProductDtoAsVm
    {
        public List<CategoryDto> Categories { get; set; }
        public List<MainProductDto>? Products { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
