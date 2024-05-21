using NaturalAndNutritious.Business.Dtos.AdminPanelDtos;

namespace NaturalAndNutritious.Business.Services.Results
{
    public class ProductServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }
        public List<AllProductsDto> AllProducts { get; set; }
        public bool ItAlreadyHasADiscount { get; set; }

        public ProductServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
            AllProducts = new List<AllProductsDto>();
            ItAlreadyHasADiscount = false;
        }
    }
}
