namespace NaturalAndNutritious.Business.Services.Results
{
    public class ProductServiceResult
    {
        public bool IsNull { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; }

        public ProductServiceResult()
        {
            IsNull = false;
            Succeeded = true;
            Message = string.Empty;
        }
    }
}
