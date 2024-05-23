namespace NaturalAndNutritious.Business.Dtos
{
    public class SearchDtoAsVm
    {
        public List<MainProductDto> FoundProducts { get; set; }
        public string Query { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
